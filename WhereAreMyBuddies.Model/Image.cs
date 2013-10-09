using System;
using System.ComponentModel.DataAnnotations;

namespace WhereAreMyBuddies.Model
{
    public class Image
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Url { get; set; }

        public DateTime DateTimeAtCapturing { get; set; }

        public virtual Coordinates Coordinates { get; set; }
    }
}
