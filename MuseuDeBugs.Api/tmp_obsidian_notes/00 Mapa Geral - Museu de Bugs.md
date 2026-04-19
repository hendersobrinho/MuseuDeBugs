# Mapa Geral - Museu de Bugs

## Ideia principal

Estas notas existem para organizar o projeto do Museu de Bugs desde o planejamento ate a implementacao.

A ideia e transformar o projeto em um sistema de estudo com contexto preservado. Em vez de depender da memoria de cada sessao, vamos manter:

- a visao do sistema
- as decisoes tecnicas
- a estrutura do projeto
- a ordem das features
- o historico do que foi feito

Assim, sempre que retomarmos o trabalho, o contexto vai estar facil de recuperar.

## Como navegar

Se voce quiser entender a ideia do sistema:

- [[01 Visao do sistema - Museu de Bugs]]

Se voce quiser entender a stack e as decisoes tecnicas:

- [[02 Stack e decisoes tecnicas]]
- [[Entity Framework Core]]

Se voce quiser ver como o projeto vai ser organizado:

- [[03 Estrutura do projeto]]
- [[04 Modelagem de dominio]]
- [[05 Banco de dados - modelagem inicial]]

Se voce quiser uma documentacao tecnica mais detalhada da implementacao:

- [[09 Arquitetura detalhada da API]]
- [[10 Plano de implementacao detalhado - fase 1]]
- [[11 Diagramas UML e fluxos]]
- [[12 Contrato da API - fase 1]]
- [[13 Implementacao por arquivo - fase 1]]

Se voce quiser saber o que vem primeiro e o que vem depois:

- [[06 Features e ordem de implementacao]]
- [[07 Roadmap e etapas de desenvolvimento]]

Se voce quiser retomar o contexto de cada sessao:

- [[08 Diario do projeto]]
- [[2026-04-14 - Descoberta e planejamento]]
- [[2026-04-14 - Setup inicial da API]]
- [[2026-04-16 - Alinhamento da persistencia e estudo de EF Core]]
- [[2026-04-19 - Consolidacao do estado atual e ajuste do bootstrap da API]]
- [[Template - Sessao do projeto]]

## Decisao atual do projeto

Projeto escolhido:

- Museu de Bugs

Stack definida para o inicio:

- C#
- .NET
- ASP.NET Core Web API
- [[Entity Framework Core]]
- MySQL

## Hubs centrais para conexao futura

Links vazios que fazem sentido como pontos de navegacao do projeto:

- [[Programacao]]
- [[Csharp]]
- [[SQL]]
- [[Banco de Dados]]
- [[API REST]]
- [[Modelagem de Dominio]]
- [[CRUD]]

## Como vamos usar estas notas

Fluxo recomendado para cada sessao:

1. revisar [[00 Mapa Geral - Museu de Bugs]] e [[08 Diario do projeto]]
2. abrir a ultima sessao registrada
3. implementar uma etapa pequena e objetiva
4. ao final, criar uma nova nota baseada em [[Template - Sessao do projeto]]
5. atualizar [[08 Diario do projeto]] com o que entrou, o que mudou e o proximo passo

## Relaciona com

Estas notas novas conversam bem com o que voce ja tem no vault:

- [[10 Como pensar uma classe em POO]]
- [[08 Associacao entre classes]]
- [[04 Coesao e responsabilidade da classe]]
- [[03 Chave primaria, chave estrangeira e relacionamento]]
- [[10 Modelagem inicial com Cliente, Pedido, Produto e ItemPedido]]

## Estado atual

Fase atual:

- setup da API consolidado e compilando
- dominio inicial parcialmente criado
- primeiro fluxo HTTP de criacao desenhado, mas ainda sem persistencia

Status geral:

- ideia do sistema definida
- stack inicial definida
- estrutura base das notas criada
- ordem inicial de implementacao definida
- base fisica do projeto criada no disco
- entidade `Bug` e enum `StatusBug` ja existem
- DTOs, `BugService` e `BugsController` ja existem
- `Program.cs` ja foi ajustado para a API real e o build esta passando
- persistencia ainda precisa ser fechada com `AppDbContext`, provider correto e registro do contexto

## Anotacao

Este mapa continua sendo a porta de entrada do projeto.

Se em algum momento a gente se perder, o caminho de volta e:

1. entender a ideia
2. revisar a stack
3. revisar a estrutura
4. checar a etapa atual
5. abrir o diario e a ultima sessao
