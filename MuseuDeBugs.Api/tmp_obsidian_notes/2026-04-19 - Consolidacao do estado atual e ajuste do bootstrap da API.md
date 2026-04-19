# 2026-04-19 - Consolidacao do estado atual e ajuste do bootstrap da API

## Objetivo da sessao

- registrar a evolucao real do projeto em relacao ao que estava documentado
- ajustar o bootstrap da API para voltar a compilar sem depender de pacote nao instalado
- deixar claro o que ja existe, o que esta parcial e o que ainda nao saiu do papel

## O que foi implementado

- foi confirmado que a estrutura planejada da fase 1 ja existe em boa parte no codigo: `Bug`, `StatusBug`, `CriarBugRequest`, `BugResponse`, `BugService` e `BugsController`
- o `Program.cs` foi ajustado para o estado real do projeto: agora registra `AddControllers`, `AddOpenApi`, `BugService` e `MapControllers`
- as chamadas `AddSwaggerGen`, `UseSwagger` e `UseSwaggerUI` foram removidas porque o projeto nao possui `Swashbuckle.AspNetCore`
- o build voltou a passar com `dotnet build`
- o fluxo inicial de criacao de bug ja esta desenhado de ponta a ponta no codigo, mas ainda sem persistencia: controller recebe `CriarBugRequest`, service cria `Bug` e devolve `BugResponse`
- foi confirmado que `appsettings.Development.json` ja contem uma connection string de MySQL, embora a persistencia ainda nao esteja fechada
- foi registrado que existe `MuseuDeBugs.slnx` na raiz do projeto e tambem `MuseuDeBugs.Api.sln` dentro da pasta da API, o que ajuda a explicar parte da confusao de execucao

## Estado atual em relacao a documentacao

- o passo de limpar o template antigo esta essencialmente concluido
- o dominio inicial esta parcialmente pronto, porque `Bug` e `StatusBug` existem, mas `AppDbContext` segue vazio
- o fluxo `POST /api/bugs` foi iniciado, mas ainda nao cumpre todo o contrato documentado em [[12 Contrato da API - fase 1]]
- a documentacao da fase 1 previa persistencia com MySQL, mas o `.csproj` ainda referencia `Microsoft.EntityFrameworkCore.SqlServer`
- a arquitetura planejada em [[09 Arquitetura detalhada da API]] ja aparece na separacao por camadas, mas a camada `Data` ainda nao entrou em funcionamento

## Decisoes tomadas

- o projeto passa a usar apenas o OpenAPI nativo ja referenciado no `.csproj`, sem Swagger do Swashbuckle por enquanto
- o estado do projeto deve ser lido como uma transicao entre setup da API, dominio inicial e inicio do fluxo principal
- o diario precisa registrar o estado real do codigo, sem assumir que a persistencia ja esta pronta so porque a estrutura de pastas existe

## Problemas encontrados

- o build quebrava porque `Program.cs` chamava metodos de Swagger sem o pacote correspondente
- `AppDbContext.cs` continua vazio
- o provider atual continua desalinhado com a decisao documentada de usar MySQL
- o endpoint `POST /api/bugs` ainda devolve `200 OK` em vez de `201 Created`
- ainda nao ha validacao basica de entrada nem persistencia no banco
- `dotnet run` neste ambiente de trabalho falhou ao bindar `http://localhost:5041`, mas isso apareceu como limitacao do sandbox, nao como erro de compilacao da API

## O que ficou pendente

- trocar `Microsoft.EntityFrameworkCore.SqlServer` por um provider de MySQL
- implementar `AppDbContext` com `DbSet<Bug>`
- registrar o contexto no `Program.cs`
- decidir onde a connection string final deve morar entre `appsettings.json` e `appsettings.Development.json`
- persistir de verdade o fluxo de criacao
- implementar `GET /api/bugs`
- implementar `GET /api/bugs/{id}`
- implementar `PATCH /api/bugs/{id}/resolver`
- alinhar o controller com o contrato HTTP documentado

## Proximo passo sugerido

1. alinhar o pacote de persistencia com MySQL
2. preencher `AppDbContext`
3. registrar `UseMySql` no `Program.cs`
4. salvar `Bug` no banco dentro do `BugService`
5. ajustar `POST /api/bugs` para responder `201 Created`
6. seguir para listagem, busca por id e resolucao de status

## Hubs desta sessao

- [[Programacao]]
- [[Csharp]]
- [[SQL]]
- [[Banco de Dados]]
- [[API REST]]
- [[Modelagem de Dominio]]
- [[CRUD]]
- [[Entity Framework Core]]

## Notas relacionadas

- [[00 Mapa Geral - Museu de Bugs]]
- [[08 Diario do projeto]]
- [[07 Roadmap e etapas de desenvolvimento]]
- [[09 Arquitetura detalhada da API]]
- [[10 Plano de implementacao detalhado - fase 1]]
- [[12 Contrato da API - fase 1]]
- [[13 Implementacao por arquivo - fase 1]]

## Resumo

Esta sessao serviu para consolidar o estado real do projeto.

O codigo ja saiu da fase de estrutura vazia e entrou numa fase intermediaria: a API compila, o dominio inicial existe e o primeiro endpoint foi esbocado, mas a persistencia ainda nao entrou em cena e o contrato HTTP ainda nao esta fechado.
