using CadastroPessoas.Application.UseCases.Endereco.AtualizarEndereco;
using CadastroPessoas.Application.UseCases.Endereco.CriarEndereco;
using CadastroPessoas.Domain.Entities;

namespace CadastroPessoas.Application.Interfaces;

public interface IEnderecoService
{
    Task<int> CriarAsync(CriarEnderecoInput entrada);
    Task<IReadOnlyList<Endereco>> PesquisarPorPessoaIdAsync(int pessoaId);
    Task AtualizarAsync(int id, AtualizarEnderecoInput entrada);
    Task ExcluirAsync(int id);
}
