﻿using System;
using System.Collections.Generic;
using System.Text;
using UltimateMovies.Models;

namespace UltimateMovies.Services
{
    public interface IHomeServices
    {
        ICollection<Movie> GetMovies();

        bool IsMovieInUserWishlist(string username, int movieId);
    }
}
