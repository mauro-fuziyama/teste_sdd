# Conventions

> Padrões que NÃO estão óbvios no código. Consulte antes de escrever código.
> Se algo pode ser imposto por lint/formatter, configure a ferramenta em vez
> de documentar aqui.

## Estrutura de pastas

```
src/
  CadastroPessoas.Domain/          # Entities, ValueObjects, Enums, Exceptions, Interfaces
  CadastroPessoas.Application/     # UseCases/Pessoa/<CasoDeUso>, Interfaces, Services
  CadastroPessoas.Infrastructure/  # Persistence/Repositories, ExternalServices, DependencyInjection.cs
  CadastroPessoas.Api/             # Controllers, DTOs, Mappers, Middlewares, Program.cs
tests/
  CadastroPessoas.Domain.Tests/
  CadastroPessoas.Application.Tests/
  CadastroPessoas.Infrastructure.Tests/
specs/001-cadastro_de_pessoa/      # spec.md, plan.md, tasks.md
docs/memory/                       # memory bank
```

A árvore completa está no `plan.md`.

Dependências entre projetos: `Api → Application, Infrastructure`; `Infrastructure → Application`; `Application → Domain`.

## Nomenclatura

- **Pastas técnicas:** em inglês, como no `plan.md` (`UseCases`, `Repositories`).
- **Termos de negócio:** em português (`Pessoa`, `CriarPessoa`).
- **Classes:** sufixo técnico em inglês, como no `plan.md` (`CriarPessoaUseCase`, `CriarPessoaInput`, `PessoaRepository`, `PessoaController`, `PessoaRequest`).
- **Value Object de CPF:** arquivo `ValueObjects/CPF.cs`.
- **Nomes em geral:** claros e sem abreviações genéricas (`CLAUDE.md`).
- **Tabela e colunas:** minúsculas, no singular e sem separador (`pessoa`, `datadenascimento`).
- **Campos do JSON:** exatamente como no `plan.md`: minúsculos sem separador (`datadenascimento`), exceto `Registros` e `Erros`.
- **Commits:** Conventional Commits em português, como no histórico (`feat(domain): ...`).
- **Branches:** sem padrão definido.

## Padrões de código

- Arquitetura hexagonal, com SOLID e Clean Code; métodos curtos e pouca indentação.
- O banco é acessado com Dapper e MySqlConnector, pelo `PessoaRepository`.
- A interface `IPessoaRepository` fica no Domain.
- O repositório abre conexões pelo `MySqlDataSource`, registrado como singleton, e sempre passa valores por parâmetros do Dapper.
- `DateOnly` vira `DateTime` nos parâmetros e na leitura (`PessoaRegistro`), sem depender de suporte do Dapper a `DateOnly`.
- No `LIKE`, o caractere de escape é `!`, e não a barra invertida, para funcionar com qualquer `sql_mode`.
- Mensagens de erro de segurança e de conexão nunca incluem a senha, a chave nem o valor criptografado (`MensagensDeSeguranca`).
- Erros de validação são reunidos em uma única `ValidacaoException`, com todas as mensagens em `Erros` (D-007). As mensagens ficam em `ErrosDePessoa`.
- A API usa apenas `IPessoaService`; os casos de uso são chamados pelo `PessoaService` (D-006).
- O domínio não lê o relógio: `Pessoa.Criar` e `Pessoa.Atualizar` recebem `momentoAtual`. Os casos de uso obtêm esse valor com `TimeProvider.GetLocalNow()`, em horário local (D-005).
- Dentro de `UseCases/Pessoa/`, a entidade é referenciada pelo alias `PessoaEntity`, porque `Pessoa` também é o nome do namespace.
- `Pessoa.Criar` valida os dados de entrada. `Pessoa.Reconstituir` serve só para dados vindos do banco e não valida nome nem data.
- Métodos assíncronos têm o sufixo `Async`.
- O tratamento de erros da API fica em `Api/Middlewares`: 400 para regras de negócio e 500 para falhas de servidor, sempre com a lista `Erros`.
- O Swagger fica disponível apenas em desenvolvimento.
- Datas usam o horário local (D-005).

## Segurança

- Senhas e tokens nunca ficam expostos no código.
- A senha da string de conexão fica criptografada com AES-GCM, no formato da D-001.
- O `appsettings.json` local não é commitado (D-002, com `skip-worktree`).
- Em produção, os segredos vêm do AWS Secrets Manager via ECS (D-003).

## Testes

- Framework: xUnit.
- Cada camada testa no projeto `tests/CadastroPessoas.<Camada>.Tests`. O `tasks.md` não pede projeto de testes para a API.
- Comandos: `dotnet build CadastroPessoas.sln` e `dotnet test CadastroPessoas.sln`.
- Testes de infraestrutura usam o `raspadinha_dev` (D-008), com a conexão do `appsettings.json` local e a `CHAVE_DE_SEGURANCA`. Cada teste gera os próprios CPFs com `banco.GerarCpfParaRemoverAoFinal()` e usa `CriarMarcadorUnico()` nos nomes pesquisados, para não depender dos dados existentes.
- Classes de teste que usam o banco ficam na coleção `ColecaoDoBancoDeTeste.Nome`.

## Faça / Não faça

| ✅ Faça | ❌ Não faça |
|--------|------------|
| Parar e expor a falha quando a spec estiver vaga | Implementar com base em suposições |
| Atualizar `spec.md`, `plan.md` e `tasks.md` ao esclarecer uma dúvida | Divergir da spec sem registrar em `decisions.md` |
| Atualizar `docs/memory/` ao concluir cada tarefa | Reaproveitar o código antigo do Git sem checar a spec atual |

## Variáveis de ambiente

| Nome | Obrigatória | Descrição |
|------|-------------|-----------|
| `CHAVE_DE_SEGURANCA` | sim | Chave AES de 256 bits em Base64, usada para descriptografar a senha da conexão |
| `ConnectionStrings__CadastroPessoas` | em produção | String de conexão com a senha criptografada; substitui o valor do `appsettings.json` (D-003) |
