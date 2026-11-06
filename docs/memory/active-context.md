# Active Context

> Responde "onde parei?". Atualize no FIM de toda sessão. Máx. ~1 página;
> mova o histórico antigo para `progress.md` ou `decisions.md`.

**Última atualização:** 2026-11-05

## Foco atual

- **Spec:** `specs/001-cadastro_de_pessoa/`
- **Tarefa:** Tarefa 4 - Infraestrutura (bloqueada pela dúvida abaixo)

## Situação do projeto

- O projeto foi reiniciado com uma spec mais simples e uma nova estrutura de pastas (D-004). Nada disso foi commitado.
- **Não commitar por enquanto**, por decisão do responsável pelo projeto.
- As Tarefas 1 (setup), 2 (domínio) e 3 (aplicação) estão concluídas no diretório de trabalho.
- Verificado em 2026-11-05: `dotnet build` com 0 erros e 0 avisos; `dotnet test` com 73 testes aprovados (55 do domínio e 18 da aplicação).

## Feito na última sessão

- Domínio ajustado para reunir todos os erros de validação (D-007).
- Tarefa 3 implementada: `IPessoaService`, `PessoaService` (D-006) e os cinco casos de uso, com testes usando repositório em memória.
- PUT com `cpf` vazio ou omitido agora é rejeitado.
- `spec.md`, `plan.md` e `tasks.md` atualizados com essas regras.

## Próximo passo

1. Responder à dúvida da Tarefa 4 e atualizar `plan.md` e `tasks.md`.
2. Implementar a Tarefa 4 em `src/CadastroPessoas.Infrastructure/`, com testes em `tests/CadastroPessoas.Infrastructure.Tests/`.

## Bloqueios e dúvidas em aberto

**Impede a Tarefa 4:**

- Os testes de infraestrutura usam o banco local `raspadinha_dev` ou outro banco? O `plan.md` não define e o teste pode alterar dados.

**Impedem tarefas seguintes:**

- **Tarefa 5:** PUT e DELETE de id inexistente retornam 400? O `plan.md` só prevê 400 para esses casos, sem citar registro inexistente. Hoje a aplicação lança `PessoaNaoEncontradaException`, separada de `ValidacaoException`; falta definir o código HTTP.
- **Tarefa 5:** id não numérico na rota deve retornar 400 com a lista `Erros`?
- **Tarefa 5:** como tratar pesquisa por nome com valor só de espaços? A spec não define e, hoje, o valor é repassado como veio.
- **Tarefa 6:** qual fuso o container de produção deve usar? Sem configuração, o horário local do container costuma ser UTC, o que afeta a D-005.

**Para o responsável pelo projeto:**

- **Escopo:** o `project-brief.md` diz que o sistema não será para produção, mas o `plan.md`, a Tarefa 6 e a D-003 definem a execução em produção no AWS ECS. Confirmar se a Tarefa 6 continua no escopo.

## Armadilhas conhecidas

- **Código antigo no Git:** o histórico tem `Result`, `Sexo`, paginação e exclusão lógica, que não fazem parte da spec atual. Não reaproveitar sem checar a spec.
- **Risco de perda:** `git restore .` ou `git checkout .` desfaz o reinício. O repositório não tem remoto, e `docs/` e as novas pastas ainda não estão no Git, então só existem neste computador.
- **Conflito de nomes em C#:** dentro de `UseCases/Pessoa/`, `Pessoa` resolve para o namespace. Usar o alias `PessoaEntity`, como nos casos de uso existentes.
- **Registro de dependências:** a Tarefa 4 ou a 5 precisa registrar `TimeProvider.System`, os casos de uso e `IPessoaService`.
- **CPF duplicado em concorrência:** duas inserções simultâneas passam pela consulta de `CriarPessoaUseCase`. O repositório precisa converter a violação de `cpf_UNIQUE` em `ValidacaoException` com `ErrosDePessoa.CpfJaCadastrado`.
- **Pesquisa por nome:** escapar `%` e `_` no `LIKE` e usar parâmetros do Dapper.
- **Linhas antigas no banco:** `new CPF(...)` valida o valor. Um registro já gravado com CPF inválido lançaria exceção ao ser lido na Tarefa 4.
- **Nomes do JSON:** o padrão camelCase do ASP.NET não produz o contrato do `plan.md` (`datadenascimento`, `Registros`, `Erros`). Definir os nomes explicitamente.
- **Erros de binding:** a resposta automática de `[ApiController]` usa `ProblemDetails`, não a lista `Erros`. Precisa ser ajustada na Tarefa 5.
- **Dependências da Tarefa 4:** o projeto Infrastructure não declara pacotes `Microsoft.Extensions.*`, que podem ser necessários para o `DependencyInjection.cs`.
- **Imagem Docker:** o `appsettings.json` local não pode entrar na imagem da Tarefa 6.
- **`appsettings.json`:** está com `skip-worktree` (D-002), então suas alterações não aparecem no `git status`.

## Arquivos-chave da tarefa atual

- `specs/001-cadastro_de_pessoa/plan.md` (formato da senha e tabela `pessoa`)
- `specs/001-cadastro_de_pessoa/tasks.md` (Tarefa 4)
- `src/CadastroPessoas.Domain/Interfaces/IPessoaRepository.cs`
- `src/CadastroPessoas.Domain/Entities/Pessoa.cs`
- `src/CadastroPessoas.Infrastructure/`
- `tests/CadastroPessoas.Infrastructure.Tests/`
