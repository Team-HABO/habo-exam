using Microsoft.EntityFrameworkCore;
using rest.Data;
using rest.DTOs;
using rest.Helpers;
using rest.Models;

namespace rest.Repositories
{
    public class MoviesRepository : IMoviesRepository
    {
        private readonly AppDbContext _context;
        public MoviesRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Movie?> GetByIdAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            return movie;
        }
        public async Task<PaginatedResult<Movie>> GetAllAsync(int page, int pageSize, string? search = null)
        {
            var query = _context.Movies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(m =>
                    m.Title.ToLower().Contains(searchLower) ||
                    m.Genre.ToLower().Contains(searchLower) ||
                    m.ReleaseYear.ToLower().Contains(searchLower));
            }

            var totalCount = await query.CountAsync();

            var movies = await query
                .Include(m => m.Director)            
                .Include(m => m.ProductionCompany)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Movie>
            {
                Data = movies,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
        public async Task<Movie> AddAsync(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            return movie;
        }

        public async Task<Movie?> UpdateAsync(int id, MovieDto movie)
        {
            var existingMovie = await _context.Movies.FindAsync(id);

            if (existingMovie == null)
            {
                return null;
            }

            existingMovie.Title = movie.Title;
            existingMovie.ReleaseYear = movie.ReleaseYear;
            existingMovie.Genre = movie.Genre;
            existingMovie.DirectorID = movie.DirectorID;
            existingMovie.ProductionCompanyID = movie.ProductionCompanyID;
            await _context.SaveChangesAsync();
            return existingMovie;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return false;
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
