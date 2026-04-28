using rest.Data;
using rest.Helpers;
using Microsoft.EntityFrameworkCore;
using rest.Models;

namespace rest.Repositories
{
    public class DirectorsRepository : IDirectorsRepository
    {
        private readonly AppDbContext _context;
        public DirectorsRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Director?> GetByIdAsync(int id)
        {
            var director = await _context.Directors.FindAsync(id);
            return director;
        }
        public async Task<PaginatedResult<Director>> GetAllAsync(int page, int pageSize, string? search = null)
        {
            var query = _context.Directors.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(d =>
                    d.FirstName.ToLower().Contains(searchLower) ||
                    d.LastName.ToLower().Contains(searchLower));
            }

            var totalCount = await query.CountAsync();

            var directors = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Director>
            {
                Data = directors,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}
