using CadastroPessoas.Application.UseCases.Pessoa.AtualizarPessoa;
using CadastroPessoas.Application.UseCases.Pessoa.CriarPessoa;
using CadastroPessoas.Domain.Entities;

namespace CadastroPessoas.Application.Interfaces;

public interface IPessoaService
{
    Task<int> CriarAsync(CriarPessoaInput dados);
    Task<Pessoa?> ConsultarPorIdAsync(int id);
    Task<Pessoa?> ConsultarPorCpfAsync(string? cpf);
    Task<IReadOnlyList<Pessoa>> PesquisarPorNomeAsync(string? nome);
    Task AtualizarAsync(int id, AtualizarPessoaInput dados);
    Task ExcluirAsync(int id);
}
