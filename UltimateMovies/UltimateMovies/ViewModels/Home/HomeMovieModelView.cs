﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UltimateMovies.Models.Enums;

namespace UltimateMovies.ViewModels.Home
{
    public class HomeMovieModelView
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public DateTime ReleaseDate { get; set; }

        public int Length { get; set; }

        public double IMDbScore { get; set; }

        public int RottenTomatoes { get; set; }

        public bool IsInUserWishList { get; set; }

        public MovieGenre Genre { get; set; }

        public MovieGenre Genre2 { get; set; }

        public MovieGenre Genre3 { get; set; }
    }
}
