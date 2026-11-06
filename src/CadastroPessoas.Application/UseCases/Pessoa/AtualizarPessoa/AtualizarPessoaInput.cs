namespace CadastroPessoas.Application.UseCases.Pessoa.AtualizarPessoa;

public sealed record AtualizarPessoaInput(string? Nome, string? Cpf, DateOnly? DataDeNascimento);
