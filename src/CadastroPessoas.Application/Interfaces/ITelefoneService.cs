using CadastroPessoas.Application.UseCases.Telefone.AtualizarTelefone;
using CadastroPessoas.Application.UseCases.Telefone.CriarTelefone;
using CadastroPessoas.Domain.Entities;

namespace CadastroPessoas.Application.Interfaces;

public interface ITelefoneService
{
    Task<int> CriarAsync(CriarTelefoneInput dados);
    Task<IReadOnlyList<Telefone>> PesquisarPorPessoaIdAsync(int pessoaId);
    Task AtualizarAsync(int id, AtualizarTelefoneInput dados);
    Task ExcluirAsync(int id);
}
