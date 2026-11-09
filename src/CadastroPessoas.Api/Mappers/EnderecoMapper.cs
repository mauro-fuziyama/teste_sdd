using CadastroPessoas.Api.DTOs;
using CadastroPessoas.Application.UseCases.Endereco.AtualizarEndereco;
using CadastroPessoas.Application.UseCases.Endereco.CriarEndereco;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.ValueObjects;

namespace CadastroPessoas.Api.Mappers;

public static class EnderecoMapper
{
    public static CriarEnderecoInput ParaCriarEnderecoInput(CriarEnderecoRequest dados)
    {
        var dadosDoEndereco = new DadosDoEndereco(
            dados.Tipo,
            dados.Logradouro,
            dados.Numero,
            dados.Complemento,
            dados.Bairro,
            dados.Municipio,
            dados.Estado,
            dados.Cep);

        return new CriarEnderecoInput(dados.PessoaId, dadosDoEndereco);
    }

    public static AtualizarEnderecoInput ParaAtualizarEnderecoInput(AtualizarEnderecoRequest dados)
    {
        var dadosDoEndereco = new DadosDoEndereco(
            dados.Tipo,
            dados.Logradouro,
            dados.Numero,
            dados.Complemento,
            dados.Bairro,
            dados.Municipio,
            dados.Estado,
            dados.Cep);

        return new AtualizarEnderecoInput(dadosDoEndereco);
    }

    public static EnderecoResponse ParaEnderecoResponse(Endereco endereco)
    {
        return new EnderecoResponse(
            endereco.Id,
            endereco.PessoaId,
            (int)endereco.Tipo,
            endereco.Logradouro,
            endereco.Numero,
            endereco.Complemento,
            endereco.Bairro,
            endereco.Municipio,
            endereco.Estado.Valor,
            endereco.Cep?.Valor);
    }

    public static ListaDeEnderecosResponse ParaListaDeEnderecosResponse(IEnumerable<Endereco> enderecos)
    {
        return new ListaDeEnderecosResponse(enderecos.Select(ParaEnderecoResponse).ToList());
    }
}
