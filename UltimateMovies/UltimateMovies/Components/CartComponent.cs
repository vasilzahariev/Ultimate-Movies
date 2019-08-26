﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UltimateMovies.Models;
using UltimateMovies.Services;
using UltimateMovies.ViewModels.Cart;

namespace UltimateMovies.Components
{
    public class CartComponent : ViewComponent
    {
        private ICartsService cartsService;

        public CartComponent(ICartsService cartsService)
        {
            this.cartsService = cartsService;
        }

        public IViewComponentResult Invoke()
        {
            Dictionary<Movie, int> movies = this.cartsService.GetAllMoviesFromUserCart(this.User.Identity.Name);

            CartListingModel cartListingModel = new CartListingModel
            {
                CartMovies = movies.Select(m => new CartMovieViewModel
                {
                    MovieName = m.Key.Name,
                    Price = m.Key.BluRayPrice,
                    Quantity = m.Value
                })
            };

            return this.View(cartListingModel);
        }
    }
}