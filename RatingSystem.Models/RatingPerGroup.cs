using System;
using System.Collections.Generic;

#nullable disable

namespace RatingSystem.Models
{
    public partial class RatingPerGroup
    {
        public string GroupId { get; set; }
        public string Category { get; set; }
        public decimal RatingAvg { get; set; }
    }
}
