﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateMovies.Data;
using UltimateMovies.Models;

namespace UltimateMovies.Services
{
    public class UsersService : IUsersService
    {
        private UltimateMoviesDbContext db;
        private readonly UserManager<UMUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UsersService(UltimateMoviesDbContext db)
        {
            this.db = db;
        }

        public void AddMovieToWishList(string username, int movieId)
        {
            UMUser user = this.db.Users.FirstOrDefault(u => u.UserName == username);
            Movie movie = this.db.Movies.FirstOrDefault(m => m.Id == movieId);

            if (user == null || movie == null)
            {
                return;
            }

            if (this.db.WishListMovies.Any(x => x.UserId == user.Id && x.MovieId == movie.Id))
            {
                this.RemoveMovieFromWishList(username, movieId);
                return;
            }

            WishListMovie wishListMovie = new WishListMovie();

            wishListMovie.MovieId = movie.Id;
            wishListMovie.UserId = user.Id;

            if (user.WishList == null)
            {
                user.WishList = new HashSet<WishListMovie>();
            }

            user.WishList.Add(wishListMovie);
            this.db.WishListMovies.Add(wishListMovie);

            this.db.SaveChanges();
        }

        public void AddPhoneToUser(string username, string phone)
        {
            this.db.Users.FirstOrDefault(u => u.UserName == username).PhoneNumber = phone;

            this.db.SaveChanges();
        }

        public ICollection<Movie> GetMoviesFromWishList(string username)
        {
            UMUser user = this.db.Users.FirstOrDefault(u => u.UserName == username);
            List<Movie> result = new List<Movie>();

            foreach (WishListMovie wishListMovie in this.db.WishListMovies)
            {
                if (wishListMovie.UserId == user.Id)
                {
                    result.Add(this.db.Movies.FirstOrDefault(m => m.Id == wishListMovie.MovieId));
                }
            }

            return result;
        }

        public UMUser GetUser(string username)
        {
            return this.db.Users.FirstOrDefault(u => u.UserName == username);
        }

        public UMUser GetUserById(string id)
        {
            return this.db.Users.FirstOrDefault(u => u.Id == id);
        }

        public void RemoveMovieFromWishList(string username, int movieId)
        {
            UMUser user = this.db.Users.FirstOrDefault(u => u.UserName == username);
            Movie movie = this.db.Movies.FirstOrDefault(m => m.Id == movieId);

            if (user == null || movie == null)
            {
                return;
            }

            if (!this.db.WishListMovies.Any(x => x.UserId == user.Id && x.MovieId == movie.Id))
            {
                return;
            }

            if (user.WishList != null)
            {
                user.WishList.Remove(user.WishList.FirstOrDefault(x => x.MovieId == movie.Id && x.UserId == user.Id));
            }

            this.db.WishListMovies.Remove(this.db.WishListMovies.FirstOrDefault(x => x.MovieId == movie.Id && x.UserId == user.Id));

            this.db.SaveChanges();
        }

        public ICollection<UMUser> GetAllUsers()
        {
            return this.db.Users.ToList();
        }

        public string GetUserRole(string id)
        {
            if (this.db.UserRoles.FirstOrDefault(ur => ur.UserId == id) == null)
            {
                return "";
            }

            string result = this.db.Roles.FirstOrDefault(r => r.Id == this.db.UserRoles.FirstOrDefault(ur => ur.UserId == id).RoleId).Name;

            return result;
        }

        public bool CheckIfReal(string id)
        {
            return this.db.Users.Any(u => u.Id == id);
        }

        public ICollection<Movie> GetMoviesFromLibrary(string username)
        {
            List<Movie> result = new List<Movie>();

            foreach (var libMovie in this.db.LibraryMovies)
            {
                if (libMovie.UserId == this.db.Users.FirstOrDefault(u => u.UserName == username).Id)
                {
                    result.Add(this.db.Movies.FirstOrDefault(m => m.Id == libMovie.MovieId));
                }
            }

            return result;
        }

        public void Delete(string id)
        {
            this.db.Addresses.RemoveRange(this.db.Addresses.ToList().FindAll(a => a.UserId == id));
            this.db.CartMovies.RemoveRange(this.db.CartMovies.ToList().FindAll(a => a.UserId == id));
            this.db.LibraryMovies.RemoveRange(this.db.LibraryMovies.ToList().FindAll(a => a.UserId == id));
            this.db.WishListMovies.RemoveRange(this.db.WishListMovies.ToList().FindAll(a => a.UserId == id));
            this.db.Reviews.RemoveRange(this.db.Reviews.ToList().FindAll(a => a.UserId == id));

            List<int> removableOrders = this.db.Orders.ToList().FindAll(o => o.UserId == id).Select(x => x.Id).ToList();

            this.db.OrderMovies.RemoveRange(this.db.OrderMovies.ToList().FindAll(o => removableOrders.Contains(o.OrderId) == true));

            this.db.Orders.RemoveRange(this.db.Orders.ToList().FindAll(a => a.UserId == id));

            this.db.SaveChanges();
        }
    }
}
