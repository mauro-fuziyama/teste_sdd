using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.Interfaces;
using CadastroPessoas.Domain.ValueObjects;
using Dapper;
using MySqlConnector;

namespace CadastroPessoas.Infrastructure.Persistence.Repositories;

public sealed class TelefoneRepository(MySqlDataSource fonteDeDados) : ITelefoneRepository
{
    private const string ColunasDoTelefone = "id, pessoaid, numero";

    private const string SqlInserir = """
        INSERT INTO telefone (pessoaid, numero)
        VALUES (@PessoaId, @Numero);
        SELECT LAST_INSERT_ID();
        """;

    private const string SqlObterPorId = $"SELECT {ColunasDoTelefone} FROM telefone WHERE id = @Id";

    private const string SqlPesquisarPorPessoaId = $"SELECT {ColunasDoTelefone} FROM telefone WHERE pessoaid = @PessoaId ORDER BY id";

    private const string SqlNumeroJaCadastradoParaPessoa = """
        SELECT EXISTS (
            SELECT 1 FROM telefone
            WHERE pessoaid = @PessoaId
              AND numero = @Numero
              AND (@IdDoTelefoneIgnorado IS NULL OR id <> @IdDoTelefoneIgnorado))
        """;

    private const string SqlAtualizar = "UPDATE telefone SET numero = @Numero WHERE id = @Id";

    private const string SqlExcluir = "DELETE FROM telefone WHERE id = @Id";

    public async Task<int> InserirAsync(Telefone telefone)
    {
        await using var conexao = await fonteDeDados.OpenConnectionAsync();

        try
        {
            return await conexao.ExecuteScalarAsync<int>(SqlInserir, ObterParametros(telefone));
        }
        catch (MySqlException excecao) when (EhViolacaoDaChaveDaPessoa(excecao))
        {
            throw new ValidacaoException([ErrosDeTelefone.PessoaInexistente]);
        }
    }

    public async Task<Telefone?> ObterPorIdAsync(int id)
    {
        await using var conexao = await fonteDeDados.OpenConnectionAsync();
        var registro = await conexao.QuerySingleOrDefaultAsync<TelefoneRegistro>(SqlObterPorId, new { Id = id });

        return registro?.ParaEntidade();
    }

    public async Task<IReadOnlyList<Telefone>> PesquisarPorPessoaIdAsync(int pessoaId)
    {
        await using var conexao = await fonteDeDados.OpenConnectionAsync();
        var registros = await conexao.QueryAsync<TelefoneRegistro>(SqlPesquisarPorPessoaId, new { PessoaId = pessoaId });

        return registros.Select(registro => registro.ParaEntidade()).ToList();
    }

    public async Task<bool> NumeroJaCadastradoParaPessoaAsync(int pessoaId, NumeroDeTelefone numero, int? idDoTelefoneIgnorado = null)
    {
        await using var conexao = await fonteDeDados.OpenConnectionAsync();

        return await conexao.ExecuteScalarAsync<bool>(
            SqlNumeroJaCadastradoParaPessoa,
            new { PessoaId = pessoaId, Numero = numero.Valor, IdDoTelefoneIgnorado = idDoTelefoneIgnorado });
    }

    public async Task AtualizarAsync(Telefone telefone)
    {
        await using var conexao = await fonteDeDados.OpenConnectionAsync();
        await conexao.ExecuteAsync(SqlAtualizar, ObterParametros(telefone));
    }

    public async Task ExcluirAsync(int id)
    {
        await using var conexao = await fonteDeDados.OpenConnectionAsync();
        await conexao.ExecuteAsync(SqlExcluir, new { Id = id });
    }

    private static object ObterParametros(Telefone telefone)
    {
        return new { telefone.Id, telefone.PessoaId, Numero = telefone.Numero.Valor };
    }

    private static bool EhViolacaoDaChaveDaPessoa(MySqlException excecao)
    {
        return excecao.ErrorCode is MySqlErrorCode.NoReferencedRow or MySqlErrorCode.NoReferencedRow2;
    }
}
