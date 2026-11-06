using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.Interfaces;

namespace CadastroPessoas.Application.UseCases.Telefone.ExcluirTelefone;

public sealed class ExcluirTelefoneUseCase(ITelefoneRepository telefoneRepository)
{
    public async Task ExecutarAsync(int id)
    {
        _ = await telefoneRepository.ObterPorIdAsync(id) ?? throw new TelefoneNaoEncontradoException();

        await telefoneRepository.ExcluirAsync(id);
    }
}
