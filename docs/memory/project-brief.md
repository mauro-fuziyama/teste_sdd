# Project Brief

> O quê e por quê. Muda raramente. Escreva para alguém que nunca viu o projeto.


## Objetivo
- Treinar o uso de SDD e outros padrões no desenvolvimento de sistemas com I.A.
- Extrair o melhor para que o desenvolvimento seja seguro, escalável e profissional.

## Escopo

**Dentro:**
- Testar o uso de SDD e memory do Claude Code para desenvolvimento de aplicações.
- Aprender técnicas para escrever de forma correta e clara para que a IA entenda.

**Fora (por enquanto):**
- O sistema não será para produção

## Restrições
- API REST em .NET 10, com Dapper e MySqlConnector sobre MySQL.
- Senhas e tokens não podem ficar expostos no código nem sem criptografia em arquivos de configuração.

## Visão geral da arquitetura
- Arquitetura hexagonal, .Net, MySQL e docker
```
Cliente HTTP → CadastroPessoas.Api → CadastroPessoas.Application → CadastroPessoas.Domain
                                                  ↑
                     MySQL ← CadastroPessoas.Infrastructure
```
