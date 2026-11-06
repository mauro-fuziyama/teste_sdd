# Progress

> Estado de cada spec num relance. Evita reler o `tasks.md` de todas as features.
> Legenda: ✅ concluída · 🔄 em andamento · ⏳ pendente · ⛔ bloqueada · 🗄️ arquivada

**Última atualização:** 2026-11-05

## Visão geral

| Spec | Status | Tarefas | Observação |
|------|--------|---------|------------|
| `001-cadastro_de_pessoa` | 🔄 | 3/6 | Setup, domínio e aplicação concluídos, sem commit. Tarefa 4 aguarda esclarecimento |

## Em andamento

### `001-cadastro_de_pessoa`

- [x] Tarefa 1 — Setup inicial (sem commit)
- [x] Tarefa 2 — Domínio (sem commit)
- [x] Tarefa 3 — Aplicação (sem commit)
- [ ] Tarefa 4 — Infraestrutura ⛔ ← **atual**; aguarda a dúvida em `active-context.md`
- [ ] Tarefa 5 — API (Swagger já configurado só para desenvolvimento; o restante está pendente)
- [ ] Tarefa 6 — Container (não há Dockerfile)

### Estado verificado em 2026-11-05

| Item | Estado |
|------|--------|
| Solution | 4 projetos em `src/` e 3 em `tests/`, em .NET 10 (SDK 10.0.403) |
| Pacotes | Dapper, MySqlConnector, Swashbuckle.AspNetCore e xUnit referenciados |
| Código | Domínio e aplicação implementados, além do `Program.cs`; as pastas de Infrastructure e Api ainda têm apenas `.gitkeep` |
| Build | 0 erros e 0 avisos |
| Testes | 73 aprovados: 55 em `CadastroPessoas.Domain.Tests` e 18 em `CadastroPessoas.Application.Tests`; Infrastructure ainda sem testes |
| Rotas da API | Nenhuma implementada |
| `CHAVE_DE_SEGURANCA` | Definida; decodifica para 32 bytes (256 bits) |
| Senha no `appsettings.json` local | Base64 de 42 bytes, compatível com o tamanho mínimo do formato da D-001 (descriptografia não testada) |
| Senha nas versões commitadas do `appsettings.json` | Nenhum valor no histórico |
| MySQL local | Porta 3306 aberta; conexão, login e tabela `pessoa` não testados |

## Concluídas (resumo)

Nenhuma spec concluída.

## Histórico

- **2026-10-30:** primeira versão da spec 001, com 15 tasks. As tasks 1 a 7 (domínio e aplicação) foram commitadas até `645f5fb`.
- **Até 2026-11-05:** spec, plano e tarefas reescritos com outro escopo e estrutura de pastas, para testar a memória do projeto (D-004). A implementação antiga foi removida do diretório de trabalho, mas continua no `HEAD`.

## Dívida técnica / pendências transversais

- [ ] Commitar o reinício do projeto (cerca de 98 caminhos alterados, removidos ou novos). Em espera: o responsável pediu para não commitar por enquanto.
- [ ] Definir um remoto para o repositório; hoje não há nenhum.
- [ ] Definir o que fazer com `memory-bank/`, que parece ser só uma cópia do modelo de `CLAUDE.md`.
- [ ] O arquivo está como `CLAUDE.MD` no disco e `CLAUDE.md` no Git; a diferença pode causar problemas em sistemas que diferenciam maiúsculas.
- [ ] O branch local é `master`, enquanto o branch principal indicado é `main`, que não existe localmente. Falta definir qual usar.
