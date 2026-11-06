using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.Interfaces;
using CadastroPessoas.Domain.ValueObjects;
using Dapper;
using MySqlConnector;

namespace CadastroPessoas.Infrastructure.Persistence.Repositories;

public sealed class PessoaRepository(MySqlDataSource fonteDeDados) : IPessoaRepository
{
    private const char CaractereDeEscape = '!';

    private const string ColunasDaPessoa = "id, nome, cpf, datadenascimento, datadecadastro, dataatualizacao";

    private const string SqlInserir = """
        INSERT INTO pessoa (nome, cpf, datadenascimento, datadecadastro, dataatualizacao)
        VALUES (@Nome, @Cpf, @DataDeNascimento, @DataDeCadastro, @DataAtualizacao);
        SELECT LAST_INSERT_ID();
        """;

    private const string SqlObterPorId = $"SELECT {ColunasDaPessoa} FROM pessoa WHERE id = @Id";

    private const string SqlObterPorCpf = $"SELECT {ColunasDaPessoa} FROM pessoa WHERE cpf = @Cpf";

    private const string SqlPesquisarPorNome = $"SELECT {ColunasDaPessoa} FROM pessoa WHERE nome LIKE @Padrao ESCAPE '!'";

    private const string SqlAtualizar = """
        UPDATE pessoa
        SET nome = @Nome, datadenascimento = @DataDeNascimento, dataatualizacao = @DataAtualizacao
        WHERE id = @Id
        """;

    private const string SqlExcluirTelefonesDaPessoa = "DELETE FROM telefone WHERE pessoaid = @Id";

    private const string SqlExcluir = "DELETE FROM pessoa WHERE id = @Id";

    public async Task<int> InserirAsync(Pessoa pessoa)
    {
        await using var conexao = await fonteDeDados.OpenConnectionAsync();

        try
        {
            return await conexao.ExecuteScalarAsync<int>(SqlInserir, ObterParametros(pessoa));
        }
        catch (MySqlException excecao) when (excecao.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
        {
            throw new ValidacaoException([ErrosDePessoa.CpfJaCadastrado]);
        }
    }

    public async Task<Pessoa?> ObterPorIdAsync(int id)
    {
        await using var conexao = await fonteDeDados.OpenConnectionAsync();
        var registro = await conexao.QuerySingleOrDefaultAsync<PessoaRegistro>(SqlObterPorId, new { Id = id });

        return registro?.ParaEntidade();
    }

    public async Task<Pessoa?> ObterPorCpfAsync(CPF cpf)
    {
        await using var conexao = await fonteDeDados.OpenConnectionAsync();
        var registro = await conexao.QuerySingleOrDefaultAsync<PessoaRegistro>(SqlObterPorCpf, new { Cpf = cpf.Valor });

        return registro?.ParaEntidade();
    }

    public async Task<IReadOnlyList<Pessoa>> PesquisarPorNomeAsync(string nome)
    {
        await using var conexao = await fonteDeDados.OpenConnectionAsync();
        var registros = await conexao.QueryAsync<PessoaRegistro>(SqlPesquisarPorNome, new { Padrao = CriarPadraoContem(nome) });

        return registros.Select(registro => registro.ParaEntidade()).ToList();
    }

    public async Task AtualizarAsync(Pessoa pessoa)
    {
        await using var conexao = await fonteDeDados.OpenConnectionAsync();
        await conexao.ExecuteAsync(SqlAtualizar, ObterParametros(pessoa));
    }

    public async Task ExcluirAsync(int id)
    {
        await using var conexao = await fonteDeDados.OpenConnectionAsync();
        await using var transacao = await conexao.BeginTransactionAsync();
        var parametros = new { Id = id };

        await conexao.ExecuteAsync(SqlExcluirTelefonesDaPessoa, parametros, transacao);
        await conexao.ExecuteAsync(SqlExcluir, parametros, transacao);
        await transacao.CommitAsync();
    }

    private static object ObterParametros(Pessoa pessoa)
    {
        return new
        {
            pessoa.Id,
            pessoa.Nome,
            Cpf = pessoa.Cpf.Valor,
            DataDeNascimento = pessoa.DataDeNascimento?.ToDateTime(TimeOnly.MinValue),
            pessoa.DataDeCadastro,
            pessoa.DataAtualizacao
        };
    }

    private static string CriarPadraoContem(string nome)
    {
        var nomeEscapado = nome
            .Replace($"{CaractereDeEscape}", $"{CaractereDeEscape}{CaractereDeEscape}")
            .Replace("%", $"{CaractereDeEscape}%")
            .Replace("_", $"{CaractereDeEscape}_");

        return $"%{nomeEscapado}%";
    }
}
