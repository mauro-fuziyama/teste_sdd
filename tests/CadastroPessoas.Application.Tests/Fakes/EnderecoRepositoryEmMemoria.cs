using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Interfaces;
using CadastroPessoas.Domain.ValueObjects;

namespace CadastroPessoas.Application.Tests.Fakes;

public sealed class EnderecoRepositoryEmMemoria : IEnderecoRepository
{
    private readonly Dictionary<int, Endereco> enderecosPorId = [];
    private int ultimoId;

    public int QuantidadeDeRegistros => enderecosPorId.Count;

    public Task<int> InserirAsync(Endereco endereco)
    {
        ultimoId++;
        enderecosPorId[ultimoId] = CopiarComId(endereco, ultimoId);

        return Task.FromResult(ultimoId);
    }

    public Task<Endereco?> ObterPorIdAsync(int id)
    {
        var endereco = enderecosPorId.GetValueOrDefault(id);

        return Task.FromResult(endereco is null ? null : CopiarComId(endereco, endereco.Id));
    }

    public Task<IReadOnlyList<Endereco>> PesquisarPorPessoaIdAsync(int pessoaId)
    {
        IReadOnlyList<Endereco> enderecos = enderecosPorId.Values
            .Where(registro => registro.PessoaId == pessoaId)
            .OrderBy(registro => registro.Id)
            .Select(registro => CopiarComId(registro, registro.Id))
            .ToList();

        return Task.FromResult(enderecos);
    }

    public Task AtualizarAsync(Endereco endereco)
    {
        if (enderecosPorId.ContainsKey(endereco.Id))
        {
            enderecosPorId[endereco.Id] = CopiarComId(endereco, endereco.Id);
        }

        return Task.CompletedTask;
    }

    public Task ExcluirAsync(int id)
    {
        enderecosPorId.Remove(id);

        return Task.CompletedTask;
    }

    private static Endereco CopiarComId(Endereco endereco, int id)
    {
        var dados = new DadosDoEndereco(
            (int)endereco.Tipo,
            endereco.Logradouro,
            endereco.Numero,
            endereco.Complemento,
            endereco.Bairro,
            endereco.Municipio,
            endereco.Estado.Valor,
            endereco.Cep?.Valor);

        return Endereco.Reconstituir(id, endereco.PessoaId, dados);
    }
}
