using CadastroPessoas.Application.Interfaces;
using CadastroPessoas.Application.UseCases.Pessoa.AtualizarPessoa;
using CadastroPessoas.Application.UseCases.Pessoa.ConsultarPessoa;
using CadastroPessoas.Application.UseCases.Pessoa.CriarPessoa;
using CadastroPessoas.Application.UseCases.Pessoa.ExcluirPessoa;
using CadastroPessoas.Application.UseCases.Pessoa.PesquisarPessoa;
using CadastroPessoas.Domain.Entities;

namespace CadastroPessoas.Application.Services;

public sealed class PessoaService(
    CriarPessoaUseCase criarPessoa,
    ConsultarPessoaUseCase consultarPessoa,
    PesquisarPessoaUseCase pesquisarPessoa,
    AtualizarPessoaUseCase atualizarPessoa,
    ExcluirPessoaUseCase excluirPessoa) : IPessoaService
{
    public Task<int> CriarAsync(CriarPessoaInput dados)
    {
        return criarPessoa.ExecutarAsync(dados);
    }

    public Task<Pessoa?> ConsultarPorIdAsync(int id)
    {
        return consultarPessoa.ExecutarPorIdAsync(id);
    }

    public Task<Pessoa?> ConsultarPorCpfAsync(string? cpf)
    {
        return consultarPessoa.ExecutarPorCpfAsync(cpf);
    }

    public Task<IReadOnlyList<Pessoa>> PesquisarPorNomeAsync(string? nome)
    {
        return pesquisarPessoa.ExecutarAsync(nome);
    }

    public Task AtualizarAsync(int id, AtualizarPessoaInput dados)
    {
        return atualizarPessoa.ExecutarAsync(id, dados);
    }

    public Task ExcluirAsync(int id)
    {
        return excluirPessoa.ExecutarAsync(id);
    }
}
