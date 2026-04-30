# MuseuDeBugs

Esse projeto e uma aplicacao em C# + Angular que eu estou montando para guardar bugs que aparecem enquanto eu estudo ou faco projetos.

A ideia e bem simples: quando eu quebrar a cabeca com algum erro, eu nao quero so resolver e esquecer. Eu quero guardar:

- qual era o erro
- em qual linguagem aconteceu
- o que causou
- como eu resolvi
- se ainda esta aberto ou se ja foi resolvido

No fim, o MuseuDeBugs vira tipo um caderno de bugs. So que em vez de ficar tudo espalhado em anotacao solta, fica salvo numa API de verdade, com banco, regras, login admin e um front proprio.

## Estado atual

Hoje o backend ja faz bastante coisa:

- cria bug
- lista bugs
- busca bug por id
- edita bug
- marca bug como resolvido
- deleta bug
- filtra por status e linguagem
- salva no MySQL usando Entity Framework Core
- valida os dados que chegam na API
- tem login admin por cookie
- protege rotas de escrita com `[Authorize(Roles = "Admin")]`
- libera CORS para front local em Angular ou Vite
- tem Swagger em desenvolvimento
- tem testes automatizados do `BugService`
- tem uma ferramenta para gerar hash da senha do admin

O frontend Angular tambem ja saiu da fase de maquete:

- lista bugs reais vindos da API
- busca bug por id e tambem filtra a lista por texto digitado
- filtra o acervo por status e linguagem pelos pads do `GrooveStripComponent`
- mostra cards com dados vindos do banco
- mostra estatisticas reais no painel lateral
- mostra linguagens mais usadas com base nos bugs cadastrados
- tem login admin por cookie na sidebar
- cria bug pelo painel lateral quando o admin esta logado
- atualiza a lista e as estatisticas depois de criar um bug
- tem logo propria na sidebar e favicon do projeto
- usa `HttpClient`, services, models e componentes standalone

Ou seja: a API ja nao esta mais so no "hello world", e o front ja conversa com ela de verdade.

## Como eu penso esse projeto

Tem dois tipos de uso:

```text
Visitante:
  pode ver os bugs.

Admin:
  faz login.
  recebe um cookie.
  pode criar, editar, resolver e deletar bugs.
```

O front ate pode esconder botoes, mas quem precisa bloquear de verdade e o backend.

Entao a regra mental e:

```text
GET publico.
Escrita so com admin logado.
```

## Tecnologias

- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- Pomelo Entity Framework Core MySQL
- MySQL
- Cookie Authentication
- Authorization com role `Admin`
- Rate Limiting do ASP.NET Core
- Swagger / OpenAPI
- xUnit
- EF Core InMemory nos testes
- Angular
- TypeScript
- RxJS / Observable
- Angular HttpClient
- HTML e CSS

## Estrutura do projeto

```text
MuseuDeBugs.Api/
  Controllers/
    AuthController.cs
    BugsController.cs
  Data/
    AppDbContext.cs
  DTOs/
    Auth/
      LoginRequest.cs
      MeResponse.cs
    AtualizarBugRequest.cs
    BugResponse.cs
    CriarBugRequest.cs
  Entities/
    Bug.cs
  Enums/
    StatusBug.cs
  Options/
    AdminOptions.cs
  Security/
    AdminOnlyAttribute.cs
    LoginAttemptLimiter.cs
  Services/
    AuthService.cs
    BugService.cs

MuseuDeBugs.Tests/
  Services/
    BugServiceTests.cs

MuseuDeBugs.Tools/
  Program.cs

frontend/
  public/
    apple-touch-icon.png
    brand-face.svg
    favicon.ico
    favicon.svg
    favicon-16x16.png
    favicon-32x32.png
    favicon-512x512.png
    museu-de-bugs-logo.png
  src/
    app/
      components/
        auth-panel/
        bug-card/
        bug-create-panel/
        bug-grid/
        groove-strip/
        right-panel/
        sidebar/
        topbar/
      config/
        api.config.ts
      models/
        atualizar-bug-response.ts
        bug-card.ts
        bug-filter.ts
        bug-response.ts
        criar-bug-request.ts
        login-request.ts
        me-response.ts
      services/
        auth.service.ts
        bug.service.ts
```

Obs: `AdminOnlyAttribute.cs` ficou como registro da fase antiga, quando eu estava usando chave no header. A protecao principal agora e por login com cookie e `[Authorize]`.

## Como rodar o backend

Na raiz do projeto:

```bash
dotnet restore MuseuDeBugs.slnx
dotnet build MuseuDeBugs.slnx
dotnet run --project MuseuDeBugs.Api/MuseuDeBugs.Api.csproj --launch-profile http
```

A API sobe em:

```text
http://localhost:5041
```

Swagger, em desenvolvimento:

```text
http://localhost:5041/swagger/index.html
```

## Como rodar o frontend

Em outro terminal:

```bash
cd frontend
npm install
npm start
```

O Angular sobe em:

```text
http://localhost:4200
```

Para testar a aplicacao completa, deixe os dois rodando ao mesmo tempo:

```text
Backend:  http://localhost:5041
Frontend: http://localhost:4200
```

Para gerar build do front:

```bash
cd frontend
npm run build
```

## Banco local

O projeto usa MySQL.

A connection string local fica em:

```text
MuseuDeBugs.Api/appsettings.Development.json
```

Esse arquivo nao vai para o GitHub. Ele esta no `.gitignore`, justamente porque pode ter senha local.

Exemplo sem segredo real:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=MuseuDeBugs;user=museu_user;password=SUA_SENHA_LOCAL"
  }
}
```

Para entrar no banco pelo terminal:

```bash
mysql -u museu_user -p MuseuDeBugs
```

Consultas que ajudam:

```sql
SELECT * FROM Bugs;
SELECT Id, Titulo, Linguagem, Status, DataCriacao, DataAtualizacao FROM Bugs;
SELECT * FROM Bugs ORDER BY DataCriacao DESC;
```

## Admin local

O admin tambem fica no `appsettings.Development.json`.

Exemplo:

```json
{
  "Admin": {
    "Username": "hnd",
    "PasswordHash": "COLE_O_HASH_AQUI"
  }
}
```

Aqui tem uma regra importante:

```text
senha pura nunca fica salva.
o que fica salvo e o hash da senha.
```

## Gerando hash da senha

Eu criei um projetinho separado so para gerar o hash:

```bash
dotnet run --project MuseuDeBugs.Tools/MuseuDeBugs.Tools.csproj
```

Ele pede o username e a senha. Depois cospe um texto grande, que e o hash.

Esse hash vai em:

```text
Admin:PasswordHash
```

## Configuracao em producao

Quando isso for para internet, a API nao deve depender do `appsettings.Development.json`.

O certo e configurar por variaveis de ambiente:

```text
Admin__Username=hnd
Admin__PasswordHash=HASH_GERADO
ConnectionStrings__DefaultConnection=CONNECTION_STRING_SEGURA
ASPNETCORE_ENVIRONMENT=Production
```

No .NET, `__` vira `:`.

Entao isso:

```text
Admin__Username
```

vira isso:

```text
Admin:Username
```

## Rotas publicas

Essas rotas podem ser chamadas sem login:

```http
GET /api/bugs
GET /api/bugs/{id}
POST /api/auth/login
POST /api/auth/logout
GET /api/auth/me
```

`/api/auth/me` serve para o front perguntar se tem alguem logado.

## Rotas protegidas

Essas aqui precisam de admin logado:

```http
POST /api/bugs
PUT /api/bugs/{id}
PATCH /api/bugs/{id}/resolver
DELETE /api/bugs/{id}
```

Sem login, a API devolve:

```http
401 Unauthorized
```

## Testando login com curl

Primeiro eu faco login e salvo o cookie:

```bash
curl -i -c cookies.txt -X POST http://localhost:5041/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "hnd",
    "password": "sua-senha"
  }'
```

Depois eu uso o cookie para chamar uma rota protegida:

```bash
curl -i -b cookies.txt -X POST http://localhost:5041/api/bugs \
  -H "Content-Type: application/json" \
  -d '{
    "titulo": "Bug criado logado",
    "linguagem": "C#",
    "descricao": "Descricao valida para testar login com cookie."
  }'
```

Para sair:

```bash
curl -i -b cookies.txt -X POST http://localhost:5041/api/auth/logout
```

## CORS e front

Por enquanto a API aceita front local nessas origens:

```text
http://localhost:4200
http://localhost:5173
```

`4200` e o padrao do Angular.

`5173` e o padrao do Vite.

Como o login usa cookie, no Angular as chamadas precisam mandar credenciais:

```ts
this.http.post(url, body, { withCredentials: true });
```

Sem isso, o navegador pode nao mandar o cookie junto.

## Endpoints de bugs

Base:

```text
/api/bugs
```

### Criar bug

```http
POST /api/bugs
```

Precisa estar logado.

Body:

```json
{
  "titulo": "NullReferenceException ao acessar propriedade",
  "linguagem": "C#",
  "mensagemErro": "System.NullReferenceException",
  "descricao": "A excecao apareceu ao tentar acessar uma propriedade de um objeto que estava null.",
  "causa": "O objeto nao foi inicializado antes do uso.",
  "solucao": "Verificar se o objeto foi criado e validar null antes de acessar."
}
```

Retornos principais:

- `201 Created`
- `400 Bad Request`
- `401 Unauthorized`

### Listar bugs

```http
GET /api/bugs
```

Essa rota e publica.

Filtros:

```http
GET /api/bugs?status=Aberto
GET /api/bugs?status=Resolvido
GET /api/bugs?linguagem=C%23
GET /api/bugs?status=Aberto&linguagem=SQL
```

### Buscar por id

```http
GET /api/bugs/{id}
```

Essa rota tambem e publica.

Retornos principais:

- `200 OK`
- `404 Not Found`

### Atualizar bug

```http
PUT /api/bugs/{id}
```

Precisa estar logado.

Body:

```json
{
  "titulo": "NullReferenceException ao acessar propriedade",
  "linguagem": "C#",
  "mensagemErro": "System.NullReferenceException",
  "descricao": "Descricao atualizada com o que foi aprendido.",
  "causa": "Causa atualizada.",
  "solucao": "Solucao atualizada."
}
```

Retornos principais:

- `200 OK`
- `400 Bad Request`
- `401 Unauthorized`
- `404 Not Found`

### Resolver bug

```http
PATCH /api/bugs/{id}/resolver
```

Precisa estar logado.

Retornos principais:

- `200 OK`
- `401 Unauthorized`
- `404 Not Found`

### Deletar bug

```http
DELETE /api/bugs/{id}
```

Precisa estar logado.

Retornos principais:

- `204 No Content`
- `401 Unauthorized`
- `404 Not Found`

## Validacoes

Campos obrigatorios:

- `Titulo`: 3 ate 120 caracteres
- `Linguagem`: 1 ate 50 caracteres
- `Descricao`: 10 ate 2000 caracteres

Campos opcionais, mas com limite:

- `MensagemErro`: ate 500 caracteres
- `Causa`: ate 2000 caracteres
- `Solucao`: ate 2000 caracteres

Isso e para evitar mandar um texto gigante onde deveria entrar so uma anotacao.

## Fluxo interno

O caminho normal da API e esse:

```text
Cliente / Swagger / Front
        |
        v
Controller
        |
        v
Service
        |
        v
AppDbContext
        |
        v
MySQL
```

Separando as responsabilidades:

- `Controller` recebe HTTP
- `Service` guarda a regra do sistema
- `Bug` e a entidade principal
- `AppDbContext` conversa com o banco
- `Request` e o que entra
- `Response` e o que sai

## Testes

Para rodar:

```bash
dotnet test MuseuDeBugs.slnx
```

Hoje os testes cobrem o `BugService`:

- criar bug aberto
- buscar bug existente
- retornar `null` quando nao acha
- marcar como resolvido
- atualizar bug
- filtrar por status e linguagem
- tentar deletar id inexistente
- deletar bug existente

Ainda quero colocar testes de API depois, principalmente para login e rotas protegidas.

## Seguranca que ja tem

Coisas que ja foram colocadas:

- senha do admin guardada como hash
- login por cookie
- cookie `HttpOnly`
- cookie `Secure` em producao
- cookie `SameSite=Lax`
- rotas admin com `[Authorize(Roles = "Admin")]`
- CORS restrito para front local
- login com limite de tentativas
- bloqueio temporario depois de muitas tentativas erradas
- limite de tamanho nos DTOs
- headers basicos de seguranca
- `Cache-Control: no-store` nas rotas principais da API
- nada de senha na URL

Antes de publicar de verdade, ainda preciso revisar:

- HTTPS com certificado valido
- variaveis de ambiente no servidor
- dominio real no CORS
- Swagger sem segredo real
- banco com senha forte
- banco sem ficar aberto para qualquer IP
- logs sem senha, hash, cookie ou connection string
- backup, se os dados ficarem importantes

## Front Angular - estado atual

O front fica em:

```text
frontend/
```

Ele usa Angular standalone, sem `AppModule`.

Configuracoes importantes:

- `app.config.ts`: registra `provideHttpClient()`
- `api.config.ts`: guarda `http://localhost:5041/api`
- `bug.service.ts`: chama endpoints de bugs
- `auth.service.ts`: chama login, logout e me

### Funcionalidades ja conectadas

Hoje o front ja faz:

- lista bugs reais com `GET /api/bugs`
- busca bug por id com `GET /api/bugs/{id}`
- filtra a lista por texto no proprio front
- filtra a lista por status e linguagem usando os pads do topo
- login admin com `POST /api/auth/login`
- verifica sessao com `GET /api/auth/me`
- logout com `POST /api/auth/logout`
- cria bug com `POST /api/bugs`
- atualiza o grid depois de criar bug
- atualiza estatisticas depois de criar bug
- usa assets proprios de marca, favicon e apple touch icon

### Componentes principais

```text
SidebarComponent
  mostra a marca do app usando a logo
  mostra o painel de login admin

AuthPanelComponent
  formulario de login
  estado logado/deslogado
  botao de logout

TopbarComponent
  recebe texto de busca
  quando o texto e numerico, tambem busca bug por id na API

GrooveStripComponent
  controla os filtros rapidos por status e linguagem

BugGridComponent
  lista cards de bugs
  mostra resultado da busca por id
  aplica filtros e busca textual
  recebe bug criado e atualiza a lista

BugCardComponent
  renderiza um bug individual

RightPanelComponent
  painel lateral com criar bug, destaque, estatisticas e linguagens

BugCreatePanelComponent
  formulario para criar bug
  chama rota protegida usando cookie
```

### Services do front

`BugService`:

```ts
listar(status?: string, linguagem?: string): Observable<BugResponse[]>
buscarPorId(id: number): Observable<BugResponse>
criar(request: CriarBugRequest): Observable<BugResponse>
```

`AuthService`:

```ts
login(request: LoginRequest): Observable<MeResponse>
logout(): Observable<void>
me(): Observable<MeResponse>
```

Como o auth usa cookie, as chamadas de login/logout/me e rotas protegidas usam:

```ts
{ withCredentials: true }
```

### Fluxo de login no front

```text
AuthPanelComponent
  chama authService.me() ao abrir
  se estiver logado, mostra usuario
  se nao estiver, mostra formulario

Login
  envia username/senha
  backend valida
  backend cria cookie museu_admin
  navegador guarda cookie

Criar bug
  Angular manda POST /api/bugs com withCredentials
  backend le cookie
  backend confere role Admin
  se estiver certo, cria bug
```

## Proximos passos do front

O front ja esta conectado e navegavel, mas ainda tem espaco para evoluir. As proximas melhorias mais naturais sao:

- editar bug pelo front
- marcar bug como resolvido pelo front
- deletar bug pelo front
- melhorar telas de vazio, loading e erro
- criar filtros melhores por linguagem
- decidir se os filtros do grid continuam locais ou passam a chamar os filtros da API
- remover ou ajustar mocks antigos
- pensar em rotas Angular quando fizer sentido
- proteger areas admin com guard, se o app ganhar rotas admin separadas
- revisar responsividade com mais calma
- quebrar alguns componentes quando eles comecarem a crescer demais

## Aprendizado importante

Teve um ponto importante com EF Core:

```text
Antes do ToList: ainda e mundo do banco / SQL.
Depois do ToList: ja e mundo do C#.
```

Isso importa porque nem todo metodo C# consegue virar SQL.

## Proximos passos

- adicionar testes de API para auth
- implementar editar, resolver e deletar bug pelo front
- testar o fluxo inteiro com cookie no navegador
- revisar e padronizar nomes internos para ingles antes de publicar
- revisar hardening de producao antes de publicar

## Checklist antes de deploy publico

Antes de colocar no ar de verdade, eu quero tratar isso como uma etapa separada:

- refatorar nomes de classes, DTOs, variaveis e metodos para ingles
- manter o nome `MuseuDeBugs` como identidade do projeto, se fizer sentido
- revisar variaveis de ambiente de producao
- configurar HTTPS
- trocar CORS local por dominio real
- garantir que Swagger nao exponha nada sensivel
- revisar banco, usuario, senha forte, acesso por IP e backup
- testar login, logout, criacao e rotas protegidas no navegador
