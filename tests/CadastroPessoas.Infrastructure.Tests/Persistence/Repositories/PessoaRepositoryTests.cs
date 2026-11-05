using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.ValueObjects;
using CadastroPessoas.Infrastructure.Persistence;
using Dapper;

namespace CadastroPessoas.Infrastructure.Tests.Persistence.Repositories;

[Collection(ColecaoDoBancoDeTeste.Nome)]
public sealed class PessoaRepositoryTests(BancoDeTesteFixture banco)
{
    private static readonly DateTime MomentoDeCriacao = new(2026, 1, 15, 10, 30, 45);
    private static readonly DateOnly DataDeNascimento = new(1990, 5, 20);

    [Fact]
    public async Task InserirAsync_GravaTodosOsCamposERetornaIdGerado()
    {
        var cpf = banco.GerarCpfParaRemoverAoFinal();

        var id = await banco.Repositorio.InserirAsync(CriarPessoa("Maria da Silva", cpf));

        var gravada = await banco.Repositorio.ObterPorIdAsync(id);
        Assert.True(id > 0);
        Assert.NotNull(gravada);
        Assert.Equal(id, gravada.Id);
        Assert.Equal("Maria da Silva", gravada.Nome);
        Assert.Equal(cpf, gravada.Cpf.Valor);
        Assert.Equal(DataDeNascimento, gravada.DataDeNascimento);
        Assert.Equal(MomentoDeCriacao, gravada.DataDeCadastro);
        Assert.Equal(MomentoDeCriacao, gravada.DataAtualizacao);
    }

    [Fact]
    public async Task InserirAsync_SemDataDeNascimento_GravaNulo()
    {
        var pessoa = Pessoa.Criar("Pessoa sem data", banco.GerarCpfParaRemoverAoFinal(), null, MomentoDeCriacao);

        var id = await banco.Repositorio.InserirAsync(pessoa);

        var gravada = await banco.Repositorio.ObterPorIdAsync(id);
        Assert.Null(gravada!.DataDeNascimento);
    }

    [Fact]
    public async Task InserirAsync_ComCpfDuplicado_LancaValidacaoExceptionComErroDeCpfCadastrado()
    {
        var cpf = banco.GerarCpfParaRemoverAoFinal();
        await banco.Repositorio.InserirAsync(CriarPessoa("Primeira pessoa", cpf));

        var excecao = await Assert.ThrowsAsync<ValidacaoException>(
            () => banco.Repositorio.InserirAsync(CriarPessoa("Segunda pessoa", cpf)));

        Assert.Equal([ErrosDePessoa.CpfJaCadastrado], excecao.Erros);
    }

    [Fact]
    public async Task ObterPorIdAsync_SemRegistro_RetornaNulo()
    {
        var pessoa = await banco.Repositorio.ObterPorIdAsync(-1);

        Assert.Null(pessoa);
    }

    [Fact]
    public async Task ObterPorCpfAsync_RetornaRegistroComCpfExato()
    {
        var cpf = banco.GerarCpfParaRemoverAoFinal();
        var id = await banco.Repositorio.InserirAsync(CriarPessoa("Busca por CPF", cpf));

        var pessoa = await banco.Repositorio.ObterPorCpfAsync(new CPF(cpf));

        Assert.Equal(id, pessoa!.Id);
    }

    [Fact]
    public async Task ObterPorCpfAsync_SemRegistro_RetornaNulo()
    {
        var pessoa = await banco.Repositorio.ObterPorCpfAsync(new CPF(banco.GerarCpfParaRemoverAoFinal()));

        Assert.Null(pessoa);
    }

    [Fact]
    public async Task PesquisarPorNomeAsync_UsaContem_RetornaTodosOsRegistrosQueContemOValor()
    {
        var marcador = CriarMarcadorUnico();
        var idDoInicio = await InserirComNomeAsync($"{marcador} no início");
        var idDoMeio = await InserirComNomeAsync($"Nome {marcador} no meio");
        var idDoFim = await InserirComNomeAsync($"Nome no fim {marcador}");

        var pessoas = await banco.Repositorio.PesquisarPorNomeAsync(marcador);

        Assert.Equal(
            new[] { idDoInicio, idDoMeio, idDoFim }.Order(),
            pessoas.Select(pessoa => pessoa.Id).Order());
    }

    [Fact]
    public async Task PesquisarPorNomeAsync_SemCorrespondencia_RetornaListaVazia()
    {
        var pessoas = await banco.Repositorio.PesquisarPorNomeAsync(CriarMarcadorUnico());

        Assert.Empty(pessoas);
    }

    [Theory]
    [InlineData("100%", "100x")]
    [InlineData("a_b", "axb")]
    [InlineData("a!b", "ab")]
    public async Task PesquisarPorNomeAsync_TrataCaracteresEspeciaisComoTexto(string valorEspecial, string valorParecido)
    {
        var marcador = CriarMarcadorUnico();
        var idEsperado = await InserirComNomeAsync($"{marcador} {valorEspecial}");
        await InserirComNomeAsync($"{marcador} {valorParecido}");

        var pessoas = await banco.Repositorio.PesquisarPorNomeAsync($"{marcador} {valorEspecial}");

        Assert.Equal(idEsperado, Assert.Single(pessoas).Id);
    }

    [Fact]
    public async Task PesquisarPorNomeAsync_ComTentativaDeInjecaoDeSql_NaoRetornaNemAlteraRegistros()
    {
        var marcador = CriarMarcadorUnico();
        var id = await InserirComNomeAsync($"{marcador} preservado");

        var pessoas = await banco.Repositorio.PesquisarPorNomeAsync($"{marcador}' OR '1'='1'; DELETE FROM pessoa; -- ");

        Assert.Empty(pessoas);
        Assert.NotNull(await banco.Repositorio.ObterPorIdAsync(id));
    }

    [Fact]
    public async Task AtualizarAsync_AlteraDadosSemAlterarCpfNemDataDeCadastro()
    {
        var cpf = banco.GerarCpfParaRemoverAoFinal();
        var id = await banco.Repositorio.InserirAsync(CriarPessoa("Nome anterior", cpf));
        var pessoa = await banco.Repositorio.ObterPorIdAsync(id);
        var momentoDaAtualizacao = MomentoDeCriacao.AddDays(1);

        pessoa!.Atualizar("Nome novo", cpf, new DateOnly(1985, 3, 2), momentoDaAtualizacao);
        await banco.Repositorio.AtualizarAsync(pessoa);

        var atualizada = await banco.Repositorio.ObterPorIdAsync(id);
        Assert.Equal("Nome novo", atualizada!.Nome);
        Assert.Equal(new DateOnly(1985, 3, 2), atualizada.DataDeNascimento);
        Assert.Equal(momentoDaAtualizacao, atualizada.DataAtualizacao);
        Assert.Equal(MomentoDeCriacao, atualizada.DataDeCadastro);
        Assert.Equal(cpf, atualizada.Cpf.Valor);
    }

    [Fact]
    public async Task AtualizarAsync_NaoAlteraOutrosRegistros()
    {
        var idDeOutraPessoa = await InserirComNomeAsync("Outra pessoa");
        var cpf = banco.GerarCpfParaRemoverAoFinal();
        var id = await banco.Repositorio.InserirAsync(CriarPessoa("Pessoa atualizada", cpf));
        var pessoa = await banco.Repositorio.ObterPorIdAsync(id);

        pessoa!.Atualizar("Nome novo", cpf, null, MomentoDeCriacao.AddDays(1));
        await banco.Repositorio.AtualizarAsync(pessoa);

        var outraPessoa = await banco.Repositorio.ObterPorIdAsync(idDeOutraPessoa);
        Assert.Equal("Outra pessoa", outraPessoa!.Nome);
    }

    [Fact]
    public async Task ExcluirAsync_RemoveFisicamenteORegistro()
    {
        var cpf = banco.GerarCpfParaRemoverAoFinal();
        var id = await banco.Repositorio.InserirAsync(CriarPessoa("Pessoa excluída", cpf));

        await banco.Repositorio.ExcluirAsync(id);

        Assert.Equal(0, await ContarRegistrosComCpfAsync(cpf));
    }

    [Fact]
    public async Task ObterPorIdAsync_ComCpfInvalidoNoBanco_LancaRegistroCorrompidoSemExporCpf()
    {
        var cpfInvalido = banco.RegistrarCpfParaRemoverAoFinal(CriarCpfInvalidoUnico());
        var id = await InserirLinhaDiretamenteAsync("Linha corrompida", cpfInvalido);

        var excecao = await Assert.ThrowsAsync<RegistroCorrompidoException>(
            () => banco.Repositorio.ObterPorIdAsync(id));

        Assert.DoesNotContain(cpfInvalido, excecao.Message);
    }

    [Fact]
    public async Task PesquisarPorNomeAsync_ComCpfInvalidoNoBanco_LancaRegistroCorrompido()
    {
        var marcador = CriarMarcadorUnico();
        await InserirLinhaDiretamenteAsync(marcador, banco.RegistrarCpfParaRemoverAoFinal(CriarCpfInvalidoUnico()));

        await Assert.ThrowsAsync<RegistroCorrompidoException>(
            () => banco.Repositorio.PesquisarPorNomeAsync(marcador));
    }

    private static Pessoa CriarPessoa(string nome, string cpf)
    {
        return Pessoa.Criar(nome, cpf, DataDeNascimento, MomentoDeCriacao);
    }

    private static string CriarMarcadorUnico()
    {
        return $"teste{Guid.NewGuid():N}";
    }

    private static string CriarCpfInvalidoUnico()
    {
        return $"INV{Random.Shared.NextInt64(10_000_000_000, 99_999_999_999)}";
    }

    private Task<int> InserirComNomeAsync(string nome)
    {
        return banco.Repositorio.InserirAsync(CriarPessoa(nome, banco.GerarCpfParaRemoverAoFinal()));
    }

    private async Task<int> InserirLinhaDiretamenteAsync(string nome, string cpf)
    {
        await using var conexao = await banco.FonteDeDados.OpenConnectionAsync();

        return await conexao.ExecuteScalarAsync<int>(
            """
            INSERT INTO pessoa (nome, cpf, datadenascimento, datadecadastro, dataatualizacao)
            VALUES (@Nome, @Cpf, NULL, @Momento, @Momento);
            SELECT LAST_INSERT_ID();
            """,
            new { Nome = nome, Cpf = cpf, Momento = MomentoDeCriacao });
    }

    private async Task<int> ContarRegistrosComCpfAsync(string cpf)
    {
        await using var conexao = await banco.FonteDeDados.OpenConnectionAsync();

        return await conexao.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM pessoa WHERE cpf = @Cpf", new { Cpf = cpf });
    }
}
