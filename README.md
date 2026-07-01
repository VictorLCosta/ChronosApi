# Chronos API

Backend do Chronos, uma API para organizar projetos, metas, tarefas, lembretes, tags e anexos.

[![.NET](https://img.shields.io/badge/.NET-10-1f6feb?style=for-the-badge)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-0a66c2?style=for-the-badge)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-7-c0392b?style=for-the-badge)](https://redis.io/)
[![License: MIT](https://img.shields.io/badge/License-MIT-2f855a?style=for-the-badge)](./LICENSE)

## Visao Geral

O **Chronos API** concentra a parte de backend de um app de produtividade pessoal. Alem dos recursos de organizacao, o projeto ja nasce com autenticacao, cache, rate limiting, health checks, OpenAPI, logs estruturados e telemetria.

A intencao aqui e manter uma base simples de entender e facil de evoluir, com separacao clara entre dominio, aplicacao, web e infraestrutura.

## O Que Ja Existe

- autenticacao com JWT e refresh token
- cadastro de usuario, confirmacao de email e fluxo de senha
- suporte a 2FA
- endpoints para projetos, metas, tarefas, lembretes, tags, anexos e logs de meta
- soft delete e restauracao para metas e tarefas
- idempotencia em operacoes sensiveis
- cache com Redis
- rate limiting configuravel
- health checks de liveness e readiness
- OpenAPI com UI via Scalar
- logs estruturados com Serilog
- OpenTelemetry preparado para integracao
- testes unitarios e testes de arquitetura

## Dominios Da API

Todos os endpoints da aplicacao ficam sob `/api`.

| Grupo | Base path | Objetivo |
| --- | --- | --- |
| Identity | `/api/identity` | login, registro, refresh token, senha, 2FA e usuario atual |
| Projects | `/api/projects` | gerenciamento de projetos |
| Goals | `/api/goals` | metas e ciclo de vida |
| Tasks | `/api/tasks` | tarefas vinculadas a projetos e metas |
| Reminders | `/api/reminders` | lembretes e recorrencia |
| Tags | `/api/tags` | classificacao e organizacao |
| Attachments | `/api/attachments` | anexos relacionados aos recursos |
| Goal Logs | `/api/goallogs` | historico e progresso das metas |

## Stack

| Camada | Tecnologias |
| --- | --- |
| Runtime | .NET 10, ASP.NET Core Minimal API |
| Aplicacao | Mediator, FluentValidation |
| Persistencia | Entity Framework Core, PostgreSQL |
| Cache | Hybrid Cache + Redis |
| Auth | JWT Bearer, refresh token e 2FA |
| Observabilidade | Serilog, OpenTelemetry, Health Checks |
| Documentacao | OpenAPI + Scalar |
| Testes | xUnit, Moq, Shouldly, EF Core InMemory/SQLite |

## Arquitetura

O repositorio esta organizado por camadas:

```text
src/
 |- Api/                -> composicao da aplicacao e endpoints
 |- Application/        -> casos de uso, validacoes, contratos e behaviors
 |- Domain/             -> entidades, enums e value objects
 |- Infrastructure/     -> persistencia, identidade, cache, logging e concerns web
 |- CrossCutting/       -> modelos e extensoes compartilhadas
tests/
 |- UnitTests/          -> testes de comportamento
 |- ArchitectureTests/  -> regras estruturais da solucao
```

Essa divisao ajuda a manter a regra de negocio menos acoplada ao ASP.NET e facilita a manutencao do projeto.

## Experiencia De Desenvolvimento

Na inicializacao, a aplicacao ja aplica migracoes e executa seed automaticamente:

- `InitializeDatabasesAsync()` executa migrate e seed
- `launchSettings.json` expoe a API em `http://localhost:5280` e `https://localhost:7200`
- a documentacao interativa fica em `/api-docs`
- o documento OpenAPI fica em `/openapi/v1.json`

## Como Rodar Localmente

### Opcao 1: ambiente local com .NET

Pre-requisitos:

- .NET SDK 10
- PostgreSQL
- Redis

Configuracao:

Use [`src/Api/appsettings.Development.json`](/C:/Users/victo/Desktop/Projetos/pessoais/ChronosApi/src/Api/appsettings.Development.json) como base. Por padrao, ele espera:

- PostgreSQL em `localhost:5432`
- banco `chronos`
- usuario `postgres`
- senha `postgres`
- Redis em `localhost:6379`

Execucao:

```bash
dotnet restore
dotnet build
dotnet run --project src/Api
```

Depois disso:

- API HTTP: `http://localhost:5280`
- API HTTPS: `https://localhost:7200`
- Docs: `http://localhost:5280/api-docs`

### Opcao 2: com Docker Compose

Tambem da para subir a API com PostgreSQL e Redis juntos:

```bash
docker compose up --build
```

Servicos previstos em [`docker-compose.yml`](/C:/Users/victo/Desktop/Projetos/pessoais/ChronosApi/docker-compose.yml):

- API em `http://localhost:5000`
- PostgreSQL 15 em `localhost:5432`
- Redis 7 em `localhost:6379`

## Health Checks

Existem endpoints dedicados para acompanhamento operacional:

- `GET /health/live`
- `GET /health/ready`

O endpoint de readiness devolve detalhes das dependencias e responde `503` quando algo critico falha.

## Seguranca E Resiliencia

Pontos que ja fazem parte da infraestrutura:

- autenticacao JWT
- refresh token com duracao configuravel
- 2FA para reforco de acesso
- rate limiting global e para auth
- security headers configuraveis
- CORS configuravel
- tratamento global de excecoes com Problem Details
- idempotencia em operacoes selecionadas

## Observabilidade

O projeto ja inclui o basico de diagnostico para desenvolvimento e operacao:

- logs estruturados em console e arquivo
- arquivos de log em `logs/log-.json`
- suporte a exportacao OpenTelemetry
- enrichment com contexto de maquina, processo, thread e excecoes

Com isso, fica mais simples plugar a API em uma stack de monitoramento depois.

## Testes

Para executar a suite:

```bash
dotnet test
```

O repositorio contem:

- testes unitarios de handlers e regras de negocio
- testes de arquitetura para proteger a separacao entre camadas

## Rotas Interessantes Para Explorar

Algumas rotas uteis para validar a API rapidamente:

```http
POST   /api/identity/register
POST   /api/identity/login
POST   /api/identity/refresh
GET    /api/identity/me

GET    /api/projects
POST   /api/projects
GET    /api/projects/{id}/goals
GET    /api/projects/{id}/tasks

GET    /api/goals
POST   /api/goals
PATCH  /api/goals/trash/{id}
PATCH  /api/goals/restore/{id}

GET    /health/live
GET    /health/ready
```

## Padroes Do Projeto

Algumas configuracoes do repositorio mostram a preocupacao com qualidade:

- `TreatWarningsAsErrors` habilitado
- `Nullable` e `ImplicitUsings` ativados
- `AnalysisLevel` e `AnalysisMode` em modo agressivo
- validacoes centralizadas com FluentValidation
- pipeline behaviors para validacao e cache

## Roadmap

Proximos passos que combinam com a base atual do projeto:

- concluir os fluxos ainda abertos de `logout` e `external-login`
- adicionar exemplos reais de request e response na documentacao
- ampliar a cobertura de testes de integracao para banco, cache e auth
- incluir CI com build, testes e validacao de arquitetura
- evoluir o modulo de lembretes para notificacoes e agendamentos
- adicionar auditoria mais detalhada para operacoes criticas
- documentar melhor o fluxo de deploy com o material em `deploy/terraform`

## Licenca

Este projeto esta licenciado sob a [MIT](./LICENSE).
