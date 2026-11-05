using CadastroPessoas.Application.Services;
using CadastroPessoas.Application.Tests.Fakes;
using CadastroPessoas.Application.UseCases.Pessoa.AtualizarPessoa;
using CadastroPessoas.Application.UseCases.Pessoa.ConsultarPessoa;
using CadastroPessoas.Application.UseCases.Pessoa.CriarPessoa;
using CadastroPessoas.Application.UseCases.Pessoa.ExcluirPessoa;
using CadastroPessoas.Application.UseCases.Pessoa.PesquisarPessoa;
using CadastroPessoas.Domain.Exceptions;

namespace CadastroPessoas.Application.Tests.Services;

public class PessoaServiceTests
{
    private const string NomeValido = "Maria da Silva";
    private const string CpfValido = "52998224725";
    private const string OutroCpfValido = "11144477735";
    private const int IdInexistente = 999;
    private static readonly DateOnly DataDeNascimentoValida = new(1990, 5, 20);
    private static readonly DateTime MomentoDoCadastro = new(2026, 11, 5, 10, 30, 0);
    private static readonly DateTime MomentoDaAtualizacao = new(2026, 11, 6, 15, 45, 0);

    private readonly ProvedorDeTempoControlado _provedorDeTempo = new(MomentoDoCadastro);
    private readonly PessoaService _servico;

    public PessoaServiceTests()
    {
        var repositorio = new PessoaRepositoryEmMemoria();

        _servico = new PessoaService(
            new CriarPessoaUseCase(repositorio, _provedorDeTempo),
            new ConsultarPessoaUseCase(repositorio),
            new PesquisarPessoaUseCase(repositorio),
            new AtualizarPessoaUseCase(repositorio, _provedorDeTempo),
            new ExcluirPessoaUseCase(repositorio));
    }

    private Task<int> CriarPessoaValidaAsync()
    {
        return _servico.CriarAsync(new CriarPessoaInput(NomeValido, CpfValido, DataDeNascimentoValida));
    }

    [Fact]
    public async Task Criar_ComDadosValidos_RetornaOIdEGravaNoHorarioLocal()
    {
        var id = await CriarPessoaValidaAsync();

        var pessoa = await _servico.ConsultarPorIdAsync(id);

        Assert.NotNull(pessoa);
        Assert.Equal(NomeValido, pessoa.Nome);
        Assert.Equal(MomentoDoCadastro, pessoa.DataDeCadastro);
        Assert.Equal(MomentoDoCadastro, pessoa.DataAtualizacao);
    }

    [Fact]
    public async Task Criar_ComCpfJaCadastrado_LancaExcecao()
    {
        await CriarPessoaValidaAsync();

        var excecao = await Assert.ThrowsAsync<ValidacaoException>(CriarPessoaValidaAsync);

        Assert.Equal([ErrosDePessoa.CpfJaCadastrado], excecao.Erros);
    }

    [Fact]
    public async Task Criar_ComCpfJaCadastradoENomeInvalido_RetornaTodosOsErros()
    {
        await CriarPessoaValidaAsync();

        var excecao = await Assert.ThrowsAsync<ValidacaoException>(
            () => _servico.CriarAsync(new CriarPessoaInput("ab", CpfValido, null)));

        Assert.Equal(2, excecao.Erros.Count);
        Assert.Contains(ErrosDePessoa.NomeInvalido, excecao.Erros);
        Assert.Contains(ErrosDePessoa.CpfJaCadastrado, excecao.Erros);
    }

    [Fact]
    public async Task Criar_ComVariosCamposInvalidos_RetornaTodosOsErros()
    {
        var dataInvalida = DateOnly.FromDateTime(MomentoDoCadastro);

        var excecao = await Assert.ThrowsAsync<ValidacaoException>(
            () => _servico.CriarAsync(new CriarPessoaInput("ab", "11111111111", dataInvalida)));

        Assert.Equal(3, excecao.Erros.Count);
        Assert.Contains(ErrosDePessoa.NomeInvalido, excecao.Erros);
        Assert.Contains(ErrosDePessoa.CpfInvalido, excecao.Erros);
        Assert.Contains(ErrosDePessoa.DataDeNascimentoInvalida, excecao.Erros);
    }

    [Fact]
    public async Task ConsultarPorId_ComIdInexistente_RetornaNulo()
    {
        Assert.Null(await _servico.ConsultarPorIdAsync(IdInexistente));
    }

    [Fact]
    public async Task ConsultarPorCpf_ComCpfCadastrado_RetornaAPessoa()
    {
        var id = await CriarPessoaValidaAsync();

        var pessoa = await _servico.ConsultarPorCpfAsync(CpfValido);

        Assert.NotNull(pessoa);
        Assert.Equal(id, pessoa.Id);
    }

    [Fact]
    public async Task ConsultarPorCpf_ComCpfNaoCadastrado_RetornaNulo()
    {
        Assert.Null(await _servico.ConsultarPorCpfAsync(OutroCpfValido));
    }

    [Fact]
    public async Task ConsultarPorCpf_ComCpfInvalido_LancaExcecao()
    {
        var excecao = await Assert.ThrowsAsync<ValidacaoException>(
            () => _servico.ConsultarPorCpfAsync("123"));

        Assert.Equal([ErrosDePessoa.CpfInvalido], excecao.Erros);
    }

    [Fact]
    public async Task PesquisarPorNome_ComParteDoNome_RetornaTodosOsRegistrosQueContemOValor()
    {
        await CriarPessoaValidaAsync();
        await _servico.CriarAsync(new CriarPessoaInput("Ana da Silva", OutroCpfValido, null));

        var pessoas = await _servico.PesquisarPorNomeAsync("Silva");

        Assert.Equal(2, pessoas.Count);
    }

    [Fact]
    public async Task PesquisarPorNome_SemCorrespondencia_RetornaListaVazia()
    {
        await CriarPessoaValidaAsync();

        Assert.Empty(await _servico.PesquisarPorNomeAsync("Inexistente"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task PesquisarPorNome_ComValorEmBranco_LancaErroDeNomeObrigatorio(string? nomeEmBranco)
    {
        var excecao = await Assert.ThrowsAsync<ValidacaoException>(
            () => _servico.PesquisarPorNomeAsync(nomeEmBranco));

        Assert.Equal([ErrosDePessoa.NomeDaPesquisaObrigatorio], excecao.Erros);
    }

    [Fact]
    public async Task Atualizar_ComDadosValidos_GravaOsDadosESoAlteraDataAtualizacao()
    {
        var id = await CriarPessoaValidaAsync();
        _provedorDeTempo.MomentoAtual = MomentoDaAtualizacao;

        await _servico.AtualizarAsync(id, new AtualizarPessoaInput("  Maria Souza  ", CpfValido, null));

        var pessoa = await _servico.ConsultarPorIdAsync(id);
        Assert.NotNull(pessoa);
        Assert.Equal("Maria Souza", pessoa.Nome);
        Assert.Null(pessoa.DataDeNascimento);
        Assert.Equal(MomentoDoCadastro, pessoa.DataDeCadastro);
        Assert.Equal(MomentoDaAtualizacao, pessoa.DataAtualizacao);
    }

    [Theory]
    [InlineData(null, ErrosDePessoa.CpfInvalido)]
    [InlineData("", ErrosDePessoa.CpfInvalido)]
    [InlineData(OutroCpfValido, ErrosDePessoa.CpfNaoPodeSerAlterado)]
    public async Task Atualizar_ComCpfOmitidoVazioOuDiferente_RejeitaENaoGrava(string? cpf, string erroEsperado)
    {
        var id = await CriarPessoaValidaAsync();

        var excecao = await Assert.ThrowsAsync<ValidacaoException>(
            () => _servico.AtualizarAsync(id, new AtualizarPessoaInput("Maria Souza", cpf, null)));

        Assert.Equal([erroEsperado], excecao.Erros);
        var pessoa = await _servico.ConsultarPorIdAsync(id);
        Assert.NotNull(pessoa);
        Assert.Equal(NomeValido, pessoa.Nome);
    }

    [Fact]
    public async Task Atualizar_ComVariosCamposInvalidos_RetornaTodosOsErros()
    {
        var id = await CriarPessoaValidaAsync();

        var excecao = await Assert.ThrowsAsync<ValidacaoException>(
            () => _servico.AtualizarAsync(id, new AtualizarPessoaInput("ab", null, null)));

        Assert.Equal(2, excecao.Erros.Count);
        Assert.Contains(ErrosDePessoa.NomeInvalido, excecao.Erros);
        Assert.Contains(ErrosDePessoa.CpfInvalido, excecao.Erros);
    }

    [Fact]
    public async Task Atualizar_ComIdInexistente_LancaExcecao()
    {
        var dados = new AtualizarPessoaInput(NomeValido, CpfValido, null);

        await Assert.ThrowsAsync<PessoaNaoEncontradaException>(
            () => _servico.AtualizarAsync(IdInexistente, dados));
    }

    [Fact]
    public async Task Excluir_ComIdExistente_RemoveORegistro()
    {
        var id = await CriarPessoaValidaAsync();

        await _servico.ExcluirAsync(id);

        Assert.Null(await _servico.ConsultarPorIdAsync(id));
    }

    [Fact]
    public async Task Excluir_ComIdInexistente_LancaExcecao()
    {
        await Assert.ThrowsAsync<PessoaNaoEncontradaException>(
            () => _servico.ExcluirAsync(IdInexistente));
    }
}
