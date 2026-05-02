using System.ComponentModel.DataAnnotations;

namespace MuseuDeBugs.Api.DTOs
{
    public class CriarBugRequest
    {  
        [Required]
        [MaxLength(120)]
        [MinLength(1)]
        [RegularExpression(@".*\S.*", ErrorMessage = "Titulo deve ser preenchido.")]
        public string Titulo { get; set; } = string.Empty; 
        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        [RegularExpression(@".*\S.*", ErrorMessage = "Linguagem deve ser preenchida.")]
        public string Linguagem { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? MensagemErro { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(2000)]
        [RegularExpression(@".*\S.*", ErrorMessage = "Descricao deve ser preenchida.")]
        public string Descricao { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Causa { get; set; }

        [MaxLength(2000)]
        public string? Solucao { get; set; }
    }
}
