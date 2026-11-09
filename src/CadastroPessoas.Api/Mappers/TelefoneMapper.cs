using CadastroPessoas.Api.DTOs;
using CadastroPessoas.Application.UseCases.Telefone.AtualizarTelefone;
using CadastroPessoas.Application.UseCases.Telefone.CriarTelefone;
using CadastroPessoas.Domain.Entities;

namespace CadastroPessoas.Api.Mappers;

public static class TelefoneMapper
{
    public static CriarTelefoneInput ParaCriarTelefoneInput(TelefoneRequest dados)
    {
        return new CriarTelefoneInput(dados.PessoaId, dados.Numero);
    }

    public static AtualizarTelefoneInput ParaAtualizarTelefoneInput(TelefoneRequest dados)
    {
        return new AtualizarTelefoneInput(dados.Numero);
    }

    public static TelefoneResponse ParaTelefoneResponse(Telefone telefone)
    {
        return new TelefoneResponse(telefone.Id, telefone.PessoaId, telefone.Numero.Valor);
    }

    public static ListaDeTelefonesResponse ParaListaDeTelefonesResponse(IEnumerable<Telefone> telefones)
    {
        return new ListaDeTelefonesResponse(telefones.Select(ParaTelefoneResponse).ToList());
    }
}
