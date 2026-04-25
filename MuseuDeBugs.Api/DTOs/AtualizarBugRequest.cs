using System.ComponentModel.DataAnnotations;

namespace MuseuDeBugs.Api.DTOs
{
    public class AtualizarBugRequest
    {
        [Required]
        [MaxLength(120)]
        [MinLength(3)]
        public string Titulo { get; set; } = string.Empty;
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Linguagem { get; set; } = string.Empty;
        public string? MensagemErro { get; set; }
        [MinLength(10)]
        public string Descricao { get; set; } = string.Empty;
        public string? Causa { get; set; }
        public string? Solucao { get; set; }
    }
}