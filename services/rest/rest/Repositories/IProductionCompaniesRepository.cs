using rest.DTOs;
using rest.Helpers;
using rest.Models;

namespace rest.Repositories
{
    public interface IProductionCompaniesRepository
    {
        Task<PaginatedResult<ProductionCompanyHateoasDto>> GetAllAsync(int page, int pageSize, string? search = null);
        Task<ProductionCompany?> GetByIdAsync(int id);
    }
}