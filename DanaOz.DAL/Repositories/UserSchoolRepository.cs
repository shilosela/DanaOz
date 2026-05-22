using DanaOz.DAL.Context;
using DanaOz.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DanaOz.DAL.Repositories
{
    public class UserSchoolRepository
    {
        private readonly DanaOzDbContext _context;

        public UserSchoolRepository(DanaOzDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserSchool>> GetByUserIdAsync(int userId)
        {
            return await _context.User_Schools
                .Where(s => s.UserId == userId)
                .OrderBy(s => s.SortOrder)
                .ToListAsync();
        }

        public async Task<UserSchool?> GetFirstByUserIdAsync(int userId)
        {
            return await _context.User_Schools
                .Where(s => s.UserId == userId)
                .OrderBy(s => s.SortOrder)
                .FirstOrDefaultAsync();
        }

        public async Task<UserSchool> CreateAsync(UserSchool userSchool)
        {
            userSchool.Creation_DateTime = DateTime.UtcNow;
            userSchool.Last_Update_DateTime = DateTime.UtcNow;
            _context.User_Schools.Add(userSchool);
            await _context.SaveChangesAsync();
            return userSchool;
        }

        public async Task DeleteByUserIdAsync(int userId)
        {
            var schools = await _context.User_Schools
                .Where(s => s.UserId == userId)
                .ToListAsync();
            _context.User_Schools.RemoveRange(schools);
            await _context.SaveChangesAsync();
        }
    }
}
