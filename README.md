# Cadastro de Pessoas

API REST de cadastro de pessoas físicas (CRUD) em C# / .NET 10, com arquitetura Hexagonal. O projeto também serve para treinar o desenvolvimento orientado a especificação (SDD).

- Especificação, plano e tarefas: [`specs/001-cadastro_de_pessoa/`](specs/001-cadastro_de_pessoa/)
- Regras gerais do projeto: [`specs/constitution.md`](specs/constitution.md)

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (versão fixada em `global.json`)
- MySQL 8.0 local com o schema e a tabela `pessoa` já criados (estrutura em `specs/001-cadastro_de_pessoa/plan.md`, seção 7.1)
- Docker, usado apenas pelos testes de integração (Testcontainers)

## Configuração do banco

A conexão com o banco não fica no `appsettings.json`. Ela é lida do arquivo `.env` na raiz do repositório, que **não é versionado**.

1. Copie `.env.example` para `.env`.
2. Preencha as variáveis:

| Variável | Descrição |
|----------|-----------|
| `DB_SERVER` | Servidor do MySQL (ex.: `localhost`) |
| `DB_PORT` | Porta (ex.: `3306`) |
| `DB_NAME` | Schema da aplicação |
| `DB_USER` | Usuário com `SELECT`, `INSERT`, `UPDATE` e `DELETE` |
| `DB_PASSWORD` | Senha do usuário |

Variáveis de ambiente já definidas na máquina têm prioridade sobre o `.env`.

## Como executar

```bash
dotnet build
dotnet run --project src/CadastroPessoas.Api
```

A API sobe em `http://localhost:5207` (perfil `http`).

## Como rodar os testes

```bash
dotnet test
```

Os testes de integração sobem um MySQL 8.0 em container, por isso precisam do Docker em execução.

## Estrutura da solução

| Projeto | Responsabilidade |
|---------|------------------|
| `src/CadastroPessoas.Domain` | Entidade `Pessoa`, Value Objects e regras de negócio. Sem dependências externas |
| `src/CadastroPessoas.Application` | Casos de uso e portas (interfaces). Depende só do Domain |
| `src/CadastroPessoas.Infrastructure` | Adaptador de saída: repositório com Dapper e MySqlConnector |
| `src/CadastroPessoas.Api` | Adaptador de entrada: controllers REST, contratos HTTP, OpenAPI |
| `tests/*` | Testes unitários, de integração e de arquitetura |

## Decisões técnicas

- **Asserções nos testes:** AwesomeAssertions, fork gratuito com a mesma API do FluentAssertions (que exige licença paga a partir da versão 8).
- **Versões de pacotes** centralizadas em `Directory.Packages.props`.
- **Build sem warnings:** `TreatWarningsAsErrors` e analisadores ativos em `Directory.Build.props`.
- **Banco existente:** a aplicação não cria nem altera tabelas; a exclusão de pessoas é lógica (`ativo = 0`).
