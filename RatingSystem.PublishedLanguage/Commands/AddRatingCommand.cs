using MediatR;

namespace RatingSystem.PublishedLanguage.Commands
{
    public class AddRatingCommand : IRequest
    {
        public string EmailUser { get; set; }
        public string GroupId { get; set; }
        public string Category { get; set; }
        public decimal Rating { get; set; }
    }
}
