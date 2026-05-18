using DanaOz.DAL.Context;
using DanaOz.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DanaOz.DAL.Repositories
{
    public class ChatLogRepository
    {
        private readonly DanaOzDbContext _context;

        public ChatLogRepository(DanaOzDbContext context)
        {
            _context = context;
        }

        // Log an incoming message
        public async Task<ChatLog> LogInboundAsync(string phoneNumber, int? userId, string message)
        {
            var log = new ChatLog
            {
                PhoneNumber = phoneNumber,
                UserId = userId,
                Direction = "Inbound",
                MessageText = message,
                Creation_DateTime = DateTime.UtcNow,
                Last_Update_DateTime = DateTime.UtcNow
            };

            _context.Chat_Logs.Add(log);
            await _context.SaveChangesAsync();
            return log;
        }

        // Log an outgoing message
        public async Task<ChatLog> LogOutboundAsync(string phoneNumber, int? userId, string message)
        {
            var log = new ChatLog
            {
                PhoneNumber = phoneNumber,
                UserId = userId,
                Direction = "Outbound",
                MessageText = message,
                Creation_DateTime = DateTime.UtcNow,
                Last_Update_DateTime = DateTime.UtcNow
            };

            _context.Chat_Logs.Add(log);
            await _context.SaveChangesAsync();
            return log;
        }

        // Log a proactive push message
        public async Task<ChatLog> LogPushAsync(string phoneNumber, int? userId, string message)
        {
            var log = new ChatLog
            {
                PhoneNumber = phoneNumber,
                UserId = userId,
                Direction = "Push",
                MessageText = message,
                Creation_DateTime = DateTime.UtcNow,
                Last_Update_DateTime = DateTime.UtcNow
            };

            _context.Chat_Logs.Add(log);
            await _context.SaveChangesAsync();
            return log;
        }

        // Get last N messages for a user (for AI conversation context)
        public async Task<List<ChatLog>> GetRecentByUserIdAsync(int userId, int count = 10)
        {
            return await _context.Chat_Logs
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.Creation_DateTime)
                .Take(count)
                .ToListAsync();
        }
    }
}