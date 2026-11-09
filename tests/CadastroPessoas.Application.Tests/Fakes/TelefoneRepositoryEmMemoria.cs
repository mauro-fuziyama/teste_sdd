using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Interfaces;
using CadastroPessoas.Domain.ValueObjects;

namespace CadastroPessoas.Application.Tests.Fakes;

public sealed class TelefoneRepositoryEmMemoria : ITelefoneRepository
{
    private readonly Dictionary<int, Telefone> telefonesPorId = [];
    private int ultimoId;

    public Task<int> InserirAsync(Telefone telefone)
    {
        ultimoId++;
        telefonesPorId[ultimoId] = CopiarComId(telefone, ultimoId);

        return Task.FromResult(ultimoId);
    }

    public Task<Telefone?> ObterPorIdAsync(int id)
    {
        var telefone = telefonesPorId.GetValueOrDefault(id);

        return Task.FromResult(telefone is null ? null : CopiarComId(telefone, telefone.Id));
    }

    public Task<IReadOnlyList<Telefone>> PesquisarPorPessoaIdAsync(int pessoaId)
    {
        IReadOnlyList<Telefone> telefones = telefonesPorId.Values
            .Where(registro => registro.PessoaId == pessoaId)
            .OrderBy(registro => registro.Id)
            .Select(registro => CopiarComId(registro, registro.Id))
            .ToList();

        return Task.FromResult(telefones);
    }

    public Task<bool> NumeroJaCadastradoParaPessoaAsync(int pessoaId, NumeroDeTelefone numero, int? idDoTelefoneIgnorado = null)
    {
        var existe = telefonesPorId.Values.Any(registro =>
            registro.PessoaId == pessoaId
            && registro.Numero == numero
            && registro.Id != idDoTelefoneIgnorado);

        return Task.FromResult(existe);
    }

    public Task AtualizarAsync(Telefone telefone)
    {
        if (telefonesPorId.ContainsKey(telefone.Id))
        {
            telefonesPorId[telefone.Id] = CopiarComId(telefone, telefone.Id);
        }

        return Task.CompletedTask;
    }

    public Task ExcluirAsync(int id)
    {
        telefonesPorId.Remove(id);

        return Task.CompletedTask;
    }

    private static Telefone CopiarComId(Telefone telefone, int id)
    {
        return Telefone.Reconstituir(id, telefone.PessoaId, telefone.Numero);
    }
}
