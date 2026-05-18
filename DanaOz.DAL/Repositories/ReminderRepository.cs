using DanaOz.DAL.Context;
using DanaOz.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DanaOz.DAL.Repositories
{
    public class ReminderRepository
    {
        private readonly DanaOzDbContext _context;

        public ReminderRepository(DanaOzDbContext context)
        {
            _context = context;
        }

        // Create reminder settings
        public async Task<UserReminderSettings> CreateSettingsAsync(UserReminderSettings settings)
        {
            settings.Creation_DateTime = DateTime.UtcNow;
            settings.Last_Update_DateTime = DateTime.UtcNow;
            _context.User_ReminderSettings.Add(settings);
            await _context.SaveChangesAsync();
            return settings;
        }

        // Get all reminder settings for a user
        public async Task<List<UserReminderSettings>> GetSettingsByUserIdAsync(int userId)
        {
            return await _context.User_ReminderSettings
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        // Log a sent reminder
        public async Task<UserReminder> LogSentReminderAsync(int reminderSettingsId)
        {
            var reminder = new UserReminder
            {
                ReminderSettingsId = reminderSettingsId,
                DatetimeSent = DateTime.UtcNow,
                Creation_DateTime = DateTime.UtcNow,
                Last_Update_DateTime = DateTime.UtcNow
            };

            _context.User_Reminders.Add(reminder);
            await _context.SaveChangesAsync();
            return reminder;
        }

        // Get all reminders due to be sent now
        public async Task<List<UserReminderSettings>> GetDueRemindersAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.User_ReminderSettings
                .Include(r => r.User)
                .Where(r => r.IsRecurring == true)
                .ToListAsync();
        }
    }
}