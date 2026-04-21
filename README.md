# MuseuDeBugs

MuseuDeBugs e uma API em C# para registrar erros encontrados durante estudos e projetos.

A ideia e transformar erros reais em memoria tecnica reutilizavel: cada bug guarda contexto, mensagem de erro, causa, solucao, status e datas. Assim, quando um problema parecido aparecer de novo, fica mais facil consultar o historico e lembrar o caminho da correcao.

## Status atual

O fluxo funcional da fase 1 esta implementado:

- criar bug
- listar bugs
- buscar bug por id
- marcar bug como resolvido
- persistir dados em MySQL com Entity Framework Core
- testar endpoints pelo Swagger UI

O projeto compila com:

```bash
dotnet build MuseuDeBugs.Api/MuseuDeBugs.Api.csproj
```

## Tecnologias

- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- Pomelo Entity Framework Core MySQL
- MySQL
- OpenAPI
- Swagger UI com Swashbuckle

## Estrutura principal

```text
MuseuDeBugs.Api/
  Controllers/
    BugsController.cs
  Data/
    AppDbContext.cs
  DTOs/
    CriarBugRequest.cs
    BugResponse.cs
  Entities/
    Bug.cs
  Enums/
    StatusBug.cs
  Services/
    BugService.cs
  Migrations/
```

## Rodando o projeto

Na raiz do repositorio:

```bash
dotnet run --project MuseuDeBugs.Api/MuseuDeBugs.Api.csproj --launch-profile http
```

A API deve subir em:

```text
http://localhost:5041
```

Swagger UI:

```text
http://localhost:5041/swagger/index.html
```

## Banco de dados

O projeto usa MySQL.

A connection string de desenvolvimento fica em `MuseuDeBugs.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=MuseuDeBugs;user=museu_user;password=..."
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

## Endpoints da fase 1

Base:

```text
/api/bugs
```

### Criar bug

```http
POST /api/bugs
```

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

Observacao: por enquanto o endpoint retorna `200 OK`. O ajuste para `201 Created` esta planejado como refinamento de contrato HTTP.

### Listar bugs

```http
GET /api/bugs
```

Retorna uma lista de `BugResponse`.

### Buscar bug por id

```http
GET /api/bugs/{id}
```

Retornos:

- `200 OK` quando encontra
- `404 Not Found` quando o id nao existe

### Marcar como resolvido

```http
PATCH /api/bugs/{id}/resolver
```

Retornos:

- `200 OK` com o bug atualizado
- `404 Not Found` quando o id nao existe

## Fluxo interno

Fluxo principal da API:

```text
Cliente/Swagger/curl
        |
        v
BugsController
        |
        v
BugService
        |
        v
AppDbContext
        |
        v
MySQL
```

Responsabilidades:

- `BugsController`: recebe requisicoes HTTP e devolve respostas HTTP
- `BugService`: executa os casos de uso
- `Bug`: representa a entidade principal do dominio
- `AppDbContext`: conversa com o banco via EF Core
- `CriarBugRequest`: representa dados de entrada
- `BugResponse`: representa dados de saida da API

## Aprendizado importante

No endpoint de listagem, foi encontrado um erro real do EF Core ao tentar usar um metodo C# dentro de uma consulta ainda ligada ao banco.

Forma corrigida:

```csharp
public List<BugResponse> ListarBugs()
{
    var bugs = _context.Bugs.ToList();

    return bugs
        .Select(bug => MapearParaResponse(bug))
        .ToList();
}
```

Regra:

```text
Antes do ToList: mundo do EF Core / SQL
Depois do ToList: mundo do C#
```

## Proximos passos

- testar novamente os endpoints pelo Swagger apos reiniciar a API
- conferir `PATCH /api/bugs/{id}/resolver` no MySQL
- ajustar `POST /api/bugs` para `201 Created`
- adicionar validacoes basicas no `CriarBugRequest`
- implementar filtros por status e linguagem
- implementar edicao de bug
- pensar em testes automatizados
