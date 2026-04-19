# Roadmap e etapas de desenvolvimento

## Ideia principal

Esta nota transforma as features em etapas praticas.

A diferenca e simples:

- a nota de features mostra o que existe
- esta nota mostra a sequencia de execucao

Esta nota se conecta naturalmente com a ideia de planejamento e sequencia de execucao.

## Etapa 0 - Planejamento

Objetivo:

- definir ideia do sistema
- definir stack
- definir estrutura das notas
- definir ordem inicial das features

Status:

- concluida

Registro:

- [[2026-04-14 - Descoberta e planejamento]]

## Etapa 1 - Setup do projeto

Objetivo:

- criar projeto ASP.NET Core Web API
- configurar o projeto .NET
- configurar MySQL
- configurar Entity Framework Core
- subir a aplicacao localmente

Resultado esperado:

- API sobe sem erro
- conexao com banco configurada

Status:

- em andamento

Progresso atual:

- `MuseuDeBugs.slnx` existe na raiz do projeto
- a API `MuseuDeBugs.Api` existe e compila
- a estrutura inicial de pastas foi criada
- o template antigo ja nao participa do pipeline atual
- `Program.cs` agora registra controllers, OpenAPI e `BugService`
- `appsettings.Development.json` ja possui uma connection string de MySQL
- `AppDbContext` ainda nao foi implementado
- o provider atual ainda precisa ser alinhado com MySQL

Registro parcial:

- [[2026-04-14 - Setup inicial da API]]
- [[2026-04-16 - Alinhamento da persistencia e estudo de EF Core]]
- [[2026-04-19 - Consolidacao do estado atual e ajuste do bootstrap da API]]

## Etapa 2 - Dominio inicial

Objetivo:

- criar enum `BugStatus`
- criar entidade `Bug`
- criar `AppDbContext`
- mapear tabela inicial

Resultado esperado:

- estrutura base do dominio pronta

Status:

- em andamento

Progresso atual:

- `Bug` ja existe com os campos principais
- `StatusBug` ja existe com `Aberto` e `Resolvido`
- `MarcarComoResolvido()` ja existe na entidade
- `AppDbContext` segue vazio
- o mapeamento inicial da tabela ainda nao foi feito

## Etapa 3 - Fluxo principal da API

Objetivo:

- criar endpoint para cadastrar bug
- criar endpoint para listar bugs
- criar endpoint para buscar bug por id

Resultado esperado:

- fluxo principal funcionando

Status:

- em andamento

Progresso atual:

- `POST /api/bugs` ja existe no controller
- `BugService.CriarBug` ja existe
- o fluxo atual ainda e em memoria, sem persistencia no banco
- `GET /api/bugs` e `GET /api/bugs/{id}` ainda nao foram implementados

## Etapa 4 - Regra de negocio de status

Objetivo:

- implementar marcacao de bug como resolvido
- atualizar data de alteracao

Resultado esperado:

- primeira regra de negocio fechada

Status:

- pendente

## Etapa 5 - Evolucao funcional

Objetivo:

- editar bug
- filtrar por status
- filtrar por linguagem

Resultado esperado:

- API mais util para consulta real

Status:

- pendente

## Etapa 6 - Polimento e crescimento

Objetivo:

- revisar estrutura
- adicionar tags
- pensar em testes
- pensar em historico de alteracoes

Resultado esperado:

- sistema mais maduro e melhor organizado

Status:

- pendente

## Regras de andamento

Para nao perder o controle:

- cada sessao deve atacar uma etapa ou subetapa
- cada sessao deve gerar anotacao no diario
- toda mudanca de direcao deve ser registrada

## Hubs conectados a esta nota

- [[Programacao]]
- [[Csharp]]
- [[Banco de Dados]]
- [[API REST]]
- [[Modelagem de Dominio]]
- [[CRUD]]

## Relaciona com

- [[Programacao]]
- [[06 Features e ordem de implementacao]]
- [[08 Diario do projeto]]
- [[Template - Sessao do projeto]]

## Conclusao

Este roadmap continua existindo para dar ritmo.

Hoje o projeto esta claramente entre tres zonas:

- setup tecnico quase fechado
- dominio inicial parcialmente pronto
- primeiro fluxo HTTP iniciado, mas ainda sem persistencia
