# MuseuDeBugs - explicacao das mudancas feitas pelo Codex

Esta nota existe porque eu mexi em varias partes do projeto rapido demais e sem explicar antes. O objetivo aqui e deixar o mapa completo: o que entrou, por que entrou, como cada sintaxe funciona e quais partes sao opcionais.

Nada aqui foi commitado automaticamente. Tudo esta no working tree.

## Visao geral

As mudancas cairam em quatro grupos:

1. Validacao e contrato entre frontend e backend.
2. Assets do besouro, favicon e atalho local.
3. Preparacao de deploy Vercel + Render + Supabase.
4. Ajustes de documentacao, testes e ignores.

O ponto mais importante:

```text
Frontend ajuda o usuario.
Backend protege o sistema.
Banco guarda so o que passou pelas regras.
```

Por isso existe validacao nos dois lados. O frontend melhora a experiencia. O backend e a barreira real contra request manual, DevTools, curl, Postman etc.

## Arquivos de deploy que eu adicionei sem explicar antes

Estes arquivos sao de infraestrutura. Eles nao fazem parte da regra principal de bugs.

```text
Dockerfile
.dockerignore
render.yaml
frontend/vercel.json
frontend/public/app-config.js
```

Se voce quiser simplificar antes de estudar deploy, estes sao os principais candidatos a serem removidos ou deixados para depois.

## Dockerfile

Arquivo:

```text
Dockerfile
```

Conteudo atual:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY MuseuDeBugs.Api/MuseuDeBugs.Api.csproj MuseuDeBugs.Api/
RUN dotnet restore MuseuDeBugs.Api/MuseuDeBugs.Api.csproj

COPY MuseuDeBugs.Api/ MuseuDeBugs.Api/
RUN dotnet publish MuseuDeBugs.Api/MuseuDeBugs.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 10000

CMD ASPNETCORE_URLS=http://0.0.0.0:${PORT:-10000} dotnet MuseuDeBugs.Api.dll
```

### Por que existe

Docker cria uma "caixa" com tudo que o backend precisa para rodar. No Render, isso evita depender de como a maquina do Render esta configurada para .NET.

Voce pode pensar assim:

```text
Dockerfile = receita para montar a API em producao.
```

### Linha por linha

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
```

- `FROM` escolhe a imagem base.
- `mcr.microsoft.com/dotnet/sdk:10.0` e uma imagem oficial da Microsoft com SDK do .NET 10.
- SDK e usado para compilar, restaurar pacotes e publicar.
- `AS build` da nome para essa etapa. Esse nome sera usado depois.

```dockerfile
WORKDIR /src
```

- `WORKDIR` define a pasta de trabalho dentro do container.
- Depois dessa linha, comandos como `COPY` e `RUN` rodam a partir de `/src`.

```dockerfile
COPY MuseuDeBugs.Api/MuseuDeBugs.Api.csproj MuseuDeBugs.Api/
```

- `COPY origem destino`.
- Copia so o arquivo `.csproj` da API.
- Isso e feito antes de copiar o resto para aproveitar cache do Docker.
- Se o `.csproj` nao mudou, o restore pode ser reaproveitado.

```dockerfile
RUN dotnet restore MuseuDeBugs.Api/MuseuDeBugs.Api.csproj
```

- `RUN` executa um comando durante a construcao da imagem.
- `dotnet restore` baixa/restaura pacotes NuGet do projeto.
- Pacotes como `Npgsql.EntityFrameworkCore.PostgreSQL` entram nessa etapa.

```dockerfile
COPY MuseuDeBugs.Api/ MuseuDeBugs.Api/
```

- Agora copia o codigo inteiro da API.
- O primeiro `COPY` copiava so o `.csproj`; este copia controllers, services, DTOs, migrations etc.

```dockerfile
RUN dotnet publish MuseuDeBugs.Api/MuseuDeBugs.Api.csproj -c Release -o /app/publish --no-restore
```

- `dotnet publish` gera uma versao pronta para rodar em producao.
- `-c Release` usa configuracao Release, mais apropriada para deploy.
- `-o /app/publish` manda o resultado para `/app/publish`.
- `--no-restore` diz: nao rode restore de novo, porque ja fizemos antes.

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
```

- Comeca uma segunda etapa.
- Aqui nao usamos SDK completo, so runtime ASP.NET Core.
- Runtime e menor e suficiente para executar a API ja publicada.
- `AS runtime` da nome para essa etapa final.

```dockerfile
WORKDIR /app
```

- Define `/app` como pasta de trabalho final.

```dockerfile
COPY --from=build /app/publish .
```

- Copia da etapa `build`.
- `--from=build` significa: pegue arquivos da etapa chamada `build`.
- Origem: `/app/publish`.
- Destino: `.` significa pasta atual, que e `/app`.

```dockerfile
ENV ASPNETCORE_ENVIRONMENT=Production
```

- `ENV` cria variavel de ambiente dentro do container.
- `ASPNETCORE_ENVIRONMENT=Production` faz a API rodar como producao.
- Em producao, Swagger fica desligado pelo `Program.cs`.

```dockerfile
EXPOSE 10000
```

- Documenta que a aplicacao escuta na porta 10000.
- Render normalmente injeta a porta pela variavel `PORT`.
- `EXPOSE` nao abre porta sozinho; e informativo para a plataforma/container.

```dockerfile
CMD ASPNETCORE_URLS=http://0.0.0.0:${PORT:-10000} dotnet MuseuDeBugs.Api.dll
```

- `CMD` e o comando que roda quando o container inicia.
- `ASPNETCORE_URLS=...` diz para o ASP.NET Core em qual endereco escutar.
- `0.0.0.0` significa: escute em todas as interfaces do container.
- `${PORT:-10000}` e sintaxe de shell:
  - se `PORT` existir, usa o valor de `PORT`;
  - se `PORT` nao existir, usa `10000`.
- `dotnet MuseuDeBugs.Api.dll` executa a API publicada.

### O que e obrigatorio aqui

Para Render com Docker, o `Dockerfile` e importante.

Para deploy sem Docker, ele pode ser removido e substituido por comandos de build/start no Render.

## .dockerignore

Arquivo:

```text
.dockerignore
```

Funciona parecido com `.gitignore`, mas para Docker.

Ele diz quais arquivos nao entram no contexto de build. Isso deixa a imagem menor e evita enviar lixo para o Render.

Exemplos:

```text
**/bin/
**/obj/
**/node_modules/
**/dist/
```

Explicacao:

- `**/bin/`: ignora qualquer pasta `bin` em qualquer nivel.
- `**/obj/`: ignora compilacao temporaria do .NET.
- `**/node_modules/`: ignora dependencias gigantes do Angular.
- `**/dist/`: ignora build do front.

Tambem foi incluido:

```text
frontend/
MuseuDeBugs.Tests/
MuseuDeBugs.Tools/
```

Motivo:

- A imagem Docker e so da API.
- Front vai para Vercel.
- Testes e tools nao precisam ir para runtime da API.

## render.yaml

Arquivo:

```text
render.yaml
```

Conteudo:

```yaml
services:
  - type: web
    name: museu-de-bugs-api
    runtime: docker
    plan: free
    dockerfilePath: ./Dockerfile
    dockerContext: .
    healthCheckPath: /health
    envVars:
      - key: ASPNETCORE_ENVIRONMENT
        value: Production
      - key: ConnectionStrings__DefaultConnection
        sync: false
      - key: Admin__Username
        sync: false
      - key: Admin__PasswordHash
        sync: false
      - key: Cors__AllowedOrigins__0
        sync: false
```

### Por que existe

Render consegue ler esse arquivo como uma "blueprint". Ele descreve o servico da API.

```text
render.yaml = formulario do Render escrito em codigo.
```

### Sintaxe YAML

YAML usa indentacao. Espaco importa.

```yaml
services:
```

- `services` e uma chave.
- O valor dela e uma lista de servicos.

```yaml
  - type: web
```

- `-` indica um item de lista.
- `type: web` diz que esse item e um servico web.

```yaml
    name: museu-de-bugs-api
```

- Nome do servico no Render.

```yaml
    runtime: docker
```

- Diz que Render deve usar Docker.
- Por isso o `Dockerfile` entra na historia.

```yaml
    plan: free
```

- Tenta usar plano gratuito.
- Pode ter limitacoes como cold start.

```yaml
    dockerfilePath: ./Dockerfile
```

- Caminho do Dockerfile.

```yaml
    dockerContext: .
```

- Contexto do build Docker.
- `.` significa raiz do repo.

```yaml
    healthCheckPath: /health
```

- Render chama `/health` para saber se API esta viva.
- Por isso eu adicionei `app.MapGet("/health", ...)` no `Program.cs`.

```yaml
    envVars:
```

- Lista variaveis de ambiente.
- Variavel de ambiente e configuracao que fica fora do codigo.

```yaml
      - key: ASPNETCORE_ENVIRONMENT
        value: Production
```

- Essa pode ficar no arquivo porque nao e segredo.
- Diz que a API roda em producao.

```yaml
      - key: ConnectionStrings__DefaultConnection
        sync: false
```

- Connection string do banco.
- `sync: false` significa: o valor nao fica no repo, voce preenche no painel do Render.
- `__` no .NET vira `:`.
- Entao `ConnectionStrings__DefaultConnection` vira `ConnectionStrings:DefaultConnection`.

Mesma logica para:

```yaml
Admin__Username
Admin__PasswordHash
Cors__AllowedOrigins__0
```

- `Admin__Username`: usuario admin.
- `Admin__PasswordHash`: hash da senha, nao senha pura.
- `Cors__AllowedOrigins__0`: primeira URL liberada para chamar a API.

## frontend/vercel.json

Arquivo:

```text
frontend/vercel.json
```

Conteudo:

```json
{
  "$schema": "https://openapi.vercel.sh/vercel.json",
  "framework": "angular",
  "buildCommand": "npm run build",
  "outputDirectory": "dist/frontend/browser",
  "rewrites": [
    {
      "source": "/(.*)",
      "destination": "/index.html"
    }
  ]
}
```

### Por que existe

A Vercel precisa saber:

- que o projeto e Angular;
- qual comando buildar;
- onde esta o resultado do build;
- como lidar com rotas do Angular.

### JSON sintaxe

JSON e composto de pares:

```json
"chave": "valor"
```

Objeto fica entre `{ }`.

Lista fica entre `[ ]`.

Cada item, menos o ultimo, termina com virgula.

### Linha por linha

```json
"$schema": "https://openapi.vercel.sh/vercel.json"
```

- Ajuda editor/IDE a entender o formato.
- Nao muda logica em runtime.

```json
"framework": "angular"
```

- Diz para Vercel tratar como Angular.

```json
"buildCommand": "npm run build"
```

- Comando que Vercel roda para gerar o site.
- No `package.json`, `npm run build` chama `ng build`.

```json
"outputDirectory": "dist/frontend/browser"
```

- Pasta que a Vercel deve publicar.
- O Angular 18 gera o build do browser nessa pasta.

```json
"rewrites": [
  {
    "source": "/(.*)",
    "destination": "/index.html"
  }
]
```

- Angular e SPA.
- SPA significa Single Page Application.
- Se usuario abre `/bugs/123`, o servidor talvez nao tenha esse arquivo fisico.
- O rewrite manda qualquer rota para `index.html`.
- Depois o Angular decide o que mostrar no navegador.

## frontend/public/app-config.js

Arquivo:

```text
frontend/public/app-config.js
```

Conteudo:

```js
window.MUSEU_DEBUGS_CONFIG = {
  apiUrl: 'http://localhost:5041/api'
};
```

### Por que existe

Antes, o front tinha a URL da API fixa no TypeScript:

```ts
export const API_URL = 'http://localhost:5041/api';
```

Isso funciona local, mas em producao a API vai ser algo como:

```text
https://museu-de-bugs-api.onrender.com/api
```

Se a URL fica compilada no bundle Angular, trocar depois e chato.

Com `app-config.js`, a URL vira arquivo externo simples.

### Linha por linha

```js
window.MUSEU_DEBUGS_CONFIG = {
```

- `window` e o objeto global do navegador.
- Tudo que voce poe em `window` pode ser lido por outros scripts da pagina.
- `MUSEU_DEBUGS_CONFIG` e um nome criado para guardar configuracoes do app.
- `{` abre um objeto JavaScript.

```js
  apiUrl: 'http://localhost:5041/api'
```

- `apiUrl` e uma propriedade do objeto.
- O valor e uma string.
- Em local aponta para API local.
- Em producao deve apontar para URL do Render.

```js
};
```

- `}` fecha o objeto.
- `;` finaliza a instrucao.

## frontend/src/app/config/api.config.ts

Arquivo:

```text
frontend/src/app/config/api.config.ts
```

Conteudo:

```ts
declare global {
  interface Window {
    MUSEU_DEBUGS_CONFIG?: {
      apiUrl?: string;
    };
  }
}

export const API_URL = window.MUSEU_DEBUGS_CONFIG?.apiUrl ?? 'http://localhost:5041/api';
```

### Por que existe

Esse arquivo e onde o Angular pega a URL base da API.

### Sintaxe linha por linha

```ts
declare global {
```

- `declare` em TypeScript adiciona informacao de tipo.
- Nao gera codigo JavaScript util no runtime.
- Serve para o TypeScript saber que uma coisa existe.
- `global` significa: estou declarando algo no escopo global.

```ts
  interface Window {
```

- `interface` descreve o formato de um objeto.
- `Window` e o tipo do objeto global `window`.
- Estamos ensinando o TypeScript que `window` pode ter uma propriedade nova.

```ts
    MUSEU_DEBUGS_CONFIG?: {
```

- `MUSEU_DEBUGS_CONFIG` e o nome da propriedade.
- `?` significa opcional.
- Ou seja: pode existir ou nao.
- `{` abre o tipo do objeto.

```ts
      apiUrl?: string;
```

- `apiUrl` e opcional.
- `string` significa texto.

```ts
    };
  }
}
```

- Fecha o objeto, a interface e o bloco global.

```ts
export const API_URL = window.MUSEU_DEBUGS_CONFIG?.apiUrl ?? 'http://localhost:5041/api';
```

- `export` permite outros arquivos importarem `API_URL`.
- `const` cria constante.
- `window.MUSEU_DEBUGS_CONFIG` le a config global criada em `app-config.js`.
- `?.` e optional chaining:
  - se `MUSEU_DEBUGS_CONFIG` existir, tenta ler `.apiUrl`;
  - se nao existir, retorna `undefined` sem quebrar.
- `??` e nullish coalescing:
  - se o lado esquerdo for `null` ou `undefined`, usa o lado direito.
- Resultado:
  - se `app-config.js` definiu `apiUrl`, usa essa URL;
  - senao, usa `http://localhost:5041/api`.

## frontend/src/index.html

Arquivo:

```text
frontend/src/index.html
```

Mudanca importante:

```html
<script src="app-config.js"></script>
```

### Por que existe

Esse script precisa carregar antes do Angular usar `API_URL`.

O Angular carrega scripts gerados no build. Como `app-config.js` esta no `public`, ele vai para a raiz do site publicado.

Quando o navegador abre a pagina:

1. Le `index.html`.
2. Carrega `app-config.js`.
3. `app-config.js` cria `window.MUSEU_DEBUGS_CONFIG`.
4. Angular inicia.
5. Angular le `window.MUSEU_DEBUGS_CONFIG?.apiUrl`.

## Program.cs - mudancas de deploy e seguranca

Arquivo:

```text
MuseuDeBugs.Api/Program.cs
```

### Imports novos

```csharp
using Microsoft.AspNetCore.HttpOverrides;
```

- `using` importa namespace.
- Namespace e uma "pasta logica" de classes.
- `HttpOverrides` contem tipos para lidar com proxy.
- Render fica na frente da sua API como proxy.
- Por isso a API precisa entender headers como `X-Forwarded-Proto`.

### CORS configuravel

Trecho:

```csharp
const string politicaCors = "PermitirFrontend";
var allowedCorsOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];
```

Linha por linha:

```csharp
const string politicaCors = "PermitirFrontend";
```

- `const` cria constante de compilacao.
- `string` e texto.
- `politicaCors` e o nome interno da politica.
- `"PermitirFrontend"` e o valor.

```csharp
var allowedCorsOrigins = builder.Configuration
```

- `var` deixa o C# inferir o tipo.
- `allowedCorsOrigins` vai guardar uma lista de origens liberadas.
- `builder.Configuration` acessa configuracao do app: appsettings, env vars etc.

```csharp
    .GetSection("Cors:AllowedOrigins")
```

- Pega a secao `Cors:AllowedOrigins`.
- Em variavel de ambiente, `:` vira `__`.
- Exemplo: `Cors__AllowedOrigins__0=https://front.vercel.app`.

```csharp
    .Get<string[]>() ?? [];
```

- `.Get<string[]>()` tenta converter a configuracao em array de string.
- `??` significa: se for null, usa o valor da direita.
- `[]` cria array vazio em C# moderno.

### Localhost automatico em desenvolvimento

```csharp
if (builder.Environment.IsDevelopment())
{
    allowedCorsOrigins = [
        .. allowedCorsOrigins,
        "http://localhost:5173",
        "http://localhost:4200"
    ];
}
```

Explicacao:

- `if` executa o bloco se a condicao for verdadeira.
- `builder.Environment.IsDevelopment()` verifica se ambiente e Development.
- Em local, adiciona Vite `5173` e Angular `4200`.
- `[` `]` cria array/lista.
- `.. allowedCorsOrigins` e spread:
  - pega todos os itens que ja existiam no array;
  - depois adiciona os localhost.

### Remover repetidos

```csharp
allowedCorsOrigins = allowedCorsOrigins
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray();
```

- `.Distinct(...)` remove duplicados.
- `StringComparer.OrdinalIgnoreCase` compara ignorando maiuscula/minuscula.
- `.ToArray()` transforma de volta em array.

### Barrar producao sem CORS

```csharp
if (allowedCorsOrigins.Length == 0)
{
    throw new InvalidOperationException("Configure Cors:AllowedOrigins para liberar o frontend em producao.");
}
```

- Se nao tiver nenhuma origem liberada, a API nao inicia.
- Isso evita subir producao com CORS quebrado.
- `throw` lanca erro.
- `InvalidOperationException` significa "operacao invalida no estado atual".

### Forwarded headers

```csharp
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});
```

Explicacao:

- `builder.Services.Configure<T>()` configura opcoes de um recurso.
- `ForwardedHeadersOptions` guarda configuracao de headers encaminhados por proxy.
- `options => { ... }` e lambda.
- Lambda e uma funcao curta passada como argumento.

```csharp
options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
```

- `XForwardedFor` informa IP original do cliente.
- `XForwardedProto` informa protocolo original: `http` ou `https`.
- `|` combina flags.

```csharp
options.KnownIPNetworks.Clear();
options.KnownProxies.Clear();
```

- Limpa restricoes de redes/proxies conhecidos.
- Isso e comum em plataformas cloud onde o proxy pode variar.

Mais tarde:

```csharp
app.UseForwardedHeaders();
```

- Ativa essa leitura de headers no pipeline.

### Cookie cross-site

Trecho:

```csharp
options.Cookie.SameSite = builder.Environment.IsDevelopment()
    ? SameSiteMode.Lax
    : SameSiteMode.None;
```

Explicacao:

- `SameSite` controla quando o navegador envia cookie entre sites.
- Local:
  - front `localhost:4200`;
  - back `localhost:5041`;
  - `Lax` costuma funcionar.
- Producao:
  - front `vercel.app`;
  - back `onrender.com`;
  - sao dominios diferentes;
  - precisa `SameSite=None`.

Sintaxe:

```csharp
condicao ? valorSeVerdadeiro : valorSeFalso
```

Isso e operador ternario.

Trecho:

```csharp
options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
    ? CookieSecurePolicy.SameAsRequest
    : CookieSecurePolicy.Always;
```

- Em producao, cookie precisa ser HTTPS.
- `Always` exige HTTPS.
- Isso combina com `SameSite=None`, porque navegadores modernos exigem cookie cross-site seguro.

### CORS usando variavel

Antes era fixo:

```csharp
.WithOrigins("http://localhost:5173", "http://localhost:4200")
```

Agora:

```csharp
.WithOrigins(allowedCorsOrigins)
```

Significa:

- em dev, libera localhost;
- em prod, libera o que voce colocar em `Cors__AllowedOrigins__0`.

### Banco PostgreSQL

Trecho:

```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
```

- Le `ConnectionStrings:DefaultConnection`.
- A variavel `connectionString` ficou sobrando depois da mudanca porque o `UseNpgsql` le direto de novo.
- Pode ser simplificado depois.

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));
```

- Registra `AppDbContext` no DI do ASP.NET Core.
- `DI` significa Dependency Injection.
- `UseNpgsql` diz que EF Core vai usar PostgreSQL.
- `DefaultConnection` vem do Supabase/Render.

### Migrations no boot

Trecho:

```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
```

Linha por linha:

```csharp
using (var scope = app.Services.CreateScope())
```

- `using` aqui nao e import.
- Aqui e bloco que descarta recurso no fim.
- `CreateScope()` cria um escopo de DI.
- `DbContext` normalmente vive dentro de escopo.

```csharp
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
```

- Pega uma instancia de `AppDbContext`.
- `GetRequiredService` falha se nao encontrar o servico.

```csharp
db.Database.Migrate();
```

- Aplica migrations pendentes no banco.
- Ou seja: cria/atualiza tabelas no Supabase quando a API sobe.

Importante:

```text
Isso e conveniente para projeto pequeno.
Em projeto grande, migrations costumam rodar em etapa separada de deploy.
```

### Health check

```csharp
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
```

Explicacao:

- `MapGet` cria uma rota GET.
- `"/health"` e o caminho.
- `() => ...` e lambda sem parametro.
- `Results.Ok(...)` retorna HTTP 200.
- `new { status = "ok" }` cria objeto anonimo.
- Render usa isso para saber se a API esta viva.

Resposta:

```json
{
  "status": "ok"
}
```

## DTOs - CriarBugRequest e AtualizarBugRequest

Arquivos:

```text
MuseuDeBugs.Api/DTOs/CriarBugRequest.cs
MuseuDeBugs.Api/DTOs/AtualizarBugRequest.cs
```

DTO significa Data Transfer Object.

Ele representa o corpo JSON que chega na API.

Exemplo:

```json
{
  "titulo": "Teste",
  "linguagem": "C#",
  "descricao": "Descricao com tamanho suficiente"
}
```

### Atributos de validacao

Exemplo:

```csharp
[Required]
[MaxLength(120)]
[MinLength(1)]
[RegularExpression(@".*\S.*", ErrorMessage = "Titulo deve ser preenchido.")]
public string Titulo { get; set; } = string.Empty;
```

Linha por linha:

```csharp
[Required]
```

- Campo obrigatorio.
- Rejeita `null`.

```csharp
[MaxLength(120)]
```

- Limite maximo de 120 caracteres.
- Protege banco e API de texto gigante.

```csharp
[MinLength(1)]
```

- Exige pelo menos 1 caractere.

```csharp
[RegularExpression(@".*\S.*", ErrorMessage = "Titulo deve ser preenchido.")]
```

- Exige pelo menos um caractere que nao seja espaco.
- `RegularExpression` usa regex.
- `@` antes da string cria verbatim string em C#.
- Em verbatim string, barras ficam mais faceis de ler.

Regex:

```text
.*\S.*
```

Quebra:

```text
.*   qualquer coisa antes
\S   um caractere nao-branco
.*   qualquer coisa depois
```

Passa:

```text
"Teste"
" C# "
"a"
```

Nao passa:

```text
""
"     "
```

```csharp
public string Titulo { get; set; } = string.Empty;
```

- `public` permite acesso de fora da classe.
- `string` e texto.
- `Titulo` e nome da propriedade.
- `{ get; set; }` cria getter e setter.
- `= string.Empty` inicializa como string vazia para evitar null.

### Como isso chega no front

O controller tem `[ApiController]`.

Com `[ApiController]`, se o DTO falha validacao, ASP.NET Core retorna 400 automaticamente.

Resposta parece:

```json
{
  "errors": {
    "Titulo": [
      "Titulo deve ser preenchido."
    ]
  }
}
```

Angular recebe isso em:

```ts
HttpErrorResponse.error
```

E o codigo do painel tenta mostrar a primeira mensagem.

## frontend/src/app/utils/bug-request-form.ts

Arquivo:

```text
frontend/src/app/utils/bug-request-form.ts
```

Esse arquivo foi criado para nao duplicar regra entre criar e editar bug.

### Interface

```ts
export interface BugRequestForm {
  titulo: string;
  linguagem: string;
  descricao: string;
}
```

Explicacao:

- `export` permite importar em outros arquivos.
- `interface` descreve formato de objeto.
- `BugRequestForm` e o nome do tipo.
- `titulo`, `linguagem`, `descricao` sao propriedades obrigatorias.
- `string` significa texto.

### Validacao

```ts
export function validateBugRequestForm(request: BugRequestForm): string | null {
```

- `function` declara funcao.
- `validateBugRequestForm` e o nome.
- `request: BugRequestForm` diz que o parametro precisa ter formato da interface.
- `: string | null` diz que a funcao retorna:
  - string com mensagem de erro; ou
  - null quando esta valido.

```ts
if (request.titulo.length < 1) {
  return 'Informe o titulo.';
}
```

- `if` testa uma condicao.
- `request.titulo.length` pega quantidade de caracteres.
- `< 1` significa menor que 1.
- Se titulo vazio, retorna mensagem.
- `return` encerra a funcao.

Mesma logica para linguagem:

```ts
if (request.linguagem.length < 1) {
  return 'Informe a linguagem. C# e aceito.';
}
```

Descricao:

```ts
if (request.descricao.length < 10) {
  return 'Informe uma descricao com pelo menos 10 caracteres.';
}
```

Por que 10?

- O backend tem `[MinLength(10)]` na descricao.
- O front precisa avisar antes de mandar.

Final:

```ts
return null;
```

- Se nenhuma regra falhou, retorna `null`.
- `null` aqui significa "sem erro".

### Normalizar linguagem

```ts
export function normalizeBugLanguage(value: string): string {
```

- Recebe uma string.
- Retorna uma string.
- Objetivo: transformar apelidos em nomes padronizados.

```ts
const normalized = value.trim();
```

- `const` cria constante.
- `trim()` remove espacos no comeco e fim.

```ts
const knownLanguages: Record<string, string> = {
```

- `Record<string, string>` e um tipo do TypeScript.
- Significa: objeto onde chaves sao strings e valores sao strings.

Exemplo:

```ts
'c#': 'C#',
'csharp': 'C#',
'c-sharp': 'C#',
```

- Se usuario digitar `csharp`, salva como `C#`.

```ts
return knownLanguages[normalized.toLowerCase()] ?? normalized;
```

- `normalized.toLowerCase()` deixa minusculo.
- `knownLanguages[...]` procura no mapa.
- `?? normalized` significa:
  - se achou no mapa, retorna valor padrao;
  - se nao achou, retorna o texto original aparado.

### Opcionais vazios viram null

```ts
export function normalizeOptionalBugText(value: string | null | undefined): string | null {
```

- Recebe texto, null ou undefined.
- Retorna texto ou null.

```ts
const normalized = value?.trim() ?? '';
```

- `value?.trim()` so chama `trim()` se value existir.
- Se value for null/undefined, retorna undefined.
- `?? ''` troca null/undefined por string vazia.

```ts
return normalized.length > 0 ? normalized : null;
```

- Operador ternario.
- Se tiver conteudo, retorna conteudo.
- Se vazio, retorna null.

## BugCreatePanelComponent

Arquivos:

```text
frontend/src/app/components/bug-create-panel/bug-create-panel.component.ts
frontend/src/app/components/bug-create-panel/bug-create-panel.component.html
```

### Ideia

Esse componente e o formulario de criar bug.

Mudancas feitas:

- usa `validateBugRequestForm`;
- usa `normalizeBugLanguage`;
- usa `normalizeOptionalBugText`;
- limpa mensagem de erro quando usuario edita;
- evita duplo submit com `isSubmitting`;
- pega mensagens de validacao vindas do backend.

### Import novo

```ts
import {
  normalizeBugLanguage,
  normalizeOptionalBugText,
  validateBugRequestForm
} from '../../utils/bug-request-form';
```

Explicacao:

- `import` puxa funcoes de outro arquivo.
- `{ ... }` importa exports nomeados.
- Caminho relativo:
  - `../../` sobe duas pastas;
  - `utils/bug-request-form` entra no helper.

### Evitar duplo submit

```ts
if (this.isSubmitting) {
  return;
}
```

- Se ja esta salvando, sai da funcao.
- Evita dois POSTs se usuario clicar duas vezes.

### Montar request

```ts
const request = this.buildRequest();
const validationMessage = validateBugRequestForm(request);
```

- `buildRequest()` pega o formulario e normaliza.
- `validateBugRequestForm(request)` valida antes de enviar.

### Se tem erro

```ts
if (validationMessage) {
  this.errorMessage = validationMessage;
  this.successMessage = '';
  return;
}
```

- Se `validationMessage` for string, existe erro.
- Mostra erro.
- Limpa sucesso antigo.
- Para a funcao.

### Enviar para API

```ts
this.bugService.criar(request).subscribe({
```

- `bugService.criar` chama `POST /api/bugs`.
- `subscribe` escuta resultado do Observable Angular.

Dentro de `next`:

```ts
next: (bug) => {
```

- Roda quando API responde sucesso.

Dentro de `error`:

```ts
error: (error: HttpErrorResponse) => {
```

- Roda quando API responde erro.
- `HttpErrorResponse` e tipo do Angular.

### Pegar mensagem do backend

```ts
private getApiValidationMessage(error: HttpErrorResponse): string | null {
```

- Funcao privada.
- Recebe erro HTTP.
- Tenta extrair primeira mensagem do JSON de validacao.

```ts
const validationErrors = error.error?.errors;
```

- `error.error` e o body JSON da resposta.
- `?.errors` tenta ler `errors` sem quebrar se `error.error` nao existir.

```ts
if (!validationErrors || typeof validationErrors !== 'object') {
  return null;
}
```

- Se nao tem `errors` ou nao e objeto, nao tem mensagem util.

```ts
const messages = Object.values(validationErrors)
  .flatMap((value) => Array.isArray(value) ? value : [value])
  .filter((value): value is string => typeof value === 'string');
```

Quebra:

- `Object.values(validationErrors)` pega os valores do objeto.
- Exemplo: `{ Titulo: ["Titulo deve ser preenchido."] }` vira `[["Titulo deve ser preenchido."]]`.
- `flatMap` achata listas.
- `Array.isArray(value) ? value : [value]`:
  - se ja e array, usa;
  - se nao e array, embrulha em array.
- `filter` deixa passar so strings.
- `value is string` e type guard do TypeScript.

```ts
return messages[0] ?? null;
```

- Retorna primeira mensagem.
- Se nao tiver, retorna null.

### HTML e novalidate

```html
<form class="create-form" (ngSubmit)="salvar()" novalidate>
```

- `class` aplica CSS.
- `(ngSubmit)="salvar()"` chama `salvar()` quando o formulario e enviado.
- `novalidate` desliga validacao nativa do navegador.
- Por que? Para deixar Angular/backend controlarem mensagens de forma consistente.

```html
[(ngModel)]="form.titulo"
```

- Two-way binding do Angular.
- Atualiza input quando `form.titulo` muda.
- Atualiza `form.titulo` quando usuario digita.

```html
(ngModelChange)="onFormInput()"
```

- Evento que dispara quando valor muda.
- Limpa mensagem de erro/sucesso antiga.

## BugGridComponent

Arquivos:

```text
frontend/src/app/components/bug-grid/bug-grid.component.ts
frontend/src/app/components/bug-grid/bug-grid.component.html
```

Esse componente tambem tem formulario, mas de editar bug.

Mudancas:

- usa o mesmo helper do create;
- normaliza linguagem na atualizacao;
- opcionais vazios viram null;
- mensagem de erro fica especifica;
- `novalidate` no form de edicao;
- limpa erro quando usuario edita.

Trecho:

```ts
const request = this.buildUpdateRequest();
const validationMessage = validateBugRequestForm(request);
```

Mesma ideia do create.

Trecho:

```ts
if (!this.selectedBug || this.isSaving) {
  return;
}
```

- Se nao tem bug selecionado, nao faz nada.
- Se ja esta salvando, nao faz nada.

Trecho:

```ts
linguagem: normalizeBugLanguage(this.editForm.linguagem),
```

- `c#` vira `C#` antes de enviar para API.

## BugService.cs

Arquivo:

```text
MuseuDeBugs.Api/Services/BugService.cs
```

### Criar bug

Antes passava strings direto.

Agora:

```csharp
var bug = new Bug(
    request.Titulo.Trim(),
    NormalizarLinguagem(request.Linguagem),
    NormalizarTextoOpcional(request.MensagemErro),
    request.Descricao.Trim(),
    NormalizarTextoOpcional(request.Causa),
    NormalizarTextoOpcional(request.Solucao));
```

Explicacao:

- `request.Titulo.Trim()` remove espacos nas pontas.
- `NormalizarLinguagem(...)` padroniza linguagem.
- `NormalizarTextoOpcional(...)` transforma vazio em null.

### Filtro por linguagem

```csharp
var linguagemNormalizada = NormalizarLinguagem(linguagem).ToLower();
query = query.Where(bug => bug.Linguagem.ToLower() == linguagemNormalizada);
```

- Normaliza o filtro recebido.
- `ToLower()` compara ignorando maiuscula/minuscula.
- Isso ajuda no PostgreSQL, que e sensivel a maiuscula/minuscula em comparacao comum.

### Normalizar linguagem

```csharp
private static string NormalizarLinguagem(string linguagem)
```

- `private` so pode ser usada dentro da classe.
- `static` nao precisa de instancia da classe.
- Recebe `string`.
- Retorna `string`.

```csharp
var normalizada = linguagem.Trim();
```

- Remove espacos nas pontas.

```csharp
return normalizada.ToLowerInvariant() switch
{
    "c#" or "csharp" or "c-sharp" => "C#",
    "c++" or "cpp" => "C++",
    "js" or "javascript" => "JavaScript",
    "ts" or "typescript" => "TypeScript",
    _ => normalizada
};
```

Explicacao:

- `switch` expression escolhe retorno conforme valor.
- `ToLowerInvariant()` deixa texto minusculo de forma independente de cultura.
- `"c#" or "csharp"` significa: qualquer uma dessas opcoes.
- `=>` aponta para retorno.
- `_` e caso padrao.

### Texto opcional

```csharp
private static string? NormalizarTextoOpcional(string? texto)
```

- `string?` significa que pode ser string ou null.

```csharp
var normalizado = texto?.Trim() ?? string.Empty;
```

- `texto?.Trim()` so chama Trim se texto nao for null.
- `?? string.Empty` troca null por string vazia.

```csharp
return normalizado.Length > 0 ? normalizado : null;
```

- Se tem conteudo, retorna conteudo.
- Se vazio, retorna null.

## Testes

Arquivos:

```text
MuseuDeBugs.Tests/Services/BugServiceTests.cs
MuseuDeBugs.Tests/DTOs/BugRequestValidationTests.cs
MuseuDeBugs.Tests/MuseuDeBugs.Tests.csproj
```

### Por que mexeu no csproj dos testes

O projeto da API estava em EF Core 10.0.6.

O projeto de testes estava puxando EF InMemory 9.0.0.

Isso gerou conflito de assembly.

Foi alinhado para:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="10.0.6" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Relational" Version="10.0.6" />
```

Explicacao XML:

- `<PackageReference ... />` adiciona pacote NuGet.
- `Include` e o nome do pacote.
- `Version` e a versao.
- `/>` fecha tag sem conteudo interno.

### Teste de normalizacao

Teste adicionado:

```csharp
CriarBug_DeveNormalizarLinguagemETextos
```

Ele garante:

- `"  Titulo  "` vira `"Titulo"`;
- `"csharp"` vira `"C#"`;
- descricao com espaco nas pontas e aparada.

### Teste de DTO

Arquivo:

```text
MuseuDeBugs.Tests/DTOs/BugRequestValidationTests.cs
```

Ele usa:

```csharp
Validator.TryValidateObject(...)
```

Para testar DataAnnotations sem subir servidor.

Casos testados:

- titulo so com espacos;
- linguagem so com espacos;
- descricao so com espacos.

## Favicon e besouro

Arquivos principais:

```text
frontend/public/favicon-1024x1024.png
frontend/public/favicon-512x512.png
frontend/public/favicon-192x192.png
frontend/public/apple-touch-icon.png
frontend/public/favicon-32x32.png
frontend/public/favicon-16x16.png
frontend/public/favicon.ico
frontend/src/index.html
portable-launcher/install-shortcut.sh
portable-launcher/README.md
```

### O que foi feito

- Removi o fundo preto do besouro localmente.
- Gereis PNGs RGBA com alpha real.
- Atualizei `index.html` para apontar para PNGs maiores.
- Atualizei o script do atalho para copiar o favicon para `~/.local/share/icons/...`.

### Importante

Essas mudancas sao asset/UI. Nao mudam regra do backend.

## .gitignore

Arquivo:

```text
.gitignore
```

Entraram ignores para:

- `.idea/`
- logs npm/yarn/pnpm
- `.angular/`
- `coverage/`
- `TestResults/`
- `.env`
- artefatos locais do portable launcher

Objetivo:

```text
Nao versionar lixo local, cache, segredo ou resultado temporario.
```

## Supabase, Render e Vercel - mapa mental

Desenho:

```text
Navegador
  |
  v
Vercel: frontend Angular
  |
  v
Render: backend ASP.NET Core
  |
  v
Supabase: PostgreSQL
```

### Vercel

Hospeda arquivos estaticos do Angular:

- HTML
- CSS
- JS
- imagens

Nao roda C#.

### Render

Roda a API ASP.NET Core.

Recebe chamadas:

```text
GET /api/bugs
POST /api/bugs
POST /api/auth/login
```

### Supabase

Banco PostgreSQL.

Guarda tabela `Bugs`.

## Segredos de producao

Nao entram no codigo.

No Render:

```text
ConnectionStrings__DefaultConnection=...
Admin__Username=...
Admin__PasswordHash=...
Cors__AllowedOrigins__0=https://seu-front.vercel.app
```

### Por que `__`

No .NET:

```text
Cors__AllowedOrigins__0
```

vira:

```text
Cors:AllowedOrigins:0
```

Isso representa:

```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://seu-front.vercel.app"
    ]
  }
}
```

## O que eu deveria ter explicado antes

Eu deveria ter parado neste ponto:

```text
Para deploy gratuito temos algumas opcoes:

1. Vercel + Render + Supabase sem Docker.
2. Vercel + Render + Supabase com Docker.
3. Outra plataforma.

Docker adiciona arquivo novo, mas deixa o runtime previsivel.
Quer que eu prepare isso?
```

Eu nao fiz isso. Entao essa nota tambem serve como reparo e como trilha de estudo.

## O que pode ser revertido se voce quiser simplificar

Pacote de deploy:

```text
Dockerfile
.dockerignore
render.yaml
frontend/vercel.json
frontend/public/app-config.js
mudancas de deploy em Program.cs
mudancas de deploy em frontend/src/app/config/api.config.ts
linha <script src="app-config.js"></script> em frontend/src/index.html
```

Pacote de validacao robusta:

```text
frontend/src/app/utils/bug-request-form.ts
mudancas no create panel
mudancas no edit form do bug grid
RegularExpression nos DTOs
normalizacao no BugService
testes novos
```

Pacote de assets:

```text
favicons novos
index.html com links de favicon
portable-launcher
```

## Como estudar isso sem se perder

Ordem recomendada:

1. Entender DTOs: `CriarBugRequest` e `AtualizarBugRequest`.
2. Entender `BugService.CriarBug` e `AtualizarBug`.
3. Entender `bug-request-form.ts`.
4. Entender `BugCreatePanelComponent`.
5. Entender `BugGridComponent` edicao.
6. So depois olhar deploy:
   - `app-config.js`
   - `vercel.json`
   - `render.yaml`
   - `Dockerfile`
   - extras do `Program.cs`

## Comandos que foram usados para validar

```bash
npm run build
dotnet test
dotnet publish MuseuDeBugs.Api/MuseuDeBugs.Api.csproj -c Release
```

Resultado na ultima validacao:

```text
Angular build: passou.
dotnet test: 15 testes passaram.
dotnet publish: passou.
docker build: nao foi rodado porque Docker nao esta instalado nesta maquina.
```

## Resumo bruto

O projeto ficou mais pronto para producao, mas ficou maior.

As validacoes no backend fazem sentido por seguranca.

Os arquivos de deploy fazem sentido se voce quiser Vercel + Render + Supabase.

O erro foi eu nao ter te guiado antes de adicionar tudo.

Essa nota e o mapa para voce recuperar controle do projeto.
