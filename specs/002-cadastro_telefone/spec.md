# Cadastro de telefones

## Objetivo
* Criar um mecanismo que permita criar, visualizar, pesquisar, atualizar e excluir registros de telefone de uma pessoa

## Requisitos funcionais.
* O sistema deve permitir inserir um registro de telefone de uma pessoa.
* O sistema deve permitir a exclusão de um registro usando o campo id.
* O sistema deve permitir excluir um registro de forma física do banco.
* O sistema deve permitir atualizar o número do telefone de uma pessoa.
* O sistema deve permitir a pesquisa de telefone por pessoaid (chave estrangeira). O retorno deve ser uma lista com 0 ou mais registros, ordenada pelo id do telefone.
* O sistema deve informar quando o registro a consultar, atualizar ou excluir não existir.
* O sistema deve permitir que uma pessoa tenha 0 ou mais telefones.
* O sistema deve excluir todos os telefones quando o registro pessoa desses telefones for excluído.
* O sistema deve bloquear o mesmo número para a mesma pessoa.
* O sistema deve bloquear se o DDD não estiver entre 11 e 99. 
* O sistema deve bloquear um numero de tamanho 11 que o terceiro número é diferente de 9 (celular).
* O sistema deve bloquear inserir quando o campo pessoaid vier null, vazio ou omitido.
* O sistema deve bloquear atualizar o campo pessoaid.
* O sistema deve permitir o mesmo telefone para pessoas diferentes.


## Modelo de dados do telefone
| Nome | Tipo | Obrigatorio | Validações | Observação |
| ---- | ---- | ----------- | ---------- | ---------- |
| id   | inteiro | sim |  | identificador único gerado no banco de dados - PK |
| pessoaid | inteiro | sim | | |
| numero | varchar(20) | sim | 10 (fixo) e 11 (celular) |  | 

## Regras de validação de campo
* O sistema deve rejeitar registro que não atende as validações.
* O sistema deve aceitar apenas números no campo numero.
* O sistema deve aceitar apenas os número de telefone com tamanho 10 (fixo) e 11 (celular) dígitos. DDD + telefone
* O sistema deve fazer a exclusão física do registro.
* O sistema só deve inserir um telefone para uma pessoa existente. o campo pessoaid deve existir no campo id da tabela pessoa.
* Na atualização, o sistema deve rejeitar a operação quando o número vier vazio ou omitido. A atualização altera apenas o número; um pessoaid enviado é ignorado.
* O sistema deve remover os espaços das pontas do campo numero antes de validar o tamanho e deve gravar o números sem esses espaços.

