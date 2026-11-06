# Conventions

> Padrões que NÃO estão óbvios no código. Consulte antes de escrever código.
> Se algo pode ser imposto por lint/formatter, configure a ferramenta em vez
> de documentar aqui.

## Estrutura de pastas

```
src/
  CadastroPessoas.Domain/          # Entities, ValueObjects, Enums, Exceptions, Interfaces
  CadastroPessoas.Application/     # UseCases/Pessoa/<CasoDeUso>, Interfaces, Services
  CadastroPessoas.Infrastructure/  # Persistence/Repositories, ExternalServices, DependencyInjection.cs
  CadastroPessoas.Api/             # Controllers, DTOs, Mappers, Middlewares, Program.cs
tests/
  CadastroPessoas.Domain.Tests/
  CadastroPessoas.Application.Tests/
  CadastroPessoas.Infrastructure.Tests/
specs/001-cadastro_de_pessoa/      # spec.md, plan.md, tasks.md
docs/memory/                       # memory bank
```

A árvore completa está no `plan.md`.

Dependências entre projetos: `Api → Application, Infrastructure`; `Infrastructure → Application`; `Application → Domain`.

## Nomenclatura

- **Pastas técnicas:** em inglês, como no `plan.md` (`UseCases`, `Repositories`).
- **Termos de negócio:** em português (`Pessoa`, `CriarPessoa`).
- **Classes:** sufixo técnico em inglês, como no `plan.md` (`CriarPessoaUseCase`, `CriarPessoaInput`, `PessoaRepository`, `PessoaController`, `PessoaRequest`).
- **Value Object de CPF:** arquivo `ValueObjects/CPF.cs`.
- **Nomes em geral:** claros e sem abreviações genéricas (`CLAUDE.md`).
- **Tabela e colunas:** minúsculas, no singular e sem separador (`pessoa`, `datadenascimento`).
- **Campos do JSON:** exatamente como no `plan.md`: minúsculos sem separador (`datadenascimento`), exceto `Registros` e `Erros`.
- **Commits:** Conventional Commits em português, como no histórico (`feat(domain): ...`).
- **Branches:** sem padrão definido.

## Padrões de código

- Arquitetura hexagonal, com SOLID e Clean Code; métodos curtos e pouca indentação.
- O banco é acessado com Dapper e MySqlConnector, pelo `PessoaRepository`.
- A interface `IPessoaRepository` fica no Domain.
- O repositório abre conexões pelo `MySqlDataSource`, registrado como singleton, e sempre passa valores por parâmetros do Dapper.
- `DateOnly` vira `DateTime` nos parâmetros e na leitura (`PessoaRegistro`), sem depender de suporte do Dapper a `DateOnly`.
- No `LIKE`, o caractere de escape é `!`, e não a barra invertida, para funcionar com qualquer `sql_mode`.
- Mensagens de erro de segurança e de conexão nunca incluem a senha, a chave nem o valor criptografado (`MensagensDeSeguranca`).
- Erros de validação são reunidos em uma única `ValidacaoException`, com todas as mensagens em `Erros` (D-007). As mensagens ficam em `ErrosDePessoa`.
- A API usa apenas `IPessoaService`; os casos de uso são chamados pelo `PessoaService` (D-006).
- O domínio não lê o relógio: `Pessoa.Criar` e `Pessoa.Atualizar` recebem `momentoAtual`. Os casos de uso obtêm esse valor com `TimeProvider.GetLocalNow()`, em horário local (D-005).
- Dentro de `UseCases/Pessoa/` e `UseCases/Telefone/`, as entidades são referenciadas pelos aliases `PessoaEntity` e `TelefoneEntity`, porque `Pessoa` e `Telefone` também são nomes de namespace.
- Nos casos de uso, as checagens que consultam o banco só rodam quando os dados necessários são válidos (por exemplo, o número repetido só é verificado quando o número é válido e a pessoa existe).
- Erros de registro inexistente derivam de `RegistroNaoEncontradoException`, que a API trata como 404.
- `NumeroDeTelefone` para no primeiro erro quando o número está vazio ou tem caracteres que não são dígitos. Nos demais casos, reúne os erros de tamanho, DDD e celular.
- `Pessoa.Criar` valida os dados de entrada. `Pessoa.Reconstituir` serve só para dados vindos do banco e não valida nome nem data.
- Métodos assíncronos têm o sufixo `Async`.
- O tratamento de erros da API fica em `Api/Middlewares`: 400 para regras de negócio, 404 para registro inexistente e 500 para falhas de servidor, sempre com a lista `Erros`. O 500 usa mensagem genérica; o detalhe fica só no log.
- Os nomes do JSON são sempre definidos com `[property: JsonPropertyName(...)]` nos DTOs.
- Parâmetros de rota do tipo texto são `string?`, para o valor em branco chegar à regra de negócio em vez de ser rejeitado pelo `[ApiController]` (D-014).
- O Swagger fica disponível apenas em desenvolvimento.
- Datas usam o horário local (D-005); no container, o fuso é America/Sao_Paulo.

## Segurança

- Senhas e tokens nunca ficam expostos no código.
- A senha da string de conexão fica criptografada com AES-GCM, no formato da D-001.
- O `appsettings.json` local não é commitado (D-002, com `skip-worktree`).
- No container, a chave e a string de conexão chegam só por variável de ambiente e nunca entram na imagem. A aplicação não vai para produção (D-017).

## Testes

- Framework: xUnit.
- Cada camada testa no projeto `tests/CadastroPessoas.<Camada>.Tests`. O `tasks.md` não pede projeto de testes para a API.
- Comandos: `dotnet build CadastroPessoas.sln` e `dotnet test CadastroPessoas.sln`.
- A API não tem testes automatizados; ao alterá-la, chame as rotas com a aplicação rodando (`dotnet run --project src/CadastroPessoas.Api --launch-profile http`).
- Testes de infraestrutura usam o `raspadinha_dev` (D-008), com a conexão do `appsettings.json` local e a `CHAVE_DE_SEGURANCA`. Cada teste gera os próprios CPFs com `banco.GerarCpfParaRemoverAoFinal()` e usa `CriarMarcadorUnico()` nos nomes pesquisados, para não depender dos dados existentes.
- Classes de teste que usam o banco ficam na coleção `ColecaoDoBancoDeTeste.Nome`.
- Testes de telefone criam a pessoa com `banco.GerarCpfParaRemoverAoFinal()`. A limpeza da fixture exclui os telefones dessas pessoas antes delas, por causa da chave estrangeira.
- Erros do MySQL que representam regra de negócio viram `ValidacaoException` no repositório: CPF duplicado (`DuplicateKeyEntry`) e pessoa inexistente na chave estrangeira do telefone (`NoReferencedRow2`).

## Container

- Build da imagem: `docker build -t cadastropessoas-api .`, na raiz do repositório.
- **Build local neste computador:** o Norton Antivirus inspeciona o HTTPS, e o `dotnet restore` dentro do container falha com `NU1301 ... PartialChain`. Para validar localmente, use uma cópia do Dockerfile fora do repositório que, no estágio `build`, copie o certificado "Norton Web/Mail Shield Root" (exportado do Windows em PEM) para `/usr/local/share/ca-certificates/` e rode `update-ca-certificates`. Não commitar essa alteração.
- Para rodar localmente contra o MySQL do Windows, use `Server=host.docker.internal` em `ConnectionStrings__CadastroPessoas` e passe as variáveis só pelo nome (`docker run -e CHAVE_DE_SEGURANCA -e ConnectionStrings__CadastroPessoas ...`), para os valores não aparecerem no comando.

## Faça / Não faça

| ✅ Faça | ❌ Não faça |
|--------|------------|
| Parar e expor a falha quando a spec estiver vaga | Implementar com base em suposições |
| Atualizar `spec.md`, `plan.md` e `tasks.md` ao esclarecer uma dúvida | Divergir da spec sem registrar em `decisions.md` |
| Atualizar `docs/memory/` ao concluir cada tarefa | Reaproveitar o código antigo do Git sem checar a spec atual |

## Variáveis de ambiente

| Nome | Obrigatória | Descrição |
|------|-------------|-----------|
| `CHAVE_DE_SEGURANCA` | sim | Chave AES de 256 bits em Base64, usada para descriptografar a senha da conexão |
| `ConnectionStrings__CadastroPessoas` | no container | String de conexão com a senha criptografada; substitui o valor do `appsettings.json` (D-003) |
| `TZ` | definida na imagem | `America/Sao_Paulo`; define o horário local do container (D-005) |
