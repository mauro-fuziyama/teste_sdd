using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Enums;
using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.ValueObjects;
using CadastroPessoas.Infrastructure.Persistence;
using Dapper;

namespace CadastroPessoas.Infrastructure.Tests.Persistence.Repositories;

[Collection(ColecaoDoBancoDeTeste.Nome)]
public sealed class EnderecoRepositoryTests(BancoDeTesteFixture banco)
{
    private const int PessoaIdInexistente = -1;
    private const int EnderecoIdInexistente = -1;
    private const string CelularDaPessoa = "11987654321";
    private static readonly DateTime MomentoDeCriacao = new(2026, 1, 15, 10, 30, 45);

    private static readonly DadosDoEndereco DadosCompletos = new(
        (int)TipoDeEndereco.Comercial,
        "Avenida Paulista",
        "1000",
        "Sala 12",
        "Bela Vista",
        "São Paulo",
        "sp",
        "01310-100");

    private static readonly DadosDoEndereco DadosSomenteComObrigatorios =
        new(null, null, null, null, null, "Curitiba", "PR", null);

    [Fact]
    public async Task InserirAsync_GravaOsCamposERetornaIdGerado()
    {
        var pessoaId = await CriarPessoaAsync();

        var id = await banco.EnderecoRepositorio.InserirAsync(Endereco.Criar(pessoaId, DadosCompletos));

        var gravado = await banco.EnderecoRepositorio.ObterPorIdAsync(id);
        Assert.True(id > 0);
        Assert.Equal(id, gravado!.Id);
        Assert.Equal(pessoaId, gravado.PessoaId);
        Assert.Equal(TipoDeEndereco.Comercial, gravado.Tipo);
        Assert.Equal("Avenida Paulista", gravado.Logradouro);
        Assert.Equal("1000", gravado.Numero);
        Assert.Equal("Sala 12", gravado.Complemento);
        Assert.Equal("Bela Vista", gravado.Bairro);
        Assert.Equal("São Paulo", gravado.Municipio);
    }

    [Fact]
    public async Task InserirAsync_GravaEstadoEmMaiusculasECepSemHifen()
    {
        var pessoaId = await CriarPessoaAsync();

        var id = await banco.EnderecoRepositorio.InserirAsync(Endereco.Criar(pessoaId, DadosCompletos));

        var (estado, cep) = await LerEstadoECepDoBancoAsync(id);
        Assert.Equal("SP", estado);
        Assert.Equal("01310100", cep);
    }

    [Fact]
    public async Task InserirAsync_SemOpcionais_GravaNuloNosCamposNaoInformados()
    {
        var pessoaId = await CriarPessoaAsync();

        var id = await banco.EnderecoRepositorio.InserirAsync(Endereco.Criar(pessoaId, DadosSomenteComObrigatorios));

        var gravado = await banco.EnderecoRepositorio.ObterPorIdAsync(id);
        Assert.Equal(TipoDeEndereco.NaoInformado, gravado!.Tipo);
        AssertCamposOpcionaisNulos(gravado);
    }

    [Fact]
    public async Task InserirAsync_ComPessoaInexistente_LancaErroDePessoaInexistente()
    {
        var excecao = await Assert.ThrowsAsync<ValidacaoException>(
            () => banco.EnderecoRepositorio.InserirAsync(Endereco.Criar(PessoaIdInexistente, DadosCompletos)));

        Assert.Equal([ErrosDeEndereco.PessoaInexistente], excecao.Erros);
    }

    [Fact]
    public async Task ObterPorIdAsync_SemRegistro_RetornaNulo()
    {
        Assert.Null(await banco.EnderecoRepositorio.ObterPorIdAsync(EnderecoIdInexistente));
    }

    [Fact]
    public async Task PesquisarPorPessoaIdAsync_RetornaSomenteOsEnderecosDaPessoaOrdenadosPeloId()
    {
        var pessoaId = await CriarPessoaAsync();
        var outraPessoaId = await CriarPessoaAsync();
        var primeiroId = await banco.EnderecoRepositorio.InserirAsync(Endereco.Criar(pessoaId, DadosCompletos));
        await banco.EnderecoRepositorio.InserirAsync(Endereco.Criar(outraPessoaId, DadosCompletos));
        var segundoId = await banco.EnderecoRepositorio.InserirAsync(Endereco.Criar(pessoaId, DadosSomenteComObrigatorios));

        var enderecos = await banco.EnderecoRepositorio.PesquisarPorPessoaIdAsync(pessoaId);

        Assert.Equal([primeiroId, segundoId], enderecos.Select(endereco => endereco.Id));
    }

    [Fact]
    public async Task PesquisarPorPessoaIdAsync_SemEnderecos_RetornaListaVazia()
    {
        var pessoaId = await CriarPessoaAsync();

        Assert.Empty(await banco.EnderecoRepositorio.PesquisarPorPessoaIdAsync(pessoaId));
    }

    [Fact]
    public async Task AtualizarAsync_AlteraSomenteOEnderecoInformadoSemAlterarOPessoaId()
    {
        var pessoaId = await CriarPessoaAsync();
        var idDoOutroEndereco = await banco.EnderecoRepositorio.InserirAsync(Endereco.Criar(pessoaId, DadosCompletos));
        var id = await banco.EnderecoRepositorio.InserirAsync(Endereco.Criar(pessoaId, DadosCompletos));
        var endereco = await banco.EnderecoRepositorio.ObterPorIdAsync(id);

        endereco!.Atualizar(new DadosDoEndereco((int)TipoDeEndereco.Outros, "Rua B", "20", "Casa", "Batel", "Curitiba", "PR", "80420"));
        await banco.EnderecoRepositorio.AtualizarAsync(endereco);

        var atualizado = await banco.EnderecoRepositorio.ObterPorIdAsync(id);
        var outroEndereco = await banco.EnderecoRepositorio.ObterPorIdAsync(idDoOutroEndereco);
        Assert.Equal(pessoaId, atualizado!.PessoaId);
        Assert.Equal(TipoDeEndereco.Outros, atualizado.Tipo);
        Assert.Equal("Rua B", atualizado.Logradouro);
        Assert.Equal("20", atualizado.Numero);
        Assert.Equal("Casa", atualizado.Complemento);
        Assert.Equal("Batel", atualizado.Bairro);
        Assert.Equal("Curitiba", atualizado.Municipio);
        Assert.Equal("PR", atualizado.Estado.Valor);
        Assert.Equal("80420", atualizado.Cep?.Valor);
        Assert.Equal("Avenida Paulista", outroEndereco!.Logradouro);
    }

    [Fact]
    public async Task AtualizarAsync_SemOpcionais_GravaNuloEMantemOTipo()
    {
        var pessoaId = await CriarPessoaAsync();
        var id = await banco.EnderecoRepositorio.InserirAsync(Endereco.Criar(pessoaId, DadosCompletos));
        var endereco = await banco.EnderecoRepositorio.ObterPorIdAsync(id);

        endereco!.Atualizar(DadosSomenteComObrigatorios);
        await banco.EnderecoRepositorio.AtualizarAsync(endereco);

        var atualizado = await banco.EnderecoRepositorio.ObterPorIdAsync(id);
        Assert.Equal(TipoDeEndereco.Comercial, atualizado!.Tipo);
        AssertCamposOpcionaisNulos(atualizado);
    }

    [Fact]
    public async Task ExcluirAsync_RemoveFisicamenteORegistro()
    {
        var pessoaId = await CriarPessoaAsync();
        var id = await banco.EnderecoRepositorio.InserirAsync(Endereco.Criar(pessoaId, DadosCompletos));

        await banco.EnderecoRepositorio.ExcluirAsync(id);

        Assert.Equal(0, await ContarLinhasDoEnderecoAsync(id));
    }

    [Fact]
    public async Task ExcluirPessoa_ComEnderecosETelefones_ExcluiTudoJunto()
    {
        var pessoaId = await CriarPessoaAsync();
        await banco.EnderecoRepositorio.InserirAsync(Endereco.Criar(pessoaId, DadosCompletos));
        await banco.EnderecoRepositorio.InserirAsync(Endereco.Criar(pessoaId, DadosSomenteComObrigatorios));
        await banco.TelefoneRepositorio.InserirAsync(Telefone.Criar(pessoaId, CelularDaPessoa));

        await banco.Repositorio.ExcluirAsync(pessoaId);

        Assert.Null(await banco.Repositorio.ObterPorIdAsync(pessoaId));
        Assert.Empty(await banco.EnderecoRepositorio.PesquisarPorPessoaIdAsync(pessoaId));
        Assert.Empty(await banco.TelefoneRepositorio.PesquisarPorPessoaIdAsync(pessoaId));
    }

    [Theory]
    [InlineData(9, "SP", "01310", "tipo")]
    [InlineData(0, "ZZ", "01310", "estado")]
    [InlineData(0, "SP", "abc", "cep")]
    public async Task ObterPorIdAsync_ComCampoInvalidoNoBanco_LancaRegistroCorrompido(
        int tipo,
        string estado,
        string cep,
        string campoInvalido)
    {
        var pessoaId = await CriarPessoaAsync();
        var id = await InserirLinhaDiretamenteAsync(pessoaId, tipo, estado, cep);

        var excecao = await Assert.ThrowsAsync<RegistroCorrompidoException>(
            () => banco.EnderecoRepositorio.ObterPorIdAsync(id));

        Assert.Contains("endereço", excecao.Message);
        Assert.Contains(campoInvalido, excecao.Message);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComCepInvalidoNoBanco_NaoExpoeOValorNaMensagem()
    {
        const string cepInvalido = "99-INVALID";
        var pessoaId = await CriarPessoaAsync();
        var id = await InserirLinhaDiretamenteAsync(pessoaId, 0, "SP", cepInvalido);

        var excecao = await Assert.ThrowsAsync<RegistroCorrompidoException>(
            () => banco.EnderecoRepositorio.ObterPorIdAsync(id));

        Assert.DoesNotContain(cepInvalido, excecao.Message);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComPessoaIdNuloNoBanco_LancaRegistroCorrompido()
    {
        var id = await InserirLinhaDiretamenteAsync(null, 0, "SP", null);

        try
        {
            var excecao = await Assert.ThrowsAsync<RegistroCorrompidoException>(
                () => banco.EnderecoRepositorio.ObterPorIdAsync(id));

            Assert.Contains("pessoaid", excecao.Message);
        }
        finally
        {
            await banco.EnderecoRepositorio.ExcluirAsync(id);
        }
    }

    [Fact]
    public async Task PesquisarPorPessoaIdAsync_ComCampoInvalidoNoBanco_LancaRegistroCorrompido()
    {
        var pessoaId = await CriarPessoaAsync();
        await banco.EnderecoRepositorio.InserirAsync(Endereco.Criar(pessoaId, DadosCompletos));
        await InserirLinhaDiretamenteAsync(pessoaId, 0, "ZZ", null);

        await Assert.ThrowsAsync<RegistroCorrompidoException>(
            () => banco.EnderecoRepositorio.PesquisarPorPessoaIdAsync(pessoaId));
    }

    private Task<int> CriarPessoaAsync()
    {
        var pessoa = Pessoa.Criar("Pessoa com endereço", banco.GerarCpfParaRemoverAoFinal(), null, MomentoDeCriacao);

        return banco.Repositorio.InserirAsync(pessoa);
    }

    private async Task<int> InserirLinhaDiretamenteAsync(int? pessoaId, int tipo, string estado, string? cep)
    {
        await using var conexao = await banco.FonteDeDados.OpenConnectionAsync();

        return await conexao.ExecuteScalarAsync<int>(
            """
            INSERT INTO endereco (pessoaid, tipo, municipio, estado, cep) VALUES (@PessoaId, @Tipo, 'Curitiba', @Estado, @Cep);
            SELECT LAST_INSERT_ID();
            """,
            new { PessoaId = pessoaId, Tipo = tipo, Estado = estado, Cep = cep });
    }

    private async Task<(string Estado, string? Cep)> LerEstadoECepDoBancoAsync(int id)
    {
        await using var conexao = await banco.FonteDeDados.OpenConnectionAsync();

        return await conexao.QuerySingleAsync<(string, string?)>(
            "SELECT estado, cep FROM endereco WHERE id = @Id",
            new { Id = id });
    }

    private async Task<int> ContarLinhasDoEnderecoAsync(int id)
    {
        await using var conexao = await banco.FonteDeDados.OpenConnectionAsync();

        return await conexao.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM endereco WHERE id = @Id", new { Id = id });
    }

    private static void AssertCamposOpcionaisNulos(Endereco endereco)
    {
        Assert.Null(endereco.Logradouro);
        Assert.Null(endereco.Numero);
        Assert.Null(endereco.Complemento);
        Assert.Null(endereco.Bairro);
        Assert.Null(endereco.Cep);
    }
}
