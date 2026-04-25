# MuseuDeBugs

MuseuDeBugs e uma API em C# para registrar erros encontrados durante estudos e projetos.

A ideia e transformar erros reais em memoria tecnica reutilizavel: cada bug guarda contexto, mensagem de erro, causa, solucao, status e datas. Assim, quando um problema parecido aparecer de novo, fica mais facil consultar o historico e lembrar o caminho da correcao.

## Status atual

O fluxo funcional da fase 1 esta implementado e alguns refinamentos da fase 2 ja entraram:

- criar bug
- listar bugs
- buscar bug por id
- marcar bug como resolvido
- persistir dados em MySQL com Entity Framework Core
- validar campos obrigatorios no request de criacao
- filtrar listagem por status e linguagem
- testar endpoints pelo Swagger UI

## O que mudou recentemente

Mudancas confirmadas no codigo atual:

- `POST /api/bugs` agora retorna `201 Created` com `CreatedAtAction`
- `CriarBugRequest` agora usa validacoes com Data Annotations
- `GET /api/bugs` agora aceita filtros opcionais por `status` e `linguagem`
- a base para edicao com `AtualizarBugRequest` e `Bug.Atualizar(...)` ja foi iniciada, mas o endpoint HTTP ainda nao foi exposto

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
    AtualizarBugRequest.cs
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

Retornos:

- `201 Created` quando cria com sucesso
- `400 Bad Request` quando os dados obrigatorios sao enviados de forma invalida

Validacoes basicas atuais:

- `Titulo`: obrigatorio, minimo de 3 caracteres, maximo de 120
- `Linguagem`: obrigatorio, minimo de 3 caracteres, maximo de 50
- `Descricao`: obrigatoria, minimo de 10 caracteres

### Listar bugs

```http
GET /api/bugs
```

Retorna uma lista de `BugResponse`.

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

- testar os filtros de listagem com mais combinacoes reais no Swagger
- implementar o endpoint HTTP de edicao de bug
- decidir se vai existir `DELETE` ou arquivamento
- pensar em testes automatizados
- revisar limpeza e padrao do codigo conforme a API crescer
