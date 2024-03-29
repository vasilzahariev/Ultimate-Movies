﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UltimateMovies.ViewModels.Orders
{
    public class AddressViewModel
    {
        public int Id { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string AdditionalInformation { get; set; }

        public int Postcode { get; set; }
    }
}
