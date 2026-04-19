# Diario do projeto

## Ideia principal

Esta nota sera o indice vivo da evolucao do projeto.

Ela nao substitui as notas de sessao. Ela centraliza:

- o estado atual
- as decisoes confirmadas
- o historico de sessoes
- os proximos passos

Ela funciona como uma nota de diario e de acompanhamento do contexto.

## Estado atual

Projeto:

- Museu de Bugs

Etapa atual:

- [[07 Roadmap e etapas de desenvolvimento|Etapa 1 - Setup do projeto]] ainda em andamento, com a API agora compilando e o fluxo `POST /api/bugs` iniciado de forma parcial

Proxima etapa:

- fechar a persistencia do primeiro fluxo antes de expandir os demais endpoints da fase 1

## Decisoes confirmadas ate agora

- o sistema sera uma base pessoal de aprendizado sobre erros tecnicos
- a stack atual sera C# + .NET + ASP.NET Core + Entity Framework Core + MySQL
- a primeira versao sera API sem frontend
- a primeira entidade sera `Bug`
- a primeira versao tera um fluxo pequeno e completo
- o projeto vai usar o OpenAPI nativo que ja esta referenciado no `.csproj`, sem `Swashbuckle.AspNetCore` neste momento
- o diario deve refletir o estado real da implementacao, mesmo quando o planejamento estiver mais avancado que o codigo

## Historico de sessoes

- [[2026-04-14 - Descoberta e planejamento]]
- [[2026-04-14 - Setup inicial da API]]
- [[2026-04-16 - Alinhamento da persistencia e estudo de EF Core]]
- [[2026-04-19 - Consolidacao do estado atual e ajuste do bootstrap da API]]

## Como atualizar este diario

Ao final de cada sessao:

1. criar uma nova nota usando [[Template - Sessao do projeto]]
2. adicionar o link da nova sessao aqui
3. atualizar a etapa atual
4. registrar o proximo passo

## Backlog atual

Curto prazo:

- alinhar o provider atual com MySQL
- implementar `AppDbContext`
- registrar o contexto no `Program.cs`
- persistir `POST /api/bugs`
- ajustar a resposta de criacao para `201 Created`

Medio prazo:

- implementar `GET /api/bugs`
- implementar `GET /api/bugs/{id}`
- implementar `PATCH /api/bugs/{id}/resolver`

Longo prazo:

- editar bug
- filtros por status e linguagem
- tags
- estatisticas
- historico de alteracoes

## Duvidas abertas

Duvidas que podem ser decididas depois:

- vamos manter so `MuseuDeBugs.slnx` ou tambem `MuseuDeBugs.Api.sln`?
- a connection string principal deve ficar em `appsettings.json`, em `appsettings.Development.json` ou nos dois com papeis diferentes?
- vamos usar Flyway cedo ou so depois?
- vamos arquivar bugs em vez de deletar?
- tags entram antes ou depois dos filtros?

## Hubs conectados a esta nota

- [[Programacao]]
- [[Csharp]]
- [[SQL]]
- [[Banco de Dados]]
- [[API REST]]
- [[Modelagem de Dominio]]
- [[CRUD]]
- [[Entity Framework Core]]

## Relaciona com

- [[Programacao]]
- [[00 Mapa Geral - Museu de Bugs]]
- [[06 Features e ordem de implementacao]]
- [[07 Roadmap e etapas de desenvolvimento]]

## Conclusao

Se a gente quiser entender rapidamente onde o projeto esta, esta nota continua sendo o primeiro lugar para abrir.

Hoje o ponto central e este: a API ja compila, o primeiro fluxo esta esbocado, mas a persistencia ainda e o gargalo que separa a estrutura montada de um MVP funcional.
