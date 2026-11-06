# CLAUDE.md

> Ponto de entrada do projeto. Mantenha este arquivo curto (máx. ~1 tela).

## Antes de qualquer tarefa

1. Leia `docs/memory/active-context.md`: onde paramos e qual é o próximo passo.
2. Leia `docs/memory/progress.md`: estado de cada spec.
3. Consulte `docs/memory/conventions.md` antes de escrever código.
4. Abra **somente** a spec da feature atual em `specs/NNN-nome/`.
5. Consulte `docs/memory/decisions.md` antes de propor mudanças de arquitetura.

Não leia todas as specs nem varra o projeto inteiro sem necessidade.

## Projeto

- **Nome:** <nome do projeto>
- **Resumo:** <uma frase; detalhes em `docs/memory/project-brief.md`>
- **Stack:** <ex.: TypeScript, Node 22, PostgreSQL, React>

## Comandos essenciais

```bash
<instalar>   # ex.: npm install
<rodar>      # ex.: npm run dev
<testar>     # ex.: npm test
<lint>       # ex.: npm run lint
```

## Fluxo SDD

- Specs ficam em `specs/NNN-nome/` (`spec.md` → `plan.md` → `tasks.md`).
- A spec é a fonte da verdade. Se a implementação precisar divergir, atualize a spec
  e registre o motivo em `docs/memory/decisions.md`.

## Ao encerrar a sessão

- [ ] Atualizar `docs/memory/active-context.md` (o que foi feito, próximo passo).
- [ ] Atualizar `docs/memory/progress.md`.
- [ ] Registrar decisões novas em `docs/memory/decisions.md`.
- [ ] Registrar convenções novas em `docs/memory/conventions.md`.
