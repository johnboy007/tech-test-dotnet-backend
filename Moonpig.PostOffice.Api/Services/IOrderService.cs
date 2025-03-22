using System;
using System.Collections.Generic;
using Moonpig.PostOffice.Api.Model;

namespace Moonpig.PostOffice.Api.Services;

public interface IOrderService
{
    DespatchDate GetDespatchDate(List<int> productIds, DateTime createdOrderDate);
}