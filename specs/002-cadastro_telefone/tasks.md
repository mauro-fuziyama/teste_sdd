#  Cadastro de telefones

## Tarefa 1 - Domínio
* Implementar o Value Object NumeroDeTelefone em ValueObjects/NumeroDeTelefone.cs:
    * Remover os espaços das pontas antes de validar e guardar o valor sem esses espaços.
    * Aceitar apenas números.
    * Aceitar apenas 10 (fixo) ou 11 (celular) dígitos, DDD + telefone.
    * Rejeitar DDD fora do intervalo de 11 a 99.
    * Rejeitar número de 11 dígitos cujo terceiro dígito não seja 9.
* Implementar a entidade Telefone em Entities/Telefone.cs:
    * Criação com pessoaid e número, reunindo todos os erros de validação encontrados (D-007).
    * Rejeitar pessoaid nulo ou omitido na criação.
    * Atualização que altera apenas o número; o pessoaid não pode ser alterado.
    * Reconstituição para dados vindos do banco.
* Criar as mensagens em Exceptions/ErrosDeTelefone.cs, uma por regra violada:
    * número obrigatório;
    * número só com dígitos;
    * número com 10 ou 11 dígitos;
    * DDD entre 11 e 99;
    * celular de 11 dígitos com o terceiro dígito 9;
    * pessoaid obrigatório;
    * pessoa inexistente;
    * número já cadastrado para a pessoa;
    * telefone não encontrado.
* Criar Exceptions/RegistroNaoEncontradoException.cs como base dos erros de registro inexistente. PessoaNaoEncontradaException passa a derivar dela, mantendo a mensagem atual, e a nova TelefoneNaoEncontradoException também deriva dela.
* Definir a interface Interfaces/ITelefoneRepository.cs, com inserir, obter por id, pesquisar por pessoaid, atualizar, excluir e verificar se o número já está cadastrado para a mesma pessoa. A verificação aceita o id de um telefone a ignorar, para que a atualização não acuse o próprio registro.
* Criar os testes em tests/CadastroPessoas.Domain.Tests cobrindo os casos válidos e inválidos, incluindo os limites do DDD (10, 11, 99) e números com espaços nas pontas.

## Tarefa 2 - Aplicação
* Definir a interface Interfaces/ITelefoneService.cs, única entrada da aplicação usada pela API para telefones.
* Implementar Services/TelefoneService.cs, que implementa ITelefoneService e chama os casos de uso.
* Implementar os casos de uso em UseCases/Telefone/:
    * CriarTelefone: rejeitar pessoaid inexistente na tabela pessoa, consultado por IPessoaRepository, e número já cadastrado para a mesma pessoa, reunindo esses erros aos demais erros de validação, e retornar o id gerado. Pular a consulta da pessoa quando o pessoaid for nulo e a de número repetido quando o número for inválido.
    * PesquisarTelefone: buscar por pessoaid, retornando uma lista com 0 ou mais registros, ordenada pelo id do telefone.
    * AtualizarTelefone: atualizar apenas o número, rejeitando número vazio ou omitido e número já cadastrado para a mesma pessoa em outro telefone. Lançar TelefoneNaoEncontradoException quando o id não existir.
    * ExcluirTelefone: fazer a exclusão física do registro. Lançar TelefoneNaoEncontradoException quando o id não existir.
* Criar os testes em tests/CadastroPessoas.Application.Tests, com repositório de telefone em memória. Cobrir pelo menos:
    * pessoa inexistente;
    * número repetido para a mesma pessoa;
    * mesmo número para pessoas diferentes;
    * atualização mantendo o próprio número;
    * pesquisa de pessoa sem telefones, com lista vazia;
    * vários erros reunidos.

## Tarefa 3 - Infraestrutura
* Implementar Persistence/Repositories/TelefoneRepository.cs usando Dapper e MySqlConnector, com a tabela telefone definida no plan.md, sem alterar o banco.
    * Ordenar a pesquisa por pessoaid pelo id do telefone (ORDER BY id).
    * Converter a violação da chave estrangeira pessoaid na inserção (pessoa excluída entre a validação e a gravação) em ValidacaoException com o erro de pessoa inexistente, como já é feito com o CPF duplicado.
* Implementar Persistence/Repositories/TelefoneRegistro.cs. Número inválido lido do banco lança RegistroCorrompidoException e resulta em 500, como na D-009.
* Generalizar Persistence/RegistroCorrompidoException.cs, que hoje cita só "pessoa" e "CPF", para receber o tipo do registro e o campo inválido. A mensagem continua sem expor o valor gravado.
* Alterar PessoaRepository.ExcluirAsync para excluir os telefones da pessoa e depois a pessoa, na mesma transação. A chave estrangeira usa NO ACTION, então o banco bloqueia a exclusão da pessoa enquanto houver telefones.
* Registrar as dependências de telefone em DependencyInjection.cs.
* Criar os testes em tests/CadastroPessoas.Infrastructure.Tests, usando o banco raspadinha_dev, incluindo a exclusão em cascata dos telefones ao excluir a pessoa, a ordem da pesquisa e a inserção com pessoaid inexistente.
* Ajustar a limpeza do BancoDeTesteFixture para excluir os telefones antes das pessoas criadas pelos testes, por causa da chave estrangeira.

## Tarefa 4 - API
* Implementar DTOs/TelefoneRequest.cs (pessoaid e numero), TelefoneResponse.cs, TelefoneCriadoResponse.cs e ListaDeTelefonesResponse.cs, com os nomes do JSON do plan.md. O pessoaid do TelefoneRequest aceita nulo, para que a regra de pessoaid omitido chegue ao domínio.
* Implementar Mappers/TelefoneMapper.cs. No Atualizar, o pessoaid do corpo é ignorado.
* Implementar Controllers/TelefoneController.cs com as rotas do plan.md:
    * Criar (post): api/telefone
    * Pesquisar por pessoaid (get): api/telefone/{pessoaid}
    * Atualizar (put): api/telefone/{id}
    * Excluir (delete): api/telefone/{id}
* Ajustar Middlewares/TratamentoDeErrosMiddleware.cs para responder 404 a qualquer RegistroNaoEncontradoException.
* Ajustar Mappers/ErrosDeRequisicaoMapper.cs para responder 400 com mensagem própria quando o pessoaid da rota não for numérico.
* Verificar as rotas com a API rodando contra o MySQL local, nos casos de sucesso e de erro, como na spec 001, incluindo a exclusão de uma pessoa com telefones pelo DELETE api/pessoa/{id}.
