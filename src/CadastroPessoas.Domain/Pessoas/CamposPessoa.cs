namespace CadastroPessoas.Domain.Pessoas;

public sealed record CamposPessoa(string? Nome, DateOnly? DataNascimento, string? Cpf, string? Sexo);
