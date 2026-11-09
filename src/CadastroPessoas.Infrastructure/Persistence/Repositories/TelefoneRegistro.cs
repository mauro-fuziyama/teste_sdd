using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.ValueObjects;

namespace CadastroPessoas.Infrastructure.Persistence.Repositories;

internal sealed class TelefoneRegistro
{
    public int Id { get; init; }
    public int PessoaId { get; init; }
    public string Numero { get; init; } = string.Empty;

    public Telefone ParaEntidade()
    {
        return Telefone.Reconstituir(Id, PessoaId, ObterNumero());
    }

    private NumeroDeTelefone ObterNumero()
    {
        if (!NumeroDeTelefone.EhValido(Numero))
        {
            throw new RegistroCorrompidoException("telefone", Id, "numero");
        }

        return new NumeroDeTelefone(Numero);
    }
}
