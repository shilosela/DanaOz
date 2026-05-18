using DanaOz.DAL.Context;
using DanaOz.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DanaOz.DAL.Repositories
{
    public class UserRepository
    {
        private readonly DanaOzDbContext _context;

        public UserRepository(DanaOzDbContext context)
        {
            _context = context;
        }

        // Get user by phone number
        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Users
                .Include(u => u.Classes)
                .Include(u => u.Settings)
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        // Get user by ID
        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Classes)
                .Include(u => u.Settings)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        // Check if user exists
        public async Task<bool> ExistsAsync(string phoneNumber)
        {
            return await _context.Users
                .AnyAsync(u => u.PhoneNumber == phoneNumber);
        }

        // Create new user
        public async Task<User> CreateAsync(string phoneNumber)
        {
            var user = new User
            {
                PhoneNumber = phoneNumber,
                DanaPoints = 50,
                OnboardingStep = 0,
                IsOnboarded = false,
                FirstUse_DateTime = DateTime.UtcNow,
                LastLogin_DateTime = DateTime.UtcNow,
                Creation_DateTime = DateTime.UtcNow,
                Last_Update_DateTime = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // Update user
        public async Task UpdateAsync(User user)
        {
            user.Last_Update_DateTime = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // Update onboarding step
        public async Task UpdateOnboardingStepAsync(int userId, int step)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.OnboardingStep = step;
                user.Last_Update_DateTime = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        // Add Dana Points
        public async Task AddPointsAsync(int userId, int points)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.DanaPoints += points;
                user.Last_Update_DateTime = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        // Update last login
        public async Task UpdateLastLoginAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.LastLogin_DateTime = DateTime.UtcNow;
                user.Last_Update_DateTime = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}