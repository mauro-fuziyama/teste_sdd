using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.Interfaces;

namespace CadastroPessoas.Application.UseCases.Endereco.AtualizarEndereco;

public sealed class AtualizarEnderecoUseCase(IEnderecoRepository enderecoRepository)
{
    public async Task ExecutarAsync(int id, AtualizarEnderecoInput entrada)
    {
        var endereco = await enderecoRepository.ObterPorIdAsync(id) ?? throw new EnderecoNaoEncontradoException();

        endereco.Atualizar(entrada.Dados);

        await enderecoRepository.AtualizarAsync(endereco);
    }
}
