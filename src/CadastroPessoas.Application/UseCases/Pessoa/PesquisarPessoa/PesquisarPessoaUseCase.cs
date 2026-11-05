using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.Interfaces;
using PessoaEntity = CadastroPessoas.Domain.Entities.Pessoa;

namespace CadastroPessoas.Application.UseCases.Pessoa.PesquisarPessoa;

public sealed class PesquisarPessoaUseCase(IPessoaRepository repositorio)
{
    public async Task<IReadOnlyList<PessoaEntity>> ExecutarAsync(string? nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ValidacaoException([ErrosDePessoa.NomeDaPesquisaObrigatorio]);
        }

        return await repositorio.PesquisarPorNomeAsync(nome);
    }
}
