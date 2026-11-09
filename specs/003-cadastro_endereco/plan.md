# Cadastro de endereços

## Objetivo
* Descrever como será feita a implementação do serviço de cadastro de endereços.

## Infra-estrutura 
* O sistema será uma API do tipo Rest.
* A arquitetura será hexagonal.
* A tabela endereço já existe no banco raspadinha_dev, usado também pelos testes de infraestrutura, e não será alterada pela aplicação.
* A tabela tem a chave estrangeira pessoaid para pessoa(id), com ON DELETE e ON UPDATE NO ACTION. O banco recusa endereço com pessoaid inexistente e bloqueia a exclusão de uma pessoa que tenha endereços.
* A exclusão de uma pessoa exclui antes os endereços dela, na mesma transação.
* A pesquisa por pessoaid retorna os endereços ordenados pelo id do endereço.

## Rotas das API
* Criar (post): api/endereco
    * Retorna 200 em caso de sucesso. Retorna o id gerado.
    * Retorna 400 em caso de falha a mensagem do erro.
* Consultar pessoaid (get): api/endereco/{pessoaid}
    * Retorna 200 em caso de sucesso.
    * Retorna 400 caso caso ocorra erro. 
* Atualizar (Put): api/endereco/{id}
    * Retorna 404 caso o registro não exista.
    * Retorna 200 em caso de sucesso. 
    * Retorna 400 caso ocorra alguma falha.
* Excluir (delete): api/endereco/{id}
    * Retorna 404 caso o registro não exista.
    * Retorna 200 em caso de sucesso.
    * Retorna 400 caso ocorra alguma falha.

## Detalhe
* Para falhas de servidor retornar 500 e a lista de erros. Os erros 400 devem ser para regras de negócios.
* A lista Erros traz todos os erros de validação encontrados na requisição.
* As respostas 404 trazem a lista Erros, com a mensagem de endereço não encontrada.
* Id não numérico, JSON malformado e corpo ausente retornam 400 com a lista Erros. Nesses casos a requisição não chega à validação de negócio, então os demais campos não são avaliados.
* A resposta 500 traz na lista Erros uma mensagem genérica, sem detalhes internos. O detalhe da falha fica apenas no log.
* No Atualizar (Put), municipio ou estado vazio ou omitido é rejeitado com 400.
* No Atualizar (Put), todos os campos são alterados, exceto o pessoaid. Os campos logradouro, numero, complemento, bairro e cep omitidos ficam sem valor (NULL), e o tipo omitido mantém o valor atual.
* Campos opcionais de texto vazios ou só com espaços, inclusive o cep, são gravados como NULL e voltam como null na consulta.
* Texto maior que o tamanho da coluna, tipo fora do enum e estado que não seja a sigla de uma UF brasileira são rejeitados com 400.
* O estado é convertido para maiúsculas antes de validar e é sempre gravado em maiúsculas.
* Os hífens do cep são excluídos antes de validar, e o cep é gravado e retornado sem hífen.
* Tipo não inteiro no corpo retorna 400 com mensagem própria, como o pessoaid.
* IEnderecoService é a única entrada da aplicação usada pela API. A classe EnderecoService implementa essa interface e chama os casos de uso.
* Criar um enum para o campo Tipo do registro endeço com os seguintes valores
	* 0 - Não informado
	* 1 - Residencial
	* 2 - Comercial
	* 3 - Outros

## Corpo de entrada da API
* Criar
``` Json 
{
    "pessoaid": 1,
    "tipo": 0,
	"logradouro": "",
	"numero": "",
	"complemento": "",
	"bairro": "",
	"municipio": "",
	"estado": "",
	"cep": ""
}
```

* Atualizar. Um pessoaid enviado no corpo é ignorado.
``` Json 
{
    "tipo": 0,
	"logradouro": "",
	"numero": "",
	"complemento": "",
	"bairro": "",
	"municipio": "",
	"estado": "",
	"cep": ""
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
			"tipo": 0,
			"logradouro": "",
			"numero": "",
			"complemento": "",
			"bairro": "",
			"municipio": "",
			"estado": "",
			"cep": ""
		},
        {
			"id": 2,
			"pessoaid": 1,
			"tipo": 0,
			"logradouro": "",
			"numero": "",
			"complemento": "",
			"bairro": "",
			"municipio": "",
			"estado": "",
			"cep": ""
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

## Tabela endereco no banco de dados
```
CREATE TABLE `endereco` (
  `id` int NOT NULL AUTO_INCREMENT,
  `pessoaid` int DEFAULT NULL,
  `tipo` int NOT NULL DEFAULT '0',
  `logradouro` varchar(100) DEFAULT NULL,
  `numero` varchar(20) DEFAULT NULL,
  `complemento` varchar(100) DEFAULT NULL,
  `bairro` varchar(100) DEFAULT NULL,
  `municipio` varchar(100) NOT NULL,
  `estado` varchar(2) NOT NULL,
  `cep` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `endereco_pessoaId_idx` (`pessoaid`),
  CONSTRAINT `endereco_pessoaId` FOREIGN KEY (`pessoaid`) REFERENCES `pessoa` (`id`)
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
│   │   ├── ValueObjects/
│   │   ├── Enums/
│   │   ├── Exceptions/
│   │   └── Interfaces/
│   │
│   ├── CadastroPessoas.Application/
│   │   ├── UseCases/
│   │   │   ├── Pessoa/
│   │   │   ├── Telefone/
│   │   │   └── Endereco/
│   │   │
│   │   ├── Interfaces/
│   │   │   ├── IPessoaService.cs
│   │   │   ├── ITelefoneService.cs
│   │   │   └── IEnderecoService.cs
│   │   │
│   │   └── Services/
│   │
│   ├── CadastroPessoas.Infrastructure/
│   │   ├── Persistence/
│   │   │   └── Repositories/
│   │   │
│   │   ├── ExternalServices/
│   │   │
│   │   └── DependencyInjection.cs
│   │
│   └── CadastroPessoas.Api/
│       ├── Controllers/
│       │   ├── PessoaController.cs
│       │   └── TelefoneController.cs
│       │   └── EnderecoController.cs
│       │
│       ├── DTOs/
│       ├── Mappers/
│       ├── Middlewares/
│       └── Program.cs
│
└── tests/
    ├── CadastroPessoas.Domain.Tests/
    ├── CadastroPessoas.Application.Tests/
    └── CadastroPessoas.Infrastructure.Tests/
```