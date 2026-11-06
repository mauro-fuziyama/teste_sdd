# Decisions

> Log de decisões (estilo ADR curto). Mais recentes no topo.
> Registre quando: escolher tecnologia/padrão, divergir de uma spec,
> descartar uma alternativa que alguém poderia propor de novo.
> Nunca apague uma decisão: marque como "Substituída por D-XXX".

## Índice

| ID | Data | Título | Status |
|----|------|--------|--------|
| D-016 | 2026-11-06 | Sessões em segundo plano sem worktree | Aceita |
| D-015 | 2026-11-06 | Imagem de produção | Aceita |
| D-014 | 2026-11-05 | Pesquisa por nome em branco rejeitada | Aceita |
| D-013 | 2026-11-05 | Requisição malformada retorna 400 com a lista Erros | Aceita |
| D-012 | 2026-11-05 | Registro inexistente retorna 404 com a lista Erros | Aceita |
| D-011 | 2026-11-05 | DependencyInjection.cs também registra a aplicação | Aceita |
| D-010 | 2026-11-05 | Commit da Tarefa 4 apenas em branch próprio | Aceita |
| D-009 | 2026-11-05 | CPF inválido lido do banco é falha de servidor | Aceita |
| D-008 | 2026-11-05 | Testes de infraestrutura no banco raspadinha_dev | Aceita |
| D-007 | 2026-11-05 | Reunir todos os erros de validação | Aceita |
| D-006 | 2026-11-05 | IPessoaService como única entrada da aplicação | Aceita |
| D-005 | 2026-11-05 | Datas no horário local | Aceita |
| D-004 | 2026-11-05 | Reinício do projeto para testar a memória | Aceita |
| D-003 | 2026-11-04 | Segredos de produção no AWS Secrets Manager via ECS | Aceita |
| D-002 | 2026-11-04 | appsettings.json apenas no computador local | Aceita |
| D-001 | 2026-11-04 | Formato do valor criptografado da senha | Aceita |

---

## D-016 — Sessões em segundo plano sem worktree

- **Data:** 2026-11-06
- **Status:** Aceita
- **Spec relacionada:** `specs/001-cadastro_de_pessoa/`

**Contexto:** as Tarefas 4 e 5 foram feitas em um worktree em `.claude/worktrees/` (D-010), e o responsável pediu para não manter vários branches ou worktrees no repositório. As sessões do Claude Code em segundo plano só editavam arquivos dentro de um worktree.

**Decisão:** trabalhar sempre no diretório principal. O `.claude/settings.json` do repositório define `"worktree": {"bgIsolation": "none"}`, criado pelo responsável.

**Motivo:** definição do responsável pelo projeto.

**Alternativas descartadas:**
- Worktree temporário por tarefa — exigiria levar os commits de volta e remover o worktree e o branch a cada tarefa.

**Consequências:** a prática da D-010 não se repete. Sessões em segundo plano editam o mesmo diretório que o responsável usa, então não devem rodar em paralelo com trabalho manual nos mesmos arquivos.

---

## D-015 — Imagem de produção

- **Data:** 2026-11-06
- **Status:** Aceita
- **Spec relacionada:** `specs/001-cadastro_de_pessoa/`

**Contexto:** a Tarefa 6 pede o Dockerfile de produção, sem segredos na imagem e com o fuso America/Sao_Paulo, mas não definia imagem base, porta, usuário nem o tratamento do `global.json`.

**Decisão:** build em duas etapas, com as imagens oficiais `sdk:10.0` e `aspnet:10.0`, porta 8080 e usuário `$APP_UID`. O fuso vem de `TZ`, com verificação no build. O `.dockerignore` exclui os `appsettings*.json` da API, e o build não copia o `global.json`.

**Motivo:** são os padrões das imagens oficiais .NET. O `global.json` fixa o SDK local `10.0.403`, que pode não coincidir com o da imagem.

**Alternativas descartadas:**
- Imagens Alpine ou chiseled — não trazem os dados de fuso sem passos extras.
- Instalar o certificado do antivírus no Dockerfile — é uma particularidade do computador de desenvolvimento e não deve ir para a imagem de produção.

**Consequências:** `UseHttpsRedirection` continua no `Program.cs`, mas o container não tem porta HTTPS e só registra um aviso no log. A terminação de TLS fica com a AWS, sob responsabilidade da área de infra/segurança. No computador de desenvolvimento, o build local exige confiar no certificado do Norton (ver `conventions.md`).

---

## D-014 — Pesquisa por nome em branco rejeitada

- **Data:** 2026-11-05
- **Status:** Aceita
- **Spec relacionada:** `specs/001-cadastro_de_pessoa/`

**Contexto:** a spec não definia a pesquisa com valor só de espaços, e o valor era repassado como veio.

**Decisão:** `PesquisarPessoaUseCase` rejeita valor nulo, vazio ou só com espaços com `ValidacaoException` e `ErrosDePessoa.NomeDaPesquisaObrigatorio`, resultando em 400.

**Motivo:** definição do responsável pelo projeto.

**Alternativas descartadas:**
- Repassar como veio — um espaço retornaria todo nome composto.
- Remover os espaços e listar tudo — sem paginação, a resposta poderia ser muito grande.

**Consequências:** o parâmetro de rota e `IPessoaService.PesquisarPorNomeAsync` aceitam `string?`, porque o ASP.NET converte o valor em branco em `null` e o `[ApiController]` o rejeitaria antes da regra de negócio, com a mensagem errada.

---

## D-013 — Requisição malformada retorna 400 com a lista Erros

- **Data:** 2026-11-05
- **Status:** Aceita
- **Spec relacionada:** `specs/001-cadastro_de_pessoa/`

**Contexto:** a resposta automática de `[ApiController]` usa `ProblemDetails`, e o plano não citava id não numérico, JSON malformado nem data fora do formato.

**Decisão:** esses casos retornam 400 com a lista `Erros`, por meio de `InvalidModelStateResponseFactory` e `ErrosDeRequisicaoMapper`, com mensagens fixas em português.

**Motivo:** definição do responsável pelo projeto. As mensagens fixas também evitam expor tipos internos.

**Alternativas descartadas:**
- Restrição de inteiro na rota — id não numérico retornaria 404 sem a lista `Erros`.

**Consequências:** com a data fora do formato, a desserialização para antes da validação de negócio, e a resposta não traz os erros dos demais campos. É um limite aceito à regra de reunir todos os erros (D-007).

---

## D-012 — Registro inexistente retorna 404 com a lista Erros

- **Data:** 2026-11-05
- **Status:** Aceita
- **Spec relacionada:** `specs/001-cadastro_de_pessoa/`

**Contexto:** o `plan.md` previa só 200 e 400 em PUT e DELETE e não definia o corpo do 404 das consultas.

**Decisão:** PUT e DELETE de id inexistente retornam 404, e todo 404 da API traz a lista `Erros`, com a mensagem de pessoa não encontrada.

**Motivo:** definição do responsável pelo projeto. Mantém as rotas coerentes e os erros em um único formato.

**Alternativas descartadas:**
- 400 em PUT e DELETE — o cliente só distinguiria os casos pela mensagem.
- 404 com corpo vazio — criaria uma exceção ao formato de erros.

**Consequências:** o controller lança `PessoaNaoEncontradaException` na consulta sem registro, e o middleware trata todos os 404. Rotas que não existem continuam retornando 404 sem corpo, pelo próprio ASP.NET.

---

## D-011 — DependencyInjection.cs também registra a aplicação

- **Data:** 2026-11-05
- **Status:** Aceita
- **Spec relacionada:** `specs/001-cadastro_de_pessoa/`

**Contexto:** o `plan.md` prevê apenas um `DependencyInjection.cs`, na infraestrutura, e os casos de uso, o `IPessoaService` e o `TimeProvider` também precisam ser registrados.

**Decisão:** `AdicionarInfraestrutura` registra as dependências da infraestrutura e da aplicação. A chamada no `Program.cs` fica para a Tarefa 5.

**Motivo:** segue a estrutura do `plan.md` e a dependência existente `Infrastructure → Application`, sem criar um arquivo que o plano não prevê.

**Alternativas descartadas:**
- Criar um `DependencyInjection.cs` na aplicação — acrescentaria um arquivo e um pacote à camada de aplicação sem necessidade.

**Consequências:** o projeto Infrastructure passa a usar os pacotes `Microsoft.Extensions.Configuration.Abstractions` e `Microsoft.Extensions.DependencyInjection.Abstractions`.

---

## D-010 — Commit da Tarefa 4 apenas em branch próprio

- **Data:** 2026-11-05
- **Status:** Aceita
- **Spec relacionada:** `specs/001-cadastro_de_pessoa/`

**Contexto:** a Tarefa 4 foi executada em uma sessão isolada em worktree, criado a partir do `645f5fb`, que tem o código antigo. O reinício (D-004) só existia sem commit no diretório de trabalho, e a orientação era não commitar por enquanto.

**Decisão:** copiar o estado atual para o worktree e commitar apenas no branch `worktree-tarefa-4-infraestrutura`. Um commit registra o reinício e outro, a Tarefa 4.

**Motivo:** definição do responsável pelo projeto, como exceção limitada a esse branch.

**Alternativas descartadas:**
- Worktree sem commit — o trabalho se perderia se o worktree fosse removido junto com a sessão.
- Sessão interativa sem worktree — adiaria a execução da tarefa.

**Consequências:** o `master` e o diretório de trabalho principal não foram alterados e continuam sem a Tarefa 4. O `appsettings.json` local não entrou em nenhum commit.

---

## D-009 — CPF inválido lido do banco é falha de servidor

- **Data:** 2026-11-05
- **Status:** Aceita
- **Spec relacionada:** `specs/001-cadastro_de_pessoa/`

**Contexto:** a leitura cria o Value Object `CPF`, que lança `ValidacaoException` para valor inválido. Sem tratamento, um dado ruim no banco viraria um erro 400, de regra de negócio.

**Decisão:** o repositório lança `RegistroCorrompidoException`, que não é uma `DominioException` e resulta em 500. A mensagem cita o id, mas não o CPF.

**Motivo:** definição do responsável pelo projeto. O problema fica visível e o dado não é alterado.

**Alternativas descartadas:**
- Ler sem validar — a API poderia devolver CPFs inválidos.
- Ignorar a linha — o registro pareceria não existir, embora o CPF continuasse ocupado pelo índice único.

**Consequências:** na pesquisa por nome, uma única linha inválida faz toda a pesquisa falhar. A tabela estava vazia na verificação de 2026-11-05.

---

## D-008 — Testes de infraestrutura no banco raspadinha_dev

- **Data:** 2026-11-05
- **Status:** Aceita
- **Spec relacionada:** `specs/001-cadastro_de_pessoa/`

**Contexto:** os testes do `PessoaRepository` gravam na tabela `pessoa`, e o `plan.md` não definia o banco.

**Decisão:** usar o `raspadinha_dev`, com a conexão do `appsettings.json` local e a `CHAVE_DE_SEGURANCA`. Os testes removem as linhas que criam.

**Motivo:** definição do responsável pelo projeto: os dados desse banco são de teste e podem ser apagados ou atualizados.

**Alternativas descartadas:**
- Banco separado só para testes — exigiria criar e dar acesso a outro banco.
- Testes sem banco real — deixariam o SQL do repositório sem teste automatizado.

**Consequências:** `dotnet test` depende do MySQL local, da chave e do `appsettings.json` local; em um clone novo, os testes de infraestrutura falham até que esses itens sejam configurados.

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

**Consequências:** o valor depende do fuso do ambiente de execução. Em 2026-11-06, o responsável definiu o fuso America/Sao_Paulo para o container de produção, a ser configurado na Tarefa 6.

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
