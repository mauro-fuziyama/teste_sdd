using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Interfaces;
using CadastroPessoas.Domain.ValueObjects;

namespace CadastroPessoas.Application.Tests.Fakes;

public sealed class PessoaRepositoryEmMemoria : IPessoaRepository
{
    private readonly Dictionary<int, Pessoa> _pessoasPorId = [];
    private int _ultimoId;

    public int QuantidadeDeConsultasPorId { get; private set; }

    public Task<int> InserirAsync(Pessoa pessoa)
    {
        _ultimoId++;
        _pessoasPorId[_ultimoId] = CopiarComId(pessoa, _ultimoId);

        return Task.FromResult(_ultimoId);
    }

    public Task<Pessoa?> ObterPorIdAsync(int id)
    {
        QuantidadeDeConsultasPorId++;
        var pessoa = _pessoasPorId.GetValueOrDefault(id);

        return Task.FromResult(pessoa is null ? null : CopiarComId(pessoa, pessoa.Id));
    }

    public Task<Pessoa?> ObterPorCpfAsync(CPF cpf)
    {
        var pessoa = _pessoasPorId.Values.SingleOrDefault(registro => registro.Cpf == cpf);

        return Task.FromResult(pessoa is null ? null : CopiarComId(pessoa, pessoa.Id));
    }

    public Task<IReadOnlyList<Pessoa>> PesquisarPorNomeAsync(string nome)
    {
        IReadOnlyList<Pessoa> pessoas = _pessoasPorId.Values
            .Where(registro => registro.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase))
            .Select(registro => CopiarComId(registro, registro.Id))
            .ToList();

        return Task.FromResult(pessoas);
    }

    public Task AtualizarAsync(Pessoa pessoa)
    {
        _pessoasPorId[pessoa.Id] = CopiarComId(pessoa, pessoa.Id);

        return Task.CompletedTask;
    }

    public Task ExcluirAsync(int id)
    {
        _pessoasPorId.Remove(id);

        return Task.CompletedTask;
    }

    private static Pessoa CopiarComId(Pessoa pessoa, int id)
    {
        return Pessoa.Reconstituir(
            id,
            pessoa.Nome,
            pessoa.Cpf,
            pessoa.DataDeNascimento,
            pessoa.DataDeCadastro,
            pessoa.DataAtualizacao);
    }
}
