namespace MuseuDeBugs.Api.DTOs
{
    public class CriarBugRequest
    {   
        public string Titulo { get; set; } = string.Empty; 
        public string Linguagem { get; set; } = string.Empty;
        public string? MensagemErro { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string? Causa { get; set; }
        public string? Solucao {get; set; }
    }
} 