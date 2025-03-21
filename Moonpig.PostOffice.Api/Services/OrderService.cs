using Moonpig.PostOffice.Api.Model;
using Moonpig.PostOffice.Data;
using System.Collections.Generic;
using System;

namespace Moonpig.PostOffice.Api.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository _repository;
        private DateTime _maxLeadTime;
        private const int DaysToAddForSaturday = 2;
        private const int DaysToAddForSunday = 1;

        public OrderService(IRepository repository)
        {
            _repository = repository;
        }

        public DespatchDate GetDespatchDate(List<int> productIds, DateTime orderDate)
        {
            _maxLeadTime = orderDate;

            foreach (var productId in productIds)
            {
                UpdateMaxLeadTime(productId, orderDate);
            }

            return new DespatchDate { Date = AdjustForWeekend(_maxLeadTime) };
        }

        private void UpdateMaxLeadTime(int productId, DateTime orderDate)
        {
            var product = _repository.GetProduct(productId);
            var supplier = _repository.GetSupplier(product.SupplierId);
            var potentialLeadTime = orderDate.AddDays(supplier.LeadTime);

            if (potentialLeadTime > _maxLeadTime)
            {
                _maxLeadTime = potentialLeadTime;
            }
        }

        private DateTime AdjustForWeekend(DateTime date)
        {
            return date.DayOfWeek switch
            {
                DayOfWeek.Saturday => date.AddDays(DaysToAddForSaturday),
                DayOfWeek.Sunday => date.AddDays(DaysToAddForSunday),
                _ => date
            };
        }
    }
}
