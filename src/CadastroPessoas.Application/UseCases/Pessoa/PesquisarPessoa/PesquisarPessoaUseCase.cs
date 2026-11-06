using CadastroPessoas.Domain.Interfaces;
using PessoaEntity = CadastroPessoas.Domain.Entities.Pessoa;

namespace CadastroPessoas.Application.UseCases.Pessoa.PesquisarPessoa;

public sealed class PesquisarPessoaUseCase(IPessoaRepository repositorio)
{
    public Task<IReadOnlyList<PessoaEntity>> ExecutarAsync(string nome)
    {
        return repositorio.PesquisarPorNomeAsync(nome);
    }
}
