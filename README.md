# MuseuDeBugs

O MuseuDeBugs e um projeto em C# criado para registrar erros encontrados durante estudos e projetos, guardando o que aconteceu, a mensagem de erro, a causa, a solucao e o status de cada bug.

A ideia e simples: em vez de resolver um problema e depois esquecer como ele aconteceu, a aplicacao serve para transformar esse erro em memoria tecnica reutilizavel. Assim, quando o mesmo tipo de problema aparecer de novo, fica mais facil consultar o historico e entender o caminho da correcao.

## Tecnologias utilizadas

Atualmente o projeto usa:

- C#
- .NET 10
- ASP.NET Core Web API
- OpenAPI nativo do ASP.NET Core
- Entity Framework Core
- MySQL como banco planejado para a aplicacao

Hoje ainda existe um ponto de ajuste na persistencia: a documentacao e a configuracao local apontam para MySQL, mas o projeto ainda tem uma referencia de provider do SQL Server no `.csproj`. Essa parte ainda sera alinhada.

## Para que servira a aplicacao

O sistema foi pensado para funcionar como uma base pessoal de aprendizado tecnico.

Cada bug registrado deve guardar informacoes como:

- titulo
- linguagem ou stack
- mensagem de erro
- descricao do problema
- causa
- solucao
- status
- data de criacao

Na primeira versao, a aplicacao deve permitir:

- cadastrar bug
- listar bugs
- buscar bug por id
- marcar bug como resolvido

Nao e um sistema corporativo de incidentes nem uma ferramenta estilo Jira. A proposta aqui e ter um lugar simples para documentar erros reais que apareceram durante o estudo e o desenvolvimento.

## O que ja foi feito

A base da API ja existe e o projeto ja esta organizado.

Hoje o projeto ja tem:

- solution criada
- API `MuseuDeBugs.Api`
- estrutura inicial de pastas
- `Program.cs` configurado para controllers, OpenAPI e injecao de dependencia
- entidade `Bug`
- enum `StatusBug`
- DTOs `CriarBugRequest` e `BugResponse`
- `BugService`
- `BugsController`
- endpoint `POST /api/bugs`

Tambem ja foi ajustado o bootstrap da aplicacao para o projeto voltar a compilar corretamente sem depender de pacote de Swagger que nao estava instalado.

## O que ainda falta

Embora a estrutura principal ja exista, a aplicacao ainda nao terminou a primeira fase.

Ainda falta:

- implementar `AppDbContext`
- alinhar o provider do Entity Framework Core com MySQL
- conectar a API ao banco de dados
- persistir de verdade o cadastro de bugs
- criar os endpoints de listagem, busca por id e resolucao
- ajustar melhor o contrato HTTP das respostas

## Status atual

Neste momento, o projeto ja compila, o dominio inicial ja existe e o primeiro fluxo da API ja foi esbocado.

O proximo passo mais importante e fechar a persistencia para que o sistema deixe de ser apenas estrutura e passe a funcionar como um MVP real.
