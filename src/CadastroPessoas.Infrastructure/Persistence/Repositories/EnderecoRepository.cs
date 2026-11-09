using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.Interfaces;
using Dapper;
using MySqlConnector;

namespace CadastroPessoas.Infrastructure.Persistence.Repositories;

public sealed class EnderecoRepository(MySqlDataSource fonteDeDados) : IEnderecoRepository
{
    private const string ColunasDoEndereco = "id, pessoaid, tipo, logradouro, numero, complemento, bairro, municipio, estado, cep";

    private const string SqlInserir = """
        INSERT INTO endereco (pessoaid, tipo, logradouro, numero, complemento, bairro, municipio, estado, cep)
        VALUES (@PessoaId, @Tipo, @Logradouro, @Numero, @Complemento, @Bairro, @Municipio, @Estado, @Cep);
        SELECT LAST_INSERT_ID();
        """;

    private const string SqlObterPorId = $"SELECT {ColunasDoEndereco} FROM endereco WHERE id = @Id";

    private const string SqlPesquisarPorPessoaId = $"SELECT {ColunasDoEndereco} FROM endereco WHERE pessoaid = @PessoaId ORDER BY id";

    private const string SqlAtualizar = """
        UPDATE endereco
        SET tipo = @Tipo,
            logradouro = @Logradouro,
            numero = @Numero,
            complemento = @Complemento,
            bairro = @Bairro,
            municipio = @Municipio,
            estado = @Estado,
            cep = @Cep
        WHERE id = @Id
        """;

    private const string SqlExcluir = "DELETE FROM endereco WHERE id = @Id";

    public async Task<int> InserirAsync(Endereco endereco)
    {
        await using var conexao = await fonteDeDados.OpenConnectionAsync();

        try
        {
            return await conexao.ExecuteScalarAsync<int>(SqlInserir, ObterParametros(endereco));
        }
        catch (MySqlException excecao) when (EhViolacaoDaChaveDaPessoa(excecao))
        {
            throw new ValidacaoException([ErrosDeEndereco.PessoaInexistente]);
        }
    }

    public async Task<Endereco?> ObterPorIdAsync(int id)
    {
        await using var conexao = await fonteDeDados.OpenConnectionAsync();
        var registro = await conexao.QuerySingleOrDefaultAsync<EnderecoRegistro>(SqlObterPorId, new { Id = id });

        return registro?.ParaEntidade();
    }

    public async Task<IReadOnlyList<Endereco>> PesquisarPorPessoaIdAsync(int pessoaId)
    {
        await using var conexao = await fonteDeDados.OpenConnectionAsync();
        var registros = await conexao.QueryAsync<EnderecoRegistro>(SqlPesquisarPorPessoaId, new { PessoaId = pessoaId });

        return registros.Select(registro => registro.ParaEntidade()).ToList();
    }

    public async Task AtualizarAsync(Endereco endereco)
    {
        await using var conexao = await fonteDeDados.OpenConnectionAsync();
        await conexao.ExecuteAsync(SqlAtualizar, ObterParametros(endereco));
    }

    public async Task ExcluirAsync(int id)
    {
        await using var conexao = await fonteDeDados.OpenConnectionAsync();
        await conexao.ExecuteAsync(SqlExcluir, new { Id = id });
    }

    private static object ObterParametros(Endereco endereco)
    {
        return new
        {
            endereco.Id,
            endereco.PessoaId,
            Tipo = (int)endereco.Tipo,
            endereco.Logradouro,
            endereco.Numero,
            endereco.Complemento,
            endereco.Bairro,
            endereco.Municipio,
            Estado = endereco.Estado.Valor,
            Cep = endereco.Cep?.Valor
        };
    }

    private static bool EhViolacaoDaChaveDaPessoa(MySqlException excecao)
    {
        return excecao.ErrorCode is MySqlErrorCode.NoReferencedRow or MySqlErrorCode.NoReferencedRow2;
    }
}
