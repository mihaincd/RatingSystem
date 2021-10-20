using System;
using System.Collections.Generic;

#nullable disable

namespace RatingSystem.Models
{
    public partial class UsersRating
    {
        public string EmailUser { get; set; }
        public string GroupId { get; set; }
        public string Category { get; set; }
        public decimal Rating { get; set; }
    }
}
