﻿using System;
using System.ComponentModel.DataAnnotations;

namespace WhereAreMyBuddies.Model
{
    public class Coordinates
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
