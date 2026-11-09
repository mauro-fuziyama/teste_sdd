using CadastroPessoas.Domain.Entities;

namespace CadastroPessoas.Domain.Interfaces;

public interface IEnderecoRepository
{
    Task<int> InserirAsync(Endereco endereco);
    Task<Endereco?> ObterPorIdAsync(int id);
    Task<IReadOnlyList<Endereco>> PesquisarPorPessoaIdAsync(int pessoaId);
    Task AtualizarAsync(Endereco endereco);
    Task ExcluirAsync(int id);
}
