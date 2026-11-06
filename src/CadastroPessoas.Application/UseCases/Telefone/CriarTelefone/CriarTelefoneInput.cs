namespace CadastroPessoas.Application.UseCases.Telefone.CriarTelefone;

public sealed record CriarTelefoneInput(int? PessoaId, string? Numero);
