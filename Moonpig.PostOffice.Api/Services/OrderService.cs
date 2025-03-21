using Moonpig.PostOffice.Api.Model;
using Moonpig.PostOffice.Data;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Moonpig.PostOffice.Api.Services
{
    public class OrderService : IOrderService
    {
        private readonly DbContext _dbContext;
        private DateTime _maxLeadTime;
        private const int DaysToAddForSaturday = 2;
        private const int DaysToAddForSunday = 1;

        public OrderService(DbContext dbContext)
        {
            _dbContext = dbContext;
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
            var supplierId = _dbContext.Products.Single(product => product.ProductId == productId).SupplierId;
            var leadTime = _dbContext.Suppliers.Single(supplier => supplier.SupplierId == supplierId).LeadTime;
            var potentialLeadTime = orderDate.AddDays(leadTime);

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
