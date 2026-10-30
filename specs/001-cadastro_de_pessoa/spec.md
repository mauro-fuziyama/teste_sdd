# Spec: Cadastro de Pessoa (CRUD)
- **Feature:** 001-cadastro_de_pessoa
- **Status:** Rascunho
- **Data:** 2026-10-30

## 1. Visão geral
Disponibilizar uma API REST que permita criar, consultar, atualizar e excluir cadastros de pessoas físicas. O cadastro identifica cada pessoa de forma única pelo CPF, que deve ser validado antes de ser aceito.

### Objetivos
- Manter uma base confiável de pessoas, sem CPFs inválidos ou duplicados.
- Oferecer operações CRUD simples e previsíveis para os sistemas consumidores.
- Retornar erros de validação claros, que indiquem qual campo está errado e por quê.

### Fora de escopo
- Autenticação e autorização (serão tratadas em outra feature).
- Consulta à situação cadastral do CPF na Receita Federal; a validação é apenas matemática.
- Endereço, contatos, documentos adicionais ou foto.
- Interface gráfica.

## 2. Histórias de usuário
| ID | Como... | Quero... | Para... |
|----|---------|----------|---------|
| US01 | sistema consumidor | cadastrar uma pessoa | registrá-la na base |
| US02 | sistema consumidor | consultar uma pessoa pelo identificador | exibir seus dados |
| US03 | sistema consumidor | listar pessoas com paginação e filtros | localizar cadastros |
| US04 | sistema consumidor | atualizar os dados de uma pessoa | corrigir ou manter o cadastro em dia |
| US05 | sistema consumidor | excluir uma pessoa | o registro será marcado como desativado e não haverá exclusão física |
| US06 | sistema consumidor | alterar CPF | o cpf pode ser alterado desde que obedeça a unicidade do campo |
| US07 | sistema consumidor | idade mínima | a idade mínima é 0 |
| US08 | sistema consumidor | campo sexo | deve aceitas os valores 'F' (feminimo), 'M' (masculino) e 'N' (indefinido) |


## 3. Modelo de dados

### Campos de entrada
| Campo | Tipo | Obrigatório | Regras |
|-------|------|-------------|--------|
| `nome` | texto | Sim | Entre 3 e 150 caracteres após remover espaços nas pontas. Não pode conter apenas espaços. Espaços repetidos internos devem ser reduzidos a um. |
| `dataNascimento` | data (`AAAA-MM-DD`) | Sim | Data válida. Não pode ser futura. Não pode indicar idade superior a 130 anos. |
| `cpf` | texto | Sim | Aceito com ou sem máscara (`000.000.000-00` ou `00000000000`). Deve ser válido conforme a seção 4. Deve ser único na base. |
| `sexo` | enumeração | Sim | Valores aceitos: `M` (masculino), `F` (feminino), `O` (outro), `N` (prefere não informar). Ver questão em aberto Q1. |
| `ativo` | lógico | Sim | Valores aceitos: 1 (true) e 0 (false) |

### Campos gerados pelo sistema
| Campo | Descrição |
|-------|-----------|
| `id` | Identificador único da pessoa, gerado na criação e imutável. |
| `criadoEm` | Data e hora da criação (UTC). |
| `atualizadoEm` | Data e hora da última alteração (UTC). |

### Armazenamento e exibição do CPF
- O CPF é armazenado somente com os 11 dígitos, sem máscara.
- Nas respostas, o CPF é retornado formatado (`000.000.000-00`).

## 4. Regra de validação do CPF
Um CPF é considerado válido quando, depois de removidos os caracteres de máscara (`.` e `-`):

1. Contém exatamente 11 dígitos numéricos. Qualquer outro caractere torna o CPF inválido.
2. Não é composto por todos os dígitos iguais (ex.: `000.000.000-00`, `111.111.111-11`), pois esses passam no cálculo mas não são CPFs reais.
3. O **primeiro dígito verificador** (10º dígito) confere:
   - Multiplicar os 9 primeiros dígitos pelos pesos 10, 9, 8, ..., 2 e somar os resultados.
   - Calcular o resto da divisão da soma por 11.
   - Se o resto for menor que 2, o dígito é `0`; caso contrário, é `11 - resto`.
4. O **segundo dígito verificador** (11º dígito) confere:
   - Multiplicar os 10 primeiros dígitos (incluindo o primeiro verificador) pelos pesos 11, 10, 9, ..., 2 e somar os resultados.
   - Aplicar a mesma regra do resto descrita no passo 3.

### Exemplos para testes
| Entrada | Resultado esperado | Motivo |
|---------|--------------------|--------|
| `529.982.247-25` | Válido | Dígitos verificadores corretos |
| `52998224725` | Válido | Mesmo CPF, sem máscara |
| `529.982.247-26` | Inválido | Segundo dígito verificador incorreto |
| `111.111.111-11` | Inválido | Todos os dígitos iguais |
| `5299822472` | Inválido | Apenas 10 dígitos |
| `529.982.247-2A` | Inválido | Contém letra |
| *(vazio)* | Inválido | Campo obrigatório |

## 5. Requisitos funcionais

### RF01 – Cadastrar pessoa
- Recebe `nome`, `dataNascimento`, `cpf` e `sexo`.
- Valida todos os campos conforme as seções 3 e 4 e retorna **todos** os erros encontrados de uma vez, não apenas o primeiro.
- Rejeita o cadastro se já existir uma pessoa **ativa** com o mesmo CPF.
- Se o CPF pertencer a uma pessoa excluída (inativa), o mesmo registro é reaproveitado: os dados são substituídos pelos do cadastro, `criadoEm` e `atualizadoEm` recebem a data atual, o registro volta a ficar ativo e mantém o seu `id`. A resposta é a mesma de um cadastro novo.
- Em caso de sucesso, retorna a pessoa criada com `id`, `criadoEm` e `atualizadoEm`, e informa a localização do novo recurso.

### RF02 – Consultar pessoa por id
- Retorna os dados da pessoa correspondente ao `id`.
- Se não existir, informa que o recurso não foi encontrado.

### RF03 – Listar pessoas
- Retorna as pessoas de forma paginada (padrão: página 1, 20 itens; máximo: 100 itens por página).
- Permite filtrar por `nome` (busca parcial, sem diferenciar maiúsculas, minúsculas e acentos) e por `cpf` (busca exata, com ou sem máscara).
- Ordenação padrão: `nome` em ordem crescente.
- A resposta informa o total de registros e o total de páginas.

### RF04 – Atualizar pessoa
- Substitui todos os campos editáveis da pessoa (`nome`, `dataNascimento`, `cpf`, `sexo`), aplicando as mesmas validações do cadastro.
- Se o CPF for alterado, o novo CPF não pode pertencer a outra pessoa, ativa ou inativa.
- `id` e `criadoEm` não podem ser alterados; `atualizadoEm` é atualizado automaticamente.
- Se a pessoa não existir, informa que o recurso não foi encontrado.

### RF05 – Excluir pessoa
- Exclui logicamente a pessoa correspondente ao `id`: o registro é marcado como inativo e mantido no banco (US05).
- Se a pessoa não existir, informa que o recurso não foi encontrado.

## 6. Contrato da API (visão funcional)

O detalhamento técnico fica no `plan.md`. Do ponto de vista funcional, a API expõe:

| Operação | Método e rota | Sucesso | Erros previstos |
|----------|---------------|---------|-----------------|
| Cadastrar | `POST /api/v1/pessoas` | 201 Created | 400 validação, 409 CPF duplicado |
| Consultar | `GET /api/v1/pessoas/{id}` | 200 OK | 404 não encontrada |
| Listar | `GET /api/v1/pessoas?nome=&cpf=&pagina=&tamanhoPagina=` | 200 OK | 400 parâmetros inválidos |
| Atualizar | `PUT /api/v1/pessoas/{id}` | 200 OK | 400 validação, 404 não encontrada, 409 CPF duplicado |
| Excluir | `DELETE /api/v1/pessoas/{id}` | 204 No Content | 404 não encontrada |

### Exemplo de requisição (cadastro)

```json
{
  "nome": "Maria da Silva",
  "dataNascimento": "1990-05-14",
  "cpf": "529.982.247-25",
  "sexo": "F"
}
```

### Exemplo de resposta (sucesso)

```json
{
  "id": "3f2b8c1e-7a4d-4e9b-9c2a-1d5e6f7a8b9c",
  "nome": "Maria da Silva",
  "dataNascimento": "1990-05-14",
  "cpf": "529.982.247-25",
  "sexo": "F",
  "criadoEm": "2026-10-30T16:14:00Z",
  "atualizadoEm": "2026-10-30T16:14:00Z"
}
```

### Formato de erro

Os erros seguem o padrão Problem Details (RFC 9457), com mensagens em português e a lista de erros por campo:

```json
{
  "type": "https://httpstatuses.io/400",
  "title": "Um ou mais campos são inválidos.",
  "status": 400,
  "errors": {
    "cpf": ["CPF inválido."],
    "dataNascimento": ["A data de nascimento não pode ser futura."]
  }
}
```

## 7. Mensagens de validação

| Situação | Mensagem |
|----------|----------|
| Campo obrigatório ausente | `O campo {campo} é obrigatório.` |
| Nome fora do tamanho | `O nome deve ter entre 3 e 150 caracteres.` |
| Data em formato inválido | `A data de nascimento deve estar no formato AAAA-MM-DD.` |
| Data futura | `A data de nascimento não pode ser futura.` |
| Idade acima do limite | `A data de nascimento indica uma idade superior a 130 anos.` |
| CPF inválido | `CPF inválido.` |
| CPF duplicado | `Já existe uma pessoa cadastrada com este CPF.` |
| Sexo inválido | `Sexo inválido. Valores aceitos: M, F, O, N.` |
| Pessoa não encontrada | `Pessoa não encontrada.` |

## 8. Requisitos não funcionais

- **RNF01 – Desempenho:** consultas por `id` e por `cpf` devem responder em até 200 ms (p95) com até 100 mil registros.
- **RNF02 – Integridade:** a unicidade do CPF deve ser garantida também no banco de dados, não apenas na aplicação, para evitar duplicidade em requisições simultâneas.
- **RNF03 – Privacidade (LGPD):** CPF e data de nascimento são dados pessoais. Não devem aparecer completos em logs; quando necessário, o CPF deve ser mascarado (ex.: `***.982.247-**`).
- **RNF04 – Documentação:** a API deve publicar documentação OpenAPI com exemplos de requisição e resposta.
- **RNF05 – Testes:** a regra de validação do CPF deve ter testes unitários cobrindo todos os exemplos da seção 4; cada endpoint deve ter testes de integração para os cenários de sucesso e de erro.

## 9. Critérios de aceite

- **CA01:** Dado um payload com todos os campos válidos e CPF ainda não cadastrado, quando eu cadastrar, então recebo 201 e os dados da pessoa com `id` gerado.
- **CA02:** Dado um CPF com dígito verificador incorreto, quando eu cadastrar, então recebo 400 com o erro `CPF inválido.` no campo `cpf`.
- **CA03:** Dado um CPF com todos os dígitos iguais, quando eu cadastrar, então recebo 400 com o erro `CPF inválido.`.
- **CA04:** Dado um CPF já cadastrado (com ou sem máscara), quando eu cadastrar outra pessoa com ele, então recebo 409.
- **CA05:** Dado um payload com vários campos inválidos, quando eu cadastrar, então recebo 400 listando todos os erros.
- **CA06:** Dada uma data de nascimento futura, quando eu cadastrar ou atualizar, então recebo 400.
- **CA07:** Dado um `id` inexistente, quando eu consultar, atualizar ou excluir, então recebo 404.
- **CA08:** Dada uma pessoa existente, quando eu alterar seu CPF para o CPF de outra pessoa, então recebo 409 e nada é alterado.
- **CA09:** Dada uma listagem com filtro `nome=joao`, então recebo as pessoas cujo nome contém "João", "joão" ou "JOAO".
- **CA10:** Dada uma pessoa excluída, quando eu consultá-la, então recebo 404.
- **CA11:** Dada uma pessoa excluída, quando eu cadastrar uma pessoa com o mesmo CPF, então recebo 201 com o mesmo `id` do registro excluído, os novos dados e `criadoEm` igual à data do novo cadastro.
- **CA12:** Dada uma pessoa excluída, quando outra pessoa tentar mudar o seu CPF para o dela, então recebo 409.

## 10. Questões em aberto

| ID | Questão | Proposta atual |
|----|---------|----------------|
| Q1 | Quais valores o campo `sexo` deve aceitar? Apenas `M`/`F` ou também outras opções? | `M`, `F`, `O`, `N` |
| Q2 | A exclusão deve ser física (remove o registro) ou lógica (marca como inativo e mantém o histórico)? | **Respondida:** lógica (US05). Cadastro com CPF de pessoa inativa reaproveita e reativa o registro |
| Q3 | Existe idade mínima para cadastro (ex.: apenas maiores de 18 anos)? | Sem idade mínima |
| Q4 | É necessária atualização parcial (`PATCH`) além da completa (`PUT`)? | Apenas `PUT` |
| Q5 | O CPF pode ser alterado depois do cadastro ou deve ser imutável? | Pode ser alterado, respeitando a unicidade |
