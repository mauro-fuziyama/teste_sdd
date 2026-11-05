# Active Context

> Responde "onde parei?". Atualize no FIM de toda sessão. Máx. ~1 página;
> mova o histórico antigo para `progress.md` ou `decisions.md`.

**Última atualização:** 2026-11-05

## Foco atual

- **Spec:** `specs/001-cadastro_de_pessoa/`
- **Tarefa:** Tarefa 6 - Container (bloqueada pelas dúvidas abaixo)

## Situação do projeto

- O projeto foi reiniciado com uma spec mais simples e uma nova estrutura de pastas (D-004).
- As Tarefas 1 a 5 estão concluídas.
- **Onde está o trabalho:** branch `worktree-tarefa-4-infraestrutura`, no worktree `.claude/worktrees/tarefa-4-infraestrutura`. Apesar do nome, o branch também tem a Tarefa 5, que depende da 4. O `master` e o diretório de trabalho principal não foram alterados e não têm nenhuma das duas (D-010).
- Fora desse branch continua valendo **não commitar por enquanto**, por decisão do responsável pelo projeto.
- Verificado em 2026-11-05: `dotnet build` com 0 erros e 0 avisos; `dotnet test` com 113 testes aprovados (55 do domínio, 21 da aplicação e 37 da infraestrutura).

## Feito na última sessão

- Tarefa 5 implementada: `PessoaController`, DTOs, `PessoaMapper`, `ErrosDeRequisicaoMapper`, `TratamentoDeErrosMiddleware` e a chamada de `AdicionarInfraestrutura` no `Program.cs`.
- Pesquisa por nome em branco agora é rejeitada na aplicação (D-014).
- Rotas chamadas com a API rodando contra o MySQL local, nos casos de sucesso e de erro, incluindo 500 com linha de CPF inválido e Swagger indisponível em produção. Essa verificação foi manual, sem teste automatizado.
- `spec.md`, `plan.md` e `tasks.md` atualizados com as regras esclarecidas (D-012 a D-014).

## Próximo passo

1. Responsável decide como trazer o branch para o diretório de trabalho principal.
2. Responder às dúvidas da Tarefa 6 e atualizar `plan.md` e `tasks.md`.
3. Implementar a Tarefa 6.

## Bloqueios e dúvidas em aberto

**Impedem a Tarefa 6:**

- Qual fuso o container de produção deve usar? Sem configuração, o horário local do container costuma ser UTC, o que afeta a D-005.
- **Escopo:** o `project-brief.md` diz que o sistema não será para produção, mas o `plan.md`, a Tarefa 6 e a D-003 definem a execução em produção no AWS ECS. Confirmar se a Tarefa 6 continua no escopo.

## Armadilhas conhecidas

- **Código antigo no Git:** o histórico até `645f5fb` tem `Result`, `Sexo`, paginação e exclusão lógica, que não fazem parte da spec atual. Não reaproveitar sem checar a spec.
- **Risco de perda:** no diretório de trabalho principal, `git restore .` ou `git checkout .` desfaz o reinício. O repositório não tem remoto, então nada disso tem cópia fora deste computador.
- **Conflito de nomes em C#:** dentro de `UseCases/Pessoa/`, `Pessoa` resolve para o namespace. Usar o alias `PessoaEntity`, como nos casos de uso existentes.
- **Pesquisa em lista:** uma única linha com CPF inválido faz a pesquisa inteira falhar com 500 (D-009).
- **Pesquisa por nome:** a collation `utf8mb4_0900_ai_ci` ignora maiúsculas e acentos, e a ordem do retorno não está definida.
- **Data fora do formato:** a resposta 400 traz só esse erro, sem avaliar os demais campos (D-013).
- **Imagem Docker:** o `appsettings.json` local não pode entrar na imagem da Tarefa 6. Sem `ConnectionStrings__CadastroPessoas` e `CHAVE_DE_SEGURANCA`, a API sobe, mas toda rota com acesso ao banco retorna 500, porque a conexão é criada no primeiro uso.
- **`UseHttpsRedirection`:** continua no `Program.cs`. Avaliar na Tarefa 6, conforme a terminação de TLS na AWS.
- **`appsettings.json`:** está com `skip-worktree` (D-002), inclusive no worktree, então suas alterações não aparecem no `git status`. A marcação vale por checkout: um worktree novo precisa repeti-la antes do `git add`.
- **Testes de infraestrutura em um clone novo:** falham enquanto o `appsettings.json` local e a `CHAVE_DE_SEGURANCA` não estiverem configurados e o MySQL não estiver acessível (D-008).

## Arquivos-chave da próxima tarefa

- `specs/001-cadastro_de_pessoa/plan.md` (execução em produção)
- `specs/001-cadastro_de_pessoa/tasks.md` (Tarefa 6)
- `docs/memory/decisions.md` (D-002, D-003 e D-005)
- `src/CadastroPessoas.Api/Program.cs`
