using DanaOz.DAL.Context;
using DanaOz.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DanaOz.DAL.Repositories
{
    public class UserSettingsRepository
    {
        private readonly DanaOzDbContext _context;

        public UserSettingsRepository(DanaOzDbContext context)
        {
            _context = context;
        }

        // Get a specific setting for a user
        public async Task<UserSetting?> GetAsync(int userId, string key)
        {
            return await _context.User_Settings
                .FirstOrDefaultAsync(s => s.UserId == userId && s.SettingsKey == key);
        }

        // Get all settings for a user
        public async Task<List<UserSetting>> GetAllByUserIdAsync(int userId)
        {
            return await _context.User_Settings
                .Where(s => s.UserId == userId)
                .ToListAsync();
        }

        // Set a setting (create or update)
        public async Task SetAsync(int userId, string key, string value)
        {
            var existing = await GetAsync(userId, key);
            if (existing != null)
            {
                existing.SettingsValue = value;
                existing.Last_Update_DateTime = DateTime.UtcNow;
            }
            else
            {
                _context.User_Settings.Add(new UserSetting
                {
                    UserId = userId,
                    SettingsKey = key,
                    SettingsValue = value,
                    Creation_DateTime = DateTime.UtcNow,
                    Last_Update_DateTime = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}