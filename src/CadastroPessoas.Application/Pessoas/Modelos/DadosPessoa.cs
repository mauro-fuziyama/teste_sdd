using CadastroPessoas.Domain.Pessoas;

namespace CadastroPessoas.Application.Pessoas.Modelos;

public sealed record DadosPessoa(string? Nome, DateOnly? DataNascimento, string? Cpf, string? Sexo)
{
    public CamposPessoa ParaCampos() => new(Nome, DataNascimento, Cpf, Sexo);
}
