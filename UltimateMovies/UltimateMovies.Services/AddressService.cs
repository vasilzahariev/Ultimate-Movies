﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimateMovies.Data;
using UltimateMovies.Models;

namespace UltimateMovies.Services
{
    public class AddressService : IAddressesService
    {
        private readonly UltimateMoviesDbContext db;

        public AddressService(UltimateMoviesDbContext db)
        {
            this.db = db;
        }

        public void CreateANewAddress(string country, string city, string street, string additionalInformation, int postCode, string username)
        {
            Address address = new Address
            {
                Country = country,
                City = city,
                Street = street,
                AdditionalInformation = additionalInformation,
                Postcode = postCode,
                UserId = this.db.Users.FirstOrDefault(u => u.UserName == username).Id,
                User = this.db.Users.FirstOrDefault(u => u.UserName == username)
            };

            this.db.Addresses.Add(address);

            this.db.SaveChanges();
        }

        public List<Address> GetAllUserAddresses(string username)
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

        public void RemoveAddress(int addressId)
        {
            this.db.Addresses.Remove(this.db.Addresses.FirstOrDefault(a => a.Id == addressId));

            this.db.SaveChanges();
        }
    }
}
