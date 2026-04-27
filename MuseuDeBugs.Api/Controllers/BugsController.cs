using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseuDeBugs.Api.DTOs;
using MuseuDeBugs.Api.Enums;
using MuseuDeBugs.Api.Services;

namespace MuseuDeBugs.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BugsController : ControllerBase
    {
        private readonly BugService _bugService;

        public BugsController(BugService bugService)
        {
            _bugService = bugService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<BugResponse> CriarBug(CriarBugRequest request)
        {
            var response = _bugService.CriarBug(request);

            return CreatedAtAction(
                nameof(BuscarId),
                new { id = response.Id },
                response
            );
        }

        [HttpGet]
        public ActionResult<List<BugResponse>> ListarBugs([FromQuery] string? status, [FromQuery] string? linguagem)
        {
            if (!string.IsNullOrWhiteSpace(status) &&
                !Enum.TryParse<StatusBug>(status, true, out _))
            {
                return BadRequest("Status invalido. Use Aberto ou Resolvido.");
            }

            var response = _bugService.ListarBugs(status, linguagem);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public ActionResult<BugResponse> BuscarId(int id)
        {
            var response = _bugService.BuscarPorId(id);

            if (response == null) { return NotFound(); }

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/resolver")]
        public ActionResult<BugResponse> ResolverBug(int id)
        {
            var response = _bugService.MarcarComoResolvido(id);

            if (response == null) { return NotFound(); }

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public ActionResult<BugResponse> AtualizarBug(int id, AtualizarBugRequest request)
        {
            var response = _bugService.AtualizarBug(id, request);

            if (response == null) { return NotFound(); }

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeletarBug(int id)
        {
            var deletou = _bugService.DeletarBug(id);

            if (!deletou) { return NotFound(); }

            return NoContent();
        }
    }
}
