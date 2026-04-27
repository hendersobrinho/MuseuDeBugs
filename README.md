# MuseuDeBugs

O MuseuDeBugs e uma API em C# para guardar bugs que apareceram nos estudos ou em projetos.

Pensa nele como uma caixinha organizada:

```text
Achei um erro.
Entendi o motivo.
Anotei a solucao.
Guardei para nao sofrer de novo depois.
```

Cada bug guarda coisas como titulo, linguagem, mensagem de erro, descricao, causa, solucao, status e datas.

O objetivo e simples: transformar erro em memoria.

## Como o projeto esta agora

Hoje a API ja consegue:

- criar um bug
- listar bugs
- buscar um bug pelo id
- editar um bug
- marcar um bug como resolvido
- deletar um bug
- filtrar bugs por status e linguagem
- salvar tudo em MySQL com Entity Framework Core
- validar os dados recebidos
- proteger rotas de admin com login por cookie
- aceitar chamadas do front local em Angular ou Vite
- mostrar a API no Swagger em desenvolvimento
- rodar testes automatizados do `BugService`
- gerar hash de senha do admin com `MuseuDeBugs.Tools`

## A ideia da seguranca

O projeto tem dois tipos de pessoa:

```text
Visitante:
  pode olhar os bugs.

Admin:
  faz login.
  recebe um cookie.
  pode criar, editar, resolver e deletar bugs.
```

O cookie e como uma pulseirinha de entrada.

Depois do login, o navegador guarda essa pulseirinha. Quando o admin chama uma rota protegida, o navegador manda o cookie junto, e a API confere se a pessoa pode passar.

## Rotas publicas

Essas rotas podem ser usadas sem login:

```http
GET /api/bugs
GET /api/bugs/{id}
POST /api/auth/login
POST /api/auth/logout
GET /api/auth/me
```

`/api/auth/me` tambem funciona sem login porque ele serve para perguntar:

```text
Tem alguem logado agora?
```

Se tiver, ele responde quem e. Se nao tiver, ele responde que nao tem admin autenticado.

## Rotas protegidas

Essas rotas precisam de login admin:

```http
POST /api/bugs
PUT /api/bugs/{id}
PATCH /api/bugs/{id}/resolver
DELETE /api/bugs/{id}
```

Se tentar usar sem login, a API deve responder:

```http
401 Unauthorized
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
- OpenAPI
- Swagger UI com Swashbuckle
- xUnit
- EF Core InMemory para testes

## Estrutura principal

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

`AdminOnlyAttribute.cs` ficou como estudo da fase anterior. A protecao real das rotas admin agora usa:

```csharp
[Authorize(Roles = "Admin")]
```

## Rodando a API

Na raiz do repositorio:

```bash
dotnet restore MuseuDeBugs.slnx
dotnet build MuseuDeBugs.slnx
dotnet run --project MuseuDeBugs.Api/MuseuDeBugs.Api.csproj --launch-profile http
```

A API sobe aqui:

```text
http://localhost:5041
```

Em desenvolvimento, o Swagger fica aqui:

```text
http://localhost:5041/swagger/index.html
```

## Configurando o banco local

O projeto usa MySQL.

No seu computador, a connection string fica em:

```text
MuseuDeBugs.Api/appsettings.Development.json
```

Esse arquivo esta no `.gitignore`, porque ele pode ter senha local.

Exemplo sem senha real:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=MuseuDeBugs;user=museu_user;password=SUA_SENHA_LOCAL"
  }
}
```

Para consultar manualmente:

```bash
mysql -u museu_user -p MuseuDeBugs
```

Consultas uteis:

```sql
SELECT * FROM Bugs;
SELECT Id, Titulo, Linguagem, Status, DataCriacao, DataAtualizacao FROM Bugs;
SELECT * FROM Bugs ORDER BY DataCriacao DESC;
```

## Configurando o admin local

O admin tambem fica no:

```text
MuseuDeBugs.Api/appsettings.Development.json
```

Exemplo sem segredo real:

```json
{
  "Admin": {
    "Username": "hnd",
    "PasswordHash": "COLE_O_HASH_AQUI"
  }
}
```

Regra de ouro:

```text
senha pura nao fica salva.
hash da senha pode ficar na configuracao.
```

## Gerando o hash da senha

Use a ferramenta do projeto:

```bash
dotnet run --project MuseuDeBugs.Tools/MuseuDeBugs.Tools.csproj
```

Ela vai pedir:

```text
Username do admin
Senha do admin
```

Depois ela mostra um texto grande. Esse texto e o hash.

Cole o hash em:

```text
Admin:PasswordHash
```

Nao coloque a senha pura no `appsettings`.

## Configurando em producao

Na internet, a API nao deve depender de `appsettings.Development.json`.

Use variaveis de ambiente:

```text
Admin__Username=hnd
Admin__PasswordHash=HASH_GERADO
ConnectionStrings__DefaultConnection=CONNECTION_STRING_SEGURA
ASPNETCORE_ENVIRONMENT=Production
```

No .NET, `__` significa "entra dentro da caixinha".

Entao:

```text
Admin__Username
```

vira:

```text
Admin:Username
```

## Testando login com curl

Primeiro faz login e guarda o cookie:

```bash
curl -i -c cookies.txt -X POST http://localhost:5041/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "hnd",
    "password": "sua-senha"
  }'
```

Depois usa o cookie para criar um bug:

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

## CORS para o front

A API aceita chamadas locais vindas de:

```text
http://localhost:4200
http://localhost:5173
```

`4200` e a porta comum do Angular.

`5173` e a porta comum do Vite.

Como o login usa cookie, o front precisa chamar a API com credenciais.

No Angular, isso aparece assim:

```ts
this.http.post(url, body, { withCredentials: true });
```

## Endpoints de bugs

Base:

```text
/api/bugs
```

### Criar bug

```http
POST /api/bugs
```

Precisa estar logado como admin.

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

Retornos comuns:

- `201 Created` quando cria
- `400 Bad Request` quando algum dado obrigatorio esta ruim
- `401 Unauthorized` quando nao tem login admin

### Listar bugs

```http
GET /api/bugs
```

Essa rota e publica.

Filtros opcionais:

```http
GET /api/bugs?status=Aberto
GET /api/bugs?status=Resolvido
GET /api/bugs?linguagem=C%23
GET /api/bugs?status=Aberto&linguagem=SQL
```

### Buscar bug por id

```http
GET /api/bugs/{id}
```

Essa rota e publica.

Retornos comuns:

- `200 OK` quando encontra
- `404 Not Found` quando o id nao existe

### Atualizar bug

```http
PUT /api/bugs/{id}
```

Precisa estar logado como admin.

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

Retornos comuns:

- `200 OK` quando atualiza
- `400 Bad Request` quando algum dado obrigatorio esta ruim
- `401 Unauthorized` quando nao tem login admin
- `404 Not Found` quando o id nao existe

### Marcar como resolvido

```http
PATCH /api/bugs/{id}/resolver
```

Precisa estar logado como admin.

Retornos comuns:

- `200 OK` com o bug resolvido
- `401 Unauthorized` quando nao tem login admin
- `404 Not Found` quando o id nao existe

### Deletar bug

```http
DELETE /api/bugs/{id}
```

Precisa estar logado como admin.

Retornos comuns:

- `204 No Content` quando remove
- `401 Unauthorized` quando nao tem login admin
- `404 Not Found` quando o id nao existe

## Validacoes dos bugs

Pense nos campos como caixinhas.

Algumas caixinhas precisam vir preenchidas:

- `Titulo`: obrigatorio, de 3 ate 120 caracteres
- `Linguagem`: obrigatoria, de 1 ate 50 caracteres
- `Descricao`: obrigatoria, de 10 ate 2000 caracteres

Outras podem vir vazias, mas tem limite:

- `MensagemErro`: ate 500 caracteres
- `Causa`: ate 2000 caracteres
- `Solucao`: ate 2000 caracteres

Isso evita que alguem tente mandar um texto gigante onde deveria caber so uma anotacao.

## Fluxo interno

Quando alguem chama a API, o caminho principal e:

```text
Cliente, Swagger ou front
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

Traduzindo:

- `Controller` recebe a chamada HTTP
- `Service` faz a regra do sistema
- `Bug` representa o bug guardado
- `AppDbContext` conversa com o banco
- `CriarBugRequest` e `AtualizarBugRequest` sao entradas
- `BugResponse` e a resposta que sai da API

## Testes automatizados

Os testes ficam em:

```text
MuseuDeBugs.Tests/
```

Para rodar:

```bash
dotnet test MuseuDeBugs.slnx
```

Hoje os testes cobrem o `BugService`:

- criar bug com status `Aberto`
- buscar bug existente
- retornar `null` quando o id nao existe
- marcar bug como `Resolvido`
- atualizar dados do bug
- filtrar por status
- filtrar por linguagem
- retornar `false` ao deletar id inexistente
- remover bug existente

Um bom proximo passo e adicionar testes de API para login e rotas protegidas.

## Cuidados de seguranca ja aplicados

O projeto ja tem estas camadas:

- senha do admin guardada como hash
- login por cookie
- cookie `HttpOnly`
- cookie `Secure` em producao
- cookie `SameSite=Lax`
- rotas admin com `[Authorize(Roles = "Admin")]`
- CORS restrito para portas locais conhecidas
- chamadas com credenciais para o front
- limite de tentativas no endpoint de login
- bloqueio temporario depois de muitas senhas erradas
- tamanho maximo nos DTOs
- headers basicos de seguranca
- `Cache-Control: no-store` em rotas da API principal
- sem senha na URL

Antes de publicar na internet, ainda precisa conferir no ambiente real:

- HTTPS funcionando com certificado valido
- variaveis de ambiente configuradas
- Swagger sem segredo real
- banco com senha forte
- banco sem ficar aberto para qualquer IP
- CORS usando o dominio real do front
- logs sem senha, cookie, hash ou connection string
- backups se os dados forem importantes

## Plano do front em Angular e TypeScript

O front pode nascer como um painel simples, mas organizado.

### Telas publicas

- `/bugs`: lista publica dos bugs
- `/bugs/:id`: detalhe publico de um bug

Essas telas nao precisam de login.

O visitante consegue ver:

- titulo
- linguagem
- status
- descricao
- causa
- solucao
- data de criacao
- data de atualizacao

### Telas admin

- `/login`: entrada do admin
- `/admin`: painel com resumo
- `/admin/bugs/novo`: formulario para criar bug
- `/admin/bugs/:id/editar`: formulario para editar bug

Essas telas usam login por cookie.

O admin consegue:

- criar bug
- editar bug
- marcar como resolvido
- deletar bug
- sair da conta

### Pecas de TypeScript

Modelos principais:

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

Tambem vale criar:

```text
CriarBugRequest
AtualizarBugRequest
LoginRequest
MeResponse
```

Services principais:

- `AuthService`: login, logout e me
- `BugService`: listar, buscar, criar, editar, resolver e deletar
- `AuthGuard`: protege as rotas admin

### Ordem boa para construir

1. Criar o app Angular.
2. Configurar `apiUrl` apontando para `http://localhost:5041`.
3. Criar os modelos TypeScript.
4. Criar `AuthService` com `withCredentials: true`.
5. Criar `BugService`.
6. Fazer tela publica de lista.
7. Fazer tela publica de detalhe.
8. Fazer login admin.
9. Fazer guard das rotas admin.
10. Fazer formulario de criar e editar bug.
11. Fazer botoes de resolver e deletar.
12. Ajustar estados de carregando, vazio e erro.

### Regra simples para o front

O front pode esconder botoes, mas quem manda de verdade e o backend.

Mesmo que o botao de deletar fique invisivel para visitante, a API tambem precisa bloquear.

E ela ja bloqueia.

## Aprendizado importante

No endpoint de listagem, apareceu um aprendizado real do EF Core:

```text
Antes do ToList: mundo do EF Core / SQL.
Depois do ToList: mundo do C#.
```

Ou seja:

```text
Se ainda esta montando consulta para o banco, cuidado com metodo C# comum.
Se ja trouxe os dados para memoria, ai o C# trabalha tranquilo.
```

## Proximos passos

- adicionar testes de API para login e autorizacao
- criar o front Angular com TypeScript
- testar o fluxo completo: login, criar bug, listar, editar, resolver e deletar
- trocar as origens CORS locais pelo dominio real quando publicar
- revisar hardening de producao antes de colocar na internet
