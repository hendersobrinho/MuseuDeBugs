# MuseuDeBugs

Esse projeto e uma API em C# que eu estou montando para guardar bugs que aparecem enquanto eu estudo ou faco projetos.

A ideia e bem simples: quando eu quebrar a cabeca com algum erro, eu nao quero so resolver e esquecer. Eu quero guardar:

- qual era o erro
- em qual linguagem aconteceu
- o que causou
- como eu resolvi
- se ainda esta aberto ou se ja foi resolvido

No fim, o MuseuDeBugs vira tipo um caderno de bugs. So que em vez de ficar tudo espalhado em anotacao solta, fica salvo numa API de verdade, com banco, regras, login admin e depois um front.

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

Ou seja: a API ja nao esta mais so no "hello world". Ela ja tem uma base real para virar aplicacao com painel.

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
```

Obs: `AdminOnlyAttribute.cs` ficou como registro da fase antiga, quando eu estava usando chave no header. A protecao principal agora e por login com cookie e `[Authorize]`.

## Como rodar

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

## Plano do front em Angular + TypeScript

A proxima parte e montar o front.

Eu imagino com duas areas:

```text
Area publica:
  lista bugs
  mostra detalhe de bug

Area admin:
  login
  painel
  criar bug
  editar bug
  resolver bug
  deletar bug
```

Rotas pensadas:

- `/bugs`
- `/bugs/:id`
- `/login`
- `/admin`
- `/admin/bugs/novo`
- `/admin/bugs/:id/editar`

No TypeScript, vou precisar de modelos como:

```ts
export interface BugResponse {
  id: number;
  titulo: string;
  linguagem: string;
  mensagemErro?: string | null;
  descricao: string;
  causa?: string | null;
  solucao?: string | null;
  status: string;
  dataCriacao: string;
  dataAtualizacao?: string | null;
}
```

Tambem vou criar:

- `CriarBugRequest`
- `AtualizarBugRequest`
- `LoginRequest`
- `MeResponse`

Services:

- `AuthService`: login, logout e `me`
- `BugService`: CRUD dos bugs
- `AuthGuard`: proteger rotas admin

Ordem que faz sentido:

1. Criar o app Angular.
2. Configurar `apiUrl` para `http://localhost:5041`.
3. Criar os modelos TypeScript.
4. Criar `AuthService` usando `withCredentials: true`.
5. Criar `BugService`.
6. Fazer lista publica.
7. Fazer detalhe publico.
8. Fazer login.
9. Fazer guard das rotas admin.
10. Fazer criar e editar bug.
11. Fazer resolver e deletar.
12. Ajustar carregamento, erro e tela vazia.

## Aprendizado importante

Teve um ponto importante com EF Core:

```text
Antes do ToList: ainda e mundo do banco / SQL.
Depois do ToList: ja e mundo do C#.
```

Isso importa porque nem todo metodo C# consegue virar SQL.

## Proximos passos

- adicionar testes de API para auth
- comecar o front em Angular
- testar o fluxo inteiro com cookie
- revisar hardening de producao antes de publicar
