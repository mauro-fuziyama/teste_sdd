using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.ValueObjects;

namespace CadastroPessoas.Domain.Tests.Entities;

public class PessoaTests
{
    private const string NomeValido = "Maria da Silva";
    private const string CpfValido = "52998224725";
    private const string OutroCpfValido = "11144477735";
    private static readonly DateOnly DataDeNascimentoValida = new(1990, 5, 20);
    private static readonly DateTime MomentoDoCadastro = new(2026, 11, 5, 10, 30, 0);
    private static readonly DateTime MomentoDaAtualizacao = new(2026, 11, 6, 15, 45, 0);

    private static Pessoa CriarPessoaValida()
    {
        return Pessoa.Criar(NomeValido, CpfValido, DataDeNascimentoValida, MomentoDoCadastro);
    }

    [Fact]
    public void Criar_ComDadosValidos_PreencheOsCampos()
    {
        var pessoa = CriarPessoaValida();

        Assert.Equal(NomeValido, pessoa.Nome);
        Assert.Equal(CpfValido, pessoa.Cpf.Valor);
        Assert.Equal(DataDeNascimentoValida, pessoa.DataDeNascimento);
        Assert.Equal(MomentoDoCadastro, pessoa.DataDeCadastro);
    }

    [Fact]
    public void Criar_ComDadosValidos_DataAtualizacaoFicaIgualADataDeCadastro()
    {
        var pessoa = CriarPessoaValida();

        Assert.Equal(pessoa.DataDeCadastro, pessoa.DataAtualizacao);
    }

    [Fact]
    public void Criar_ComNomeComEspacosNasPontas_GuardaONomeSemOsEspacos()
    {
        var pessoa = Pessoa.Criar("  Maria da Silva  ", CpfValido, null, MomentoDoCadastro);

        Assert.Equal(NomeValido, pessoa.Nome);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(100)]
    public void Criar_ComNomeNoLimiteDoTamanho_Aceita(int tamanho)
    {
        var nome = new string('a', tamanho);

        var pessoa = Pessoa.Criar(nome, CpfValido, null, MomentoDoCadastro);

        Assert.Equal(nome, pessoa.Nome);
    }

    [Fact]
    public void Criar_ComNomeDeTamanhoMaximoMaisEspacosNasPontas_Aceita()
    {
        var nome = new string('a', 100);

        var pessoa = Pessoa.Criar($"  {nome}  ", CpfValido, null, MomentoDoCadastro);

        Assert.Equal(nome, pessoa.Nome);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("     ")]
    [InlineData("ab")]
    [InlineData("  ab  ")]
    public void Criar_ComNomeCurtoOuVazio_LancaExcecao(string? nome)
    {
        var excecao = Assert.Throws<ValidacaoException>(() => Pessoa.Criar(nome, CpfValido, null, MomentoDoCadastro));

        Assert.Equal([ErrosDePessoa.NomeInvalido], excecao.Erros);
    }

    [Fact]
    public void Criar_ComNomeAcimaDoTamanhoMaximo_LancaExcecao()
    {
        var nome = new string('a', 101);

        var excecao = Assert.Throws<ValidacaoException>(() => Pessoa.Criar(nome, CpfValido, null, MomentoDoCadastro));

        Assert.Equal([ErrosDePessoa.NomeInvalido], excecao.Erros);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("11111111111")]
    public void Criar_ComCpfInvalido_LancaExcecao(string? cpf)
    {
        var excecao = Assert.Throws<ValidacaoException>(() => Pessoa.Criar(NomeValido, cpf, null, MomentoDoCadastro));

        Assert.Equal([ErrosDePessoa.CpfInvalido], excecao.Erros);
    }

    [Fact]
    public void Criar_SemDataDeNascimento_Aceita()
    {
        var pessoa = Pessoa.Criar(NomeValido, CpfValido, null, MomentoDoCadastro);

        Assert.Null(pessoa.DataDeNascimento);
    }

    [Theory]
    [InlineData("1910-01-02")]
    [InlineData("2026-11-04")]
    public void Criar_ComDataDeNascimentoNoLimite_Aceita(string data)
    {
        var dataDeNascimento = DateOnly.Parse(data);

        var pessoa = Pessoa.Criar(NomeValido, CpfValido, dataDeNascimento, MomentoDoCadastro);

        Assert.Equal(dataDeNascimento, pessoa.DataDeNascimento);
    }

    [Theory]
    [InlineData("1909-12-31")]
    [InlineData("1910-01-01")]
    [InlineData("2026-11-05")]
    [InlineData("2026-11-06")]
    public void Criar_ComDataDeNascimentoForaDoIntervalo_LancaExcecao(string data)
    {
        var dataDeNascimento = DateOnly.Parse(data);

        var excecao = Assert.Throws<ValidacaoException>(
            () => Pessoa.Criar(NomeValido, CpfValido, dataDeNascimento, MomentoDoCadastro));

        Assert.Equal([ErrosDePessoa.DataDeNascimentoInvalida], excecao.Erros);
    }

    [Fact]
    public void Criar_ComVariosCamposInvalidos_RetornaTodosOsErros()
    {
        var dataInvalida = DateOnly.FromDateTime(MomentoDoCadastro);

        var excecao = Assert.Throws<ValidacaoException>(
            () => Pessoa.Criar("ab", "123", dataInvalida, MomentoDoCadastro));

        Assert.Equal(3, excecao.Erros.Count);
        Assert.Contains(ErrosDePessoa.NomeInvalido, excecao.Erros);
        Assert.Contains(ErrosDePessoa.CpfInvalido, excecao.Erros);
        Assert.Contains(ErrosDePessoa.DataDeNascimentoInvalida, excecao.Erros);
    }

    [Fact]
    public void Atualizar_ComDadosValidos_AlteraOsCamposEPreservaOCadastro()
    {
        var pessoa = CriarPessoaValida();
        var novaDataDeNascimento = new DateOnly(1985, 1, 15);

        pessoa.Atualizar("  Maria Souza  ", CpfValido, novaDataDeNascimento, MomentoDaAtualizacao);

        Assert.Equal("Maria Souza", pessoa.Nome);
        Assert.Equal(novaDataDeNascimento, pessoa.DataDeNascimento);
        Assert.Equal(MomentoDaAtualizacao, pessoa.DataAtualizacao);
        Assert.Equal(MomentoDoCadastro, pessoa.DataDeCadastro);
    }

    [Fact]
    public void Atualizar_ComOutroCpf_LancaExcecaoENaoAlteraOsDados()
    {
        var pessoa = CriarPessoaValida();

        var excecao = Assert.Throws<ValidacaoException>(
            () => pessoa.Atualizar("Maria Souza", OutroCpfValido, null, MomentoDaAtualizacao));

        Assert.Equal([ErrosDePessoa.CpfNaoPodeSerAlterado], excecao.Erros);
        AssertDadosOriginaisPreservados(pessoa);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Atualizar_ComCpfVazioOuOmitido_LancaExcecaoENaoAlteraOsDados(string? cpf)
    {
        var pessoa = CriarPessoaValida();

        var excecao = Assert.Throws<ValidacaoException>(
            () => pessoa.Atualizar("Maria Souza", cpf, null, MomentoDaAtualizacao));

        Assert.Equal([ErrosDePessoa.CpfInvalido], excecao.Erros);
        AssertDadosOriginaisPreservados(pessoa);
    }

    [Fact]
    public void Atualizar_ComNomeInvalido_LancaExcecaoENaoAlteraOsDados()
    {
        var pessoa = CriarPessoaValida();

        var excecao = Assert.Throws<ValidacaoException>(
            () => pessoa.Atualizar("ab", CpfValido, null, MomentoDaAtualizacao));

        Assert.Equal([ErrosDePessoa.NomeInvalido], excecao.Erros);
        AssertDadosOriginaisPreservados(pessoa);
    }

    [Fact]
    public void Atualizar_ComDataDeNascimentoInvalida_LancaExcecaoENaoAlteraOsDados()
    {
        var pessoa = CriarPessoaValida();
        var dataInvalida = DateOnly.FromDateTime(MomentoDaAtualizacao);

        var excecao = Assert.Throws<ValidacaoException>(
            () => pessoa.Atualizar("Maria Souza", CpfValido, dataInvalida, MomentoDaAtualizacao));

        Assert.Equal([ErrosDePessoa.DataDeNascimentoInvalida], excecao.Erros);
        AssertDadosOriginaisPreservados(pessoa);
    }

    [Fact]
    public void Atualizar_ComVariosCamposInvalidos_RetornaTodosOsErros()
    {
        var pessoa = CriarPessoaValida();
        var dataInvalida = DateOnly.FromDateTime(MomentoDaAtualizacao);

        var excecao = Assert.Throws<ValidacaoException>(
            () => pessoa.Atualizar("ab", OutroCpfValido, dataInvalida, MomentoDaAtualizacao));

        Assert.Equal(3, excecao.Erros.Count);
        Assert.Contains(ErrosDePessoa.NomeInvalido, excecao.Erros);
        Assert.Contains(ErrosDePessoa.CpfNaoPodeSerAlterado, excecao.Erros);
        Assert.Contains(ErrosDePessoa.DataDeNascimentoInvalida, excecao.Erros);
        AssertDadosOriginaisPreservados(pessoa);
    }

    [Fact]
    public void Reconstituir_ComDadosDoBanco_PreservaTodosOsCampos()
    {
        var cpf = new CPF(CpfValido);

        var pessoa = Pessoa.Reconstituir(
            7, NomeValido, cpf, DataDeNascimentoValida, MomentoDoCadastro, MomentoDaAtualizacao);

        Assert.Equal(7, pessoa.Id);
        Assert.Equal(NomeValido, pessoa.Nome);
        Assert.Equal(cpf, pessoa.Cpf);
        Assert.Equal(DataDeNascimentoValida, pessoa.DataDeNascimento);
        Assert.Equal(MomentoDoCadastro, pessoa.DataDeCadastro);
        Assert.Equal(MomentoDaAtualizacao, pessoa.DataAtualizacao);
    }

    private static void AssertDadosOriginaisPreservados(Pessoa pessoa)
    {
        Assert.Equal(NomeValido, pessoa.Nome);
        Assert.Equal(CpfValido, pessoa.Cpf.Valor);
        Assert.Equal(DataDeNascimentoValida, pessoa.DataDeNascimento);
        Assert.Equal(MomentoDoCadastro, pessoa.DataAtualizacao);
    }
}
