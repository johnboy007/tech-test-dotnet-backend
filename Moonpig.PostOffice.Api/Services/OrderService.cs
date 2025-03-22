using System;
using System.Collections.Generic;
using Moonpig.PostOffice.Api.Model;
using Moonpig.PostOffice.Data;

namespace Moonpig.PostOffice.Api.Services;

public class OrderService : IOrderService
{
    private const int DaysToAddForSaturday = 2;
    private const int DaysToAddForSunday = 1;
    private readonly IRepository _repository;
    private DateTime _dispatchDate;

    public OrderService(IRepository repository)
    {
        _repository = repository;
    }

    public DespatchDate GetDespatchDate(List<int> productIds, DateTime createdOrderDate)
    {
        if (DateIsOverTheWeekend(createdOrderDate)) 
            createdOrderDate = UpdateToBeProcessOnMonday(createdOrderDate);

        _dispatchDate = createdOrderDate;

        foreach (var productId in productIds) 
            UpdateMaxLeadTime(productId, createdOrderDate);

        AddWeekendDaysTheLeadTimeOverruns(createdOrderDate);

        if (DateIsOverTheWeekend(_dispatchDate)) 
            UpdateToBeDispatchedMonday();

        return new DespatchDate { Date = _dispatchDate };
    }

    private void AddWeekendDaysTheLeadTimeOverruns(DateTime createdOrderDate)
    {
        var weekendDayCount = CountWeekends(createdOrderDate, _dispatchDate);
        if (weekendDayCount > 0) 
            _dispatchDate = _dispatchDate.AddDays(weekendDayCount);
    }

    private int CountWeekends(DateTime startDate, DateTime endDate)
    {
        var weekendDayCount = 0;
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                weekendDayCount++;
            }
        }
        return weekendDayCount;
    }

    private static bool DateIsOverTheWeekend(DateTime orderDate)
    {
        return orderDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }

    private static DateTime UpdateToBeProcessOnMonday(DateTime orderDate)
    {
        orderDate = orderDate.DayOfWeek switch
        {
            // Has order been created on the weekend? If so, adjust the order date to the next Monday
            DayOfWeek.Sunday => orderDate.AddDays(1),
            DayOfWeek.Saturday => orderDate.AddDays(2),
            _ => orderDate
        };
        return orderDate;
    }

    private void UpdateMaxLeadTime(int productId, DateTime orderDate)
    {
        var product = _repository.GetProduct(productId);
        var supplier = _repository.GetSupplier(product.SupplierId);
        var potentialLeadTime = orderDate.AddDays(supplier.LeadTime);

        if (potentialLeadTime > _dispatchDate) _dispatchDate = potentialLeadTime;
    }

    private void UpdateToBeDispatchedMonday()
    {
        if (_dispatchDate.DayOfWeek == DayOfWeek.Saturday)
            _dispatchDate = _dispatchDate.AddDays(DaysToAddForSaturday);
        if (_dispatchDate.DayOfWeek == DayOfWeek.Sunday)
            _dispatchDate = _dispatchDate.AddDays(DaysToAddForSunday);
    }
}