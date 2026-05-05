using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rest.DTOs;
using rest.Helpers;
using rest.Models;
using rest.Repositories;
using System.ComponentModel.DataAnnotations;


namespace rest.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesRepository _repository;
        private readonly IHtmlSanitizer _sanitizer;
        public MoviesController(IMoviesRepository repository, IHtmlSanitizer sanitizer)
        {
            _repository = repository;
            _sanitizer = sanitizer;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginatedResult<Movie>>> GetAll(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery]
            [StringLength(100)]
            [RegularExpression(@"^[^<>]*$", ErrorMessage = "Search contains invalid characters.")]
            string? search = null)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("Query parameters 'page' and 'pageSize' must be greater than 0.");
            PaginatedResult<Movie> result = await _repository.GetAllAsync(page, pageSize, search);
            if (result.TotalCount == 0)
                return NoContent();

            result._links.Add(new Link(
                href: Url.Action(nameof(GetAll), new { page, pageSize }) ?? string.Empty,
                rel: "self",
                method: "GET"
            ));

            result._links.Add(new Link(
                href: Url.Action(nameof(GetAll), new { page = result.TotalPages, pageSize, search }) ?? string.Empty,
                rel: "last",
                method: "GET"
            ));

            result._links.Add(new Link(
                href: Url.Action(nameof(GetAll), new { page = 1, pageSize, search }) ?? string.Empty,
                rel: "first",
                method: "GET"
            ));

            if (page > 1)
            {
                result._links.Add(new Link(
                    href: Url.Action(nameof(GetAll), new { page = page - 1, pageSize, search }) ?? string.Empty,
                    rel: "prev",
                    method: "GET"
                ));
            }
            if (page < result.TotalPages)
            {
                result._links.Add(new Link(
                    href: Url.Action(nameof(GetAll), new { page = page + 1, pageSize, search }) ?? string.Empty,
                    rel: "next",
                    method: "GET"
                ));
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest("ID must be a positive number.");
            var movie = await _repository.GetByIdAsync(id);
            if (movie == null)
                return NotFound();

            var result = new SingleResult<Movie>(movie);

            result._links.Add(new Link(
                href: Url.Action(nameof(GetById), new { id }) ?? string.Empty,
                rel: "self",
                method: "GET"
            ));

            result._links.Add(new Link(
                href: Url.Action(nameof(Put), new { id }) ?? string.Empty,
                rel: "update",
                method: "PUT"
            ));

            result._links.Add(new Link(
                href: Url.Action(nameof(Delete), new { id }) ?? string.Empty,
                rel: "delete",
                method: "DELETE"
            ));

            result._links.Add(new Link(
                href: Url.Action(nameof(GetAll)) ?? string.Empty,
                rel: "collection",
                method: "GET"
            ));

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] MovieDto dto)
        {
            var movie = new Movie
            {
                Title = _sanitizer.Sanitize(dto.Title),
                Genre = _sanitizer.Sanitize(dto.Genre),
                ReleaseYear = dto.ReleaseYear,
                DirectorID = dto.DirectorID,
                ProductionCompanyID = dto.ProductionCompanyID
            };

            var created = await _repository.AddAsync(movie);

            var result = new SingleResult<Movie>(created);

            result._links.Add(new Link(
                href: Url.Action(nameof(GetById), new { id = created.Id }) ?? string.Empty,
                rel: "self",
                method: "GET"
            ));
            result._links.Add(new Link(
                href: Url.Action(nameof(Put), new { id = created.Id }) ?? string.Empty,
                rel: "update",
                method: "PUT"
            ));
            result._links.Add(new Link(
                href: Url.Action(nameof(Delete), new { id = created.Id }) ?? string.Empty,
                rel: "delete",
                method: "DELETE"
            ));

            result._links.Add(new Link(
                href: Url.Action(nameof(GetAll)) ?? string.Empty,
                rel: "collection",
                method: "GET"
            ));

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SingleResult<Movie>>> Put(int id, [FromBody] MovieDto movie)
        {
            if (id <= 0) return BadRequest("ID must be a positive number.");
            movie.Title = _sanitizer.Sanitize(movie.Title);
            movie.Genre = _sanitizer.Sanitize(movie.Genre);

            Movie? updatedMovie = await _repository.UpdateAsync(id, movie);
            if (updatedMovie == null) return NotFound();

            SingleResult<Movie?> result = new(updatedMovie);

            result._links.Add(new Link(
                href: Url.Action(nameof(GetById), new { id }) ?? string.Empty,
                rel: "self",
                method: "GET"
            ));

            result._links.Add(new Link(
                href: Url.Action(nameof(Delete), new { id = updatedMovie.Id }) ?? string.Empty,
                rel: "delete",
                method: "DELETE"
            ));

            result._links.Add(new Link(
                href: Url.Action(nameof(GetAll)) ?? string.Empty,
                rel: "collection",
                method: "GET"
            ));

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest("ID must be a positive number.");

            var deleted = await _repository.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(); 
            }

            return NoContent();
        }
    }
}
