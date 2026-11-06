using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.ValueObjects;

namespace CadastroPessoas.Domain.Interfaces;

public interface IPessoaRepository
{
    Task<int> InserirAsync(Pessoa pessoa);
    Task<Pessoa?> ObterPorIdAsync(int id);
    Task<Pessoa?> ObterPorCpfAsync(CPF cpf);
    Task<IReadOnlyList<Pessoa>> PesquisarPorNomeAsync(string nome);
    Task AtualizarAsync(Pessoa pessoa);
    Task ExcluirAsync(int id);
}
