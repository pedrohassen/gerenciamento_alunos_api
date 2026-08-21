# Gerenciamento de Alunos — API

Back-end do sistema de gerenciamento de alunos ("LearningLoop"), em ASP.NET Core. Expõe a API consumida pelo front-end em [`gerenciamento_alunos_front`](../gerenciamento_alunos_front).

## 🔗 No ar

API rodando no Render (free tier): **https://gerenciamento-alunos-api.onrender.com** — Swagger não fica exposto em produção (só em `Development`). Consumida pelo front em **[gerenciamento-alunos-front.vercel.app](https://gerenciamento-alunos-front.vercel.app)**.

Free tier "dorme" depois de ~15min sem uso — a primeira requisição depois disso pode levar 30-50s pra responder.

## Stack

- [.NET 8](https://dotnet.microsoft.com/) / ASP.NET Core Web API
- [Dapper](https://github.com/DapperLib/Dapper) + [Npgsql](https://www.npgsql.org/) — acesso a dados (sem EF Core)
- [PostgreSQL](https://www.postgresql.org/) — banco de dados
- [Liquibase](https://www.liquibase.org/) — migrações de banco
- JWT (`Microsoft.AspNetCore.Authentication.JwtBearer`) — autenticação, com policies `AdminOnly`/`UserOrAdmin`
- [AutoMapper](https://automapper.org/) — mapeamento entre DTOs/Models/Requests/Responses
- [BCrypt.Net](https://github.com/BcryptNet/bcrypt.net) — hash de senha
- [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) — documentação Swagger/OpenAPI
- [xUnit](https://xunit.net/) + [Moq](https://github.com/devlooped/moq) — testes automatizados

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) rodando localmente (ou acessível pela connection string configurada)
- [Liquibase CLI](https://docs.liquibase.com/start/install/home.html) instalado — só é necessário pra rodar as migrações, não é uma dependência do projeto em si

## Setup

### 1. Restaurar dependências

```bash
dotnet restore
```

### 2. Configurar o banco de dados e os segredos

`appsettings.json` não guarda mais valores reais de connection string nem o `Jwt:Secret` (não podem ir pro repo). Localmente, use o [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) do ASP.NET Core — fica fora do repo e entra automaticamente na configuração em ambiente `Development`, sem precisar mudar nada em `Program.cs`:

```bash
cd LearningLoop.GerenciamentoAlunosApp
dotnet user-secrets init
dotnet user-secrets set "Jwt:Secret" "<uma-chave-secreta-qualquer>"
dotnet user-secrets set "ConnectionStrings:PostgresConnection" "Host=localhost;Port=5433;Database=LEARNINGLOOP_ALUNOS;Username=admin;Password=papiro"
```

Crie o banco PostgreSQL correspondente e ajuste também `liquibase/liquibase.properties` (as credenciais precisam bater com o que foi configurado acima):

```
url=jdbc:postgresql://localhost:5433/LEARNINGLOOP_ALUNOS
username=admin
password=papiro
```

### 3. Rodar as migrações

Dentro da pasta `liquibase/`:

```bash
liquibase update
```

Isso cria as tabelas `usuarios` e `alunos` (changelogs em `liquibase/changelog-*.xml`).

## Rodando localmente

```bash
dotnet run --project LearningLoop.GerenciamentoAlunosApp
```

- HTTPS: `https://localhost:7188`
- HTTP: `http://localhost:5175`
- Swagger UI abre automaticamente em `/swagger` (só em ambiente `Development`)

## Testes

```bash
dotnet test
```

Cobre testes unitários dos `Services` (regra de negócio, com os `Repositories` mockados) e testes de integração dos `Controllers` via `WebApplicationFactory` (sobe a API real em memória — roteamento, autenticação, autorização, middleware de exceção — trocando só os `Repositories` por mocks via DI, sem precisar de Postgres rodando).

## Deploy

A API tem um `Dockerfile` multi-stage (build com `sdk:8.0`, runtime com `aspnet:8.0`) pronto pra rodar em qualquer serviço que suba containers (pensado pro [Render](https://render.com/), como Web Service via Docker).

Nenhum segredo fica na imagem — em produção eles são passados como variáveis de ambiente (convenção do ASP.NET Core: `__` duplo substitui o `:` de configuração aninhada):

| Variável | Equivale a |
|---|---|
| `Jwt__Secret` | `Jwt:Secret` |
| `ConnectionStrings__PostgresConnection` | `ConnectionStrings:PostgresConnection` |
| `PORT` | porta que o container deve escutar (o Render injeta automaticamente) |

Build e run local da imagem:

```bash
docker build -t learningloop-api .
docker run -p 8080:8080 \
  -e Jwt__Secret="<chave-secreta>" \
  -e ConnectionStrings__PostgresConnection="Host=...;Port=...;Database=...;Username=...;Password=..." \
  learningloop-api
```

Como o app roda atrás de um proxy que termina TLS na borda (o container recebe tráfego HTTP puro), `UseInfra()` (`Extensions/InfraExtensions.cs`) configura `UseForwardedHeaders` antes do `UseHttpsRedirection()`, lendo os headers `X-Forwarded-For`/`X-Forwarded-Proto` pra API reconhecer o protocolo original em vez de tentar redirecionar pra uma porta HTTPS que não existe dentro do container.

O banco de produção não roda migração automática no boot — aplique as changelogs do Liquibase manualmente contra o Postgres remoto (mesmo comando do passo 3 do setup, apontando `liquibase.properties` pra connection string de produção).

## Autenticação

A API usa JWT Bearer. O fluxo:

1. `POST /api/Usuario/registrar` ou `POST /api/Usuario/login` retornam um token.
2. Enviar esse token em requisições autenticadas no header `Authorization: Bearer <token>`.

No Swagger UI, clique em **Authorize** (canto superior direito) e cole o token (sem precisar digitar `Bearer ` na frente — o Swagger já adiciona).

O token carrega três claims relevantes (definidas em `Services/JwtService.cs`):
- `sub` — id do usuário (claim JWT padrão)
- `email` — e-mail do usuário
- a claim de perfil (`USER`/`ADMIN`) usa a URI completa `http://schemas.microsoft.com/ws/2008/06/identity/claims/role` — é assim que o `ClaimTypes.Role` do .NET serializa quando o token é montado direto via `new JwtSecurityToken(...)`, sem passar pelo mapeamento de nome curto. Isso importa pra qualquer consumidor da API que decodifique o token manualmente (o front, por exemplo, lê exatamente essa chave em `tokenService.ts`).

Existem duas policies de autorização:
- **`AdminOnly`** — só usuários com perfil `ADMIN`.
- **`UserOrAdmin`** — qualquer usuário autenticado (`USER` ou `ADMIN`).

## Endpoints principais

**Usuário** (`/api/Usuario`)

| Método | Rota | Acesso |
|---|---|---|
| POST | `/registrar` | Público |
| POST | `/login` | Público |
| GET | `/` | `AdminOnly` |
| GET | `/{id}` | `UserOrAdmin` |
| PUT | `/` | `UserOrAdmin` |
| DELETE | `/{id}` | `AdminOnly` |

**Aluno** (`/api/Aluno`) — tudo `AdminOnly`

| Método | Rota | Descrição |
|---|---|---|
| POST | `/` | Cadastrar aluno |
| GET | `/` | Listar (filtros `nome`, `curso`, paginação `pular`/`quantidade`) |
| GET | `/{id}` | Obter por id |
| PUT | `/` | Atualizar |
| DELETE | `/{id}` | Excluir (soft delete) |

Documentação completa e interativa de cada endpoint (parâmetros, schemas, respostas) está no Swagger UI (`/swagger`) com a API rodando.

## Estrutura do projeto

```
LearningLoop.GerenciamentoAlunosApp/
├── Controllers/        # endpoints HTTP
├── Services/            # regra de negócio
├── Repositories/        # acesso a dados (Dapper)
├── DTOs/                # base compartilhada de campos (Models/Requests/Responses/Arguments herdam daqui)
├── Requests/             # payloads de entrada dos controllers
├── Responses/            # payloads de saída dos controllers
├── Models/               # retorno dos repositórios
├── Arguments/            # entrada dos repositórios
├── Mapper/               # configuração e profiles do AutoMapper
├── Security/             # hash de senha (BCrypt)
├── CrossCutting/         # exceções customizadas, middleware de erro, validações, constantes
├── Extensions/            # configuração de DI, JWT, CORS, Swagger, AutoMapper (chamadas em Program.cs)
└── DI/                    # registro centralizado de dependências

LearningLoop.GerenciamentoAlunosApp.Tests/
├── Services/             # testes unitários (Services + Repositories mockados)
└── Controllers/          # testes de integração (WebApplicationFactory)
```

Cada entidade (`Usuario`, `Aluno`) tem seu DTO base e quatro variações que herdam dele: `Request` (entrada do controller), `Response` (saída do controller), `Model` (retorno do repositório) e `Argument` (entrada do repositório) — o AutoMapper faz a conversão entre elas.
