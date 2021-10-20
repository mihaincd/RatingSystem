using MediatR;

namespace RatingSystem.PublishedLanguage.Commands
{
    public class AvarageRatingCommand : IRequest
    {
        public string GroupId { get; set; }
        public string Category { get; set; }
        public decimal RatingAvg { get; set; }
    }
}
