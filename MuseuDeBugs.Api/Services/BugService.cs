using MuseuDeBugs.Api.DTOs;
using MuseuDeBugs.Api.Entities;

namespace MuseuDeBugs.Api.Services
{
    public class BugService
    {
        public BugResponse CriarBug(CriarBugRequest request)
        {
            var bug = new Bug(
                request.Titulo, 
                request.Linguagem, 
                request.MensagemErro, 
                request.Descricao, 
                request.Causa, 
                request.Solucao);
        
        var response = new BugResponse
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
        return response;
        }
    }   
}