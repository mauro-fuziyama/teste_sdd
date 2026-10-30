# Plan: Cadastro de Pessoa (CRUD)

- **Feature:** 001-cadastro-pessoa
- **Spec de referência:** [spec.md](./spec.md)
- **Status:** Rascunho
- **Data:** 2026-10-30

## 1. Resumo técnico

API REST em C# / .NET com arquitetura **Hexagonal (Ports & Adapters)**. As regras de negócio ficam no domínio, isoladas de frameworks. Os casos de uso orquestram o domínio por meio de portas (interfaces). Os adaptadores de entrada (HTTP) e de saída (banco de dados) ficam nas bordas e dependem do núcleo, nunca o contrário.

| Item | Escolha |
|------|---------|
| Runtime | .NET 10 (LTS) |
| Estilo da API | ASP.NET Core com Controllers |
| Banco de dados | MySQL 8.0 local já existente (schema `raspadinha_dev`, tabela `pessoa`) |
| Acesso a dados | Dapper + MySqlConnector (driver ADO.NET) |
| Versionamento do banco | Nenhum: a estrutura já existe no servidor e é mantida fora da aplicação |
| Validação | Value Objects no domínio (sem dependência de bibliotecas externas) |
| Fluxo de erros | Padrão Result (sem exceções para regras de negócio) |
| Documentação | OpenAPI nativo do ASP.NET Core + Scalar UI |
| Testes | xUnit, AwesomeAssertions, Testcontainers (MySQL) |
| Configuração | `appsettings.json` com Options Pattern (inclusive a connection string) |

### Acesso a dados com Dapper

| Componente | Escolha | Motivo |
|------------|---------|--------|
| Micro-ORM | Dapper | SQL explícito e previsível, mapeamento leve, sem rastreamento de entidades |
| Driver | MySqlConnector | Driver ADO.NET assíncrono de verdade, open source e bem mantido; preferido ao `MySql.Data` |

Consequências desta escolha, tratadas ao longo do plano:

- **O SQL é escrito à mão.** Fica centralizado em uma classe por tabela (seção 7.3), sempre com parâmetros, nunca concatenando valores.
- **Não há migrations.** A tabela `pessoa` já existe no servidor; a aplicação apenas lê e grava (seção 7.1). Um script de teste replica a estrutura no container dos testes de integração (seção 7.6).
- **Não há mapeamento automático para Value Objects.** O repositório lê as linhas em um registro simples e reconstrói a entidade de domínio (seção 7.2).
- **Erros de SQL só aparecem em execução.** Os testes de integração com MySQL real cobrem todas as consultas (seção 11).

Dapper e MySqlConnector ficam isolados no projeto `Infrastructure`. Domínio e casos de uso não sabem qual tecnologia de acesso a dados é usada.

### Decisões sobre as questões em aberto da spec

Adotadas as propostas da spec até confirmação. Todas são de baixo impacto para mudar depois:

| ID | Decisão adotada |
|----|-----------------|
| Q1 | Apenas `PUT` |
| Q2 | Exclusão lógica (`ativo = 0`), conforme US05 da spec. Cadastro com CPF de pessoa inativa reaproveita o registro: substitui os dados, `criadoEm` e `atualizadoEm` recebem a data atual, `ativo = 1`, mantém o `id` e responde 201 |
| — | No `PUT`, CPF que pertence a outra pessoa (ativa ou inativa) responde 409 |
| — | Todas as configurações, inclusive a connection string, ficam no `appsettings.json`. Em produção a pipeline substitui o arquivo |
| — | `id` é `INT AUTO_INCREMENT`, gerado pelo banco (tabela existente), e não GUID |

### Pendências resolvidas

| ID | Pendência | Resolução |
|----|-----------|-----------|
| P1 | Faltava índice único em `cpf` (RNF02) | Criado no banco o índice único `cpf_UNIQUE` |
| P2 | CPF de pessoa inativa impede novo cadastro? | Não: o cadastro reaproveita o registro inativo, atualiza os dados e o reativa |
| P3 | Senha em texto puro no `.env` × constitution | A constitution foi revista. Todas as configurações ficam no `appsettings.json`; no Git ele é versionado com a senha vazia e a pipeline de produção substitui o arquivo |
| P4 | Em qual fuso horário se calcula a data de "hoje" para validar a data de nascimento? | Fuso configurável no `appsettings.json` (`Aplicacao:FusoHorario`, ex.: `America/Sao_Paulo`). `RelogioAplicacao` (Application) combina o instante UTC do `TimeProvider` com a data de hoje nesse fuso e entrega um `MomentoAtual(Utc, Hoje)` à entidade. As datas de auditoria continuam em UTC |

### Pendências em aberto

Nenhuma.

## 2. Arquitetura

### Visão hexagonal

```
              ┌──────────────────────────────────────────┐
  HTTP  ───▶  │  Adaptador de entrada (Api)              │
              │  Controllers, DTOs, mapeamento de erros  │
              └───────────────┬──────────────────────────┘
                              │ usa portas de entrada
              ┌───────────────▼──────────────────────────┐
              │  Application (casos de uso)              │
              │  Portas de entrada e de saída            │
              │  ┌────────────────────────────────────┐  │
              │  │  Domain                            │  │
              │  │  Entidade Pessoa, Value Objects,   │  │
              │  │  erros de domínio, Result          │  │
              │  └────────────────────────────────────┘  │
              └───────────────▲──────────────────────────┘
                              │ implementa portas de saída
              ┌───────────────┴──────────────────────────┐
  Banco ◀───  │  Adaptador de saída (Infrastructure)     │
              │  Dapper, Repositório, scripts SQL        │
              └──────────────────────────────────────────┘
```

### Regra de dependência

```
Api ──────────────▶ Application ──▶ Domain
Infrastructure ───▶ Application ──▶ Domain
Api ──▶ Infrastructure   (somente para registrar dependências no Program.cs)
```

- `Domain` não referencia nenhum outro projeto nem pacote de framework.
- `Application` referencia apenas `Domain`.
- Nenhuma classe de `Api` ou `Infrastructure` é usada dentro de `Application` ou `Domain`.
- Um teste de arquitetura (NetArchTest) garante essas regras no CI.

## 3. Estrutura da solução

```
Treinameto_SDD/
├── CadastroPessoas.sln
├── specs/
│   └── 001-cadastro_de_pessoa/
│       ├── spec.md
│       ├── plan.md
│       └── tasks.md
├── src/
│   ├── CadastroPessoas.Domain/
│   │   ├── Comum/
│   │   │   ├── Result.cs
│   │   │   ├── Erro.cs
│   │   │   └── TipoErro.cs
│   │   └── Pessoas/
│   │       ├── Pessoa.cs
│   │       ├── PessoaErros.cs
│   │       └── ValueObjects/
│   │           ├── Cpf.cs
│   │           ├── Nome.cs
│   │           ├── DataNascimento.cs
│   │           └── Sexo.cs
│   │
│   ├── CadastroPessoas.Application/
│   │   ├── Comum/
│   │   │   └── ResultadoPaginado.cs
│   │   └── Pessoas/
│   │       ├── Portas/
│   │       │   ├── Entrada/            ← interfaces dos casos de uso
│   │       │   │   ├── ICadastrarPessoa.cs
│   │       │   │   ├── IConsultarPessoa.cs
│   │       │   │   ├── IListarPessoas.cs
│   │       │   │   ├── IAtualizarPessoa.cs
│   │       │   │   └── IExcluirPessoa.cs
│   │       │   └── Saida/
│   │       │       ├── IPessoaRepositorio.cs
│   │       │       └── CpfDuplicadoException.cs  ← conflito detectado na gravação
│   │       ├── CasosDeUso/
│   │       │   ├── CadastrarPessoa.cs
│   │       │   ├── ConsultarPessoa.cs
│   │       │   ├── ListarPessoas.cs
│   │       │   ├── AtualizarPessoa.cs
│   │       │   └── ExcluirPessoa.cs
│   │       └── Modelos/
│   │           ├── DadosPessoa.cs       ← entrada dos casos de uso
│   │           ├── FiltroPessoas.cs
│   │           └── PessoaResultado.cs   ← saída dos casos de uso
│   │
│   ├── CadastroPessoas.Infrastructure/
│   │   ├── Persistencia/
│   │   │   ├── Conexao/
│   │   │   │   ├── IConexaoFactory.cs
│   │   │   │   └── MySqlConexaoFactory.cs
│   │   │   ├── Pessoas/
│   │   │   │   ├── PessoaRepositorio.cs
│   │   │   │   ├── PessoaSql.cs         ← comandos SQL da tabela pessoas
│   │   │   │   └── PessoaRegistro.cs    ← linha do banco ↔ entidade
│   │   │   └── BancoOptions.cs          ← timeout de comando
│   │   └── InjecaoDependencia.cs
│   │
│   └── CadastroPessoas.Api/
│       ├── Controllers/
│       │   └── PessoasController.cs
│       ├── Contratos/
│       │   ├── PessoaRequest.cs
│       │   ├── PessoaResponse.cs
│       │   └── PaginaResponse.cs
│       ├── Mapeamentos/
│       │   ├── PessoaMapeamento.cs
│       │   └── ResultParaHttp.cs        ← converte Result em ProblemDetails
│       ├── Configuracoes/
│       │   └── PaginacaoOptions.cs
│       ├── Program.cs
│       ├── appsettings.json
│       └── appsettings.Development.json
│
└── tests/
    ├── Scripts/
    │   └── criar_tabela_pessoa.sql      ← réplica da tabela do banco local (testes de integração)
    ├── CadastroPessoas.Domain.Tests/
    ├── CadastroPessoas.Application.Tests/
    ├── CadastroPessoas.Infrastructure.IntegrationTests/
    ├── CadastroPessoas.Api.IntegrationTests/
    └── CadastroPessoas.ArchitectureTests/
```

## 4. Camada de domínio

### 4.1 Padrão Result

Regras de negócio violadas não lançam exceção. Os métodos de criação retornam `Result<T>`, que carrega o valor ou a lista de erros. Isso permite acumular **todos** os erros de validação (exigência do RF01) e mantém o fluxo explícito.

```csharp
public enum TipoErro { Validacao, NaoEncontrado, Conflito }

public sealed record Erro(string Campo, string Mensagem, TipoErro Tipo);

public sealed class Result<T>
{
    public T? Valor { get; }
    public IReadOnlyList<Erro> Erros { get; }
    public bool Sucesso => Erros.Count == 0;

    private Result(T? valor, IReadOnlyList<Erro> erros)
    {
        Valor = valor;
        Erros = erros;
    }

    public static Result<T> Ok(T valor) => new(valor, []);
    public static Result<T> Falha(params Erro[] erros) => new(default, erros);

    public static implicit operator Result<T>(T valor) => Ok(valor);
    public static implicit operator Result<T>(Erro erro) => Falha(erro);
}
```

O analisador CA1000 (membros estáticos em tipo genérico) é suprimido apenas na classe `Result<T>`, com justificativa, porque `Ok` e `Falha` são a API de criação escolhida.

### 4.2 Value Objects

Cada campo com regra é um Value Object imutável, com construtor privado e um método `Criar` que valida e retorna `Result`. Um objeto inválido nunca chega a existir.

| Value Object | Responsabilidade |
|--------------|------------------|
| `Nome` | Remove espaços nas pontas, reduz espaços repetidos, valida tamanho 3–150 |
| `DataNascimento` | Valida que não é futura e que a idade não passa de 130 anos (recebe a data de hoje como `DateOnly`) |
| `Cpf` | Remove máscara, valida o algoritmo da spec (seção 4), expõe `Numero` (sem máscara) e `Formatado()` |
| `Sexo` | Converte `M`, `F`, `O`, `N` para o enum `TipoSexo`. Aceita minúsculas e espaços nas pontas e guarda o código em maiúscula (decisão de 2026-10-30) |

**Campos especiais com máscara (CPF, e no futuro telefone e CEP):** o Value Object guarda apenas os dígitos em `Numero`, que é o valor persistido. A máscara é aplicada somente na saída, pelo método `Formatado()`. Novos campos especiais seguem a mesma estrutura.

Exemplo de implementação, ilustrando os padrões de clean code desta feature (métodos curtos, nomes descritivos, no máximo um nível de indentação):

```csharp
public sealed record Cpf
{
    private const int QuantidadeDigitos = 11;

    public string Numero { get; }

    private Cpf(string numero) => Numero = numero;

    public static Result<Cpf> Criar(string? entrada)
    {
        var digitos = RemoverMascara(entrada);

        if (!EhValido(digitos))
            return PessoaErros.CpfInvalido;

        return new Cpf(digitos);
    }

    public string Formatado() =>
        $"{Numero[..3]}.{Numero[3..6]}.{Numero[6..9]}-{Numero[9..]}";

    private static string RemoverMascara(string? entrada) =>
        (entrada ?? string.Empty).Trim().Replace(".", "").Replace("-", "");

    private static bool EhValido(string digitos) =>
        TemFormatoCorreto(digitos)
        && !TemTodosDigitosIguais(digitos)
        && DigitosVerificadoresConferem(digitos);

    private static bool TemFormatoCorreto(string digitos) =>
        digitos.Length == QuantidadeDigitos && digitos.All(char.IsAsciiDigit);

    private static bool TemTodosDigitosIguais(string digitos) =>
        digitos.Distinct().Count() == 1;

    private static bool DigitosVerificadoresConferem(string digitos) =>
        ValorDoDigito(digitos[9]) == CalcularDigitoVerificador(digitos, 9)
        && ValorDoDigito(digitos[10]) == CalcularDigitoVerificador(digitos, 10);

    private static int CalcularDigitoVerificador(string digitos, int quantidadeBase)
    {
        var pesoInicial = quantidadeBase + 1;
        var soma = digitos
            .Take(quantidadeBase)
            .Select((digito, indice) => ValorDoDigito(digito) * (pesoInicial - indice))
            .Sum();

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    private static int ValorDoDigito(char digito) => digito - '0';
}
```

Na implementação, `Cpf.Criar` verifica antes se a entrada é nula, vazia ou só espaços e, nesse caso, retorna `PessoaErros.CpfObrigatorio` ("O campo cpf é obrigatório.", spec seção 7). Os demais casos inválidos retornam `PessoaErros.CpfInvalido`. `Cpf` também expõe `Mascarado()` (logs, seção 9) e `Restaurar(numero)` (repositório).

### 4.3 Entidade `Pessoa`

- Propriedades com `private set`, alteradas apenas por métodos da própria entidade.
- `Criar(...)` recebe os dados brutos, cria os Value Objects, **acumula os erros de todos eles** e só então retorna `Result<Pessoa>`.
- `Atualizar(...)` aplica a mesma validação e atualiza `AtualizadoEm`.
- `Id` é um `int` gerado pelo banco (`AUTO_INCREMENT`). Uma pessoa recém-criada tem `Id = 0` até ser gravada; o repositório devolve o Id gerado e a entidade o recebe por `DefinirId(id)`.
- `Ativo` indica se a pessoa está ativa. `Desativar(agora)` marca a pessoa como inativa e atualiza `AtualizadoEm` (exclusão lógica, US05).
- A data atual vem de fora (`TimeProvider`), para que o domínio seja testável sem depender do relógio.
- `Restaurar(...)` reconstrói uma pessoa a partir de dados já gravados, sem revalidar e sem gerar novo `Id`. É usado só pelo repositório, porque o que está no banco já passou pelas regras. Os Value Objects têm um método equivalente (`Cpf.Restaurar(numero)`, por exemplo).

```csharp
public static Result<Pessoa> Criar(DadosPessoa dados, DateTimeOffset agora)
{
    var nome = Nome.Criar(dados.Nome);
    var dataNascimento = DataNascimento.Criar(dados.DataNascimento, agora);
    var cpf = Cpf.Criar(dados.Cpf);
    var sexo = Sexo.Criar(dados.Sexo);

    var erros = ErrosDe(nome, dataNascimento, cpf, sexo);
    if (erros.Length > 0)
        return Result<Pessoa>.Falha(erros);

    return new Pessoa(nome.Valor!, dataNascimento.Valor!, cpf.Valor!, sexo.Valor!, agora);
}
```

### 4.4 Catálogo de erros

`PessoaErros` centraliza todos os erros com as mensagens da spec (seção 7), evitando textos espalhados pelo código:

```csharp
public static class PessoaErros
{
    public static readonly Erro CpfInvalido =
        new("cpf", "CPF inválido.", TipoErro.Validacao);

    public static readonly Erro CpfDuplicado =
        new("cpf", "Já existe uma pessoa cadastrada com este CPF.", TipoErro.Conflito);

    public static readonly Erro NaoEncontrada =
        new("id", "Pessoa não encontrada.", TipoErro.NaoEncontrado);

    // demais erros da seção 7 da spec...
}
```

### 4.5 Regras de negócio fixas × configuráveis

Os limites de negócio da spec (tamanho do nome, idade máxima de 130 anos, valores de sexo) ficam como **constantes no domínio**, porque fazem parte da regra e mudar um deles exige revisar a spec. O `appsettings.json` guarda as configurações técnicas e operacionais (seção 8).

## 5. Camada de aplicação

### 5.1 Portas de entrada (casos de uso)

Uma interface por caso de uso (Interface Segregation). O controller depende só da interface que usa.

| Porta | Método | Requisito |
|-------|--------|-----------|
| `ICadastrarPessoa` | `Task<Result<PessoaResultado>> ExecutarAsync(DadosPessoa dados, CancellationToken ct)` | RF01 |
| `IConsultarPessoa` | `Task<Result<PessoaResultado>> ExecutarAsync(int id, CancellationToken ct)` | RF02 |
| `IListarPessoas` | `Task<ResultadoPaginado<PessoaResultado>> ExecutarAsync(FiltroPessoas filtro, CancellationToken ct)` | RF03 |
| `IAtualizarPessoa` | `Task<Result<PessoaResultado>> ExecutarAsync(int id, DadosPessoa dados, CancellationToken ct)` | RF04 |
| `IExcluirPessoa` | `Task<Result<bool>> ExecutarAsync(int id, CancellationToken ct)` | RF05 |

### 5.2 Porta de saída

```csharp
public interface IPessoaRepositorio
{
    Task<Pessoa?> ObterPorIdAsync(int id, CancellationToken ct);
    Task<bool> ExisteCpfAsync(Cpf cpf, int? ignorarId, CancellationToken ct);
    Task<int?> ObterIdInativoPorCpfAsync(Cpf cpf, CancellationToken ct);
    Task<(IReadOnlyList<Pessoa> Itens, int Total)> ListarAsync(FiltroPessoas filtro, CancellationToken ct);
    Task<int> AdicionarAsync(Pessoa pessoa, CancellationToken ct);
    Task AtualizarAsync(Pessoa pessoa, CancellationToken ct);
    Task ReativarAsync(Pessoa pessoa, CancellationToken ct);
    Task DesativarAsync(Pessoa pessoa, CancellationToken ct);
}
```

O parâmetro `ignorarId` em `ExisteCpfAsync` atende ao RF04: na atualização, o CPF da própria pessoa não conta como duplicado.

`AdicionarAsync` devolve o `id` gerado pelo banco. `ObterPorIdAsync` e `ListarAsync` consideram apenas pessoas ativas. `ExisteCpfAsync` considera pessoas ativas e inativas, porque o índice único vale para todos os registros. `ObterIdInativoPorCpfAsync` e `ReativarAsync` atendem ao reaproveitamento do registro inativo no cadastro.

`AdicionarAsync` e `AtualizarAsync` podem lançar `CpfDuplicadoException` quando outra requisição grava o mesmo CPF no mesmo instante (seção 7.5). A exceção faz parte do contrato da porta, por isso é declarada em `Application`, e não em `Infrastructure`.

### 5.3 Fluxo de um caso de uso

Cada caso de uso tem um único método público e delega cada etapa a um método privado com nome descritivo:

```csharp
public sealed class CadastrarPessoa(IPessoaRepositorio repositorio, TimeProvider relogio)
    : ICadastrarPessoa
{
    public async Task<Result<PessoaResultado>> ExecutarAsync(DadosPessoa dados, CancellationToken ct)
    {
        var pessoa = Pessoa.Criar(dados, relogio.GetUtcNow());
        if (!pessoa.Sucesso)
            return Result<PessoaResultado>.Falha([.. pessoa.Erros]);

        var idInativo = await repositorio.ObterIdInativoPorCpfAsync(pessoa.Valor!.Cpf, ct);
        if (idInativo is not null)
            return await ReativarAsync(pessoa.Valor!, idInativo.Value, ct);

        if (await CpfJaCadastradoAsync(pessoa.Valor!, ct))
            return PessoaErros.CpfDuplicado;

        return await AdicionarAsync(pessoa.Valor!, ct);
    }

    private Task<bool> CpfJaCadastradoAsync(Pessoa pessoa, CancellationToken ct) =>
        repositorio.ExisteCpfAsync(pessoa.Cpf, ignorarId: null, ct);

    // AdicionarAsync e ReativarAsync gravam, definem o Id e convertem CpfDuplicadoException em PessoaErros.CpfDuplicado.
}
```

Os casos de uso que dependem da data atual recebem `RelogioAplicacao` (e não o `TimeProvider` diretamente), que fornece o `MomentoAtual`: instante UTC para auditoria e data de hoje no fuso configurado para as regras de calendário (P4).

Na reativação, a pessoa criada por `Pessoa.Criar` (com `CriadoEm` e `AtualizadoEm` iguais à data atual) recebe o `id` do registro inativo e substitui todos os dados dele.

### 5.4 Modelos de entrada e saída

- `DadosPessoa`: record com os campos brutos (`string Nome`, `DateOnly? DataNascimento`, `string Cpf`, `string Sexo`). Não é o DTO HTTP; o adaptador converte um no outro.
- `PessoaResultado`: record de saída já com o CPF formatado via `Cpf.Formatado()`. Assim, a regra "persistir sem máscara, retornar com máscara" fica em um só lugar.
- `FiltroPessoas`: `Nome`, `Cpf`, `Pagina`, `TamanhoPagina`. O CPF do filtro passa pela mesma remoção de máscara antes da busca.

## 6. Adaptador de entrada: API REST

### 6.1 Endpoints

| Método e rota | Porta usada | Sucesso | Erros |
|---------------|-------------|---------|-------|
| `POST /api/v1/pessoas` | `ICadastrarPessoa` | 201 + header `Location` | 400, 409 |
| `GET /api/v1/pessoas/{id:int}` | `IConsultarPessoa` | 200 | 404 |
| `GET /api/v1/pessoas` | `IListarPessoas` | 200 | 400 |
| `PUT /api/v1/pessoas/{id:int}` | `IAtualizarPessoa` | 200 | 400, 404, 409 |
| `DELETE /api/v1/pessoas/{id:int}` | `IExcluirPessoa` | 204 | 404 |

### 6.2 Controller enxuto

O controller só traduz HTTP ↔ caso de uso. Não contém regra de negócio nem `if` de validação.

```csharp
[HttpPost]
public async Task<IActionResult> Cadastrar(
    PessoaRequest request, [FromServices] ICadastrarPessoa casoDeUso, CancellationToken ct)
{
    var resultado = await casoDeUso.ExecutarAsync(request.ParaDados(), ct);

    return resultado.ParaCreated(
        pessoa => CreatedAtAction(nameof(Consultar), new { id = pessoa.Id }, pessoa.ParaResponse()));
}
```

### 6.3 Conversão de erros para HTTP

`ResultParaHttp` converte os erros do `Result` em `ProblemDetails` (RFC 9457), agrupando as mensagens por campo:

| `TipoErro` | Status HTTP |
|------------|-------------|
| `Validacao` | 400 Bad Request |
| `NaoEncontrado` | 404 Not Found |
| `Conflito` | 409 Conflict |

Exceções inesperadas são capturadas por um `IExceptionHandler` global, que registra o erro no log e retorna 500 com `ProblemDetails` genérico, sem expor detalhes internos.

### 6.4 Contratos

- `PessoaRequest`: `Nome`, `DataNascimento` (`DateOnly?`), `Cpf`, `Sexo`. Os campos são anuláveis para que a ausência seja reportada pelo domínio com a mensagem da spec, e não por um erro genérico de desserialização.
- `PessoaResponse`: `Id`, `Nome`, `DataNascimento`, `Cpf` (formatado), `Sexo`, `CriadoEm`, `AtualizadoEm`.
- `PaginaResponse<T>`: `Itens`, `Pagina`, `TamanhoPagina`, `TotalItens`, `TotalPaginas`.
- JSON em camelCase e datas em ISO 8601 (padrão do `System.Text.Json`).

## 7. Adaptador de saída: persistência

### 7.1 Estrutura da tabela existente (`pessoa`)

A tabela já existe no schema `raspadinha_dev` do MySQL 8.0 local. A aplicação não cria nem altera a estrutura: o usuário de conexão tem apenas `SELECT`, `INSERT`, `UPDATE` e `DELETE`.

```sql
CREATE TABLE `pessoa` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nome` varchar(250) NOT NULL,
  `cpf` varchar(20) DEFAULT NULL,
  `nascimento` date DEFAULT NULL,
  `cadastro` datetime NOT NULL,
  `ultimaalteracao` datetime DEFAULT NULL,
  `ativo` bit(1) NOT NULL DEFAULT b'1',
  `sexo` char(1) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `cpf_UNIQUE` (`cpf`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

| Coluna | Campo do domínio | Observação |
|--------|------------------|------------|
| `id` | `Id` | Gerado pelo banco; lido com `LAST_INSERT_ID()` logo após o `INSERT` |
| `nome` | `Nome` | A coluna aceita 250 caracteres; a regra da spec (3 a 150) é garantida pelo domínio |
| `cpf` | `Cpf` | Gravado só com os 11 dígitos, **sem máscara**. A máscara é aplicada na saída pelo domínio |
| `nascimento` | `DataNascimento` | Anulável no banco, obrigatória no domínio |
| `cadastro` | `CriadoEm` | `DATETIME` sem fração de segundo: o repositório grava em UTC, truncado em segundos |
| `ultimaalteracao` | `AtualizadoEm` | Preenchida pela aplicação em toda gravação; se vier nula, assume o valor de `cadastro` |
| `ativo` | `Ativo` | `1` = ativa, `0` = excluída logicamente. As consultas filtram `ativo = 1` no SQL, sem ler a coluna `BIT` |
| `sexo` | `Sexo` | Anulável no banco, obrigatório no domínio |

- As tabelas `emailpessoa` e `telefonepessoa` existem no schema, mas estão fora do escopo desta feature.
- A collation `utf8mb4_0900_ai_ci` não diferencia acentos nem maiúsculas, o que atende o CA09 ("joao" encontra "João") sem lógica extra.
- O índice único `cpf_UNIQUE` garante a unicidade do CPF no banco (RNF02), para pessoas ativas e inativas.
- Não há índice em `nome`, que ajudaria a atender o RNF01 com volumes maiores.

### 7.2 Registro de leitura e conversão para o domínio

O Dapper mapeia cada linha para um registro simples, com tipos primitivos. A conversão para a entidade fica em um só lugar:

```csharp
internal sealed record PessoaRegistro(
    int Id,
    string Nome,
    DateTime DataNascimento,
    string Cpf,
    string Sexo,
    DateTime CriadoEm,
    DateTime? AtualizadoEm)
{
    public Pessoa ParaEntidade() =>
        Pessoa.Restaurar(
            Id,
            Nome,
            DateOnly.FromDateTime(DataNascimento),
            Cpf,
            Sexo,
            ComoUtc(CriadoEm),
            ComoUtc(AtualizadoEm ?? CriadoEm));

    public static object ParametrosDe(Pessoa pessoa) => new
    {
        pessoa.Id,
        Nome = pessoa.Nome.Valor,
        DataNascimento = pessoa.DataNascimento.Valor.ToDateTime(TimeOnly.MinValue),
        Cpf = pessoa.Cpf.Numero,
        Sexo = pessoa.Sexo.Codigo,
        CriadoEm = TruncarEmSegundos(pessoa.CriadoEm),
        AtualizadoEm = TruncarEmSegundos(pessoa.AtualizadoEm)
    };

    private static DateTimeOffset ComoUtc(DateTime valor) =>
        new(DateTime.SpecifyKind(valor, DateTimeKind.Utc));

    private static DateTime TruncarEmSegundos(DateTimeOffset valor) =>
        valor.UtcDateTime.AddTicks(-(valor.UtcDateTime.Ticks % TimeSpan.TicksPerSecond));
}
```

- As datas trafegam como `DateTime` entre aplicação e banco, porque é o tipo que o driver lê das colunas `DATE` e `DATETIME`. A conversão para `DateOnly` e `DateTimeOffset` acontece no registro, sem depender de configuração global de tipos do Dapper.
- `ParametrosDe` grava o CPF por `Cpf.Numero`, ou seja, **sem máscara**.

### 7.3 Comandos SQL (`PessoaSql`)

Todo o SQL da tabela fica em constantes de uma única classe, com nomes que dizem o que cada comando faz. As colunas são renomeadas com `AS` para bater com as propriedades do `PessoaRegistro`.

```csharp
internal static class PessoaSql
{
    private const string Colunas = """
        id AS Id, nome AS Nome, nascimento AS DataNascimento, cpf AS Cpf,
        sexo AS Sexo, cadastro AS CriadoEm, ultimaalteracao AS AtualizadoEm
        """;

    private const string FiltroListagem = """
        WHERE ativo = 1
          AND (@Nome IS NULL OR nome LIKE CONCAT('%', @Nome, '%'))
          AND (@Cpf IS NULL OR cpf = @Cpf)
        """;

    public const string ObterPorId = $"SELECT {Colunas} FROM pessoa WHERE id = @Id AND ativo = 1;";

    // Considera ativas e inativas: o índice único vale para todos os registros.
    public const string ExisteCpf = """
        SELECT EXISTS(
            SELECT 1 FROM pessoa
            WHERE cpf = @Cpf AND (@IgnorarId IS NULL OR id <> @IgnorarId));
        """;

    public const string Listar = $"""
        SELECT {Colunas} FROM pessoa {FiltroListagem}
        ORDER BY nome LIMIT @Tamanho OFFSET @Deslocamento;

        SELECT COUNT(*) FROM pessoa {FiltroListagem};
        """;

    public const string Inserir = """
        INSERT INTO pessoa (nome, nascimento, cpf, sexo, cadastro, ultimaalteracao)
        VALUES (@Nome, @DataNascimento, @Cpf, @Sexo, @CriadoEm, @AtualizadoEm);
        SELECT LAST_INSERT_ID();
        """;

    public const string Atualizar = """
        UPDATE pessoa
        SET nome = @Nome, nascimento = @DataNascimento, cpf = @Cpf,
            sexo = @Sexo, ultimaalteracao = @AtualizadoEm
        WHERE id = @Id AND ativo = 1;
        """;

    public const string ObterIdInativoPorCpf = "SELECT id FROM pessoa WHERE cpf = @Cpf AND ativo = 0;";

    public const string Reativar = """
        UPDATE pessoa
        SET nome = @Nome, nascimento = @DataNascimento, cpf = @Cpf, sexo = @Sexo,
            cadastro = @CriadoEm, ultimaalteracao = @AtualizadoEm, ativo = 1
        WHERE id = @Id AND ativo = 0;
        """;

    public const string Desativar = """
        UPDATE pessoa SET ativo = 0, ultimaalteracao = @AtualizadoEm WHERE id = @Id;
        """;
}
```

- **Sempre parâmetros (`@Nome`, `@Cpf`...)**, nunca valores concatenados no texto: evita SQL injection e permite ao MySQL reaproveitar o plano de execução.
- **Filtros opcionais sem montar SQL dinâmico:** o padrão `(@Parametro IS NULL OR coluna = @Parametro)` mantém um único texto de consulta, sem `if` encadeado no repositório.
- **Listagem em uma ida ao banco:** a página e o total vêm juntos, lidos com `QueryMultipleAsync`.
- `id`, `cadastro` e `ativo` não aparecem no `UPDATE` de atualização, garantindo que não são alterados (RF04).
- **Exclusão lógica:** não existe `DELETE`. `Desativar` marca `ativo = 0` e mantém o histórico (US05).
- **Reativação:** `Reativar` substitui os dados do registro inativo, redefine `cadastro` com a data atual e volta `ativo = 1`.

### 7.4 Repositório

Cada método abre uma conexão pela fábrica, executa um comando e devolve o resultado. Conexões vêm do pool do driver, então abrir uma por operação é barato.

```csharp
internal sealed class PessoaRepositorio(IConexaoFactory conexoes) : IPessoaRepositorio
{
    private const int CodigoChaveDuplicada = 1062;

    public async Task<Pessoa?> ObterPorIdAsync(int id, CancellationToken ct)
    {
        await using var conexao = await conexoes.AbrirAsync(ct);
        var registro = await conexao.QuerySingleOrDefaultAsync<PessoaRegistro>(
            Comando(PessoaSql.ObterPorId, new { Id = id }, ct));

        return registro?.ParaEntidade();
    }

    public async Task<int> AdicionarAsync(Pessoa pessoa, CancellationToken ct)
    {
        await using var conexao = await conexoes.AbrirAsync(ct);
        return await InserirTratandoCpfDuplicadoAsync(conexao, pessoa, ct);
    }

    private async Task<int> InserirTratandoCpfDuplicadoAsync(
        DbConnection conexao, Pessoa pessoa, CancellationToken ct)
    {
        try
        {
            return await conexao.ExecuteScalarAsync<int>(
                Comando(PessoaSql.Inserir, PessoaRegistro.ParametrosDe(pessoa), ct));
        }
        catch (MySqlException ex) when (ex.Number == CodigoChaveDuplicada)
        {
            throw new CpfDuplicadoException();
        }
    }

    private static CommandDefinition Comando(string sql, object parametros, CancellationToken ct) =>
        new(sql, parametros, cancellationToken: ct);

    // ExisteCpfAsync, ObterIdInativoPorCpfAsync, ListarAsync, AtualizarAsync, ReativarAsync e DesativarAsync seguem o mesmo formato.
    // AtualizarAsync e ReativarAsync também convertem o erro 1062 em CpfDuplicadoException.
}
```

- **`CommandDefinition` em todas as chamadas**, para repassar o `CancellationToken` ao banco.
- **Sem transação explícita:** cada operação é um único comando SQL (o `INSERT` e o `LAST_INSERT_ID()` rodam na mesma conexão).
- **Timeout** de comando vem do `appsettings.json` (seção 8), aplicado pela fábrica de conexões.

### 7.5 Concorrência no CPF

A verificação `ExisteCpfAsync` no caso de uso cobre o cenário comum. Para duas requisições simultâneas com o mesmo CPF, o índice único `cpf_UNIQUE` rejeita a segunda gravação com o erro **1062** (`ER_DUP_ENTRY`) do MySQL. O repositório captura esse erro específico e o converte em `CpfDuplicadoException`, uma exceção da camada de aplicação que o caso de uso transforma em `PessoaErros.CpfDuplicado`. O cliente recebe 409 em vez de 500.

Essa é a única exceção usada para regra de negócio, porque o conflito só é detectável no momento da gravação. Ela fica restrita à porta de saída e não sai do caso de uso.

### 7.6 Banco nos testes de integração

- Não há DbUp nem scripts executados pela aplicação.
- `tests/Scripts/criar_tabela_pessoa.sql` replica o DDL da seção 7.1, inclusive o índice único `cpf_UNIQUE`. As fixtures dos testes de integração sobem um MySQL 8.0 com Testcontainers (mesma versão do servidor local) e executam esse script.
- Qualquer mudança feita na tabela do servidor local deve ser refletida nesse script.

## 8. Configuração (`appsettings.json`)

Todas as configurações ficam no `appsettings.json`, inclusive a connection string. São lidas por Options Pattern (`IOptions<T>`) com validação na inicialização (`ValidateDataAnnotations().ValidateOnStart()`). Uma configuração inválida impede a aplicação de subir, em vez de falhar em tempo de execução.

```json
{
  "ConnectionStrings": {
    "CadastroPessoas": "Server=localhost;Port=3306;Database=raspadinha_dev;User=raspadinha_dev;Password=;"
  },
  "Paginacao": {
    "TamanhoPadrao": 20,
    "TamanhoMaximo": 100
  },
  "Banco": {
    "TimeoutComandoSegundos": 30
  },
  "Aplicacao": {
    "FusoHorario": "America/Sao_Paulo"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

- **Senha fora do Git:** o `appsettings.json` é versionado com `Password=` vazio. Cada desenvolvedor preenche a senha localmente e marca o arquivo com `git update-index --skip-worktree src/CadastroPessoas.Api/appsettings.json`, para que a senha não seja commitada.
- **Produção:** a pipeline de deploy substitui o `appsettings.json` pelo arquivo com os dados de produção.
- `appsettings.Development.json` contém apenas ajustes de log; não repete a connection string.
- O usuário do banco deve ter apenas `SELECT`, `INSERT`, `UPDATE` e `DELETE`. Hoje o usuário local tem essas permissões em `*.*`; o ideal é restringi-las ao schema da aplicação.

## 9. Segurança e privacidade (LGPD)

- CPF nunca é registrado completo em log. Um método `Cpf.Mascarado()` retorna `***.982.247-**` para uso em logs.
- O repositório não registra em log os parâmetros dos comandos SQL, que contêm CPF e data de nascimento. Em caso de erro de banco, o log traz apenas o nome da operação e o código do erro.
- Respostas 500 não expõem stack trace nem mensagens internas.

## 10. Padrões de código

Estas regras valem para toda a implementação e são verificadas no code review:

| Regra | Como aplicar |
|-------|--------------|
| **Indentação máxima de 2 níveis** dentro de um método | Usar guard clauses e retorno antecipado (`if (...) return ...;`) em vez de `if/else` aninhado |
| **Métodos curtos** (meta: até ~15 linhas) | Extrair cada etapa para um método privado com nome que descreva a intenção (`CpfJaCadastradoAsync`, `TemTodosDigitosIguais`) |
| **Uma responsabilidade por classe** (SRP) | Um caso de uso por classe; um Value Object por conceito |
| **Aberto para extensão** (OCP) | Novos campos especiais (telefone, CEP) viram novos Value Objects, sem alterar os existentes |
| **Substituição de Liskov** (LSP) | Implementações de portas (ex.: repositório em memória nos testes) respeitam o mesmo contrato |
| **Interfaces pequenas** (ISP) | Uma porta de entrada por caso de uso |
| **Inversão de dependência** (DIP) | `Application` depende de `IPessoaRepositorio`; a implementação com Dapper é injetada pela `Api` |
| **Sem números ou textos mágicos** | Constantes nomeadas no domínio; mensagens em `PessoaErros` |
| **Nomes em português**, alinhados à linguagem da spec | `Pessoa`, `CadastrarPessoa`, `DataNascimento` |
| **Sem comentários que repetem o código** | O nome do método deve explicar o que ele faz; comentários só para o *porquê* |
| **Analisadores no build** | `<Nullable>enable</Nullable>`, `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`, `.editorconfig` compartilhado |

## 11. Estratégia de testes

| Projeto | Escopo | Ferramentas |
|---------|--------|-------------|
| `Domain.Tests` | Value Objects e entidade. Todos os exemplos da seção 4 da spec viram casos de `[Theory]` para `Cpf` | xUnit, AwesomeAssertions |
| `Application.Tests` | Casos de uso com repositório fake em memória e `FakeTimeProvider` | xUnit, AwesomeAssertions, Microsoft.Extensions.TimeProvider.Testing |
| `Infrastructure.IntegrationTests` | Cada método do `PessoaRepositorio` contra MySQL real em container: SQL, conversão de tipos, filtros, paginação e erro 1062 | xUnit, Testcontainers.MySql |
| `Api.IntegrationTests` | Endpoints de ponta a ponta com MySQL 8.0 real em container (mesma versão do servidor local). Cobre os critérios de aceite CA01–CA10 | xUnit, `WebApplicationFactory`, Testcontainers.MySql |
| `ArchitectureTests` | Regra de dependência entre camadas (seção 2) | NetArchTest.Rules |

Os testes de integração verificam explicitamente que o CPF é **gravado sem máscara** no banco e **retornado com máscara** na API.

## 12. Rastreabilidade spec → implementação

| Requisito | Onde é atendido |
|-----------|-----------------|
| RF01 | `CadastrarPessoa`, `Pessoa.Criar`, `POST /api/v1/pessoas`, `ReativarAsync` (CPF de pessoa inativa) |
| RF02 | `ConsultarPessoa`, `GET /api/v1/pessoas/{id}` |
| RF03 | `ListarPessoas`, `PessoaRepositorio.ListarAsync`, `PaginacaoOptions` |
| RF04 | `AtualizarPessoa`, `Pessoa.Atualizar`, `ExisteCpfAsync(ignorarId)` |
| RF05 | `ExcluirPessoa`, `DELETE /api/v1/pessoas/{id}` |
| Seção 4 (CPF) | `Cpf` Value Object + testes `[Theory]` |
| Seção 7 (mensagens) | `PessoaErros` |
| RNF01 | Chave primária `id` e índice único `cpf_UNIQUE` |
| RNF02 | Índice único `cpf_UNIQUE` + tratamento do erro MySQL 1062 |
| RNF03 | `Cpf.Mascarado()`, repositório sem log de parâmetros SQL |
| RNF04 | OpenAPI + Scalar |
| RNF05 | Seção 11 |

## 13. Riscos e pontos de atenção

| Risco | Mitigação |
|-------|-----------|
| Erros de digitação no SQL só aparecerem em execução | Testes de integração do repositório com MySQL real cobrem todos os comandos de `PessoaSql` |
| Código e banco ficarem fora de sincronia (coluna renomeada em um e não no outro) | O script `tests/Scripts/criar_tabela_pessoa.sql` replica a tabela; mudanças no servidor devem ser refletidas nele |
| SQL espalhado e duplicado conforme o projeto cresce | Regra: SQL só em classes `*Sql` dentro de `Infrastructure`, uma por tabela |
| Diferença de maiúsculas em nomes de tabela entre Windows e Linux | Nomes em minúsculo definidos explicitamente; testes de integração rodam em container Linux |
| Servidor MySQL com charset ou collation padrão diferentes | A tabela existente já usa `utf8mb4_0900_ai_ci`; o script de testes replica a mesma collation |
| CPF duplicado em requisições simultâneas | Índice único `cpf_UNIQUE` + erro 1062 convertido em 409 |
| Senha commitada por engano no `appsettings.json` | Arquivo versionado com senha vazia e marcado com `--skip-worktree` localmente |
| Respostas das questões em aberto mudarem decisões | Q1 é a de maior impacto restante; alterar apenas `Sexo` |
| Listagem lenta com filtro `Contains` em bases grandes | Monitorar; se necessário, busca por prefixo ou índice full-text |
