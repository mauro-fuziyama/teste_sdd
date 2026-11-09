using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.Interfaces;

namespace CadastroPessoas.Application.UseCases.Endereco.ExcluirEndereco;

public sealed class ExcluirEnderecoUseCase(IEnderecoRepository enderecoRepository)
{
    public async Task ExecutarAsync(int id)
    {
        _ = await enderecoRepository.ObterPorIdAsync(id) ?? throw new EnderecoNaoEncontradoException();

        await enderecoRepository.ExcluirAsync(id);
    }
}
