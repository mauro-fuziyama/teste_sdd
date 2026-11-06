# Cadastro de pessoas

## Objetivo
* Descrever como será feita a implementação do serviço de cadastro de pessoa

## Infra-estrutura 
* O sistema será uma API do tipo Rest.
* A arquitetura será hexagonal.
* Será usado a instância MySQL que roda no computador local para desenvolvimento. Esta instância já esta rodando.
* A aplicação quando em produção irá rodar em container no AWS ECS, associado ao AWS Secrets Manager. A configuração na AWS é responsabilidade da área de infra/segurança.
* Em produção, o Secrets Manager fornece ao container as variáveis de ambiente CHAVE_DE_SEGURANCA e ConnectionStrings__CadastroPessoas, com a senha da string de conexão criptografada no formato definido neste plano.
* As tabelas já existem no banco de dados local.
* O framework de testes será o xUnit.
* Os testes de infraestrutura usam o banco raspadinha_dev, cujos dados são de teste e podem ser apagados ou atualizados. Os testes removem as linhas que criam.
* A senha da string de conexão será criptografada com AES-GCM, usando a chave de 256 bits em Base64 da variável de ambiente CHAVE_DE_SEGURANCA.
* O valor criptografado da senha é um Base64 dos bytes na ordem [nonce de 12 bytes][texto cifrado][tag de 16 bytes]. O texto original é UTF-8 e não há dados associados (AAD).
* O appsettings.json com a senha criptografada fica apenas no computador local e não é commitado.
* O banco de dados de desenvolvimento é o raspadinha_dev, acessado com o usuário teste_sdd.
* implementar endpoints de pesquisas por nome e cpf. O nome pode ser parcial e retornar vários registros, o de cpf retorna 0 ou 1 registro
* As datas usam o horário local: o "hoje" da validação de datadenascimento, datadecadastro e dataatualizacao.
* O container de produção usa o fuso America/Sao_Paulo como horário local.

## Container de produção
* O Dockerfile fica na raiz do repositório e gera a imagem a partir das imagens oficiais mcr.microsoft.com/dotnet/sdk:10.0 (build) e mcr.microsoft.com/dotnet/aspnet:10.0 (execução).
* A aplicação escuta na porta 8080, padrão das imagens .NET, e roda com usuário sem privilégios de administrador ($APP_UID).
* O fuso é definido pela variável TZ=America/Sao_Paulo. O build falha se os dados desse fuso não existirem na imagem.
* O .dockerignore impede que o appsettings.json local, os testes, a documentação e o .git entrem na imagem. Sem appsettings.json, a configuração vem só das variáveis de ambiente.
* O build da imagem não usa o global.json: usa o SDK da imagem sdk:10.0.
* O ambiente da imagem é Production, então o Swagger fica desligado.

## Rotas das API
* Criar (post): api/pessoa
    * Retorna 200 em caso de sucesso. Retorna o id gerado.
    * Retorna 400 em caso de falha a mensagem do erro.
* Consultar id (get): api/pessoa/id
    * Retorna 200 em caso de sucesso.
    * Retorna 404 caso não retorne registro
    * Retorna 400 caso caso ocorra erro. 
* consultar cpf (get): api/pessoa/cpf/valor
    * Retorna 200 em caso de sucesso.
    * Retorna 404 caso não retorne registro
    * Retorna 400 caso caso ocorra erro. 
* consultar lista (get): api/pessoa/nome/valor
    * Retorna 200 em caso retorne 0 ou mais registros.
    * Retorna 400 caso caso ocorra erro. 
* Atualizar (Put): api/pessoa/id
    * Retorna 404 caso o registro não exista.
    * Retorna 200 em caso de sucesso. 
    * Retorna 400 caso ocorra alguma falha.
* Excluir (delete): api/pessoa/id
    * Retorna 404 caso o registro não exista.
    * Retorna 200 em caso de sucesso.
    * Retorna 400 caso ocorra alguma falha.

## Detalhe
* Para falhas de servidor retornar 500 e a lista de errrs. Os erros 400 devem ser para regras de negócios.
* Um registro já gravado com CPF inválido é tratado como falha de servidor: a leitura lança RegistroCorrompidoException e a API retorna 500, sem expor o CPF. Na pesquisa por nome, uma única linha nessa situação faz toda a pesquisa falhar.
* A lista Erros traz todos os erros de validação encontrados na requisição.
* As respostas 404 trazem a lista Erros, com a mensagem de pessoa não encontrada.
* Id não numérico, JSON malformado, corpo ausente e datadenascimento fora do formato AAAA-MM-DD retornam 400 com a lista Erros. Nesses casos a requisição não chega à validação de negócio, então os demais campos não são avaliados.
* Na pesquisa por nome, valor vazio ou só com espaços é rejeitado com 400.
* A resposta 500 traz na lista Erros uma mensagem genérica, sem detalhes internos. O detalhe da falha fica apenas no log.
* Na consulta, datadenascimento não informada é retornada como null.
* No Atualizar (Put), cpf vazio ou omitido é rejeitado com 400.
* IPessoaService é a única entrada da aplicação usada pela API. A classe PessoaService implementa essa interface e chama os casos de uso.

## Corpo de entrada da API
* Criar/Atualizar 
``` Json 
{
    "nome": "",
    "cpf": "",
    "datadenascimento": "AAAA-MM-DD"
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

* Consultar. Quando retorna apenas um registro
``` Json 
{
    "id": 0,
    "nome": "",
    "cpf": "",
    "datadenascimento": "AAAA-MM-DD",
    "datadecadastro": "AAAA-MM-DDTHH:MM:SS",
    "dataatualizacao": "AAAA-MM-DDTHH:MM:SS"    
}
```

* Consultar. Quando retorna N registros
``` Json 
{
    "Registros": [   
        {
            "id": 1,
            "nome": "",
            "cpf": "",
            "datadenascimento": "AAAA-MM-DD",
            "datadecadastro": "AAAA-MM-DDTHH:MM:SS",
            "dataatualizacao": "AAAA-MM-DDTHH:MM:SS"    
        },
        {
            "id": 2,
            "nome": "",
            "cpf": "",
            "datadenascimento": "AAAA-MM-DD",
            "datadecadastro": "AAAA-MM-DDTHH:MM:SS",
            "dataatualizacao": "AAAA-MM-DDTHH:MM:SS"    
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

## Tabela pessoa no banco de dados
```
CREATE TABLE `pessoa` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nome` varchar(100) NOT NULL,
  `cpf` varchar(20) NOT NULL,
  `datadenascimento` date DEFAULT NULL,
  `datadecadastro` datetime NOT NULL,
  `dataatualizacao` datetime NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `cpf_UNIQUE` (`cpf`)
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
│   │   │   └── Pessoa.cs
│   │   │
│   │   ├── ValueObjects/
│   │   │   └── CPF.cs
│   │   │
│   │   ├── Enums/
│   │   │
│   │   ├── Exceptions/
│   │   │
│   │   └── Interfaces/
│   │       └── IPessoaRepository.cs
│   │
│   ├── CadastroPessoas.Application/
│   │   ├── UseCases/
│   │   │   └── Pessoa/
│   │   │       ├── CriarPessoa/
│   │   │       │   ├── CriarPessoaUseCase.cs
│   │   │       │   └── CriarPessoaInput.cs
│   │   │       │
│   │   │       ├── ConsultarPessoa/
│   │   │       │   └── ConsultarPessoaUseCase.cs
│   │   │       │
│   │   │       └── ExcluirPessoa/
│   │   │           └── ExcluirPessoaUseCase.cs
│   │   │
│   │   │       └── AtualizarPessoa/
│   │   │           ├── AtualizarPessoaUseCase.cs
│   │   │           └── AtualizarPessoaInput.cs
│   │   │
│   │   │       └── PesquisarPessoa/
│   │   │           └── PesquisarPessoaUseCase.cs
│   │   │
│   │   └── Interfaces/
│   │       └── IPessoaService.cs
│   │
│   │   └── Services/
│   │       └── PessoaService.cs
│   │
│   ├── CadastroPessoas.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── RegistroCorrompidoException.cs
│   │   │   ├── StringDeConexaoSegura.cs
│   │   │   └── Repositories/
│   │   │       ├── PessoaRepository.cs
│   │   │       └── PessoaRegistro.cs
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
│       │
│       ├── DTOs/
│       │   ├── PessoaRequest.cs
│       │   ├── PessoaResponse.cs
│       │   ├── PessoaCriadaResponse.cs
│       │   ├── ListaDePessoasResponse.cs
│       │   └── ErrosResponse.cs
│       │
│       ├── Mappers/
│       │   ├── PessoaMapper.cs
│       │   └── ErrosDeRequisicaoMapper.cs
│       │
│       ├── Middlewares/
│       │   └── TratamentoDeErrosMiddleware.cs
│       │
│       └── Program.cs
│
└── tests/
    ├── CadastroPessoas.Domain.Tests/
    ├── CadastroPessoas.Application.Tests/
    └── CadastroPessoas.Infrastructure.Tests/
```