using rest.DTOs;
using rest.Helpers;
using rest.Models;

namespace rest.Repositories
{
    public interface IDirectorsRepository
    {
        Task<PaginatedResult<DirectorHateoasDto>> GetAllAsync(int page, int pageSize, string? search = null);
        Task<Director?> GetByIdAsync(int id);
    }
}