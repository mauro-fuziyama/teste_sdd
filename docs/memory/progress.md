# Progress

> Estado de cada spec num relance. Evita reler o `tasks.md` de todas as features.
> Legenda: ✅ concluída · 🔄 em andamento · ⏳ pendente · ⛔ bloqueada · 🗄️ arquivada

**Última atualização:** 2026-11-05

## Visão geral

| Spec | Status | Tarefas | Observação |
|------|--------|---------|------------|
| `001-cadastro_de_pessoa` | 🔄 | 5/6 | Tarefas 1 a 5 concluídas no branch `worktree-tarefa-4-infraestrutura` (D-010). Tarefa 6 aguarda esclarecimento |

## Em andamento

### `001-cadastro_de_pessoa`

- [x] Tarefa 1 — Setup inicial (commit só no branch da Tarefa 4)
- [x] Tarefa 2 — Domínio (commit só no branch da Tarefa 4)
- [x] Tarefa 3 — Aplicação (commit só no branch da Tarefa 4)
- [x] Tarefa 4 — Infraestrutura (commit só no branch da Tarefa 4)
- [x] Tarefa 5 — API (commit só no branch da Tarefa 4)
- [ ] Tarefa 6 — Container ⛔ ← **atual**; aguarda as dúvidas em `active-context.md` (não há Dockerfile)

### Estado verificado em 2026-11-05

| Item | Estado |
|------|--------|
| Solution | 4 projetos em `src/` e 3 em `tests/`, em .NET 10 (SDK 10.0.403) |
| Pacotes | Dapper, MySqlConnector, Swashbuckle.AspNetCore e xUnit referenciados |
| Código | Domínio, aplicação, infraestrutura e API implementados |
| Build | 0 erros e 0 avisos |
| Testes | 113 aprovados: 55 em `CadastroPessoas.Domain.Tests`, 21 em `CadastroPessoas.Application.Tests` e 37 em `CadastroPessoas.Infrastructure.Tests` |
| Rotas da API | As seis implementadas e chamadas com a API rodando, contra o MySQL local; Swagger responde em desenvolvimento e retorna 404 em produção |
| `CHAVE_DE_SEGURANCA` | Definida; decodifica para 32 bytes (256 bits) |
| Senha no `appsettings.json` local | Descriptografada com sucesso no formato da D-001 e aceita pelo MySQL |
| Senha nas versões commitadas do `appsettings.json` | Nenhum valor no histórico |
| MySQL local | Conexão e login testados; tabela `pessoa` idêntica à do `plan.md` e vazia depois dos testes |

## Concluídas (resumo)

Nenhuma spec concluída.

## Histórico

- **2026-10-30:** primeira versão da spec 001, com 15 tasks. As tasks 1 a 7 (domínio e aplicação) foram commitadas até `645f5fb`.
- **Até 2026-11-05:** spec, plano e tarefas reescritos com outro escopo e estrutura de pastas, para testar a memória do projeto (D-004). A implementação antiga foi removida do diretório de trabalho, mas continua no `HEAD`.

## Dívida técnica / pendências transversais

- [ ] Levar o reinício do projeto e as Tarefas 4 e 5 para o `master`. Hoje estão só no branch `worktree-tarefa-4-infraestrutura` (D-010). Em espera: o responsável pediu para não commitar por enquanto fora desse branch.
- [ ] A API não tem testes automatizados: o `tasks.md` não pede o projeto, e a verificação da Tarefa 5 foi manual.
- [ ] Definir um remoto para o repositório; hoje não há nenhum.
- [ ] Definir o que fazer com `memory-bank/`, que parece ser só uma cópia do modelo de `CLAUDE.md`.
- [ ] O arquivo está como `CLAUDE.MD` no disco e `CLAUDE.md` no Git; a diferença pode causar problemas em sistemas que diferenciam maiúsculas.
- [ ] O branch local é `master`, enquanto o branch principal indicado é `main`, que não existe localmente. Falta definir qual usar.
