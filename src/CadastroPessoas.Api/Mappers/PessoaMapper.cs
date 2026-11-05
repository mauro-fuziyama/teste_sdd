using CadastroPessoas.Api.DTOs;
using CadastroPessoas.Application.UseCases.Pessoa.AtualizarPessoa;
using CadastroPessoas.Application.UseCases.Pessoa.CriarPessoa;
using CadastroPessoas.Domain.Entities;

namespace CadastroPessoas.Api.Mappers;

public static class PessoaMapper
{
    public static CriarPessoaInput ParaCriarPessoaInput(PessoaRequest dados)
    {
        return new CriarPessoaInput(dados.Nome, dados.Cpf, dados.DataDeNascimento);
    }

    public static AtualizarPessoaInput ParaAtualizarPessoaInput(PessoaRequest dados)
    {
        return new AtualizarPessoaInput(dados.Nome, dados.Cpf, dados.DataDeNascimento);
    }

    public static PessoaResponse ParaPessoaResponse(Pessoa pessoa)
    {
        return new PessoaResponse(
            pessoa.Id,
            pessoa.Nome,
            pessoa.Cpf.Valor,
            pessoa.DataDeNascimento,
            pessoa.DataDeCadastro,
            pessoa.DataAtualizacao);
    }

    public static ListaDePessoasResponse ParaListaDePessoasResponse(IEnumerable<Pessoa> pessoas)
    {
        return new ListaDePessoasResponse(pessoas.Select(ParaPessoaResponse).ToList());
    }
}
