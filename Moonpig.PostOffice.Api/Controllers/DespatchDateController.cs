namespace Moonpig.PostOffice.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Model;
    using Moonpig.PostOffice.Api.Services;

    [Route("api/[controller]")]
    public class DespatchDateController : Controller
    {
        private readonly IOrderService _orderService;

        public DespatchDateController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public DespatchDate Get(List<int> productIds, DateTime orderDate)
        {
            return _orderService.GetDespatchDate(productIds, orderDate);
        }
    }
}