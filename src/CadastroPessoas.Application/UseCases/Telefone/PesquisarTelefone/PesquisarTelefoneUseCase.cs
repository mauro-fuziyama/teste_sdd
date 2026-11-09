using CadastroPessoas.Domain.Interfaces;
using TelefoneEntity = CadastroPessoas.Domain.Entities.Telefone;

namespace CadastroPessoas.Application.UseCases.Telefone.PesquisarTelefone;

public sealed class PesquisarTelefoneUseCase(ITelefoneRepository telefoneRepository)
{
    public Task<IReadOnlyList<TelefoneEntity>> ExecutarAsync(int pessoaId)
    {
        return telefoneRepository.PesquisarPorPessoaIdAsync(pessoaId);
    }
}
