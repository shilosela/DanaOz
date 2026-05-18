using DanaOz.DAL.Context;
using DanaOz.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DanaOz.DAL.Repositories
{
    public class UserMaterialRepository
    {
        private readonly DanaOzDbContext _context;

        public UserMaterialRepository(DanaOzDbContext context)
        {
            _context = context;
        }

        // Save a new material
        public async Task<UserMaterial> CreateAsync(UserMaterial material)
        {
            material.Creation_DateTime = DateTime.UtcNow;
            material.Last_Update_DateTime = DateTime.UtcNow;
            _context.User_Materials.Add(material);
            await _context.SaveChangesAsync();
            return material;
        }

        // Get all materials for a user
        public async Task<List<UserMaterial>> GetByUserIdAsync(int userId)
        {
            return await _context.User_Materials
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.Creation_DateTime)
                .ToListAsync();
        }

        // Get materials for a specific class
        public async Task<List<UserMaterial>> GetByClassIdAsync(int classId)
        {
            return await _context.User_Materials
                .Where(m => m.ClassId == classId)
                .OrderByDescending(m => m.Creation_DateTime)
                .ToListAsync();
        }

        // Search materials by title keyword
        public async Task<List<UserMaterial>> SearchByTitleAsync(int userId, string keyword)
        {
            return await _context.User_Materials
                .Where(m => m.UserId == userId &&
                       m.Title != null &&
                       m.Title.ToLower().Contains(keyword.ToLower()))
                .ToListAsync();
        }
    }
}