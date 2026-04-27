using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MuseuDeBugs.Api.Security
{
    /// <summary>
    /// Atributo de seguranca usado para proteger rotas que so o administrador pode usar.
    ///
    /// Pense nessa classe como um porteiro da API:
    /// antes de deixar alguem entrar em uma rota marcada com [AdminOnly],
    /// ela olha se a pessoa trouxe uma chave chamada "X-Admin-Key" no header da requisicao.
    ///
    /// Explicando a sintaxe por partes:
    ///
    /// - "public class AdminOnlyAttribute" cria uma classe publica chamada AdminOnlyAttribute.
    ///   Em C#, quando uma classe termina com "Attribute", podemos usar ela nos controllers
    ///   escrevendo apenas [AdminOnly], sem precisar escrever [AdminOnlyAttribute].
    ///
    /// - ": Attribute" significa que essa classe e um atributo do C#.
    ///   Um atributo e uma etiqueta que colocamos em cima de classes ou metodos para dar
    ///   uma instrucao especial ao framework.
    ///
    /// - "IAuthorizationFilter" e uma interface do ASP.NET Core.
    ///   Interface e como um contrato: ela diz que essa classe precisa ter o metodo
    ///   OnAuthorization. O ASP.NET Core chama esse metodo antes de executar a rota.
    ///
    /// - "OnAuthorization(AuthorizationFilterContext context)" e o metodo que roda antes
    ///   do controller. O parametro "context" carrega informacoes da requisicao atual,
    ///   como headers, servicos registrados e a resposta que podemos devolver.
    ///
    /// - "IConfiguration" e o servico que sabe ler configuracoes da aplicacao,
    ///   como appsettings.Development.json e variaveis de ambiente.
    ///
    /// - "configuration["Admin:ApiKey"]" le a chave correta configurada no backend.
    ///   O ":" significa "entre dentro de Admin e pegue ApiKey".
    ///
    /// - "Request.Headers["X-Admin-Key"]" pega a chave que veio na requisicao.
    ///   Header e como um bilhete extra enviado junto com a chamada HTTP.
    ///
    /// - "UnauthorizedObjectResult" devolve HTTP 401 Unauthorized.
    ///   Isso quer dizer: "voce nao tem permissao para passar por aqui".
    ///
    /// Traducao dos metodos e propriedades usados nesta classe:
    ///
    /// - "context.HttpContext" significa "a requisicao HTTP que esta acontecendo agora".
    ///   E como olhar para a chamada atual que chegou na API.
    ///
    /// - "RequestServices" e a caixa de ferramentas da aplicacao para esta requisicao.
    ///   Dentro dela ficam servicos registrados no Program.cs e pelo proprio ASP.NET Core.
    ///
    /// - "GetRequiredService&lt;IConfiguration&gt;()" significa:
    ///   "pegue obrigatoriamente o servico de configuracao".
    ///   Se esse servico nao existir, o ASP.NET Core gera erro, porque ele e necessario.
    ///
    /// - "configuration["Admin:ApiKey"]" significa:
    ///   "va nas configuracoes e procure o valor que fica em Admin -> ApiKey".
    ///   No JSON isso parece uma caixinha dentro de outra caixinha.
    ///
    /// - "Request.Headers" e a lista de headers enviados na requisicao.
    ///   Header e como um bilhete extra que viaja junto com o pedido HTTP.
    ///
    /// - "Headers["X-Admin-Key"]" procura um header especifico chamado X-Admin-Key.
    ///   Esse e o nome que escolhemos para carregar a chave secreta do administrador.
    ///
    /// - "FirstOrDefault()" pega o primeiro valor encontrado ou devolve null se nao existir.
    ///   Usamos isso porque um header pode tecnicamente ter mais de um valor.
    ///
    /// - "string.IsNullOrWhiteSpace(valor)" pergunta:
    ///   "esse texto e nulo, vazio ou feito so de espacos?"
    ///
    /// - "context.Result = ..." define a resposta final da requisicao.
    ///   Quando fazemos isso dentro de um filtro, a rota do controller nao continua.
    ///
    /// - "return" significa "pare de executar este metodo agora".
    ///   Aqui ele impede que o codigo continue depois que uma resposta de erro ja foi definida.
    ///
    /// - "providedApiKey != expectedApiKey" pergunta:
    ///   "a chave enviada e diferente da chave esperada?"
    ///   Se for diferente, a pessoa nao pode passar.
    ///
    /// Exemplo de uso no controller:
    ///
    /// [AdminOnly]
    /// [HttpPost]
    /// public ActionResult&lt;BugResponse&gt; CriarBug(CriarBugRequest request)
    /// {
    ///     ...
    /// }
    ///
    /// Assim, GET pode continuar publico, mas POST, PUT, PATCH e DELETE podem exigir a chave.
    /// </summary>
    public class AdminOnlyAttribute : Attribute, IAuthorizationFilter
    {
        /// <summary>
        /// Metodo chamado automaticamente pelo ASP.NET Core antes da rota protegida executar.
        ///
        /// Se a chave do header "X-Admin-Key" for igual a chave configurada em "Admin:ApiKey",
        /// a requisicao continua normalmente.
        ///
        /// Se a chave estiver vazia, errada ou se o backend nao tiver chave configurada,
        /// a requisicao para aqui e recebe 401 Unauthorized.
        /// </summary>
        /// <param name="context">
        /// Objeto com informacoes da requisicao atual e acesso aos servicos da aplicacao.
        /// </param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            var expectedApiKey = configuration["Admin:ApiKey"];
            var providedApiKey = context.HttpContext.Request.Headers["X-Admin-Key"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(expectedApiKey))
            {
                context.Result = new UnauthorizedObjectResult("Chave de admin nao configurada.");
                return;
            }

            if (providedApiKey != expectedApiKey)
            {
                context.Result = new UnauthorizedObjectResult("Chave de admin invalida.");
                return;
            }
        }
    }
}
