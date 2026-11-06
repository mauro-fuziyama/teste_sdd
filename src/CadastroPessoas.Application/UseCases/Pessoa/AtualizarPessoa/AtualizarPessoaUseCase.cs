using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.Interfaces;

namespace CadastroPessoas.Application.UseCases.Pessoa.AtualizarPessoa;

public sealed class AtualizarPessoaUseCase(IPessoaRepository repositorio, TimeProvider provedorDeTempo)
{
    public async Task ExecutarAsync(int id, AtualizarPessoaInput dados)
    {
        var pessoa = await repositorio.ObterPorIdAsync(id) ?? throw new PessoaNaoEncontradaException();
        var momentoAtual = provedorDeTempo.GetLocalNow().DateTime;

        pessoa.Atualizar(dados.Nome, dados.Cpf, dados.DataDeNascimento, momentoAtual);

        await repositorio.AtualizarAsync(pessoa);
    }
}
