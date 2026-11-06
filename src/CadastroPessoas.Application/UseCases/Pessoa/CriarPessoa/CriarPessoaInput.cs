namespace CadastroPessoas.Application.UseCases.Pessoa.CriarPessoa;

public sealed record CriarPessoaInput(string? Nome, string? Cpf, DateOnly? DataDeNascimento);
