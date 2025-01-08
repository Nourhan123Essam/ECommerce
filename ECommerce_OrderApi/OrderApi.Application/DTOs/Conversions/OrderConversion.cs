﻿using OrderApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.DTOs.Conversions
{
    public static class OrderConversion
    {
        public static Order ToEntity(OrderDTO order) => new()
        {
            Id = order.Id,
            ClientId = order.ClientId,
            ProductId = order.ProductId,
            OrderedDate = order.OrderedDate,
            PurchaseQuantity = order.PurchaseQuantity,
        };

        public static (OrderDTO?, IEnumerable<OrderDTO>?) FromEntity(Order? order, IEnumerable<Order>? orders)
        {
            //return single
            if (order is not null)
            {
                var singleOrder = new OrderDTO(
                    order!.Id,
                    order!.ClientId,
                    order!.ProductId,
                    order.PurchaseQuantity,
                    order.OrderedDate
                    );
                return (singleOrder, null);
            }

            //return list
            if (orders is not null)
            {
                var _orders = orders!.Select(o =>
                    new OrderDTO(
                        o.Id,
                        o.ClientId,
                        o.ProductId,
                        o.PurchaseQuantity,
                        o.OrderedDate
                    )
                );
                return (null, _orders);
            }

            
            return (null, null);
        }
    }
}
