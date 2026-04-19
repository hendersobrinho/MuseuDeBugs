using Microsoft.AspNetCore.Mvc;
using MuseuDeBugs.Api.Services;
using MuseuDeBugs.Api.DTOs;

namespace MuseuDeBugs.Api.Controllers
{
    [ApiController] // avisa ao ASP.NET Core que esta classe é um controller de API
    [Route("api/[controller]")] // define a rota base como api/bugs
    public class BugsController : ControllerBase // herda recursos base para controllers de API
    {
        private readonly BugService _bugService; // campo que guarda a referência do service

        public BugsController(BugService bugService) // o ASP.NET Core injeta uma instância de BugService no construtor
        {
            _bugService = bugService; // armazena o service recebido no campo da classe para uso posterior
        }

        [HttpPost] // este método responde a requisições HTTP POST
        // ActionResult = representa uma resposta HTTP
        // <BugResponse> = informa que o conteúdo esperado da resposta, quando houver corpo, é do tipo BugResponse
        public ActionResult<BugResponse> CriarBug(CriarBugRequest request) // recebe os dados enviados na requisição
        {
            var response = _bugService.CriarBug(request); // chama o service, recebe o BugResponse retornado e guarda na variável local response
            return Ok(response); // devolve HTTP 200 com o objeto response no corpo da resposta
        }
    }
}