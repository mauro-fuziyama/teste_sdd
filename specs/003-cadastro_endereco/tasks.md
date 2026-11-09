# Cadastro de endereços

## Tarefa 1 - Domínio
* Criar o enum Enums/TipoDeEndereco.cs com os valores 0 - NaoInformado, 1 - Residencial, 2 - Comercial e 3 - Outros.
* Implementar o Value Object Cep em ValueObjects/Cep.cs:
    * Remover os espaços das pontas antes de validar e guardar o valor sem esses espaços.
    * Excluir os hífens antes de validar e guardar o valor sem eles.
    * Aceitar apenas números.
    * Aceitar apenas 5 ou 8 dígitos.
* Implementar o Value Object SiglaDeEstado em ValueObjects/SiglaDeEstado.cs:
    * Remover os espaços das pontas antes de validar e guardar o valor sem esses espaços.
    * Exigir exatamente 2 caracteres.
    * Converter o valor para maiúsculas antes de validar e guardar a sigla sempre em maiúsculas.
    * Aceitar apenas as siglas das 27 UFs brasileiras listadas na spec.md.
* Criar o record ValueObjects/DadosDoEndereco.cs, com os campos recebidos na criação, na atualização e na reconstituição.
* Implementar a entidade Endereco em Entities/Endereco.cs:
    * Criação com pessoaid, tipo, logradouro, numero, complemento, bairro, municipio, estado e cep, reunindo todos os erros de validação encontrados (D-007).
    * Rejeitar pessoaid nulo ou omitido na criação.
    * Considerar o tipo NaoInformado quando ele não for informado na criação.
    * Rejeitar tipo fora do enum TipoDeEndereco.
    * Rejeitar municipio e estado vazios, só com espaços ou omitidos.
    * Remover os espaços das pontas dos campos de texto.
    * Tratar os campos opcionais de texto vazios ou só com espaços, inclusive o cep, como não informados (nulos).
    * Rejeitar texto maior que o limite da coluna: logradouro, complemento, bairro e municipio com 100 caracteres e numero com 20 caracteres.
    * Atualização que altera todos os campos, exceto o pessoaid. Tipo não informado mantém o valor atual; logradouro, numero, complemento, bairro e cep não informados ficam nulos.
    * Reconstituição para dados vindos do banco.
* Criar as mensagens em Exceptions/ErrosDeEndereco.cs, uma por regra violada:
    * pessoaid obrigatório;
    * pessoa inexistente;
    * tipo inválido;
    * município obrigatório;
    * estado obrigatório;
    * estado com 2 caracteres;
    * sigla de estado inválida;
    * CEP só com dígitos;
    * CEP com 5 ou 8 dígitos;
    * tamanho máximo de logradouro, numero, complemento, bairro e município;
    * endereço não encontrado.
* Criar Exceptions/EnderecoNaoEncontradoException.cs, derivada de RegistroNaoEncontradoException.
* Definir a interface Interfaces/IEnderecoRepository.cs, com inserir, obter por id, pesquisar por pessoaid, atualizar e excluir.
* Criar os testes em tests/CadastroPessoas.Domain.Tests cobrindo os casos válidos e inválidos, incluindo:
    * CEP com 4, 5, 6, 7, 8 e 9 dígitos, com letras, com hífen e com espaços nas pontas;
    * as 27 siglas válidas, sigla inexistente e estado com 1 ou 3 caracteres;
    * sigla em minúsculas ou com maiúsculas e minúsculas misturadas, guardada em maiúsculas;
    * cada valor do enum e valores fora dele;
    * limites de tamanho de cada campo de texto;
    * campos opcionais vazios ou só com espaços tratados como nulos;
    * atualização com tipo omitido e com tipo informado;
    * vários erros reunidos.

## Tarefa 2 - Aplicação
* Definir a interface Interfaces/IEnderecoService.cs, única entrada da aplicação usada pela API para endereços.
* Implementar Services/EnderecoService.cs, que implementa IEnderecoService e chama os casos de uso.
* Implementar os casos de uso em UseCases/Endereco/, referenciando a entidade pelo alias EnderecoEntity, porque Endereco também é nome de namespace:
    * CriarEndereco: rejeitar pessoaid inexistente na tabela pessoa, consultado por IPessoaRepository, reunindo esse erro aos demais erros de validação, e retornar o id gerado. Pular a consulta da pessoa quando o pessoaid for nulo.
    * PesquisarEndereco: buscar por pessoaid, retornando uma lista com 0 ou mais registros, ordenada pelo id do endereço. Pessoa sem endereços ou inexistente retorna a lista vazia.
    * AtualizarEndereco: atualizar todos os campos, exceto o pessoaid, aplicando as regras de atualização da entidade. Lançar EnderecoNaoEncontradoException quando o id não existir.
    * ExcluirEndereco: fazer a exclusão física do registro. Lançar EnderecoNaoEncontradoException quando o id não existir.
* Criar os testes em tests/CadastroPessoas.Application.Tests, com repositório de endereço em memória. Cobrir pelo menos:
    * pessoa inexistente;
    * pessoaid nulo, sem consultar a pessoa;
    * tipo omitido na criação, gravado como NaoInformado;
    * tipo omitido na atualização, mantendo o valor atual;
    * campos opcionais omitidos na atualização, que ficam nulos;
    * atualização que não altera o pessoaid;
    * atualização e exclusão de id inexistente;
    * pesquisa de pessoa sem endereços, com lista vazia;
    * vários erros reunidos.

## Tarefa 3 - Infraestrutura
* Implementar Persistence/Repositories/EnderecoRepository.cs usando Dapper e MySqlConnector, com a tabela endereco definida no plan.md, sem alterar o banco.
    * Ordenar a pesquisa por pessoaid pelo id do endereço (ORDER BY id).
    * Converter a violação da chave estrangeira pessoaid na inserção (pessoa excluída entre a validação e a gravação) em ValidacaoException com o erro de pessoa inexistente, como já é feito no telefone.
* Implementar Persistence/Repositories/EnderecoRegistro.cs. Pessoaid nulo ou tipo, estado ou CEP inválido lido do banco lança RegistroCorrompidoException e resulta em 500, como na D-009.
* Alterar PessoaRepository.ExcluirAsync para excluir os endereços e os telefones da pessoa e depois a pessoa, na mesma transação. A chave estrangeira usa NO ACTION, então o banco bloqueia a exclusão da pessoa enquanto houver endereços.
* Registrar as dependências de endereço em DependencyInjection.cs.
* Criar os testes em tests/CadastroPessoas.Infrastructure.Tests, usando o banco raspadinha_dev, incluindo:
    * inserção, leitura, atualização e exclusão física;
    * gravação de NULL nos campos opcionais não informados;
    * gravação do estado em maiúsculas;
    * ordem da pesquisa;
    * inserção com pessoaid inexistente;
    * exclusão em cascata dos endereços e dos telefones ao excluir a pessoa.
* Ajustar a limpeza do BancoDeTesteFixture para excluir os endereços antes das pessoas criadas pelos testes, por causa da chave estrangeira.

## Tarefa 4 - API
* Implementar DTOs/CriarEnderecoRequest.cs e AtualizarEnderecoRequest.cs, EnderecoResponse.cs, EnderecoCriadoResponse.cs e ListaDeEnderecosResponse.cs, com os nomes do JSON do plan.md. O pessoaid e o tipo aceitam nulo nos requests, para que as regras de campo omitido cheguem ao domínio.
* Implementar Mappers/EnderecoMapper.cs. O tipo sai como inteiro na resposta, e os campos opcionais sem valor saem como null.
* Implementar Controllers/EnderecoController.cs com as rotas do plan.md:
    * Criar (post): api/endereco
    * Pesquisar por pessoaid (get): api/endereco/{pessoaid}
    * Atualizar (put): api/endereco/{id}
    * Excluir (delete): api/endereco/{id}
* Ajustar Mappers/ErrosDeRequisicaoMapper.cs para responder 400 com mensagem própria quando o tipo do corpo não for um número inteiro.
* Verificar as rotas com a API rodando contra o MySQL local, nos casos de sucesso e de erro, como nas specs 001 e 002, incluindo:
    * um pessoaid enviado no PUT, que é ignorado;
    * tipo omitido na criação e na atualização;
    * CEP e estado inválidos;
    * estado enviado em minúsculas, que volta em maiúsculas na consulta;
    * CEP enviado com hífen, que volta sem hífen na consulta;
    * a exclusão de uma pessoa com endereços e telefones pelo DELETE api/pessoa/{id}.
