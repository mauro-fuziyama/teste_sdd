# Cadastro de endereço.

## Objetivo
* Criar um mecanismo que permita criar, visualizar, pesquisar, atualizar e excluir registros de endereço de uma pessoa

## Requisitos funcionais.
* O sistema deve permitir inserir um registro de endereço de uma pessoa.
* O sistema deve permitir a exclusão de um registro usando o campo id.
* O sistema deve permitir excluir um registro de forma física do banco.
* O sistema deve permitir atualizar o endereço de uma pessoa.
* O sistema deve permitir a pesquisa de endereço por pessoaid (chave estrangeira). O retorno deve ser uma lista com 0 ou mais registros, ordenada pelo id do endereço.
* O sistema deve informar quando o registro a consultar, atualizar ou excluir não existir.
* O sistema deve permitir que uma pessoa tenha 0 ou mais endereços.
* O sistema deve excluir todos os endereços quando o registro pessoa desses endereços for excluído.
* O sistema deve bloquear inserir quando o campo pessoaid vier null, vazio ou omitido.
* O sistema deve bloquear atualizar o campo pessoaid.
* O sistema deve bloquear quando um valor de CEP for inválido. O campo deve ter apenas número de tamnho 5 ou 8.
* O sistema deve considerar o valor "Não informado" para o campo "tipo" caso ele não tenha sido informado no método de criação.
* O sistema deve manter o valor do campo "tipo" caso ele não tenha sido informado no método de atualização.

## Modelo de dados do endereço
| Nome | Tipo | Obrigatorio | Validações | Observação |
| ---- | ---- | ----------- | ---------- | ---------- |
| id   | inteiro | sim |  | identificador único gerado no banco de dados - PK |
| pessoaid | inteiro | sim | | |
| tipo | inteiro | não | Enum(0 - não informado, 1-residencial, 2-comercial , 3-outros) | |
| logradouro | varchar(100) | não | | |
| numero | varchar(20) | não | | | 
| complemento | varchar(100) | não | | |
| bairro | varchar(100) | não | | |
| municipio | varchar(100) | sim | | |
| estado | varchar(2) | sim | Sigla de uma das 27 UFs brasileiras, com exatamente 2 caracteres | |
| cep | varchar(10) | não | Apenas números, com 5 ou 8 dígitos | |
 
## Regras de validação de campo
* O sistema deve rejeitar registro que não atende as validações.
* O sistema deve aceitar apenas números no campo CEP.
* O sistema deve excluir os caracteres "-" (hífen) do CEP antes das validações de tamanho e de dígitos. O CEP é gravado sem hífen.
* O sistema deve fazer a exclusão física do registro.
* O sistema só deve inserir um endereço para uma pessoa existente. o campo pessoaid deve existir no campo id da tabela pessoa.
* Na atualização, o sistema deve rejeitar a operação quando o municipio ou o estado vier vazio ou omitido. A atualização altera todos os campos do endereço, exceto o pessoaid; um pessoaid enviado é ignorado.
* Na atualização, os campos logradouro, numero, complemento, bairro e cep omitidos ficam sem valor (NULL). Apenas o tipo omitido mantém o valor atual.
* Os campos opcionais de texto vazios ou só com espaços, inclusive o cep, são tratados como não informados e gravados como NULL.
* O sistema deve rejeitar texto maior que o tamanho definido no modelo de dados.
* O sistema deve rejeitar estado que não seja a sigla de uma UF brasileira (AC, AL, AP, AM, BA, CE, DF, ES, GO, MA, MT, MS, MG, PA, PB, PR, PE, PI, RJ, RN, RS, RO, RR, SC, SP, SE, TO). A sigla pode ser enviada em maiúsculas ou minúsculas e deve ser gravada sempre em maiúsculas.
* O sistema deve remover os espaços das pontas dos campos tipo texto (varchar).
