using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rest.DTOs;
using rest.Helpers;
using rest.Repositories;
using System.ComponentModel.DataAnnotations;

namespace rest.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductionCompaniesController : ControllerBase
    {
        private readonly IProductionCompaniesRepository _repository;
        public ProductionCompaniesController(IProductionCompaniesRepository repository)
        {
            _repository = repository;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginatedResult<ProductionCompanyHateoasDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery]
            [StringLength(100)]
            string? search = null)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("Query parameters 'page' and 'pageSize' must be greater than 0.");
            PaginatedResult<ProductionCompanyHateoasDto> result = await _repository.GetAllAsync(page, pageSize, search);
            if (result.TotalCount == 0)
                return NoContent();
            foreach (var director in result.Embedded["productionCompanies"])
            {
                director.Links.Add(new Link(
                    href: Url.Action(nameof(GetById), new { id = director.Id }) ?? string.Empty,
                    rel: "self",
                    method: "GET"
                ));
            }

            result.Links.Add(new Link(
                href: Url.Action(nameof(GetAll), new { page, result.PageSize, search }) ?? string.Empty,
                rel: "self",
                method: "GET"
            ));

            result.Links.Add(new Link(
                href: Url.Action(nameof(GetAll), new { page = 1, result.PageSize, search }) ?? string.Empty,
                rel: "first",
                method: "GET"
            ));

            result.Links.Add(new Link(
                href: Url.Action(nameof(GetAll), new { page = result.TotalPages, result.PageSize, search }) ?? string.Empty,
                rel: "last",
                method: "GET"
            ));
            if (page > 1)
                result.Links.Add(new Link(
                    href: Url.Action(nameof(GetAll), new { page = page - 1, result.PageSize, search }) ?? string.Empty,
                    rel: "prev",
                    method: "GET"
                ));

            if (page < result.TotalPages)
                result.Links.Add(new Link(
                    href: Url.Action(nameof(GetAll), new { page = page + 1, result.PageSize, search }) ?? string.Empty,
                    rel: "next",
                    method: "GET"
                ));
            result.Links.Add(new Link(
                href: $"/api/v1/ProductCompanies?search={{search}}",
                rel: "search",
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
            if (id <= 0) return BadRequest("ID must be a positive number.");

            var productionCompany = await _repository.GetByIdAsync(id);
            if (productionCompany == null)
                return NotFound();

            var result = new ProductionCompanyHateoasDto(productionCompany);

            result.Links.Add(new Link(
                href: Url.Action(nameof(GetById), new { id }) ?? string.Empty,
                rel: "self",
                method: "GET"
            ));

            result.Links.Add(new Link(
                href: Url.Action(nameof(GetAll)) ?? string.Empty,
                rel: "collection",
                method: "GET"
            ));

            return Ok(result);
        }
    }
}
