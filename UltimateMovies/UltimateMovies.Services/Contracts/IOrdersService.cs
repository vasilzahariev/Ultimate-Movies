﻿using System;
using System.Collections.Generic;
using System.Text;
using UltimateMovies.Models;
using UltimateMovies.Models.Enums;

namespace UltimateMovies.Services
{
    public interface IOrdersService
    {
        List<Address> GetAllUserAddresses(string username);

        UMUser GetUser(string username);

        Dictionary<Movie, int> GetCartMovies(string username);

        void CreateAnOrder(double cartPrice, DeliveryType deliveryType, PaymentType paymentType, int addressId, string username, string phone);

        void PayAnOrder(string username, int orderId);

        Order GetLastOrder(string username);

        List<OrderMovie> GetOrderMovies(int orderId);
    }
}
