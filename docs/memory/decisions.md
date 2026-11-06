# Decisions

> Log de decisões (estilo ADR curto). Mais recentes no topo.
> Registre quando: escolher tecnologia/padrão, divergir de uma spec,
> descartar uma alternativa que alguém poderia propor de novo.
> Nunca apague uma decisão: marque como "Substituída por D-XXX".

## Índice

| ID | Data | Título | Status |
|----|------|--------|--------|
| D-007 | 2026-11-05 | Reunir todos os erros de validação | Aceita |
| D-006 | 2026-11-05 | IPessoaService como única entrada da aplicação | Aceita |
| D-005 | 2026-11-05 | Datas no horário local | Aceita |
| D-004 | 2026-11-05 | Reinício do projeto para testar a memória | Aceita |
| D-003 | 2026-11-04 | Segredos de produção no AWS Secrets Manager via ECS | Aceita |
| D-002 | 2026-11-04 | appsettings.json apenas no computador local | Aceita |
| D-001 | 2026-11-04 | Formato do valor criptografado da senha | Aceita |

---

## D-007 — Reunir todos os erros de validação

- **Data:** 2026-11-05
- **Status:** Aceita
- **Spec relacionada:** `specs/001-cadastro_de_pessoa/`

**Contexto:** o `plan.md` define a lista `Erros`, mas a primeira versão do domínio lançava uma exceção no primeiro erro encontrado.

**Decisão:** retornar todos os erros de validação encontrados. `Pessoa` recebe o CPF como texto e reúne os erros em uma única `ValidacaoException`. `CriarPessoaUseCase` acrescenta o erro de CPF duplicado aos demais.

**Motivo:** definição do responsável pelo projeto.

**Alternativas descartadas:**
- Parar no primeiro erro — o cliente precisaria de várias chamadas para descobrir todos os problemas.

**Consequências:** as exceções específicas de cada campo foram substituídas por `ValidacaoException`, com as mensagens em `ErrosDePessoa`. A validação de criação roda antes da consulta de CPF duplicado e novamente em `Pessoa.Criar`.

---

## D-006 — IPessoaService como única entrada da aplicação

- **Data:** 2026-11-05
- **Status:** Aceita
- **Spec relacionada:** `specs/001-cadastro_de_pessoa/`

**Contexto:** o `plan.md` e o `tasks.md` citavam `IPessoaService` e um caso de uso por operação, sem definir como se relacionam.

**Decisão:** a API usa apenas `IPessoaService`. A classe `Services/PessoaService.cs` implementa a interface e chama os casos de uso.

**Motivo:** definição do responsável pelo projeto.

**Alternativas descartadas:**
- Controller chamar cada caso de uso diretamente — expõe os casos de uso à API.

**Consequências:** o `plan.md` passou a incluir `Services/PessoaService.cs`. Toda nova operação precisa ser exposta em `IPessoaService`.

---

## D-005 — Datas no horário local

- **Data:** 2026-11-05
- **Status:** Aceita
- **Spec relacionada:** `specs/001-cadastro_de_pessoa/`

**Contexto:** a spec não definia o fuso usado no "hoje" da validação de `datadenascimento` nem em `datadecadastro` e `dataatualizacao`.

**Decisão:** usar o horário local nos três casos.

**Motivo:** definição do responsável pelo projeto.

**Alternativas descartadas:**
- UTC — usado na primeira versão da spec e não escolhido nesta.

**Consequências:** o valor depende do fuso do ambiente de execução. O fuso do container de produção ainda não está definido (ver `active-context.md`).

---

## D-004 — Reinício do projeto para testar a memória

- **Data:** 2026-11-05
- **Status:** Aceita
- **Spec relacionada:** `specs/001-cadastro_de_pessoa/`

**Contexto:** a primeira versão da spec tinha 15 tasks, com as 7 primeiras commitadas até `645f5fb`.

**Decisão:** reiniciar o projeto com spec, plano, tarefas e estrutura de pastas novos, descartando a implementação anterior.

**Motivo:** o projeto serve para testar o SDD, e o reinício permite testar o uso da memória em `docs/memory/`.

**Alternativas descartadas:**
- Continuar a implementação anterior — não atenderia ao objetivo de testar a memória desde o início.

**Consequências:** o código antigo continua no histórico do Git, mas não vale como referência. O reinício ainda não foi commitado, por decisão do responsável pelo projeto.

---

## D-003 — Segredos de produção no AWS Secrets Manager via ECS

- **Data:** 2026-11-04
- **Status:** Aceita
- **Spec relacionada:** `specs/001-cadastro_de_pessoa/`

**Contexto:** com o appsettings.json apenas no computador local (D-002), faltava definir como a conexão e a chave chegam ao container de produção. O `CLAUDE.md` determina que a string de conexão fique no appsettings.json.

**Decisão:** em produção, a aplicação roda no AWS ECS, que injeta `CHAVE_DE_SEGURANCA` e `ConnectionStrings__CadastroPessoas` a partir do AWS Secrets Manager. A senha da string continua criptografada no formato da D-001. A configuração na AWS é da área de infra/segurança.

**Motivo:** o ambiente de produção usa ECS com Secrets Manager, e o ASP.NET Core já usa essa variável no lugar do valor do appsettings.json.

**Alternativas descartadas:**
- Montar um appsettings.json de produção como volume — exigiria manter o arquivo fora do Secrets Manager.

**Consequências:** a regra do `CLAUDE.md` sobre a string de conexão no appsettings.json vale para o desenvolvimento local; produção é uma exceção. Segredos alterados só chegam aos containers em uma nova implantação do serviço.

---

## D-002 — appsettings.json apenas no computador local

- **Data:** 2026-11-04
- **Status:** Aceita
- **Spec relacionada:** `specs/001-cadastro_de_pessoa/`

**Contexto:** o appsettings.json local contém a string de conexão com a senha criptografada. O arquivo é rastreado pelo Git e está marcado com `skip-worktree`, e a versão commitada usa outro usuário e senha vazia.

**Decisão:** manter o arquivo apenas no computador local, sem commitar o valor e mantendo a marcação `skip-worktree`.

**Motivo:** definição do responsável pelo projeto.

**Alternativas descartadas:**
- Commitar o appsettings.json com a senha criptografada — o responsável pelo projeto optou por não versionar o valor.

**Consequências:** um clone novo recebe a versão commitada, sem a conexão válida, e precisa configurar o arquivo. A forma de entregar a conexão ao container de produção continua pendente na Tarefa 6 do `tasks.md`.

---

## D-001 — Formato do valor criptografado da senha

- **Data:** 2026-11-04
- **Status:** Aceita
- **Spec relacionada:** `specs/001-cadastro_de_pessoa/`

**Contexto:** o `plan.md` definia o AES-GCM e a chave, mas não a organização dos dados no valor em Base64, o que bloqueava a Tarefa 4.

**Decisão:** usar o Base64 dos bytes na ordem `[nonce de 12 bytes][texto cifrado][tag de 16 bytes]`, com texto original em UTF-8 e sem dados associados (AAD).

**Motivo:** é o formato gerado pela aplicação de criptografia já existente, `C:\Repositorio\Criptografia\criptografar.py`.

**Alternativas descartadas:**
- Definir outro formato — exigiria alterar a aplicação de criptografia e gerar novamente os valores já criptografados.

**Consequências:** a descriptografia em C# precisa separar os três trechos por posição. Qualquer mudança de formato precisa ser feita nas duas aplicações.
