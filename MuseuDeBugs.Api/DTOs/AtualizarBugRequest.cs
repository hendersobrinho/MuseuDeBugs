using System.ComponentModel.DataAnnotations;

namespace MuseuDeBugs.Api.DTOs
{
    public class AtualizarBugRequest
    {
        [Required(ErrorMessage = "Titulo deve ser preenchido.")]
        [MaxLength(120)]
        [MinLength(1)]
        public string Titulo { get; set; } = string.Empty;
        [Required(ErrorMessage = "Linguagem deve ser preenchida.")]
        [MinLength(1)]
        [MaxLength(50)]
        public string Linguagem { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? MensagemErro { get; set; }

        [Required(ErrorMessage = "Descricao deve ser preenchida.")]
        [MinLength(10)]
        [MaxLength(8000)]
        public string Descricao { get; set; } = string.Empty;

        [MaxLength(8000)]
        public string? Causa { get; set; }

        [MaxLength(8000)]
        public string? Solucao { get; set; }
    }
}
