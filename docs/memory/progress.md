# Progress

> Estado de cada spec num relance. Evita reler o `tasks.md` de todas as features.
> Legenda: ✅ concluída · 🔄 em andamento · ⏳ pendente · ⛔ bloqueada · 🗄️ arquivada

**Última atualização:** 2026-11-05

## Visão geral

| Spec | Status | Tarefas | Observação |
|------|--------|---------|------------|
| `001-cadastro_de_pessoa` | 🔄 | 4/6 | Tarefas 1 a 4 concluídas no branch `worktree-tarefa-4-infraestrutura` (D-010). Tarefa 5 aguarda esclarecimento |

## Em andamento

### `001-cadastro_de_pessoa`

- [x] Tarefa 1 — Setup inicial (commit só no branch da Tarefa 4)
- [x] Tarefa 2 — Domínio (commit só no branch da Tarefa 4)
- [x] Tarefa 3 — Aplicação (commit só no branch da Tarefa 4)
- [x] Tarefa 4 — Infraestrutura (commit só no branch da Tarefa 4)
- [ ] Tarefa 5 — API ⛔ ← **atual**; aguarda as dúvidas em `active-context.md` (Swagger já configurado só para desenvolvimento; o restante está pendente)
- [ ] Tarefa 6 — Container (não há Dockerfile)

### Estado verificado em 2026-11-05

| Item | Estado |
|------|--------|
| Solution | 4 projetos em `src/` e 3 em `tests/`, em .NET 10 (SDK 10.0.403) |
| Pacotes | Dapper, MySqlConnector, Swashbuckle.AspNetCore e xUnit referenciados |
| Código | Domínio, aplicação e infraestrutura implementados, além do `Program.cs`; as pastas da Api ainda têm apenas `.gitkeep` |
| Build | 0 erros e 0 avisos |
| Testes | 110 aprovados: 55 em `CadastroPessoas.Domain.Tests`, 18 em `CadastroPessoas.Application.Tests` e 37 em `CadastroPessoas.Infrastructure.Tests` |
| Rotas da API | Nenhuma implementada |
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

- [ ] Levar o reinício do projeto e a Tarefa 4 para o `master`. Hoje estão só no branch `worktree-tarefa-4-infraestrutura` (D-010). Em espera: o responsável pediu para não commitar por enquanto fora desse branch.
- [ ] Definir um remoto para o repositório; hoje não há nenhum.
- [ ] Definir o que fazer com `memory-bank/`, que parece ser só uma cópia do modelo de `CLAUDE.md`.
- [ ] O arquivo está como `CLAUDE.MD` no disco e `CLAUDE.md` no Git; a diferença pode causar problemas em sistemas que diferenciam maiúsculas.
- [ ] O branch local é `master`, enquanto o branch principal indicado é `main`, que não existe localmente. Falta definir qual usar.
