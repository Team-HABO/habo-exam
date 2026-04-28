using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using rest.Helpers;
using rest.Models;
using rest.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace rest.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DirectorsController : ControllerBase
    {
        private readonly IDirectorsRepository _repository;
        public DirectorsController(IDirectorsRepository repository)
        {
            _repository = repository;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Director>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            PaginatedResult<Director> result = await _repository.GetAllAsync(page, pageSize, search);
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
            var director = await _repository.GetByIdAsync(id);
            if (director == null)
                return NotFound();

            var result = new SingleResult<Director>(director);

            result._links.Add(new Link(
                href: Url.Action(nameof(GetById), new { id }) ?? string.Empty,
                rel: "self",
                method: "GET"
            ));

            result._links.Add(new Link(
                href: Url.Action(nameof(GetAll)) ?? string.Empty,
                rel: "collection",
                method: "GET"
            ));

            return Ok(result);
        }
    }
}
