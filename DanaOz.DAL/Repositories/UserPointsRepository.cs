using DanaOz.DAL.Context;
using DanaOz.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DanaOz.DAL.Repositories
{
    public class UserPointsRepository
    {
        private readonly DanaOzDbContext _context;

        public UserPointsRepository(DanaOzDbContext context)
        {
            _context = context;
        }

        // Add a points event
        public async Task<UserPoints> AddAsync(int userId, int points, int? chatLogId = null, int? eventTypeId = null)
        {
            var pointsRecord = new UserPoints
            {
                UserId = userId,
                PointsGiven = points,
                OriginalChatLogId = chatLogId,
                EventTypeId = eventTypeId,
                Creation_DateTime = DateTime.UtcNow,
                Last_Update_DateTime = DateTime.UtcNow
            };

            _context.User_Points.Add(pointsRecord);
            await _context.SaveChangesAsync();
            return pointsRecord;
        }

        // Get total points for a user
        public async Task<int> GetTotalByUserIdAsync(int userId)
        {
            return await _context.User_Points
                .Where(p => p.UserId == userId)
                .SumAsync(p => p.PointsGiven);
        }

        // Get points history for a user
        public async Task<List<UserPoints>> GetHistoryByUserIdAsync(int userId)
        {
            return await _context.User_Points
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.Creation_DateTime)
                .ToListAsync();
        }
    }
}