using rest.Helpers;
using rest.Models;

namespace rest.Repositories
{
    public interface IDirectorsRepository
    {
        Task<PaginatedResult<Director>> GetAllAsync(int page, int pageSize, string? search = null);
        Task<Director?> GetByIdAsync(int id);
    }
}