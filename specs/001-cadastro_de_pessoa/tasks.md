#  Cadastro de pessoas

## Tarefa 1
* Executar o Setup inicial da aplicação. A implementação deve ficar no sub-diretório src/
* Criar todos os diretórios que estão definidas no plan.md.
* Criar os arquivos de configuração, projeto e solutions.
* Instalar as bibliotecas.

## Tarefa 2 - Domínio
* Implementar o Value Object CPF em ValueObjects/CPF.cs:
    * Validar tamanho = 11 e somente números.
    * Validar os dígitos verificadores.
    * Rejeitar CPF com todos os dígitos iguais (ex.: 11111111111).
* Implementar a entidade Pessoa em Entities/Pessoa.cs com as validações do spec.md:
    * nome obrigatório, com tamanho >= 3 e no máximo 100 caracteres, validado e gravado sem os espaços das pontas.
    * cpf obrigatório.
    * datadenascimento opcional, com 1910-01-01 < valor < hoje, usando o horário local.
    * Na criação, dataatualizacao fica com a data de criação, usando o horário local.
    * Rejeitar a alteração do cpf e, na atualização, o cpf vazio ou omitido.
    * Reunir todos os erros de validação encontrados em vez de parar no primeiro.
* Criar as exceções de domínio em Exceptions/.
* Definir a interface Interfaces/IPessoaRepository.cs.
* Criar os testes em tests/CadastroPessoas.Domain.Tests cobrindo os casos válidos e inválidos.

## Tarefa 3 - Aplicação
* Definir a interface Interfaces/IPessoaService.cs, única entrada da aplicação usada pela API.
* Implementar Services/PessoaService.cs, que implementa IPessoaService e chama os casos de uso.
* Implementar os casos de uso em UseCases/Pessoa/:
    * CriarPessoa: rejeitar cpf já existente, reunindo esse erro aos demais erros de validação, e retornar o id gerado.
    * ConsultarPessoa: buscar por id ou por cpf, com valor exato, retornando apenas um registro.
    * PesquisarPessoa: buscar por nome usando "contém", retornando uma lista sem paginação.
    * AtualizarPessoa: atualizar os dados, menos o cpf, e registrar a dataatualizacao no horário local.
    * ExcluirPessoa: fazer a exclusão física do registro.
* Criar os testes em tests/CadastroPessoas.Application.Tests.

## Tarefa 4 - Infraestrutura
* Implementar a descriptografia da senha da string de conexão com AES-GCM, usando a chave da variável de ambiente CHAVE_DE_SEGURANCA e o formato do valor definido no plan.md.
* Implementar Persistence/Repositories/PessoaRepository.cs usando Dapper e MySqlConnector, com a tabela pessoa definida no plan.md.
    * Converter a violação de cpf_UNIQUE em ValidacaoException com o erro de cpf já cadastrado.
    * Lançar RegistroCorrompidoException ao ler um registro com cpf inválido, para resultar em 500.
* Registrar as dependências em DependencyInjection.cs.
* Criar os testes em tests/CadastroPessoas.Infrastructure.Tests, usando o banco raspadinha_dev.

## Tarefa 5 - API
* Chamar AdicionarInfraestrutura no Program.cs.
* Implementar DTOs/PessoaRequest.cs e os retornos definidos no plan.md, incluindo as listas Registros e Erros.
* Implementar os mapeamentos em Mappers/.
* Implementar Controllers/PessoaController.cs com as rotas do plan.md:
    * Criar (post): api/pessoa
    * Consultar id (get): api/pessoa/id
    * Consultar cpf (get): api/pessoa/cpf/valor
    * Consultar lista (get): api/pessoa/nome/valor
    * Atualizar (put): api/pessoa/id
    * Excluir (delete): api/pessoa/id
* Implementar em Middlewares/ o tratamento de erros, com 400 para regras de negócio, 404 para registro inexistente e 500 para falhas de servidor, sempre com a lista Erros.
* Responder com 400 e a lista Erros quando o id não for numérico, o JSON estiver malformado, o corpo estiver ausente ou a datadenascimento estiver fora do formato AAAA-MM-DD.
* Rejeitar com 400 a pesquisa por nome com valor vazio ou só com espaços, na aplicação, com teste em tests/CadastroPessoas.Application.Tests.
* Implementar o Swagger no projeto para o ambiente de desenvolvimento. Manter o Swagger disponível apenas no ambiente de desenvolvimento.

## Tarefa 6 - Container
* Criar o Dockerfile para execução da aplicação em produção.
* Configurar o fuso America/Sao_Paulo no container, garantindo que os dados de fuso estejam disponíveis na imagem.
* Não incluir a chave CHAVE_DE_SEGURANCA na imagem. Ela deve ser fornecida por variável de ambiente.
* Não incluir a string de conexão de produção na imagem. Ela deve ser fornecida pela variável de ambiente ConnectionStrings__CadastroPessoas.
* As duas variáveis são injetadas pelo AWS ECS a partir do AWS Secrets Manager, com configuração feita pela área de infra/segurança.
