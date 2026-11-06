using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.ValueObjects;

namespace CadastroPessoas.Domain.Tests.Entities;

public class TelefoneTests
{
    private const int PessoaIdValido = 7;
    private const string NumeroValido = "11987654321";
    private const string OutroNumeroValido = "1134567890";

    [Fact]
    public void Criar_ComDadosValidos_PreencheOsCampos()
    {
        var telefone = Telefone.Criar(PessoaIdValido, NumeroValido);

        Assert.Equal(0, telefone.Id);
        Assert.Equal(PessoaIdValido, telefone.PessoaId);
        Assert.Equal(NumeroValido, telefone.Numero.Valor);
    }

    [Fact]
    public void Criar_ComNumeroComEspacosNasPontas_GuardaONumeroSemOsEspacos()
    {
        var telefone = Telefone.Criar(PessoaIdValido, "  11987654321  ");

        Assert.Equal(NumeroValido, telefone.Numero.Valor);
    }

    [Fact]
    public void Criar_SemPessoaId_LancaErroDePessoaIdObrigatorio()
    {
        var excecao = Assert.Throws<ValidacaoException>(() => Telefone.Criar(null, NumeroValido));

        Assert.Equal([ErrosDeTelefone.PessoaIdObrigatorio], excecao.Erros);
    }

    [Fact]
    public void Criar_ComNumeroInvalido_LancaErroDoNumero()
    {
        var excecao = Assert.Throws<ValidacaoException>(() => Telefone.Criar(PessoaIdValido, "1034567890"));

        Assert.Equal([ErrosDeTelefone.DddInvalido], excecao.Erros);
    }

    [Fact]
    public void Criar_ComNumeroEPessoaIdInvalidos_ReuneTodosOsErros()
    {
        var excecao = Assert.Throws<ValidacaoException>(() => Telefone.Criar(null, "10887654321"));

        Assert.Equal(
            [ErrosDeTelefone.DddInvalido, ErrosDeTelefone.CelularSemNoveNoTerceiroDigito, ErrosDeTelefone.PessoaIdObrigatorio],
            excecao.Erros);
    }

    [Fact]
    public void ValidarCriacao_ComDadosValidos_NaoRetornaErros()
    {
        Assert.Empty(Telefone.ValidarCriacao(PessoaIdValido, NumeroValido));
    }

    [Fact]
    public void ValidarCriacao_SemNumeroESemPessoaId_RetornaOsDoisErros()
    {
        var erros = Telefone.ValidarCriacao(null, null);

        Assert.Equal([ErrosDeTelefone.NumeroObrigatorio, ErrosDeTelefone.PessoaIdObrigatorio], erros);
    }

    [Fact]
    public void Reconstituir_PreencheOsCamposSemAlterarOsValores()
    {
        var telefone = Telefone.Reconstituir(15, PessoaIdValido, new NumeroDeTelefone(NumeroValido));

        Assert.Equal(15, telefone.Id);
        Assert.Equal(PessoaIdValido, telefone.PessoaId);
        Assert.Equal(NumeroValido, telefone.Numero.Valor);
    }

    [Fact]
    public void AtualizarNumero_ComNumeroValido_AlteraSomenteONumero()
    {
        var telefone = Telefone.Reconstituir(15, PessoaIdValido, new NumeroDeTelefone(NumeroValido));

        telefone.AtualizarNumero($" {OutroNumeroValido} ");

        Assert.Equal(OutroNumeroValido, telefone.Numero.Valor);
        Assert.Equal(15, telefone.Id);
        Assert.Equal(PessoaIdValido, telefone.PessoaId);
    }

    [Theory]
    [InlineData(null, ErrosDeTelefone.NumeroObrigatorio)]
    [InlineData("", ErrosDeTelefone.NumeroObrigatorio)]
    [InlineData("11 98765", ErrosDeTelefone.NumeroSomenteComDigitos)]
    [InlineData("11887654321", ErrosDeTelefone.CelularSemNoveNoTerceiroDigito)]
    public void AtualizarNumero_ComNumeroInvalido_LancaErroENaoAlteraONumero(string? numeroInvalido, string erroEsperado)
    {
        var telefone = Telefone.Reconstituir(15, PessoaIdValido, new NumeroDeTelefone(NumeroValido));

        var excecao = Assert.Throws<ValidacaoException>(() => telefone.AtualizarNumero(numeroInvalido));

        Assert.Equal([erroEsperado], excecao.Erros);
        Assert.Equal(NumeroValido, telefone.Numero.Valor);
    }
}
