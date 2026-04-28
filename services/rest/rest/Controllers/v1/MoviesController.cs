using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using rest.DTOs;
using rest.Helpers;
using rest.Models;
using rest.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace rest.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesRepository _repository;
        public MoviesController(IMoviesRepository repository)
        {
            _repository = repository;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Movie>>> GetAll(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            PaginatedResult<Movie> result = await _repository.GetAllAsync(page, pageSize, search);
            if (result.TotalCount == 0)
                return NoContent();

            result._links.Add(new Link(
                href: Url.Action(nameof(GetAll), new { page, pageSize }) ?? string.Empty,
                rel: "self",
                method: "GET"
            ));

            result._links.Add(new Link(
                href: Url.Action(nameof(GetAll), new { page = 1, pageSize }) ?? string.Empty,
                rel: "first",
                method: "GET"
            ));

            result._links.Add(new Link(
                href: Url.Action(nameof(GetAll), new { page = result.TotalPages, pageSize }) ?? string.Empty,
                rel: "last",
                method: "GET"
            ));
            if (page > 1)
                result._links.Add(new Link(
                    href: Url.Action(nameof(GetAll), new { page = page - 1, pageSize }) ?? string.Empty,
                    rel: "prev",
                    method: "GET"
                ));

            if (page < result.TotalPages)
                result._links.Add(new Link(
                    href: Url.Action(nameof(GetAll), new { page = page + 1, pageSize }) ?? string.Empty,
                    rel: "next",
                    method: "GET"
                ));
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
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
                href: Url.Action(nameof(Update), new { id }) ?? string.Empty,
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
                Title = dto.Title,
                ReleaseYear = dto.ReleaseYear,
                Genre = dto.Genre,
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
                href: Url.Action(nameof(Update), new { id = created.Id }) ?? string.Empty,
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
        public async Task<ActionResult<Movie>> Put(int id, [FromBody] MovieDto movie)
        {

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

            return Ok(updatedMovie);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _repository.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(); 
            }

            return NoContent();
        }
    }
}
