using CadastroPessoas.Application.Interfaces;
using CadastroPessoas.Application.UseCases.Telefone.AtualizarTelefone;
using CadastroPessoas.Application.UseCases.Telefone.CriarTelefone;
using CadastroPessoas.Application.UseCases.Telefone.ExcluirTelefone;
using CadastroPessoas.Application.UseCases.Telefone.PesquisarTelefone;
using CadastroPessoas.Domain.Entities;

namespace CadastroPessoas.Application.Services;

public sealed class TelefoneService(
    CriarTelefoneUseCase criarTelefone,
    PesquisarTelefoneUseCase pesquisarTelefone,
    AtualizarTelefoneUseCase atualizarTelefone,
    ExcluirTelefoneUseCase excluirTelefone) : ITelefoneService
{
    public Task<int> CriarAsync(CriarTelefoneInput dados)
    {
        return criarTelefone.ExecutarAsync(dados);
    }

    public Task<IReadOnlyList<Telefone>> PesquisarPorPessoaIdAsync(int pessoaId)
    {
        return pesquisarTelefone.ExecutarAsync(pessoaId);
    }

    public Task AtualizarAsync(int id, AtualizarTelefoneInput dados)
    {
        return atualizarTelefone.ExecutarAsync(id, dados);
    }

    public Task ExcluirAsync(int id)
    {
        return excluirTelefone.ExecutarAsync(id);
    }
}
