using DanaOz.DAL.Context;
using DanaOz.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DanaOz.DAL.Repositories
{
    public class UserClassRepository
    {
        private readonly DanaOzDbContext _context;

        public UserClassRepository(DanaOzDbContext context)
        {
            _context = context;
        }

        // Get all classes for a teacher
        public async Task<List<UserClass>> GetByUserIdAsync(int userId)
        {
            return await _context.User_Classes
                .Include(c => c.Subject)
                .Include(c => c.School)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        // Add a class for a teacher
        public async Task<UserClass> CreateAsync(UserClass userClass)
        {
            userClass.Creation_DateTime = DateTime.UtcNow;
            userClass.Last_Update_DateTime = DateTime.UtcNow;
            _context.User_Classes.Add(userClass);
            await _context.SaveChangesAsync();
            return userClass;
        }

        // Delete a class
        public async Task DeleteAsync(int classId)
        {
            var userClass = await _context.User_Classes.FindAsync(classId);
            if (userClass != null)
            {
                _context.User_Classes.Remove(userClass);
                await _context.SaveChangesAsync();
            }
        }
    }
}