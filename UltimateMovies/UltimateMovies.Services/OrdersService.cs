﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimateMovies.Data;
using UltimateMovies.Models;
using UltimateMovies.Models.Enums;

namespace UltimateMovies.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly UltimateMoviesDbContext db;

        public OrdersService(UltimateMoviesDbContext db)
        {
            this.db = db;
        }

        public ICollection<Address> GetAllUserAddresses(string username)
        {
            List<Address> result = new List<Address>();

            foreach (var address in this.db.Addresses)
            {
                if (address.UserId == this.db.Users.FirstOrDefault(u => u.UserName == username).Id)
                {
                    result.Add(address);
                }
            }

            return result;
        }

        public UMUser GetUser(string username)
        {
            return this.db.Users.FirstOrDefault(u => u.UserName == username);
        }

        public Dictionary<Movie, int> GetCartMovies(string username)
        {
            Dictionary<Movie, int> result = new Dictionary<Movie, int>();

            foreach (var cartMovie in this.db.CartMovies)
            {
                if (cartMovie.UserId == this.db.Users.FirstOrDefault(u => u.UserName == username).Id)
                {
                    result[this.db.Movies.FirstOrDefault(m => m.Id == cartMovie.MovieId)] = cartMovie.Quantity;
                }
            }

            return result;
        }

        public void CreateAnOrder(double cartPrice, DeliveryType deliveryType, PaymentType paymentType, int addressId, string username, string phone)
        {
            double deliveryPrice = deliveryType == DeliveryType.Normal ? (double)4.99 : (double)9.99;

            UsersService usersService = new UsersService(db);

            usersService.AddPhoneToUser(username, phone);

            if (paymentType == PaymentType.CashOnDelivery)
            {
                Order order = new Order
                {
                    DeliveryAddressId = addressId,
                    DeliveryAddress = this.db.Addresses.FirstOrDefault(a => a.Id == addressId),
                    DeliveryType = deliveryType,
                    DeliveryPrice = deliveryPrice,
                    OrderDate = DateTime.Now,
                    OrderStatus = OrderStatus.Unprocessed,
                    PaymentType = paymentType,
                    TotalPrice = cartPrice + deliveryPrice,
                    UserId = this.db.Users.FirstOrDefault(u => u.UserName == username).Id,
                    User = this.db.Users.FirstOrDefault(u => u.UserName == username)
                };

                this.db.Orders.Add(order);

                this.db.Users.FirstOrDefault(u => u.UserName == username).Orders.Add(order);

                this.db.SaveChanges();

                this.RemoveCartItems(this.db.Orders.Last().Id, username);
            }
            else
            {
                Order order = new Order
                {
                    DeliveryAddressId = addressId,
                    DeliveryAddress = this.db.Addresses.FirstOrDefault(a => a.Id == addressId),
                    DeliveryType = deliveryType,
                    DeliveryPrice = deliveryPrice,
                    OrderDate = DateTime.Now,
                    OrderStatus = OrderStatus.Unfinished,
                    PaymentType = paymentType,
                    TotalPrice = cartPrice + deliveryPrice,
                    UserId = this.db.Users.FirstOrDefault(u => u.UserName == username).Id,
                    User = this.db.Users.FirstOrDefault(u => u.UserName == username)
                };

                this.db.Orders.Add(order);

                this.db.Users.FirstOrDefault(u => u.UserName == username).Orders.Add(order);

                this.db.SaveChanges();
            }
        }

        private void RemoveCartItems(int orderId, string username)
        {
            List<CartMovie> cartMovies = new List<CartMovie>();
            List<OrderMovie> orderMovies = new List<OrderMovie>();

            foreach (var cartMovie in this.db.CartMovies)
            {
                if (cartMovie.UserId == this.db.Users.FirstOrDefault(u => u.UserName == username).Id)
                {
                    cartMovies.Add(cartMovie);

                    orderMovies.Add(new OrderMovie
                    {
                        MovieId = cartMovie.MovieId,
                        OrderId = orderId,
                        Quantity = cartMovie.Quantity,
                        Price = this.db.Movies.FirstOrDefault(m => m.Id == cartMovie.MovieId).BluRayPrice
                    });
                }
            }

            this.db.CartMovies.RemoveRange(cartMovies);
            this.db.OrderMovies.AddRange(orderMovies);

            this.db.SaveChanges();
        }

        public void PayAnOrder(string username, int orderId)
        {
            this.db.Orders.FirstOrDefault(o => o.Id == orderId).OrderStatus = OrderStatus.Unprocessed;

            this.db.SaveChanges();

            this.RemoveCartItems(orderId, username);
        }

        public Order GetLastOrder(string username)
        {
            return this.db.Orders.Last();
        }

        public List<OrderMovie> GetOrderMovies(int orderId)
        {
            List<OrderMovie> orderMovies = new List<OrderMovie>();

            foreach (var orderMovie in this.db.OrderMovies)
            {
                if (orderMovie.OrderId == orderId)
                {
                    orderMovies.Add(orderMovie);
                }
            }

            return orderMovies;
        }

        public List<Order> GetAllUserOrders(string username)
        {
            List<Order> result = new List<Order>();

            foreach (var order in this.db.Orders)
            {
                if (order.UserId == this.db.Users.FirstOrDefault(u => u.UserName == username).Id)
                {
                    result.Add(order);
                }
            }

            return result;
        }

        public bool CheckIfOrderBelongsToUser(int orderId, string username)
        {
            return this.db.Orders.Any(o => o.Id == orderId && o.UserId == this.db.Users.FirstOrDefault(u => u.UserName == username).Id);
        }

        public Order GetOrder(int id)
        {
            return this.db.Orders.FirstOrDefault(o => o.Id == id);
        }

        public List<Order> GetAllOrders()
        {
            return this.db.Orders.ToList();
        }

        public void ProcessOrder(int id)
        {
            this.db.Orders.FirstOrDefault(o => o.Id == id).OrderStatus = OrderStatus.Procesed;
            this.db.Orders.FirstOrDefault(o => o.Id == id).ProcesedDate = DateTime.Now;

            this.db.SaveChanges();
        }

        public void DeliverOrder(int id)
        {
            this.db.Orders.FirstOrDefault(o => o.Id == id).OrderStatus = OrderStatus.Delivered;
            this.db.Orders.FirstOrDefault(o => o.Id == id).DeliveredDate = DateTime.Now;

            this.db.SaveChanges();
        }
    }
}
