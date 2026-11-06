using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.Interfaces;

namespace CadastroPessoas.Application.UseCases.Pessoa.ExcluirPessoa;

public sealed class ExcluirPessoaUseCase(IPessoaRepository repositorio)
{
    public async Task ExecutarAsync(int id)
    {
        _ = await repositorio.ObterPorIdAsync(id) ?? throw new PessoaNaoEncontradaException();

        await repositorio.ExcluirAsync(id);
    }
}
