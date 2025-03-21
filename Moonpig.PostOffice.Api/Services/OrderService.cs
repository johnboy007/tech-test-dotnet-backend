using Moonpig.PostOffice.Api.Model;
using Moonpig.PostOffice.Data;
using System.Collections.Generic;
using System;

namespace Moonpig.PostOffice.Api.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository _repository;
        private DateTime _dispatchDate;
        private const int DaysToAddForSaturday = 2;
        private const int DaysToAddForSunday = 1;

        public OrderService(IRepository repository)
        {
            _repository = repository;
        }

        public DespatchDate GetDespatchDate(List<int> productIds, DateTime orderDate)
        {
            if (DateIsOverTheWeekend(orderDate))
            {
                orderDate = UpdateToBeProcessOnMonday(orderDate);
            }

            _dispatchDate = orderDate;

            foreach (var productId in productIds)
            {
                UpdateMaxLeadTime(productId, orderDate);
            }

            if (DateIsOverTheWeekend(_dispatchDate))
            {
                UpdateToBeDispatchedMonday();
            }

            return new DespatchDate { Date = _dispatchDate };
        }

        private static bool DateIsOverTheWeekend(DateTime orderDate) =>
            orderDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

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

            if (potentialLeadTime > _dispatchDate)
            {
                _dispatchDate = potentialLeadTime;
            }
        }

        private void UpdateToBeDispatchedMonday()
        {
            if (_dispatchDate.DayOfWeek == DayOfWeek.Saturday)
                _dispatchDate = _dispatchDate.AddDays(DaysToAddForSaturday);
            if (_dispatchDate.DayOfWeek == DayOfWeek.Sunday)
                _dispatchDate = _dispatchDate.AddDays(DaysToAddForSunday);
        }
    }
}
