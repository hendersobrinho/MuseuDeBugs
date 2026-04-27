using System.ComponentModel.DataAnnotations;

namespace MuseuDeBugs.Api.DTOs
{
    public class CriarBugRequest
    {  
        [Required]
        [MaxLength(120)]
        [MinLength(3)]
        public string Titulo { get; set; } = string.Empty; 
        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        public string Linguagem { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? MensagemErro { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(2000)]
        public string Descricao { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Causa { get; set; }

        [MaxLength(2000)]
        public string? Solucao { get; set; }
    }
}
