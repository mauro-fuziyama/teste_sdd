using CadastroPessoas.Domain.ValueObjects;

namespace CadastroPessoas.Application.UseCases.Endereco.CriarEndereco;

public sealed record CriarEnderecoInput(int? PessoaId, DadosDoEndereco Dados);
