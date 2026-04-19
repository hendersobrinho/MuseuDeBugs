namespace MuseuDeBugs.Api.DTOs
{
    public class BugResponse
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Linguagem { get; set; } = string.Empty;
        public string? MensagemErro { get; set; }
        public string Descricao { get; set; } = string.Empty;   
        public string? Causa { get; set; }
        public string? Solucao { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }      
    }
}