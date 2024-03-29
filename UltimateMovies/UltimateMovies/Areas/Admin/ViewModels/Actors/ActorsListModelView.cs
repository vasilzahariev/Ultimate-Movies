﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UltimateMovies.Areas.Admin.ViewModels
{
    public class ActorsListModelView
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Actor's Name")]
        public string Name { get; set; }

        public string PictureUrl { get; set; }
    }
}
