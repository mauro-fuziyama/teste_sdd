## Fluxo SDD
- Specs ficam em specs/001-cadastro_de_pessoa/
- Antes de implementar, leia spec.md e plan.md da feature
- Marque os itens em tasks.md conforme concluir
- Siga as regras de specs/constitution.md
- Caso falte alguma definição não tente adivinhar pela sua experiência, pare a implementação e aguarde a definição do usuário.
- Atualize o arquivo README.md

## Especificações técnicas do sistema
* A implementação deve implementar uma API do tipo Rest
* A implementação deve ser desenvolvido em C# e .Net
* A arquitetura deve ser Hexagonal, com separação clara de domínio, casos de uso e infra-estrutura
* A implementação deve ser feita usando boas práticas SOLID e Clean code
* A implementação deve evitar múltiplas identações e blocos de código longos, buscando sempre a simplicidade e clareza. 
* A implementação deve sugerir dividir códigos longos em métodos menores, mais coesos e com nomes descritivos.
* As configurações devem fica no arquivo appsettings.json.
* Os campos especiais, como telefone, cep e cpf devem ser persistidos sem máscara, mas devem ser retornados com máscara.

## Regras do projeto
- API REST de cadastro de pessoas em C# / .NET 10, arquitetura Hexagonal
- Domain não referencia nenhum outro projeto nem pacote externo
- Application referencia apenas Domain
- Regras de negócio retornam Result, não lançam exceção
- SQL somente em classes *Sql dentro de Infrastructure, sempre com parâmetros
- Máximo 2 níveis de indentação por método; métodos curtos com nomes descritivos
- Campos privados com prefixo `_` (`_campo`)
- Asserções nos testes com AwesomeAssertions
- Todas as configurações no appsettings.json, inclusive a connection string. No Git ele fica com a senha vazia (preencher localmente e usar `git update-index --skip-worktree`); em produção a pipeline substitui o arquivo
- Tabela existente `pessoa` (id INT AUTO_INCREMENT, índice único `cpf_UNIQUE`); exclusão é lógica (ativo = 0); cadastro com CPF de pessoa inativa reaproveita e reativa o registro

## Comandos
- Banco: MySQL local já existente (schema e tabelas criados); conexão em src/CadastroPessoas.Api/appsettings.json
- Build: dotnet build
- Testes: dotnet test
- Rodar API: dotnet run --project src/CadastroPessoas.Api



