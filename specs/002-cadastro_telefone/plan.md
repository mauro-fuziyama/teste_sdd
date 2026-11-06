# Cadastro de telefones

## Objetivo
* Descrever como será feita a implementação do serviço de cadastro de telefone

## Infra-estrutura 
* O sistema será uma API do tipo Rest.
* A arquitetura será hexagonal.
* A tabela telefone já existe no banco raspadinha_dev, usado também pelos testes de infraestrutura, e não será alterada pela aplicação.
* A tabela tem a chave estrangeira pessoaid para pessoa(id), com ON DELETE e ON UPDATE NO ACTION. O banco recusa telefone com pessoaid inexistente e bloqueia a exclusão de uma pessoa que tenha telefones.
* Como não há índice único em (pessoaid, numero), a regra de número repetido para a mesma pessoa é validada só na aplicação.
* A exclusão de uma pessoa exclui antes os telefones dela, na mesma transação.
* A pesquisa por pessoaid retorna os telefones ordenados pelo id do telefone.

## Rotas das API
* Criar (post): api/telefone
    * Retorna 200 em caso de sucesso. Retorna o id gerado.
    * Retorna 400 em caso de falha a mensagem do erro.
* Consultar pessoaid (get): api/telefone/{pessoaid}
    * Retorna 200 em caso de sucesso.
    * Retorna 400 caso caso ocorra erro. 
* Atualizar (Put): api/telefone/{id}
    * Retorna 404 caso o registro não exista.
    * Retorna 200 em caso de sucesso. 
    * Retorna 400 caso ocorra alguma falha.
* Excluir (delete): api/telefone/{id}
    * Retorna 404 caso o registro não exista.
    * Retorna 200 em caso de sucesso.
    * Retorna 400 caso ocorra alguma falha.

## Detalhe
* Para falhas de servidor retornar 500 e a lista de erros. Os erros 400 devem ser para regras de negócios.
* A lista Erros traz todos os erros de validação encontrados na requisição.
* As respostas 404 trazem a lista Erros, com a mensagem de telefone não encontrada.
* Id não numérico, JSON malformado e corpo ausente retornam 400 com a lista Erros. Nesses casos a requisição não chega à validação de negócio, então os demais campos não são avaliados.
* A resposta 500 traz na lista Erros uma mensagem genérica, sem detalhes internos. O detalhe da falha fica apenas no log.
* No Atualizar (Put), numero vazio ou omitido é rejeitado com 400.
* ITelefoneService é a única entrada da aplicação usada pela API. A classe TelefoneService implementa essa interface e chama os casos de uso.

## Corpo de entrada da API
* Criar
``` Json 
{
    "pessoaid": 1,
    "numero": "11987654321"
}
```

* Atualizar. Um pessoaid enviado no corpo é ignorado.
``` Json 
{
    "numero": "11987654321"
}
```

## Corpo de retorno das API
* Criar. Quando insere Ok.
``` Json 
{
    "id": 0
}
```

* Atualizar e Excluir. Quando Ok não retornar nada.

* Consultar. Quando retorna N registros
``` Json 
{
    "Registros": [   
        {
            "id": 1,
            "pessoaid": 1,
            "numero": "1111111111"
        },
        {
            "id": 2,
            "pessoaid": 1,
            "numero": "2222222222"
        }
    ]
}
```

## Retorno em caso de erros
* Quando ocorrer alguma falha na chamada da API retorne o código de erro e o body a seguir:
```
{
    "Erros": [
        "Erro 1", "Erro 2", "Erro 3"
    ]
}

```

## Tabela telefone no banco de dados
```
CREATE TABLE `telefone` (
  `id` int NOT NULL AUTO_INCREMENT,
  `pessoaid` int NOT NULL,
  `numero` varchar(20) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `pessoaid_idx` (`pessoaid`),
  CONSTRAINT `pessoaid` FOREIGN KEY (`pessoaid`) REFERENCES `pessoa` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

## Estrutura/Diretório da aplicação
```
./
│
├── Dockerfile
├── .dockerignore
│
├── src/
│   │
│   ├── CadastroPessoas.Domain/
│   │   ├── Entities/
│   │   │   └── Telefone.cs
│   │   ├── ValueObjects/
│   │   │   └── NumeroDeTelefone.cs
│   │   ├── Enums/
│   │   ├── Exceptions/
│   │   │   ├── ErrosDeTelefone.cs
│   │   │   ├── RegistroNaoEncontradoException.cs
│   │   │   └── TelefoneNaoEncontradoException.cs
│   │   └── Interfaces/
│   │       └── IPessoaRepository.cs
│   │       └── ITelefoneRepository.cs
│   │
│   ├── CadastroPessoas.Application/
│   │   ├── UseCases/
│   │   │   ├── Pessoa/
│   │   │   │   ├── CriarPessoa/
│   │   │   │   ├── ConsultarPessoa/
│   │   │   │   ├── ExcluirPessoa/
│   │   │   │   ├── AtualizarPessoa/
│   │   │   │   └── PesquisarPessoa/
│   │   │   │
│   │   │   └── Telefone/
│   │   │       ├── CriarTelefone/
│   │   │       │   └── CriarTelefoneUseCase.cs
│   │   │       ├── PesquisarTelefone/
│   │   │       ├── AtualizarTelefone/
│   │   │       └── ExcluirTelefone/
│   │   │
│   │   ├── Interfaces/
│   │   │   └── IPessoaService.cs
│   │   │   └── ITelefoneService.cs
│   │   │
│   │   └── Services/
│   │       └── PessoaService.cs
│   │       └── TelefoneService.cs
│   │
│   ├── CadastroPessoas.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── RegistroCorrompidoException.cs
│   │   │   ├── StringDeConexaoSegura.cs
│   │   │   └── Repositories/
│   │   │       ├── PessoaRepository.cs
│   │   │       ├── PessoaRegistro.cs
│   │   │       ├── TelefoneRepository.cs
│   │   │       └── TelefoneRegistro.cs
│   │   │
│   │   ├── ExternalServices/
│   │   │   ├── DescriptografadorDeSenha.cs
│   │   │   └── MensagensDeSeguranca.cs
│   │   │
│   │   └── DependencyInjection.cs
│   │
│   └── CadastroPessoas.Api/
│       ├── Controllers/
│       │   └── PessoaController.cs
│       │   └── TelefoneController.cs
│       ├── DTOs/
│       │   ├── TelefoneRequest.cs
│       │   ├── TelefoneResponse.cs
│       │   ├── TelefoneCriadoResponse.cs
│       │   └── ListaDeTelefonesResponse.cs
│       ├── Mappers/
│       │   └── TelefoneMapper.cs
│       ├── Middlewares/
│       └── Program.cs
│
└── tests/
    ├── CadastroPessoas.Domain.Tests/
    ├── CadastroPessoas.Application.Tests/
    └── CadastroPessoas.Infrastructure.Tests/
```