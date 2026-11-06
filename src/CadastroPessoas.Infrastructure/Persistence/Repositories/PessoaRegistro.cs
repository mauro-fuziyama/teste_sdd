using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.ValueObjects;

namespace CadastroPessoas.Infrastructure.Persistence.Repositories;

internal sealed class PessoaRegistro
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Cpf { get; init; } = string.Empty;
    public DateTime? DataDeNascimento { get; init; }
    public DateTime DataDeCadastro { get; init; }
    public DateTime DataAtualizacao { get; init; }

    public Pessoa ParaEntidade()
    {
        return Pessoa.Reconstituir(
            Id,
            Nome,
            ObterCpf(),
            DataDeNascimento is null ? null : DateOnly.FromDateTime(DataDeNascimento.Value),
            DataDeCadastro,
            DataAtualizacao);
    }

    private CPF ObterCpf()
    {
        if (!CPF.EhValido(Cpf))
        {
            throw new RegistroCorrompidoException("pessoa", Id, "cpf");
        }

        return new CPF(Cpf);
    }
}
