using System;
using System.Collections.Generic;
using Moonpig.PostOffice.Api.Controllers;
using Moonpig.PostOffice.Api.Services;
using Moonpig.PostOffice.Data;
using Shouldly;
using Xunit;

namespace Moonpig.PostOffice.Tests;

public class PostOfficeTests
{
    private readonly DespatchDateController _controller;

    public PostOfficeTests()
    {
        var dbContext = new DbContext();
        var repository = new Repository(dbContext);
        var orderService = new OrderService(repository);
        _controller = new DespatchDateController(orderService);
    }

    /// <summary>
    /// Lead time added to despatch date  
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="leadTime"></param>
    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 3)]
    public void OrderCreated_OnMonday_WillAddLeadTimeToDispatchDate(int productId, int leadTime)
    {
        // Arrange
        var orderDateMonday = new DateTime(2018, 1, 1);

        // Act
        var date = _controller.Get(new List<int> { productId }, orderDateMonday);

        // Assert
        date.Date.Date.ShouldBe(orderDateMonday.Date.AddDays(leadTime));
    }

    /// <summary>
    /// Lead time is not counted over a weekend
    /// </summary>
    /// <param name="dayOrdered"></param>
    /// <param name="productId"></param>
    /// <param name="leadTime"></param>
    [Theory]
    [InlineData(5, 1, 1, 8)]
    [InlineData(6, 1, 1, 9)]
    [InlineData(7, 1, 1, 9)]
    public void OrderReceived_OnWeekend_Will_DispatchMondayPlusLeadTime(int dayOrdered, int productId, int leadTime, int dayExpected)
    {
        // Arrange
        var orderDate = new DateTime(2018, 1, dayOrdered);
        var expectedDispatchDate = new DateTime(2018, 1, dayExpected);

        // Act
        var date = _controller.Get(new List<int> { productId }, orderDate);

        // Assert
        date.Date.ShouldBe(expectedDispatchDate);
    }

    [Fact]
    public void Supplier_WithLongestLeadTime_Will_BeUsedForCalculation()
    {
        // Arrange
        var orderDateMonday = new DateTime(2018, 1, 1);
        var expectedDispatchDate = new DateTime(2018, 1, 3);

        // Act
        var date = _controller.Get(new List<int> { 1, 2 }, orderDateMonday);

        // Assert
        date.Date.Date.ShouldBe(expectedDispatchDate);
    }

    [Theory]
    [InlineData(5, 9, 6, 15)]
    [InlineData(5, 8, 11, 22)]
    public void OrderService_CanProcessDispatchDates_WithLongSupplierLeadTimes(int dayOrdered, int productId, int leadTime, int dayExpected)
    {
        // Arrange
        var orderDate = new DateTime(2018, 1, dayOrdered);
        var expectedDispatchDate = new DateTime(2018, 1, dayExpected);

        // Act
        var date = _controller.Get(new List<int> { productId }, orderDate);

        // Assert
        date.Date.ShouldBe(expectedDispatchDate);
    }
}