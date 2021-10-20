using MediatR;
using RatingSystem.Data;
using RatingSystem.Models;
using RatingSystem.PublishedLanguage.Commands;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RatingSystem.Application.CommandHandlers
{
    public class AddRating : IRequestHandler<AddRatingCommand>
    {
        private readonly IMediator _mediator;
        private readonly RatingDbContext _dbContext;

        public AddRating(IMediator mediator, RatingDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(AddRatingCommand request, CancellationToken cancellationToken)
        {
            var userRating = _dbContext.UsersRatings.FirstOrDefault(p => p.GroupId == request.GroupId
                                                                      && p.EmailUser == request.EmailUser);
            if (userRating == null)
            {
                userRating = new UsersRating
                {
                    EmailUser = request.EmailUser,
                    GroupId = request.GroupId,
                    Category = request.Category,
                    Rating = request.Rating
                }; 
                _dbContext.UsersRatings.Add(userRating);
                _dbContext.SaveChanges();
            }
            else
            {
                userRating.Rating = request.Rating;
                _dbContext.UsersRatings.Update(userRating);
                _dbContext.SaveChanges();

            }


            var average = _dbContext.UsersRatings.Select(p => p.Rating).Average(); 
            var ratingPerConf = _dbContext.RatingPerGroups.FirstOrDefault(p => p.GroupId == request.GroupId);
            if (ratingPerConf == null)
            {
                _dbContext.RatingPerGroups.Add(new RatingPerGroup
                {
                    GroupId = request.GroupId,
                    Category = request.Category,
                    RatingAvg = average
                });
            }
            else
            {
                ratingPerConf.RatingAvg = average;
            }
            _dbContext.SaveChanges();
            return await Unit.Task;
        }
    }
}
