using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.Interfaces;
using CadastroPessoas.Domain.ValueObjects;
using PessoaEntity = CadastroPessoas.Domain.Entities.Pessoa;

namespace CadastroPessoas.Application.UseCases.Pessoa.CriarPessoa;

public sealed class CriarPessoaUseCase(IPessoaRepository repositorio, TimeProvider provedorDeTempo)
{
    public async Task<int> ExecutarAsync(CriarPessoaInput dados)
    {
        var momentoAtual = provedorDeTempo.GetLocalNow().DateTime;
        await ValidarAsync(dados, momentoAtual);

        var pessoa = PessoaEntity.Criar(dados.Nome, dados.Cpf, dados.DataDeNascimento, momentoAtual);

        return await repositorio.InserirAsync(pessoa);
    }

    private async Task ValidarAsync(CriarPessoaInput dados, DateTime momentoAtual)
    {
        var erros = PessoaEntity
            .ValidarCriacao(dados.Nome, dados.Cpf, dados.DataDeNascimento, momentoAtual)
            .ToList();

        if (await CpfJaExisteAsync(dados.Cpf))
        {
            erros.Add(ErrosDePessoa.CpfJaCadastrado);
        }

        if (erros.Count > 0)
        {
            throw new ValidacaoException(erros);
        }
    }

    private async Task<bool> CpfJaExisteAsync(string? cpf)
    {
        return CPF.EhValido(cpf) && await repositorio.ObterPorCpfAsync(new CPF(cpf)) is not null;
    }
}
