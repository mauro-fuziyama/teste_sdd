# Active Context

> Responde "onde parei?". Atualize no FIM de toda sessão. Máx. ~1 página;
> mova o histórico antigo para `progress.md` ou `decisions.md`.

**Última atualização:** 2026-11-05

## Foco atual

- **Spec:** `specs/001-cadastro_de_pessoa/`
- **Tarefa:** Tarefa 5 - API (bloqueada pelas dúvidas abaixo)

## Situação do projeto

- O projeto foi reiniciado com uma spec mais simples e uma nova estrutura de pastas (D-004).
- As Tarefas 1 a 4 estão concluídas.
- **Onde está o trabalho:** branch `worktree-tarefa-4-infraestrutura`, no worktree `.claude/worktrees/tarefa-4-infraestrutura`. O `master` e o diretório de trabalho principal não foram alterados e ainda não têm a Tarefa 4 (D-010).
- Fora desse branch continua valendo **não commitar por enquanto**, por decisão do responsável pelo projeto.
- Verificado em 2026-11-05: `dotnet build` com 0 erros e 0 avisos; `dotnet test` com 110 testes aprovados (55 do domínio, 18 da aplicação e 37 da infraestrutura).

## Feito na última sessão

- Tarefa 4 implementada: `DescriptografadorDeSenha`, `StringDeConexaoSegura`, `PessoaRepository`, `RegistroCorrompidoException` e `DependencyInjection.AdicionarInfraestrutura`.
- Testes de infraestrutura rodando contra o `raspadinha_dev` (D-008), com a senha real descriptografada.
- Verificado depois dos testes: nenhuma linha de teste sobrou e a tabela `pessoa` é idêntica à do `plan.md`.
- `spec.md`, `plan.md` e `tasks.md` atualizados com as regras esclarecidas.

## Próximo passo

1. Responsável decide como trazer o branch da Tarefa 4 para o diretório de trabalho principal.
2. Responder às dúvidas da Tarefa 5 e atualizar `plan.md` e `tasks.md`.
3. Implementar a Tarefa 5, incluindo `builder.Services.AdicionarInfraestrutura(builder.Configuration)` no `Program.cs`, que ainda não foi chamado.

## Bloqueios e dúvidas em aberto

**Impedem a Tarefa 5:**

- PUT e DELETE de id inexistente retornam 400? O `plan.md` só prevê 400 para esses casos, sem citar registro inexistente. Hoje a aplicação lança `PessoaNaoEncontradaException`, separada de `ValidacaoException`; falta definir o código HTTP.
- id não numérico na rota deve retornar 400 com a lista `Erros`?
- Como tratar pesquisa por nome com valor só de espaços? A spec não define e, hoje, o valor é repassado como veio. Valor vazio retorna todos os registros (`LIKE '%%'`).

**Impede a Tarefa 6:**

- Qual fuso o container de produção deve usar? Sem configuração, o horário local do container costuma ser UTC, o que afeta a D-005.

**Para o responsável pelo projeto:**

- **Escopo:** o `project-brief.md` diz que o sistema não será para produção, mas o `plan.md`, a Tarefa 6 e a D-003 definem a execução em produção no AWS ECS. Confirmar se a Tarefa 6 continua no escopo.

## Armadilhas conhecidas

- **Código antigo no Git:** o histórico até `645f5fb` tem `Result`, `Sexo`, paginação e exclusão lógica, que não fazem parte da spec atual. Não reaproveitar sem checar a spec.
- **Risco de perda:** no diretório de trabalho principal, `git restore .` ou `git checkout .` desfaz o reinício. O repositório não tem remoto, então nada disso tem cópia fora deste computador.
- **Conflito de nomes em C#:** dentro de `UseCases/Pessoa/`, `Pessoa` resolve para o namespace. Usar o alias `PessoaEntity`, como nos casos de uso existentes.
- **Middleware da Tarefa 5:** `RegistroCorrompidoException` e os erros de `MensagensDeSeguranca` não são `DominioException`, então devem cair no 500 (D-009). A resposta não deve expor detalhes de conexão.
- **Pesquisa em lista:** uma única linha com CPF inválido faz a pesquisa inteira falhar com 500 (D-009).
- **Pesquisa por nome:** a collation `utf8mb4_0900_ai_ci` ignora maiúsculas e acentos, e a ordem do retorno não está definida.
- **Nomes do JSON:** o padrão camelCase do ASP.NET não produz o contrato do `plan.md` (`datadenascimento`, `Registros`, `Erros`). Definir os nomes explicitamente.
- **Erros de binding:** a resposta automática de `[ApiController]` usa `ProblemDetails`, não a lista `Erros`. Precisa ser ajustada na Tarefa 5.
- **Imagem Docker:** o `appsettings.json` local não pode entrar na imagem da Tarefa 6.
- **`appsettings.json`:** está com `skip-worktree` (D-002), inclusive no worktree da Tarefa 4, então suas alterações não aparecem no `git status`. A marcação vale por checkout: um worktree novo precisa repeti-la antes do `git add`.
- **Testes de infraestrutura em um clone novo:** falham enquanto o `appsettings.json` local e a `CHAVE_DE_SEGURANCA` não estiverem configurados e o MySQL não estiver acessível (D-008).

## Arquivos-chave da próxima tarefa

- `specs/001-cadastro_de_pessoa/plan.md` (rotas e corpos de entrada e retorno)
- `specs/001-cadastro_de_pessoa/tasks.md` (Tarefa 5)
- `src/CadastroPessoas.Application/Interfaces/IPessoaService.cs`
- `src/CadastroPessoas.Infrastructure/DependencyInjection.cs`
- `src/CadastroPessoas.Api/`
