# CadastroPessoas

## Antes de implementar ou alterar código

1. Leia `docs/memory/active-context.md`: onde paramos e qual é o próximo passo.
2. Leia `docs/memory/progress.md`: estado de cada spec.
3. Consulte `docs/memory/conventions.md` antes de escrever código.
4. Abra **somente** a spec da feature atual em `specs/001-cadastro_de_pessoa/`.
5. Consulte `docs/memory/decisions.md` antes de propor mudanças de arquitetura.

Não leia todas as specs nem varra o projeto inteiro sem necessidade.

## Projeto

- **Nome:** CadastroPessoa
- **Resumo:** aplicação para testar o SDD no desenvolvimento de API Rest em C#
- **Stack:** MySQL, C#, .Net 10, Docker

## Regras onipresentes
* As informações de senha e tokens de acessos não devem ficar espostas no código. Quando armazenadas em arquivos de configuração devem ser criptografadas.
* Não faça adivinhões ou suposições, caso falte alguma especifação ou ela esteja vaga a implementação deve parar e expor esta falha e só continuar quando a especificação estiver esclarecida. Os arquivos plan.md, spec.md e tasks.md devem ser atualizados.
* O código deve seguir as boas práticas SOLID e Clean code.
* Evitar códigos longos e muitas indentações. Neste caso dividir em métodos menores com nomes claros e intuitivos.
* O nomes devem ser sempre claros e intuitivos, evite nomes genéricos e abreviações.
* A string de conexão deve ficar no appsettings.json, com a senha criptograda.
* Uma variável de ambiente chamada CHAVE_DE_SEGURANCA contém uma chave AES que deve ser usado na criptografia. 

## Informações técnicas
* O sistema implementa uma API do tipo Rest.
* O sistema é desenvolvido em .Net 10.
* Usar o Dapper para acesso ao banco de dados.
* Banco de dados é um MySQL, com uma instância local para desenvolvimento já disponível.
* Usar MySqlConnector e Swashbuckle.AspNetCore para conexão com o banco de dados.
* O sistema não vai para produção. Ele pode rodar em um container, gerado pelo Dockerfile.

## Fluxo SDD
- Specs ficam em `specs/001-cadastro_de_pessoa/` (`spec.md` → `plan.md` → `tasks.md`).
- A spec é a fonte da verdade. Se a implementação precisar divergir, atualize a spec e registre o motivo em `docs/memory/decisions.md`.

## Ao concluir cada tarefa do tasks.md
- [ ] Atualizar `docs/memory/active-context.md` (o que foi feito, próximo passo).
- [ ] Atualizar `docs/memory/progress.md`.
- [ ] Registrar decisões novas em `docs/memory/decisions.md`.
- [ ] Registrar convenções novas em `docs/memory/conventions.md`.
