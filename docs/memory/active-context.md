# Active Context

> Responde "onde parei?". Atualize no FIM de toda sessão. Máx. ~1 página;
> mova o histórico antigo para `progress.md` ou `decisions.md`.

**Última atualização:** 2026-11-06

## Foco atual

- **Spec:** `specs/001-cadastro_de_pessoa/`
- **Tarefa:** todas as seis tarefas concluídas; falta decidir o encerramento da spec.

## Situação do projeto

- O projeto foi reiniciado com uma spec mais simples e uma nova estrutura de pastas (D-004).
- As Tarefas 1 a 6 estão concluídas no branch `us001-cadastro-pessoa`, publicado em `origin` (GitHub `mauro-fuziyama/teste_sdd`). O `main` está no `8e0f139 first commit`.
- O trabalho é feito sempre no diretório principal, sem worktrees (D-016).
- Verificado em 2026-11-05: `dotnet build` com 0 erros e 0 avisos; `dotnet test` com 113 testes aprovados (55 do domínio, 21 da aplicação e 37 da infraestrutura).

## Feito na última sessão

- Tarefa 6: `Dockerfile` e `.dockerignore` na raiz (D-015).
- Imagem construída e executada localmente com as duas variáveis de ambiente, contra o MySQL do Windows:
  - CRUD funcionando e `datadecadastro` no horário de São Paulo.
  - Imagem sem `appsettings.json`, com usuário 1654 e Swagger com 404.
  - Logs sem senha e sem chave.
  - Sem as variáveis, as rotas retornam 500 com mensagem genérica.
- `plan.md`, `tasks.md` e memória atualizados.

## Próximo passo

1. Pull request do `us001-cadastro-pessoa` para o `main`: **ainda não**, por decisão do responsável em 2026-11-06.
2. Marcar a spec 001 como concluída no `progress.md` depois do merge.

## Bloqueios e dúvidas em aberto

- **Escopo:** o sistema **não vai para produção** (D-017), e o `plan.md`, o `tasks.md` e o `conventions.md` já refletem isso. Ainda citam produção: o `CLAUDE.md` e os comentários do `Dockerfile`, que não foram alterados.
- **Branch remoto antigo:** `origin/worktree-tarefa-4-infraestrutura` continua no GitHub, com o mesmo conteúdo do `b06d812`. A exclusão foi bloqueada para a sessão e depende do responsável.

## Armadilhas conhecidas

- **Código antigo no Git:** o histórico até `645f5fb` tem `Result`, `Sexo`, paginação e exclusão lógica, que não fazem parte da spec atual. Não reaproveitar sem checar a spec.
- **Build Docker local:** o Norton Antivirus quebra o `dotnet restore` dentro do container. Ver `conventions.md`, seção Container.
- **Conflito de nomes em C#:** dentro de `UseCases/Pessoa/`, `Pessoa` resolve para o namespace. Usar o alias `PessoaEntity`, como nos casos de uso existentes.
- **Pesquisa em lista:** uma única linha com CPF inválido faz a pesquisa inteira falhar com 500 (D-009).
- **Pesquisa por nome:** a collation `utf8mb4_0900_ai_ci` ignora maiúsculas e acentos, e a ordem do retorno não está definida.
- **Data fora do formato:** a resposta 400 traz só esse erro, sem avaliar os demais campos (D-013).
- **`appsettings.json`:** está com `skip-worktree` (D-002), então suas alterações não aparecem no `git status`.
- **Testes de infraestrutura em um clone novo:** falham enquanto o `appsettings.json` local e a `CHAVE_DE_SEGURANCA` não estiverem configurados e o MySQL não estiver acessível (D-008).

## Arquivos-chave

- `Dockerfile` e `.dockerignore`
- `specs/001-cadastro_de_pessoa/plan.md` (seção Container de produção)
- `docs/memory/decisions.md` (D-015 e D-016)
