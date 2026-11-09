using CadastroPessoas.Domain.Interfaces;
using EnderecoEntity = CadastroPessoas.Domain.Entities.Endereco;

namespace CadastroPessoas.Application.UseCases.Endereco.PesquisarEndereco;

public sealed class PesquisarEnderecoUseCase(IEnderecoRepository enderecoRepository)
{
    public Task<IReadOnlyList<EnderecoEntity>> ExecutarAsync(int pessoaId)
    {
        return enderecoRepository.PesquisarPorPessoaIdAsync(pessoaId);
    }
}
