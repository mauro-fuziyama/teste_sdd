# Cadastro de pessoas

## Objetivo
* Criar um mecanismo que permita criar, visualizar, pesquisar, atualizar e excluir registros de pessoa

## Requisitos funcionais.
* O sistema deve permitir inserir um registro de pessoa
* O sistema deve permitir pesquisar por cpf, nome ou id
* O sistema deve permitir excluir um registro de forma física do banco
* O sistema deve permitri atualizar os dados de um registro de pessoa, menos o CPF.
* Quando for uma busca por nome o sistema deve usar "contém", ou seja o valor fornecido é parte do campo nome. Deverá retornar uma lista de registros.
* Não usar paginação no retorno da lista de pessoa.
* Na busca por cpf e Id o valor deve ser exato e retornar apenas um registro

## Modelo de dados da pessoa
| Nome | Tipo | Obrigatorio | Validações | Observação |
| ---- | ---- | ----------- | ---------- | ---------- |
| id   | inteiro | sim |  | identificador único gerado no banco de dados - PK |
| nome | varchar(100) | sim | Tamanho >= 3, após remover os espaços das pontas | |
| cpf  | varchar(20) | sim | tamanho = 11, só numeros, dígitos não podem ser todos iguais. | | 
| datadenascimento | Date | não | 1910-01-01 < valor < hoje | | 
| datadecadastro | DateTime | sim | | data que foi inserido o registro | 
| dataatualizacao | Datetime | sim | | data da última atualização do registro. na criação fica com a data de criação |

## Regras de validação de campo
* O sistema deve rejeitar registro que não atende as validações.
* O sistema deve garantir que um cpf é unico na base de dados.
* O sistema deve fazer a exclusão física do registro.
* O sistema deve rejeitar a alteração do campo cpf.
* Na atualização, o sistema deve rejeitar a operação quando o cpf vier vazio ou omitido.
* Ao rejeitar um registro, o sistema deve retornar todos os erros de validação encontrados, e não apenas o primeiro.
* O sistema deve validar os dígitos validadores do CPF
* O sistema deve rejeitar CPF com todos os dígitos iguais (ex.: 11111111111), mesmo que passe no cálculo dos dígitos validadores.
* O sistema deve remover os espaços das pontas do campo nome antes de validar o tamanho e deve gravar o nome sem esses espaços.
* O sistema deve usar o horário local para o "hoje" da validação de datadenascimento e para os campos datadecadastro e dataatualizacao.
* O sistema deve tratar como falha de servidor a leitura de um registro já gravado com cpf inválido, sem expor o valor do cpf.

