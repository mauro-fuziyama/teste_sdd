# Active Context

> Responde "onde parei?". Atualize no FIM de toda sessão. Máx. ~1 página;
> mova o histórico antigo para `progress.md` ou `decisions.md`.

**Última atualização:** 2026-11-09

## Foco atual

- **Spec atual:** `specs/003-cadastro_endereco/`, esclarecida (D-019).
- **Tarefa 1 - Domínio:** concluída em 2026-11-09.
  - `TipoDeEndereco`, `Cep`, `SiglaDeEstado`, `DadosDoEndereco`, `Endereco`, `ErrosDeEndereco`, `EnderecoNaoEncontradoException` e `IEnderecoRepository`, com testes.
  - O `Cep` exclui os hífens antes de validar.
  - Build com 0 erros e 0 avisos; 211 testes de domínio aprovados. Os testes das outras camadas não foram executados nesta tarefa.
- **Tarefa 2 - Aplicação:** concluída em 2026-11-09.
  - `IEnderecoService` e `EnderecoService`.
  - Casos de uso `CriarEndereco`, `PesquisarEndereco`, `AtualizarEndereco` e `ExcluirEndereco`.
  - `EnderecoRepositoryEmMemoria` e `EnderecoServiceTests`.
  - Build com 0 erros e 0 avisos; 54 testes de aplicação aprovados.
  - O registro no `DependencyInjection.cs` é da Tarefa 3.
- **Tarefa 3 - Infraestrutura:** concluída em 2026-11-09.
  - `EnderecoRepository` (pesquisa com `ORDER BY id`) e `EnderecoRegistro`.
  - Violação da chave estrangeira na inserção vira erro de pessoa inexistente (400).
  - `pessoaid` nulo ou `tipo`, `estado` ou `cep` inválido lido do banco lança `RegistroCorrompidoException` (500).
  - `PessoaRepository.ExcluirAsync` exclui endereços, telefones e a pessoa, na mesma transação.
  - Endereço registrado no `DependencyInjection.cs`, e a limpeza do `BancoDeTesteFixture` exclui os endereços antes das pessoas.
  - Build com 0 erros e 0 avisos; 334 testes aprovados (211 do domínio, 54 da aplicação e 69 da infraestrutura).
- **Tarefa 4 - API:** concluída em 2026-11-09.
  - `EnderecoController`, DTOs de endereço e `EnderecoMapper`.
  - `ErrosDeRequisicaoMapper` tem mensagem própria para `tipo` não inteiro.
  - Rotas e exclusão em cascata verificadas com a API rodando contra o MySQL local.
- **Git:** a spec 003 fica no branch `us003-cadastro-endereco`, publicado em `origin` e criado a partir do `us002-cadastro-telefone`.

## Situação do projeto

- As specs 001 e 002 têm todas as tarefas concluídas, nos branches `us001-cadastro-pessoa` e `us002-cadastro-telefone`, publicados em `origin`. Nenhuma foi para o `main`.
- O trabalho é feito sempre no diretório principal, sem worktrees (D-016).
- O sistema **não vai para produção** (D-017).

## Feito na última sessão

- Memória reiniciada para a spec 003. O histórico detalhado das specs anteriores está no Git.
- `spec.md` e `plan.md` da 003 corrigidos e `tasks.md` gerado, com os esclarecimentos da D-019.
- Tarefas 1 a 4 da spec 003 implementadas e publicadas no branch `us003-cadastro-endereco`.

## Próximo passo

1. Definir o encerramento das specs 001, 002 e 003 (merge no `main`).

## Bloqueios e dúvidas em aberto

- **Pull request do `us001-cadastro-pessoa` para o `main`:** ainda não, por decisão do responsável em 2026-11-06.
- **Branch remoto antigo:** `origin/worktree-tarefa-4-infraestrutura` continua no GitHub, e a exclusão depende do responsável.

## Armadilhas conhecidas

- **Dado do responsável no banco:** a pessoa id 131 ("José João da Silva") não é dos testes; não apagar.
- **Código antigo no Git:** o histórico até `645f5fb` tem `Result`, `Sexo`, paginação e exclusão lógica, que não fazem parte das specs atuais. Não reaproveitar sem checar a spec.
- **Build Docker local:** o Norton Antivirus quebra o `dotnet restore` dentro do container. Ver `conventions.md`, seção Container.
- **Conflito de nomes em C#:** dentro de `UseCases/Pessoa/` e `UseCases/Telefone/`, `Pessoa` e `Telefone` resolvem para o namespace. Usar os aliases `PessoaEntity` e `TelefoneEntity`.
- **Chaves estrangeiras de telefone e endereço:** as tabelas `telefone` e `endereco` referenciam `pessoa` com `NO ACTION`. Testes e exclusão de pessoa precisam apagar os telefones e os endereços antes da pessoa (D-018 e D-019).
- **Pesquisa em lista:** uma única linha com CPF inválido faz a pesquisa inteira falhar com 500 (D-009).
- **Pesquisa por nome:** a collation `utf8mb4_0900_ai_ci` ignora maiúsculas e acentos, e a ordem do retorno não está definida.
- **Data fora do formato:** a resposta 400 traz só esse erro, sem avaliar os demais campos (D-013).
- **JSON cortado:** um corpo cortado logo depois de `"pessoaid":`, `"tipo":` ou `"datadenascimento":` devolve a mensagem de campo inválido, e não a de corpo inválido.
- **`appsettings.json`:** está com `skip-worktree` (D-002), então suas alterações não aparecem no `git status`.
- **Testes de infraestrutura em um clone novo:** falham enquanto o `appsettings.json` local e a `CHAVE_DE_SEGURANCA` não estiverem configurados e o MySQL não estiver acessível (D-008).

## Arquivos-chave

- `specs/003-cadastro_endereco/spec.md`, `plan.md` e `tasks.md`
