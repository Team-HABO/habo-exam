using Microsoft.EntityFrameworkCore;
using rest.Data;
using rest.DTOs;
using rest.Helpers;
using rest.Models;

namespace rest.Repositories
{
    public class ProductionCompaniesRepository : IProductionCompaniesRepository
    {
        private readonly AppDbContext _context;
        public ProductionCompaniesRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ProductionCompany?> GetByIdAsync(int id)
        {
            var productionCompany = await _context.ProductionCompanies.FindAsync(id);
            return productionCompany;
        }
        public async Task<PaginatedResult<ProductionCompanyHateoasDto>> GetAllAsync(int page, int pageSize, string? search = null)
        {
            var query = _context.ProductionCompanies.AsQueryable();
            if (pageSize > 50) pageSize = 50;

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(pc =>
                    pc.Name.ToLower().Contains(searchLower));
            }

            var totalCount = await query.CountAsync();

            var productionCompanies = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(pc => new ProductionCompanyHateoasDto(pc))
                .ToListAsync();

            var result = new PaginatedResult<ProductionCompanyHateoasDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
            result.Embedded["productionCompanies"] = productionCompanies;
            return result;
        }
    }
}
