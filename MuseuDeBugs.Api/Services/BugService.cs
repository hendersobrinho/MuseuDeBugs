using MuseuDeBugs.Api.DTOs;
using MuseuDeBugs.Api.Entities;
using MuseuDeBugs.Api.Data;
using MuseuDeBugs.Api.Enums;
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
                request.Titulo.Trim(),
                NormalizarLinguagem(request.Linguagem),
                NormalizarTextoOpcional(request.MensagemErro),
                request.Descricao.Trim(),
                NormalizarTextoOpcional(request.Causa),
                NormalizarTextoOpcional(request.Solucao));
        
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

        public List<BugResponse> ListarBugs(string? status, string? linguagem)
        {
            var query = _context.Bugs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (Enum.TryParse<StatusBug>(status, true, out var statusConvertido))
                {
                    query = query.Where(bug => bug.Status == statusConvertido);
                }
            }
            if (!string.IsNullOrWhiteSpace(linguagem))
            {
                var linguagemNormalizada = NormalizarLinguagem(linguagem).ToLower();
                query = query.Where(bug => bug.Linguagem.ToLower() == linguagemNormalizada);
            }

            var bugs = query.ToList();
            return bugs.Select(MapearParaResponse).ToList();
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

        public BugResponse? AtualizarBug(int id, AtualizarBugRequest request)
        {
            var bug = _context.Bugs.Find(id);

            if (bug == null) { return null; }

            bug.Atualizar(
                request.Titulo.Trim(),
                NormalizarLinguagem(request.Linguagem),
                NormalizarTextoOpcional(request.MensagemErro),
                request.Descricao.Trim(),
                NormalizarTextoOpcional(request.Causa),
                NormalizarTextoOpcional(request.Solucao));

            _context.SaveChanges();

            return MapearParaResponse(bug);
        }

        public bool DeletarBug(int id)
        {
            var bug = _context.Bugs.Find(id);

            if (bug == null) { return false; }

            _context.Bugs.Remove(bug);
            _context.SaveChanges();

            return true;
        }

        private static string NormalizarLinguagem(string linguagem)
        {
            var normalizada = linguagem.Trim();

            return normalizada.ToLowerInvariant() switch
            {
                "c#" or "csharp" or "c-sharp" => "C#",
                "c++" or "cpp" => "C++",
                "js" or "javascript" => "JavaScript",
                "ts" or "typescript" => "TypeScript",
                _ => normalizada
            };
        }

        private static string? NormalizarTextoOpcional(string? texto)
        {
            var normalizado = texto?.Trim() ?? string.Empty;
            return normalizado.Length > 0 ? normalizado : null;
        }
    }
}
