using Microsoft.AspNetCore.Mvc;
using MuseuDeBugs.Api.Services;
using MuseuDeBugs.Api.DTOs;

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

        [HttpPost]
        public ActionResult<BugResponse> CriarBug(CriarBugRequest request)
        {
            var response = _bugService.CriarBug(request);
            
            return CreatedAtAction(
                nameof(BuscarId),
                new {id = response.Id},
                response
            );
        }
        [HttpGet]
        public ActionResult<List<BugResponse>> ListarBugs([FromQuery] string? status, [FromQuery] string linguagem)
        {
            var response = _bugService.ListarBugs(status, linguagem);

            return Ok(response);
        }
        [HttpGet("{id}")]
        public ActionResult<BugResponse> BuscarId(int id)
        {
            var response = _bugService.BuscarPorId(id);

            if (response == null){return NotFound();}

            return Ok(response);
        }
        [HttpPatch("{id}/resolver")]
        public ActionResult<BugResponse> ResolverBug(int id)
        {
            var response = _bugService.MarcarComoResolvido(id);

            if(response == null){return NotFound();}

            return Ok(response);
        }
    }
}
