using Microsoft.EntityFrameworkCore;
using MuseuDeBugs.Api.Data;
using MuseuDeBugs.Api.DTOs;
using MuseuDeBugs.Api.Services;

namespace MuseuDeBugs.Tests.Services;

public class BugServiceTests
{
    private static DbContextOptions<AppDbContext> CriarOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    private static AppDbContext CriarContexto(DbContextOptions<AppDbContext> options)
    {
        return new AppDbContext(options);
    }

    private static CriarBugRequest CriarRequest(
        string titulo = "NullReferenceException ao acessar propriedade",
        string linguagem = "C#",
        string descricao = "Erro ao acessar uma propriedade de um objeto que estava null.")
    {
        return new CriarBugRequest
        {
            Titulo = titulo,
            Linguagem = linguagem,
            MensagemErro = "System.NullReferenceException",
            Descricao = descricao,
            Causa = "O objeto nao foi inicializado antes do uso.",
            Solucao = "Validar null antes de acessar a propriedade."
        };
    }

    [Fact]
    public void CriarBug_DeveSalvarBugComStatusAberto()
    {
        var options = CriarOptions();
        using var context = CriarContexto(options);
        var service = new BugService(context);

        var response = service.CriarBug(CriarRequest());

        Assert.NotEqual(0, response.Id);
        Assert.Equal("NullReferenceException ao acessar propriedade", response.Titulo);
        Assert.Equal("C#", response.Linguagem);
        Assert.Equal("Aberto", response.Status);
        Assert.Single(context.Bugs);
    }

    [Fact]
    public void BuscarPorId_DeveRetornarBug_QuandoIdExiste()
    {
        var options = CriarOptions();
        using var context = CriarContexto(options);
        var service = new BugService(context);
        var bugCriado = service.CriarBug(CriarRequest(titulo: "Bug encontrado"));

        var response = service.BuscarPorId(bugCriado.Id);

        Assert.NotNull(response);
        Assert.Equal(bugCriado.Id, response.Id);
        Assert.Equal("Bug encontrado", response.Titulo);
    }

    [Fact]
    public void BuscarPorId_DeveRetornarNull_QuandoIdNaoExiste()
    {
        var options = CriarOptions();
        using var context = CriarContexto(options);
        var service = new BugService(context);

        var response = service.BuscarPorId(999);

        Assert.Null(response);
    }

    [Fact]
    public void MarcarComoResolvido_DeveAlterarStatusEDataAtualizacao()
    {
        var options = CriarOptions();
        using var context = CriarContexto(options);
        var service = new BugService(context);
        var bugCriado = service.CriarBug(CriarRequest());

        var response = service.MarcarComoResolvido(bugCriado.Id);

        Assert.NotNull(response);
        Assert.Equal("Resolvido", response.Status);
        Assert.NotNull(response.DataAtualizacao);
    }

    [Fact]
    public void AtualizarBug_DeveAlterarDadosDoBug()
    {
        var options = CriarOptions();
        using var context = CriarContexto(options);
        var service = new BugService(context);
        var bugCriado = service.CriarBug(CriarRequest(titulo: "Titulo antigo"));

        var response = service.AtualizarBug(
            bugCriado.Id,
            new AtualizarBugRequest
            {
                Titulo = "Titulo novo",
                Linguagem = "SQL",
                MensagemErro = "Erro atualizado",
                Descricao = "Descricao nova com tamanho suficiente.",
                Causa = "Causa atualizada.",
                Solucao = "Solucao atualizada."
            });

        Assert.NotNull(response);
        Assert.Equal("Titulo novo", response.Titulo);
        Assert.Equal("SQL", response.Linguagem);
        Assert.Equal("Erro atualizado", response.MensagemErro);
        Assert.NotNull(response.DataAtualizacao);
    }

    [Fact]
    public void ListarBugs_DeveFiltrarPorStatusELinguagem()
    {
        var options = CriarOptions();
        using var context = CriarContexto(options);
        var service = new BugService(context);

        service.CriarBug(CriarRequest(titulo: "Bug C#", linguagem: "C#"));
        var bugSqlResolvido = service.CriarBug(CriarRequest(titulo: "Bug SQL resolvido", linguagem: "SQL"));
        service.CriarBug(CriarRequest(titulo: "Bug SQL aberto", linguagem: "SQL"));
        service.MarcarComoResolvido(bugSqlResolvido.Id);

        var bugsResolvidos = service.ListarBugs("Resolvido", null);
        var bugsSql = service.ListarBugs(null, "SQL");

        Assert.Single(bugsResolvidos);
        Assert.Equal("Resolvido", bugsResolvidos[0].Status);
        Assert.Equal(2, bugsSql.Count);
        Assert.All(bugsSql, bug => Assert.Equal("SQL", bug.Linguagem));
    }

    [Fact]
    public void DeletarBug_DeveRetornarFalse_QuandoIdNaoExiste()
    {
        var options = CriarOptions();
        using var context = CriarContexto(options);
        var service = new BugService(context);

        var deletou = service.DeletarBug(999);

        Assert.False(deletou);
    }

    [Fact]
    public void DeletarBug_DeveRemoverBug_QuandoIdExiste()
    {
        var options = CriarOptions();
        int bugId;

        using (var context = CriarContexto(options))
        {
            var service = new BugService(context);
            var bugCriado = service.CriarBug(CriarRequest());
            bugId = bugCriado.Id;

            var deletou = service.DeletarBug(bugId);

            Assert.True(deletou);
        }

        using var novoContext = CriarContexto(options);

        Assert.Null(novoContext.Bugs.Find(bugId));
    }
}
