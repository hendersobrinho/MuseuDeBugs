using MuseuDeBugs.Api.DTOs;
using MuseuDeBugs.Api.Entities;
using MuseuDeBugs.Api.Data;
namespace MuseuDeBugs.Api.Services
{
    public class BugService
    {   
        private readonly AppDbContext _context;
        public BugService(AppDbContext context)
        {
            _context = context;
        }
        public BugResponse CriarBug(CriarBugRequest request)
        {
            var bug = new Bug(
                request.Titulo, 
                request.Linguagem, 
                request.MensagemErro, 
                request.Descricao, 
                request.Causa, 
                request.Solucao);
        
            _context.Bugs.Add(bug);
            _context.SaveChanges();

            return MapearParaResponse(bug);
        }
        
        private BugResponse MapearParaResponse(Bug bug)
        {
            return new BugResponse
            {
                Id = bug.Id,
                Titulo = bug.Titulo,
                Linguagem = bug.Linguagem,
                MensagemErro = bug.MensagemErro,
                Descricao = bug.Descricao,
                Causa = bug.Causa,
                Solucao = bug.Solucao,
                Status = bug.Status.ToString(),
                DataCriacao = bug.DataCriacao,
                DataAtualizacao = bug.DataAtualizacao
            };
        }
        public List<BugResponse> ListarBugs()
        {
            var bugs = _context.Bugs.ToList();

            return bugs
                .Select(bug => MapearParaResponse(bug))
                .ToList();
        }
    
        public BugResponse? BuscarPorId(int id)
        {
            var bug = _context.Bugs.Find(id);

            if (bug == null){return null;}

            return MapearParaResponse(bug);
        }
        public BugResponse? MarcarComoResolvido(int id)
        {
            var bug = _context.Bugs.Find(id);

            if (bug == null){return null;}

            bug.MarcarComoResolvido();
            _context.SaveChanges();

            return MapearParaResponse(bug);
        }
    }
}
