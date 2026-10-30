# Tasks: Cadastro de Pessoa (CRUD)

- **Feature:** 001-cadastro-pessoa
- **Spec:** [spec.md](./spec.md)
- **Plan:** [plan.md](./plan.md)
- **Status:** Em andamento (Tasks 1 a 7 concluídas)
- **Data:** 2026-10-30

## Como usar este arquivo

- Execute as tasks **na ordem**. Cada uma indica de quais depende.
- Marque `[x]` em cada item ao concluir. Uma task só está pronta quando todos os **critérios de conclusão** forem atendidos.
- Testes vêm antes ou junto da implementação (TDD sempre que possível). Nenhuma task termina com teste falhando ou warning no build.
- Ao final de cada task: `dotnet build` sem warnings, `dotnet test` verde e um commit com a mensagem sugerida.
- Siga os padrões de código da seção 10 do plano em todas as tasks.

## Visão geral

| # | Task | Camada | Depende de |
|---|------|--------|------------|
| 1 | Configuração do ambiente, estrutura de diretórios e arquivos iniciais | Todas | — |
| 2 | Tipos base do domínio (`Result`, `Erro`, `TipoErro`) | Domain | 1 |
| 3 | Value Object `Cpf` | Domain | 2 |
| 4 | Value Objects `Nome`, `DataNascimento` e `Sexo` | Domain | 2 |
| 5 | Catálogo `PessoaErros` e entidade `Pessoa` | Domain | 3, 4 |
| 6 | Portas e modelos da aplicação | Application | 5 |
| 7 | Casos de uso | Application | 6 |
| 8 | Script de teste da tabela `pessoa` e `BancoOptions` | Infrastructure | 1 |
| 9 | Fábrica de conexões, `PessoaSql` e `PessoaRegistro` | Infrastructure | 5, 8 |
| 10 | `PessoaRepositorio` e testes de integração do repositório | Infrastructure | 6, 9 |
| 11 | Contratos HTTP e conversão de `Result` para HTTP | Api | 6 |
| 12 | `PessoasController`, `Program.cs` e documentação OpenAPI | Api | 7, 10, 11 |
| 13 | Testes de integração da API (CA01–CA10) | Api | 12 |
| 14 | Testes de arquitetura | Todas | 12 |
| 15 | Revisão final e atualização da documentação | Todas | 13, 14 |

---

## Task 1 – Configuração do ambiente, estrutura de diretórios e arquivos iniciais

**Objetivo:** deixar a solução compilando, com todos os projetos criados, referências entre camadas corretas, pacotes instalados, MySQL rodando localmente e padrões de código aplicados no build. Nenhuma regra de negócio é implementada aqui.

**Requisitos:** plan.md seções 1, 2, 3, 8 e 10.

### 1.1 Pré-requisitos da máquina

- [x] Instalar o **.NET 10 SDK** e conferir com `dotnet --version`
- [x] Instalar o **Docker** (Docker Desktop ou equivalente) e conferir com `docker --version` (usado só pelos testes de integração com Testcontainers)
- [x] Ter acesso ao **MySQL 8.0 local** já existente, com o schema e a tabela `pessoa` criados
- [x] Instalar o **Git** e conferir com `git --version`
- [x] (Opcional) Instalar um cliente MySQL, como DBeaver ou MySQL Workbench, para inspecionar o banco

### 1.2 Repositório e arquivos da raiz

- [x] Iniciar o Git na raiz `Treinameto_SDD/`: `git init`
- [x] Criar `.gitignore` padrão do .NET: `dotnet new gitignore`
- [x] Criar `global.json` fixando a versão do SDK: `dotnet new globaljson --sdk-version <versão instalada> --roll-forward latestFeature`
- [x] Criar `.editorconfig`: `dotnet new editorconfig` e ajustar (indentação de 4 espaços, `var` quando o tipo é evidente, namespaces no escopo do arquivo, campos privados sem `_` ou com `_` conforme o padrão do time)
- [x] Criar `Directory.Build.props` na raiz, aplicado a todos os projetos:

```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <AnalysisLevel>latest-recommended</AnalysisLevel>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
  </PropertyGroup>
</Project>
```

- [x] Criar `README.md` com: objetivo do projeto, pré-requisitos, como subir o banco, como rodar a API, como rodar os testes e link para `specs/`

### 1.3 Estrutura de diretórios

- [x] Conferir as pastas da raiz (os arquivos SDD já estão no lugar):

```
Treinameto_SDD/
├── specs/001-cadastro_de_pessoa/   ← spec.md, plan.md, tasks.md
├── src/
└── tests/
```

- [x] Completar o `CLAUDE.md` existente com as regras do projeto e os comandos, sem apagar o conteúdo atual

### 1.4 Solução e projetos

- [x] Criar a solução no formato `.sln` (o .NET 10 usa `.slnx` como padrão):

```bash
dotnet new sln -n CadastroPessoas --format sln
```

- [x] Criar os projetos de código:

```bash
dotnet new classlib -n CadastroPessoas.Domain         -o src/CadastroPessoas.Domain
dotnet new classlib -n CadastroPessoas.Application    -o src/CadastroPessoas.Application
dotnet new classlib -n CadastroPessoas.Infrastructure -o src/CadastroPessoas.Infrastructure
dotnet new webapi   -n CadastroPessoas.Api            -o src/CadastroPessoas.Api --use-controllers
```

- [x] Criar os projetos de teste:

```bash
dotnet new xunit -n CadastroPessoas.Domain.Tests                    -o tests/CadastroPessoas.Domain.Tests
dotnet new xunit -n CadastroPessoas.Application.Tests               -o tests/CadastroPessoas.Application.Tests
dotnet new xunit -n CadastroPessoas.Infrastructure.IntegrationTests -o tests/CadastroPessoas.Infrastructure.IntegrationTests
dotnet new xunit -n CadastroPessoas.Api.IntegrationTests            -o tests/CadastroPessoas.Api.IntegrationTests
dotnet new xunit -n CadastroPessoas.ArchitectureTests               -o tests/CadastroPessoas.ArchitectureTests
```

- [x] Adicionar todos à solução: `dotnet sln add src/**/*.csproj tests/**/*.csproj` (no Windows, adicionar um por um ou usar `Get-ChildItem -Recurse *.csproj`)
- [x] Remover os arquivos de exemplo gerados pelos templates (`Class1.cs`, `UnitTest1.cs`, `WeatherForecast*.cs`)
- [x] Remover do `.csproj` de cada projeto as propriedades já definidas no `Directory.Build.props` (`TargetFramework`, `Nullable`, `ImplicitUsings`)

### 1.5 Referências entre projetos (regra de dependência do plano, seção 2)

- [x] `Application` → `Domain`
- [x] `Infrastructure` → `Application`
- [x] `Api` → `Application` e `Infrastructure`
- [x] `Domain.Tests` → `Domain`
- [x] `Application.Tests` → `Application`
- [x] `Infrastructure.IntegrationTests` → `Infrastructure`
- [x] `Api.IntegrationTests` → `Api`
- [x] `ArchitectureTests` → `Domain`, `Application`, `Infrastructure` e `Api`

```bash
dotnet add src/CadastroPessoas.Application    reference src/CadastroPessoas.Domain
dotnet add src/CadastroPessoas.Infrastructure reference src/CadastroPessoas.Application
dotnet add src/CadastroPessoas.Api            reference src/CadastroPessoas.Application src/CadastroPessoas.Infrastructure
# ...e as referências dos projetos de teste listadas acima
```

- [x] Em `Infrastructure`, adicionar `<InternalsVisibleTo Include="CadastroPessoas.Infrastructure.IntegrationTests" />` ao `.csproj`, porque o repositório e as classes de SQL são `internal`

### 1.6 Pacotes NuGet

| Projeto | Pacotes |
|---------|---------|
| `Domain` | nenhum |
| `Application` | nenhum |
| `Infrastructure` | `Dapper`, `MySqlConnector`, `Microsoft.Extensions.Options.ConfigurationExtensions` |
| `Api` | `Microsoft.AspNetCore.OpenApi` (já vem no template), `Scalar.AspNetCore` |
| Todos os testes | `xunit`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk` (já vêm no template), `AwesomeAssertions` |
| `Application.Tests` | `Microsoft.Extensions.TimeProvider.Testing` |
| `Infrastructure.IntegrationTests` | `Testcontainers.MySql` |
| `Api.IntegrationTests` | `Microsoft.AspNetCore.Mvc.Testing`, `Testcontainers.MySql` |
| `ArchitectureTests` | `NetArchTest.Rules` |

- [x] Instalar os pacotes da tabela com `dotnet add <projeto> package <pacote>`
- [x] **Asserções:** usar `AwesomeAssertions` (fork gratuito com a mesma API do FluentAssertions, que exige licença paga a partir da versão 8). Registrar a decisão no `README.md`
- [x] (Recomendado) Centralizar as versões em `Directory.Packages.props` (Central Package Management) para que todos os projetos usem a mesma versão de cada pacote

### 1.7 Pastas e arquivos iniciais de cada projeto

Criar as pastas da seção 3 do plano. Os arquivos de código serão preenchidos nas próximas tasks; aqui ficam só as pastas e os arquivos de configuração.

- [x] `Domain`: `Comum/`, `Pessoas/`, `Pessoas/ValueObjects/`
- [x] `Application`: `Comum/`, `Pessoas/Portas/Entrada/`, `Pessoas/Portas/Saida/`, `Pessoas/CasosDeUso/`, `Pessoas/Modelos/`
- [x] `Infrastructure`: `Persistencia/Conexao/`, `Persistencia/Pessoas/`
- [x] `tests/Scripts/` (script da tabela usado pelos testes de integração, Task 8)

- [x] `Api`: `Controllers/`, `Contratos/`, `Mapeamentos/`, `Configuracoes/`
- [x] `Api/appsettings.json` com o conteúdo da seção 8 do plano
- [x] `Api/appsettings.Development.json` com nível de log `Debug` para o namespace da aplicação
- [x] `Api/Program.cs` reduzido ao mínimo (builder, controllers, `app.Run()`), sem o endpoint de exemplo do template

### 1.8 Banco de dados local

- [x] Usar o MySQL 8.0 local já existente (schema, tabela `pessoa` e índice único `cpf_UNIQUE` criados). Não há Docker Compose nem scripts de criação executados pela aplicação
- [x] Connection string no `appsettings.json`, versionada com a senha vazia; senha preenchida só localmente, com `git update-index --skip-worktree`
- [x] Conferir a conexão e a estrutura da tabela `pessoa` (plan.md 7.1)

> Revisão (2026-10-30): a configuração inicial por `.env` + DotNetEnv foi substituída por `appsettings.json` (pendência P3 do plano).

### Critérios de conclusão

- [x] `dotnet build` termina sem erros e sem warnings
- [x] `dotnet test` executa (ainda sem testes) sem falhas
- [x] É possível conectar ao MySQL local com a connection string do `appsettings.json`
- [x] `dotnet run --project src/CadastroPessoas.Api` sobe a API sem erros
- [x] `Domain` e `Application` não têm nenhum pacote NuGet
- [x] Nenhuma senha aparece em arquivo versionado (`git status` e `git diff` conferidos)
- [x] A árvore de pastas confere com a seção 3 do plano

**Commit sugerido:** `chore: configura ambiente e estrutura inicial da solução`

---

## Task 2 – Tipos base do domínio

**Depende de:** 1 · **Requisitos:** plan.md 4.1

- [x] Criar `Domain/Comum/TipoErro.cs` (`Validacao`, `NaoEncontrado`, `Conflito`)
- [x] Criar `Domain/Comum/Erro.cs` (`record Erro(string Campo, string Mensagem, TipoErro Tipo)`)
- [x] Criar `Domain/Comum/Result.cs` com `Ok`, `Falha`, `Sucesso` e conversões implícitas
- [x] Testes em `Domain.Tests/Comum/ResultTests.cs`:
  - [x] `Ok` tem `Sucesso = true`, valor preenchido e lista de erros vazia
  - [x] `Falha` com vários erros mantém todos, na ordem
  - [x] Conversão implícita de valor e de `Erro` funciona

**Critérios de conclusão:** testes verdes; `Result` sem dependências externas.

**Commit sugerido:** `feat(domain): adiciona Result e tipos de erro`

---

## Task 3 – Value Object `Cpf`

**Depende de:** 2 · **Requisitos:** spec seções 3 e 4, RNF03; plan.md 4.2

- [x] Escrever primeiro os testes em `Domain.Tests/Pessoas/CpfTests.cs`, com `[Theory]` cobrindo **todos** os exemplos da seção 4 da spec:
  - [x] `529.982.247-25` e `52998224725` → válidos
  - [x] `529.982.247-26` → inválido (dígito verificador)
  - [x] `111.111.111-11` e demais sequências de dígitos iguais → inválidos
  - [x] `5299822472` → inválido (10 dígitos)
  - [x] `529.982.247-2A` → inválido (letra)
  - [x] vazio, espaços e `null` → inválidos
- [x] Implementar `Domain/Pessoas/ValueObjects/Cpf.cs` conforme o plano (`Criar`, `Numero`, `Formatado`)
- [x] Adicionar `Mascarado()` → `***.982.247-**` (para logs) e `Restaurar(string numero)` (para o repositório)
- [x] Testes adicionais:
  - [x] `Numero` sempre tem 11 dígitos sem máscara
  - [x] `Formatado()` retorna `529.982.247-25`
  - [x] `Mascarado()` retorna `***.982.247-**`
  - [x] Dois `Cpf` com o mesmo número são iguais (igualdade por valor)

**Critérios de conclusão:** todos os exemplos da spec cobertos; nenhum método com mais de 2 níveis de indentação.

**Commit sugerido:** `feat(domain): adiciona Value Object Cpf com validação`

---

## Task 4 – Value Objects `Nome`, `DataNascimento` e `Sexo`

**Depende de:** 2 · **Requisitos:** spec seções 3 e 7; plan.md 4.2 e 4.5

- [x] `Nome`: testes e implementação
  - [x] Remove espaços nas pontas e reduz espaços internos repetidos
  - [x] Aceita 3 e 150 caracteres; rejeita 2 e 151
  - [x] Rejeita vazio, só espaços e `null` (mensagem de campo obrigatório)
- [x] `DataNascimento`: testes e implementação (recebe a data atual como parâmetro)
  - [x] Aceita hoje e datas passadas
  - [x] Rejeita amanhã (data futura)
  - [x] Aceita exatamente 130 anos; rejeita 130 anos e 1 dia
  - [x] Rejeita `null`
- [x] `Sexo`: testes e implementação
  - [x] Aceita `M`, `F`, `O`, `N`; minúsculas e espaços nas pontas são aceitos e normalizados para maiúscula (decisão documentada no teste)
  - [x] Rejeita qualquer outro valor e `null`
  - [x] Expõe `Codigo` (para persistência)
- [x] Limites como constantes nomeadas (`TamanhoMinimo`, `TamanhoMaximo`, `IdadeMaximaAnos`)
- [x] Cada Value Object com método `Restaurar` para uso do repositório

**Critérios de conclusão:** mensagens idênticas às da seção 7 da spec; testes verdes.

**Commit sugerido:** `feat(domain): adiciona Value Objects Nome, DataNascimento e Sexo`

---

## Task 5 – Catálogo `PessoaErros` e entidade `Pessoa`

**Depende de:** 3, 4 · **Requisitos:** RF01, RF04, spec seção 7; plan.md 4.3 e 4.4

- [x] Criar `PessoaErros` com **todas** as mensagens da seção 7 da spec
- [x] Criar a entidade `Pessoa` com propriedades `private set`
- [x] `Pessoa.Criar(dados, agora)`: valida todos os campos e **acumula todos os erros**
- [x] `Pessoa.Atualizar(dados, agora)`: mesma validação; atualiza `AtualizadoEm`; não altera `Id` nem `CriadoEm`
- [x] `Pessoa.Restaurar(...)`: reconstrói sem validar e sem gerar novo `Id`
- [x] `Id` `int` gerado pelo banco: `0` até a gravação, atribuído com `DefinirId(id)`
- [x] `Desativar(agora)` marca `Ativo = false` e atualiza `AtualizadoEm` (exclusão lógica)
- [x] Testes:
  - [x] Criação válida preenche `CriadoEm` e `AtualizadoEm`, com `Id = 0` e `Ativo = true`
  - [x] `Desativar` marca a pessoa como inativa e atualiza `AtualizadoEm`
  - [x] Payload com 4 campos inválidos retorna 4 erros (CA05)
  - [x] Atualização válida muda os campos e `AtualizadoEm`, mantendo `Id` e `CriadoEm`
  - [x] Atualização inválida não altera nenhum campo

> Observação: `Pessoa.Criar` recebe `DadosPessoa`, que é definido em `Application` no plano. Para manter a regra de dependência, o domínio recebe os campos como parâmetros individuais (ou um record próprio do domínio), e `Application` converte `DadosPessoa` nesses parâmetros.

**Critérios de conclusão:** domínio completo e 100% coberto por testes unitários.

**Commit sugerido:** `feat(domain): adiciona entidade Pessoa e catálogo de erros`

---

## Task 6 – Portas e modelos da aplicação

**Depende de:** 5 · **Requisitos:** plan.md 5.1, 5.2 e 5.4

- [x] Modelos em `Application/Pessoas/Modelos/`: `DadosPessoa`, `FiltroPessoas`, `PessoaResultado` (com CPF formatado)
- [x] `Application/Comum/ResultadoPaginado.cs` (`Itens`, `Pagina`, `TamanhoPagina`, `TotalItens`, `TotalPaginas`)
- [x] Portas de entrada: `ICadastrarPessoa`, `IConsultarPessoa`, `IListarPessoas`, `IAtualizarPessoa`, `IExcluirPessoa`
- [x] Porta de saída: `IPessoaRepositorio`
- [x] `CpfDuplicadoException` em `Portas/Saida/`
- [x] Teste: `PessoaResultado.De(pessoa)` retorna o CPF **com máscara**

**Critérios de conclusão:** compila; `Application` continua sem pacotes NuGet.

**Commit sugerido:** `feat(application): define portas e modelos de pessoa`

---

## Task 7 – Casos de uso

**Depende de:** 6 · **Requisitos:** RF01–RF05; plan.md 5.3

- [x] Criar em `Application.Tests` um `PessoaRepositorioEmMemoria` que implementa `IPessoaRepositorio` (usado em todos os testes desta task)
- [x] `CadastrarPessoa` + testes:
  - [x] Sucesso grava e retorna a pessoa
  - [x] Dados inválidos retornam erros de validação sem gravar
  - [x] CPF de pessoa ativa retorna `CpfDuplicado`
  - [x] CPF de pessoa inativa reaproveita o registro: mesmo `id`, dados novos, `CriadoEm` e `AtualizadoEm` com a data atual e pessoa ativa (CA11)
  - [x] `CpfDuplicadoException` lançada pelo repositório vira `CpfDuplicado`
- [x] `ConsultarPessoa` + testes: encontrada; não encontrada → `NaoEncontrada`
- [x] `ListarPessoas` + testes: paginação, cálculo de `TotalPaginas`, filtro de CPF com máscara é normalizado
- [x] `AtualizarPessoa` + testes:
  - [x] Sucesso
  - [x] Não encontrada
  - [x] CPF de outra pessoa → `CpfDuplicado` e nada é alterado (CA08)
  - [x] Manter o próprio CPF não conta como duplicado
  - [x] CPF de pessoa inativa → `CpfDuplicado` (CA12)
- [x] `ExcluirPessoa` + testes: sucesso (pessoa desativada, não removida); não encontrada
- [x] Usar `FakeTimeProvider` para controlar a data atual nos testes
- [x] `RelogioAplicacao` (P4): data de hoje calculada no fuso configurado, com testes da virada de dia entre UTC e o fuso local

**Critérios de conclusão:** cada caso de uso com um único método público; testes verdes.

**Commit sugerido:** `feat(application): implementa casos de uso do cadastro de pessoa`

---

## Task 8 – Script de teste da tabela `pessoa` e `BancoOptions`

**Depende de:** 1 · **Requisitos:** plan.md 7.1, 7.6 e 8

- [ ] Criar `tests/Scripts/criar_tabela_pessoa.sql` replicando exatamente o DDL da tabela `pessoa`, inclusive o índice único `cpf_UNIQUE` (plan.md 7.1)
- [ ] Incluir o script como arquivo vinculado nos dois projetos de testes de integração (copiado para a saída)
- [ ] Criar `Persistencia/BancoOptions.cs` (`TimeoutComandoSegundos`) com validação
- [ ] Teste de integração (Testcontainers, MySQL 8.0): executar o script em um banco vazio cria a tabela `pessoa` com as mesmas colunas do servidor local

**Critérios de conclusão:** estrutura do container igual à do banco local; testes verdes.

**Commit sugerido:** `test(infra): adiciona script da tabela pessoa para testes de integração`

---

## Task 9 – Fábrica de conexões, `PessoaSql` e `PessoaRegistro`

**Depende de:** 5, 8 · **Requisitos:** plan.md 7.2 e 7.3

- [ ] `IConexaoFactory` e `MySqlConexaoFactory` (lê a connection string do `appsettings.json` e aplica o timeout configurado)
- [ ] `PessoaSql` com todos os comandos da seção 7.3 do plano
- [ ] `PessoaRegistro` com `ParaEntidade()` e `ParametrosDe(pessoa)`
- [ ] Testes unitários de `PessoaRegistro`:
  - [ ] `ParametrosDe` envia o CPF **sem máscara**
  - [ ] Datas convertidas para UTC e truncadas em segundos (`DATETIME` sem fração)
  - [ ] `AtualizadoEm` nulo no banco assume o valor de `CriadoEm`
  - [ ] Ida e volta (`ParametrosDe` → `ParaEntidade`) preserva todos os valores

**Critérios de conclusão:** nenhum SQL fora de `PessoaSql`; nenhum valor concatenado em SQL.

**Commit sugerido:** `feat(infra): adiciona fábrica de conexões, SQL e mapeamento de pessoa`

---

## Task 10 – `PessoaRepositorio` e testes de integração

**Depende de:** 6, 9 · **Requisitos:** RF01–RF05, RNF02, RNF03; plan.md 7.4 e 7.5

- [ ] Criar fixture de testes que sobe MySQL 8.0 com Testcontainers, executa `tests/Scripts/criar_tabela_pessoa.sql` e informa a connection string do container à aplicação
- [ ] Implementar `PessoaRepositorio` com `CommandDefinition` e `CancellationToken` em todas as chamadas
- [ ] Testes de integração, um ou mais por método:
  - [ ] `AdicionarAsync` grava o CPF sem máscara (conferir com consulta direta na tabela) e retorna o `id` gerado
  - [ ] `ObterPorIdAsync` encontra, não encontra e ignora pessoa inativa
  - [ ] `ExisteCpfAsync` com e sem `ignorarId`, considerando pessoas ativas e inativas
  - [ ] `ObterIdInativoPorCpfAsync` encontra só registro inativo
  - [ ] `ReativarAsync` substitui os dados, redefine `cadastro` e volta `ativo = 1`
  - [ ] `ListarAsync`: paginação, total, ordenação por nome, filtro por CPF e filtro por nome sem acento (`joao` encontra `João`, CA09)
  - [ ] `AtualizarAsync` não altera `id`, `cadastro` nem `ativo`
  - [ ] `DesativarAsync` marca `ativo = 0`, mantém o registro e ele deixa de aparecer em `ObterPorIdAsync` e `ListarAsync`
  - [ ] Duas gravações com o mesmo CPF: a segunda lança `CpfDuplicadoException` (erro 1062 do índice `cpf_UNIQUE`)
- [ ] `InjecaoDependencia.cs` com o método de extensão que registra fábrica, repositório e options
- [ ] Conferir que nenhum log contém parâmetros SQL (RNF03)

**Critérios de conclusão:** todos os comandos de `PessoaSql` exercitados por pelo menos um teste.

**Commit sugerido:** `feat(infra): implementa repositório de pessoas com Dapper`

---

## Task 11 – Contratos HTTP e conversão de `Result`

**Depende de:** 6 · **Requisitos:** spec seção 6; plan.md 6.3 e 6.4

- [ ] `Contratos/PessoaRequest.cs` com campos anuláveis
- [ ] `Contratos/PessoaResponse.cs` e `Contratos/PaginaResponse.cs`
- [ ] `Mapeamentos/PessoaMapeamento.cs` (`ParaDados`, `ParaResponse`)
- [ ] `Mapeamentos/ResultParaHttp.cs`: `Validacao` → 400, `NaoEncontrado` → 404, `Conflito` → 409, com `ProblemDetails` agrupando erros por campo
- [ ] `Configuracoes/PaginacaoOptions.cs` com validação (`TamanhoPadrao` ≤ `TamanhoMaximo`)
- [ ] Testes unitários de `ResultParaHttp` para os 3 tipos de erro e para múltiplos erros no mesmo campo

**Critérios de conclusão:** formato de erro idêntico ao exemplo da seção 6 da spec.

**Commit sugerido:** `feat(api): adiciona contratos HTTP e conversão de erros`

---

## Task 12 – Controller, `Program.cs` e OpenAPI

**Depende de:** 7, 10, 11 · **Requisitos:** RF01–RF05, RNF04; plan.md 6.1, 6.2 e 8

- [ ] `PessoasController` com os 5 endpoints da seção 6.1 do plano, sem regra de negócio
- [ ] `POST` retorna 201 com header `Location`
- [ ] `GET` de listagem aplica `PaginacaoOptions` (padrão e máximo)
- [ ] `IExceptionHandler` global que registra o erro e retorna 500 genérico
- [ ] `Program.cs`: registrar casos de uso, infraestrutura, options com `ValidateOnStart`, `TimeProvider.System`, ProblemDetails, OpenAPI e Scalar
- [ ] Adicionar `Aplicacao:FusoHorario` ao `appsettings.json` e registrar `RelogioAplicacao` com o `TimeZoneInfo` correspondente (falha na inicialização se o fuso não existir)
- [ ] Documentar exemplos de requisição e resposta no OpenAPI
- [ ] Teste manual: abrir a UI do Scalar e executar o CRUD completo contra o banco local

**Critérios de conclusão:** API funcional ponta a ponta no ambiente local.

**Commit sugerido:** `feat(api): expõe endpoints do cadastro de pessoa`

---

## Task 13 – Testes de integração da API

**Depende de:** 12 · **Requisitos:** spec seção 9 (CA01–CA10), RNF05

- [ ] Criar `WebApplicationFactory` personalizada que usa MySQL em container
- [ ] Um teste por critério de aceite:
  - [ ] CA01 – cadastro válido → 201 com `id`
  - [ ] CA02 – CPF com dígito incorreto → 400 `CPF inválido.`
  - [ ] CA03 – CPF com dígitos iguais → 400
  - [ ] CA04 – CPF duplicado, com e sem máscara → 409
  - [ ] CA05 – vários campos inválidos → 400 listando todos
  - [ ] CA06 – data futura no cadastro e na atualização → 400
  - [ ] CA07 – `id` inexistente em GET, PUT e DELETE → 404
  - [ ] CA08 – trocar CPF para o de outra pessoa → 409 e nada alterado
  - [ ] CA09 – filtro `nome=joao` encontra "João"
  - [ ] CA10 – consultar pessoa excluída → 404 (registro continua no banco com `ativo = 0`)
  - [ ] CA11 – cadastrar CPF de pessoa excluída → 201 com o mesmo `id`, dados novos e `criadoEm` atual
  - [ ] CA12 – trocar CPF para o de pessoa excluída → 409
- [ ] Teste extra: CPF gravado sem máscara no banco e retornado com máscara na API

**Critérios de conclusão:** todos os critérios de aceite da spec automatizados e verdes.

**Commit sugerido:** `test(api): cobre critérios de aceite com testes de integração`

---

## Task 14 – Testes de arquitetura

**Depende de:** 12 · **Requisitos:** plan.md seção 2

- [ ] `Domain` não depende de nenhum outro projeto nem de `Dapper`, `MySqlConnector` ou `Microsoft.AspNetCore`
- [ ] `Application` depende apenas de `Domain`
- [ ] `Infrastructure` não depende de `Api`
- [ ] Classes com SQL (`*Sql`) existem apenas em `Infrastructure`
- [ ] Controllers não dependem diretamente de `Infrastructure`

**Critérios de conclusão:** regras verdes; quebrar uma regra de propósito faz o teste falhar.

**Commit sugerido:** `test: adiciona testes de arquitetura hexagonal`

---

## Task 15 – Revisão final

**Depende de:** 13, 14

- [ ] Revisar o código contra a seção 10 do plano (indentação, tamanho de métodos, nomes, SOLID)
- [ ] Conferir a tabela de rastreabilidade (plan.md seção 12): cada requisito tem código e teste
- [ ] Atualizar `README.md` com instruções finais e exemplos de chamada
- [ ] Atualizar `spec.md` (status e respostas das questões em aberto, se houver) e `plan.md` (decisões que mudaram durante a implementação)
- [ ] Mudar o status deste arquivo para **Concluído**

**Critérios de conclusão:** build e testes verdes; documentação SDD condizente com o código entregue.

**Commit sugerido:** `docs: atualiza documentação da feature 001-cadastro-pessoa`
