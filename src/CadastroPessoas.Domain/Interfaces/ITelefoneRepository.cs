using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.ValueObjects;

namespace CadastroPessoas.Domain.Interfaces;

public interface ITelefoneRepository
{
    Task<int> InserirAsync(Telefone telefone);
    Task<Telefone?> ObterPorIdAsync(int id);
    Task<IReadOnlyList<Telefone>> PesquisarPorPessoaIdAsync(int pessoaId);
    Task<bool> NumeroJaCadastradoParaPessoaAsync(int pessoaId, NumeroDeTelefone numero, int? idDoTelefoneIgnorado = null);
    Task AtualizarAsync(Telefone telefone);
    Task ExcluirAsync(int id);
}
