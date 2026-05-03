using System.ComponentModel.DataAnnotations;
using MuseuDeBugs.Api.DTOs;

namespace MuseuDeBugs.Tests.DTOs;

public class BugRequestValidationTests
{
    [Theory]
    [InlineData(" ", "C#", "Descricao com tamanho suficiente.")]
    [InlineData("Titulo", " ", "Descricao com tamanho suficiente.")]
    [InlineData("Titulo", "C#", "          ")]
    public void CriarBugRequest_DeveRejeitarCamposObrigatoriosSoComEspacos(
        string titulo,
        string linguagem,
        string descricao)
    {
        var request = new CriarBugRequest
        {
            Titulo = titulo,
            Linguagem = linguagem,
            Descricao = descricao
        };

        Assert.False(Validar(request));
    }

    [Theory]
    [InlineData(" ", "C#", "Descricao com tamanho suficiente.")]
    [InlineData("Titulo", " ", "Descricao com tamanho suficiente.")]
    [InlineData("Titulo", "C#", "          ")]
    public void AtualizarBugRequest_DeveRejeitarCamposObrigatoriosSoComEspacos(
        string titulo,
        string linguagem,
        string descricao)
    {
        var request = new AtualizarBugRequest
        {
            Titulo = titulo,
            Linguagem = linguagem,
            Descricao = descricao
        };

        Assert.False(Validar(request));
    }

    [Fact]
    public void CriarBugRequest_DeveAceitarDescricaoMultilinhaComTexto()
    {
        var request = new CriarBugRequest
        {
            Titulo = "Titulo",
            Linguagem = "C#",
            Descricao = "Primeira linha\nsegunda linha com contexto."
        };

        Assert.True(Validar(request));
    }

    [Fact]
    public void AtualizarBugRequest_DeveAceitarDescricaoMultilinhaComTexto()
    {
        var request = new AtualizarBugRequest
        {
            Titulo = "Titulo",
            Linguagem = "C#",
            Descricao = "Primeira linha\nsegunda linha com contexto."
        };

        Assert.True(Validar(request));
    }

    private static bool Validar(object request)
    {
        var context = new ValidationContext(request);
        var results = new List<ValidationResult>();

        return Validator.TryValidateObject(request, context, results, validateAllProperties: true);
    }
}
