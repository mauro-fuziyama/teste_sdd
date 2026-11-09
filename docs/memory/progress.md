# Progress

> Estado de cada spec num relance. Evita reler o `tasks.md` de todas as features.
> Legenda: ✅ concluída · 🔄 em andamento · ⏳ pendente · ⛔ bloqueada · 🗄️ arquivada

**Última atualização:** 2026-11-09

## Visão geral

| Spec | Status | Tarefas | Observação |
|------|--------|---------|------------|
| `001-cadastro_de_pessoa` | 🔄 | 6/6 | Todas as tarefas no branch `us001-cadastro-pessoa`; falta o merge no `main` |
| `002-cadastro_telefone` | 🔄 | 4/4 | Todas as tarefas no branch `us002-cadastro-telefone`; falta o merge |
| `003-cadastro_endereco` | 🔄 | 3/4 | Spec esclarecida (D-019); Tarefas 1 a 3 concluídas no branch `us003-cadastro-endereco` |

## Em andamento

### `003-cadastro_endereco`

- [x] Tarefa 1 — Domínio (2026-11-09: build com 0 erros e 0 avisos; 211 testes de domínio aprovados)
- [x] Tarefa 2 — Aplicação (2026-11-09: build com 0 erros e 0 avisos; 54 testes de aplicação aprovados)
- [x] Tarefa 3 — Infraestrutura (2026-11-09: build com 0 erros e 0 avisos; 69 testes de infraestrutura aprovados contra o MySQL local)
- [ ] Tarefa 4 — API

## Último estado verificado (2026-11-09, spec 003, Tarefa 3)

| Item | Estado |
|------|--------|
| Build | 0 erros e 0 avisos |
| Testes | 334 aprovados: 211 do domínio, 54 da aplicação e 69 da infraestrutura |

## Concluídas (resumo)

Nenhuma spec concluída.

## Dívida técnica / pendências transversais

- [ ] Apagar o branch remoto `origin/worktree-tarefa-4-infraestrutura`.
- [ ] A API não tem testes automatizados: os `tasks.md` não pedem o projeto, e a verificação é manual.
- [ ] Definir o que fazer com `memory-bank/`, que parece ser só uma cópia do modelo de `CLAUDE.md`.
- [ ] O arquivo está como `CLAUDE.MD` no disco e `CLAUDE.md` no Git; a diferença pode causar problemas em sistemas que diferenciam maiúsculas.
