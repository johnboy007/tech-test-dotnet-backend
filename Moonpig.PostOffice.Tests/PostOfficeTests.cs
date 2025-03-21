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

    [Fact]
    public void OrderCreated_OnMonday_WithOneDayLeadTime_Will_BeReceivedInOneDay()
    {
        // Arrange
        const int productId = 1;
        var orderDateMonday = new DateTime(2018, 1, 21);

        // Act
        var date = _controller.Get(new List<int> { productId }, orderDateMonday);

        // Assert
        date.Date.Date.ShouldBe(orderDateMonday.Date.AddDays(1));
    }

    [Fact]
    public void OrderCreated_OnMonday_WithTwoDayLeadTime_Will_BeReceivedInTwoDays()
    {
        // Arrange
        const int productId = 2;
        var orderDateMonday = new DateTime(2018, 1, 21);

        // Act
        var date = _controller.Get(new List<int> { productId }, orderDateMonday);

        // Assert
        date.Date.Date.ShouldBe(orderDateMonday.Date.AddDays(2));
    }

    [Fact]
    public void OrderCreated_OnMonday_WithThreeDayLeadTime_Will_BeReceivedInThreeDays()
    {
        // Arrange
        const int productId = 3;
        var orderDateMonday = new DateTime(2018, 1, 21);

        // Act
        var date = _controller.Get(new List<int> { productId }, orderDateMonday);

        // Assert
        date.Date.Date.ShouldBe(orderDateMonday.Date.AddDays(3));
    }

    [Fact]
    public void OrderReceived_OnSaturday_Will_DespatchMonday()
    {
        // Arrange
        const int productId = 1;
        const int supplierLeadTime = 1;
        const int daysTilMonday = 2;

        var orderDateFriday = new DateTime(2018, 1, 26);
        var receivedDate = orderDateFriday.Date.AddDays(supplierLeadTime);
        var expectedDispatchDate = receivedDate.Date.AddDays(daysTilMonday);

        // Act
        var date = _controller.Get(new List<int> { productId }, orderDateFriday);

        // Assert
        date.Date.ShouldBe(expectedDispatchDate);
    }

    [Fact]
    public void OrderReceived_OnSunday_Will_DespatchMonday()
    {
        // Arrange
        const int productId = 3;
        const int supplierLeadTime = 3;
        const int daysTilMonday = 1;

        var orderDateThursday = new DateTime(2018, 1, 25);
        var receivedDate = orderDateThursday.Date.AddDays(supplierLeadTime);
        var expectedDispatchDate = receivedDate.Date.AddDays(daysTilMonday);

        // Act
        var date = _controller.Get(new List<int> { productId }, orderDateThursday);

        // Assert
        date.Date.ShouldBe(expectedDispatchDate);
    }
}