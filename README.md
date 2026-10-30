# Cadastro de Pessoas

API REST de cadastro de pessoas físicas (CRUD) em C# / .NET 10, com arquitetura Hexagonal. O projeto também serve para treinar o desenvolvimento orientado a especificação (SDD).

- Especificação, plano e tarefas: [`specs/001-cadastro_de_pessoa/`](specs/001-cadastro_de_pessoa/)
- Regras gerais do projeto: [`specs/constitution.md`](specs/constitution.md)

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (versão fixada em `global.json`)
- MySQL 8.0 local com o schema, a tabela `pessoa` e o índice único `cpf_UNIQUE` já criados (estrutura em `specs/001-cadastro_de_pessoa/plan.md`, seção 7.1)
- Docker, usado apenas pelos testes de integração (Testcontainers)

## Configuração do banco

Todas as configurações ficam em `src/CadastroPessoas.Api/appsettings.json`, inclusive a connection string `ConnectionStrings:CadastroPessoas`. No Git, o arquivo é versionado com a senha **vazia** (`Password=;`).

1. Preencha a senha do banco local na connection string do `appsettings.json`.
2. Impeça que a senha seja commitada:

```bash
git update-index --skip-worktree src/CadastroPessoas.Api/appsettings.json
```

Para commitar uma mudança legítima no arquivo, desfaça a marcação com `--no-skip-worktree`, remova a senha, faça o commit e marque de novo.

Em produção, a pipeline de deploy substitui o `appsettings.json` pelo arquivo com os dados de produção.

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
- **Banco existente:** a aplicação não cria nem altera tabelas; a exclusão de pessoas é lógica (`ativo = 0`) e um novo cadastro com o CPF de uma pessoa excluída reaproveita e reativa o registro.
