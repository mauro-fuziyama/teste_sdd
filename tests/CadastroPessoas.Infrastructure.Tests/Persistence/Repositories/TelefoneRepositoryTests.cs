using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.ValueObjects;
using CadastroPessoas.Infrastructure.Persistence;
using Dapper;

namespace CadastroPessoas.Infrastructure.Tests.Persistence.Repositories;

[Collection(ColecaoDoBancoDeTeste.Nome)]
public sealed class TelefoneRepositoryTests(BancoDeTesteFixture banco)
{
    private const string Celular = "11987654321";
    private const string Fixo = "1134567890";
    private const int PessoaIdInexistente = -1;
    private static readonly DateTime MomentoDeCriacao = new(2026, 1, 15, 10, 30, 45);

    [Fact]
    public async Task InserirAsync_GravaOsCamposERetornaIdGerado()
    {
        var pessoaId = await CriarPessoaAsync();

        var id = await banco.TelefoneRepositorio.InserirAsync(Telefone.Criar(pessoaId, Celular));

        var gravado = await banco.TelefoneRepositorio.ObterPorIdAsync(id);
        Assert.True(id > 0);
        Assert.Equal(id, gravado!.Id);
        Assert.Equal(pessoaId, gravado.PessoaId);
        Assert.Equal(Celular, gravado.Numero.Valor);
    }

    [Fact]
    public async Task InserirAsync_ComPessoaInexistente_LancaErroDePessoaInexistente()
    {
        var excecao = await Assert.ThrowsAsync<ValidacaoException>(
            () => banco.TelefoneRepositorio.InserirAsync(Telefone.Criar(PessoaIdInexistente, Celular)));

        Assert.Equal([ErrosDeTelefone.PessoaInexistente], excecao.Erros);
    }

    [Fact]
    public async Task ObterPorIdAsync_SemRegistro_RetornaNulo()
    {
        Assert.Null(await banco.TelefoneRepositorio.ObterPorIdAsync(-1));
    }

    [Fact]
    public async Task PesquisarPorPessoaIdAsync_RetornaSomenteOsTelefonesDaPessoaOrdenadosPeloId()
    {
        var pessoaId = await CriarPessoaAsync();
        var outraPessoaId = await CriarPessoaAsync();
        var primeiroId = await banco.TelefoneRepositorio.InserirAsync(Telefone.Criar(pessoaId, Fixo));
        await banco.TelefoneRepositorio.InserirAsync(Telefone.Criar(outraPessoaId, Fixo));
        var segundoId = await banco.TelefoneRepositorio.InserirAsync(Telefone.Criar(pessoaId, Celular));

        var telefones = await banco.TelefoneRepositorio.PesquisarPorPessoaIdAsync(pessoaId);

        Assert.Equal([primeiroId, segundoId], telefones.Select(telefone => telefone.Id));
    }

    [Fact]
    public async Task PesquisarPorPessoaIdAsync_SemTelefones_RetornaListaVazia()
    {
        var pessoaId = await CriarPessoaAsync();

        Assert.Empty(await banco.TelefoneRepositorio.PesquisarPorPessoaIdAsync(pessoaId));
    }

    [Fact]
    public async Task NumeroJaCadastradoParaPessoaAsync_ConsideraPessoaENumero()
    {
        var pessoaId = await CriarPessoaAsync();
        var outraPessoaId = await CriarPessoaAsync();
        await banco.TelefoneRepositorio.InserirAsync(Telefone.Criar(pessoaId, Celular));

        Assert.True(await banco.TelefoneRepositorio.NumeroJaCadastradoParaPessoaAsync(pessoaId, new NumeroDeTelefone(Celular)));
        Assert.False(await banco.TelefoneRepositorio.NumeroJaCadastradoParaPessoaAsync(pessoaId, new NumeroDeTelefone(Fixo)));
        Assert.False(await banco.TelefoneRepositorio.NumeroJaCadastradoParaPessoaAsync(outraPessoaId, new NumeroDeTelefone(Celular)));
    }

    [Fact]
    public async Task NumeroJaCadastradoParaPessoaAsync_IgnoraOTelefoneInformado()
    {
        var pessoaId = await CriarPessoaAsync();
        var id = await banco.TelefoneRepositorio.InserirAsync(Telefone.Criar(pessoaId, Celular));

        var cadastrado = await banco.TelefoneRepositorio.NumeroJaCadastradoParaPessoaAsync(
            pessoaId,
            new NumeroDeTelefone(Celular),
            idDoTelefoneIgnorado: id);

        Assert.False(cadastrado);
    }

    [Fact]
    public async Task AtualizarAsync_AlteraSomenteONumeroDoTelefone()
    {
        var pessoaId = await CriarPessoaAsync();
        var idDoOutroTelefone = await banco.TelefoneRepositorio.InserirAsync(Telefone.Criar(pessoaId, Fixo));
        var id = await banco.TelefoneRepositorio.InserirAsync(Telefone.Criar(pessoaId, Celular));
        var telefone = await banco.TelefoneRepositorio.ObterPorIdAsync(id);

        telefone!.AtualizarNumero("21987654321");
        await banco.TelefoneRepositorio.AtualizarAsync(telefone);

        var atualizado = await banco.TelefoneRepositorio.ObterPorIdAsync(id);
        var outroTelefone = await banco.TelefoneRepositorio.ObterPorIdAsync(idDoOutroTelefone);
        Assert.Equal("21987654321", atualizado!.Numero.Valor);
        Assert.Equal(pessoaId, atualizado.PessoaId);
        Assert.Equal(Fixo, outroTelefone!.Numero.Valor);
    }

    [Fact]
    public async Task ExcluirAsync_RemoveFisicamenteORegistro()
    {
        var pessoaId = await CriarPessoaAsync();
        var id = await banco.TelefoneRepositorio.InserirAsync(Telefone.Criar(pessoaId, Celular));

        await banco.TelefoneRepositorio.ExcluirAsync(id);

        Assert.Null(await banco.TelefoneRepositorio.ObterPorIdAsync(id));
    }

    [Fact]
    public async Task ObterPorIdAsync_ComNumeroInvalidoNoBanco_LancaRegistroCorrompidoSemExporONumero()
    {
        var pessoaId = await CriarPessoaAsync();
        const string numeroInvalido = "12-INVALIDO";
        var id = await InserirLinhaDiretamenteAsync(pessoaId, numeroInvalido);

        var excecao = await Assert.ThrowsAsync<RegistroCorrompidoException>(
            () => banco.TelefoneRepositorio.ObterPorIdAsync(id));

        Assert.DoesNotContain(numeroInvalido, excecao.Message);
        Assert.Contains("telefone", excecao.Message);
    }

    [Fact]
    public async Task PesquisarPorPessoaIdAsync_ComNumeroInvalidoNoBanco_LancaRegistroCorrompido()
    {
        var pessoaId = await CriarPessoaAsync();
        await banco.TelefoneRepositorio.InserirAsync(Telefone.Criar(pessoaId, Celular));
        await InserirLinhaDiretamenteAsync(pessoaId, "0000");

        await Assert.ThrowsAsync<RegistroCorrompidoException>(
            () => banco.TelefoneRepositorio.PesquisarPorPessoaIdAsync(pessoaId));
    }

    private Task<int> CriarPessoaAsync()
    {
        var pessoa = Pessoa.Criar("Pessoa com telefone", banco.GerarCpfParaRemoverAoFinal(), null, MomentoDeCriacao);

        return banco.Repositorio.InserirAsync(pessoa);
    }

    private async Task<int> InserirLinhaDiretamenteAsync(int pessoaId, string numero)
    {
        await using var conexao = await banco.FonteDeDados.OpenConnectionAsync();

        return await conexao.ExecuteScalarAsync<int>(
            """
            INSERT INTO telefone (pessoaid, numero) VALUES (@PessoaId, @Numero);
            SELECT LAST_INSERT_ID();
            """,
            new { PessoaId = pessoaId, Numero = numero });
    }
}
