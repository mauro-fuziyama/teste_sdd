using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Enums;
using CadastroPessoas.Domain.ValueObjects;

namespace CadastroPessoas.Infrastructure.Persistence.Repositories;

internal sealed class EnderecoRegistro
{
    private const string TipoDoRegistro = "endereço";

    public int Id { get; init; }
    public int? PessoaId { get; init; }
    public int Tipo { get; init; }
    public string? Logradouro { get; init; }
    public string? Numero { get; init; }
    public string? Complemento { get; init; }
    public string? Bairro { get; init; }
    public string Municipio { get; init; } = string.Empty;
    public string Estado { get; init; } = string.Empty;
    public string? Cep { get; init; }

    public Endereco ParaEntidade()
    {
        LancarExcecaoSeHouverCampoCorrompido();

        return Endereco.Reconstituir(Id, PessoaId!.Value, ObterDados());
    }

    private void LancarExcecaoSeHouverCampoCorrompido()
    {
        if (PessoaId is null)
        {
            throw new RegistroCorrompidoException(TipoDoRegistro, Id, "pessoaid");
        }

        if (!Enum.IsDefined((TipoDeEndereco)Tipo))
        {
            throw new RegistroCorrompidoException(TipoDoRegistro, Id, "tipo");
        }

        if (!SiglaDeEstado.EhValido(Estado))
        {
            throw new RegistroCorrompidoException(TipoDoRegistro, Id, "estado");
        }

        if (CepEstaCorrompido())
        {
            throw new RegistroCorrompidoException(TipoDoRegistro, Id, "cep");
        }
    }

    private bool CepEstaCorrompido()
    {
        return !string.IsNullOrWhiteSpace(Cep) && !Domain.ValueObjects.Cep.EhValido(Cep);
    }

    private DadosDoEndereco ObterDados()
    {
        return new DadosDoEndereco(Tipo, Logradouro, Numero, Complemento, Bairro, Municipio, Estado, Cep);
    }
}
