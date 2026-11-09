using CadastroPessoas.Application.Interfaces;
using CadastroPessoas.Application.UseCases.Endereco.AtualizarEndereco;
using CadastroPessoas.Application.UseCases.Endereco.CriarEndereco;
using CadastroPessoas.Application.UseCases.Endereco.ExcluirEndereco;
using CadastroPessoas.Application.UseCases.Endereco.PesquisarEndereco;
using CadastroPessoas.Domain.Entities;

namespace CadastroPessoas.Application.Services;

public sealed class EnderecoService(
    CriarEnderecoUseCase criarEndereco,
    PesquisarEnderecoUseCase pesquisarEndereco,
    AtualizarEnderecoUseCase atualizarEndereco,
    ExcluirEnderecoUseCase excluirEndereco) : IEnderecoService
{
    public Task<int> CriarAsync(CriarEnderecoInput entrada)
    {
        return criarEndereco.ExecutarAsync(entrada);
    }

    public Task<IReadOnlyList<Endereco>> PesquisarPorPessoaIdAsync(int pessoaId)
    {
        return pesquisarEndereco.ExecutarAsync(pessoaId);
    }

    public Task AtualizarAsync(int id, AtualizarEnderecoInput entrada)
    {
        return atualizarEndereco.ExecutarAsync(id, entrada);
    }

    public Task ExcluirAsync(int id)
    {
        return excluirEndereco.ExecutarAsync(id);
    }
}
