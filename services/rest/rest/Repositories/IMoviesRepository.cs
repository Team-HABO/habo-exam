using rest.DTOs;
using rest.Helpers;
using rest.Models;

namespace rest.Repositories
{
    public interface IMoviesRepository
    {
        Task<Movie> AddAsync(Movie movie);
        Task<bool> DeleteAsync(int id);
        Task<PaginatedResult<Movie>> GetAllAsync(int page, int pageSize, string? search = null);
        Task<Movie?> GetByIdAsync(int id);
        Task<Movie?> UpdateAsync(int id, MovieDto movie);
    }
}