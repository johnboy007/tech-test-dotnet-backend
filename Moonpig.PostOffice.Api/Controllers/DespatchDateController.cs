namespace Moonpig.PostOffice.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Model;
    using Services;

    [Route("api/[controller]")]
    public class DespatchDateController : Controller
    {
        private readonly IDispatchService _dispatchService;

        public DespatchDateController(IDispatchService dispatchService)
        {
            _dispatchService = dispatchService;
        }

        [HttpGet]
        public DespatchDate Get(List<int> productIds, DateTime orderDate)
        {
            return _dispatchService.GetDespatchDate(productIds, orderDate);
        }
    }
}