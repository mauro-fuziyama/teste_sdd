using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.ValueObjects;

namespace CadastroPessoas.Domain.Tests.ValueObjects;

public class NumeroDeTelefoneTests
{
    [Theory]
    [InlineData("1134567890")]
    [InlineData("11987654321")]
    [InlineData("9934567890")]
    [InlineData("99987654321")]
    [InlineData("1194567890")]
    public void Criar_ComNumeroValido_GuardaOValor(string valor)
    {
        var numero = new NumeroDeTelefone(valor);

        Assert.Equal(valor, numero.Valor);
    }

    [Theory]
    [InlineData("  11987654321  ")]
    [InlineData("\t1134567890 ")]
    public void Criar_ComEspacosNasPontas_GuardaOValorSemOsEspacos(string valor)
    {
        var numero = new NumeroDeTelefone(valor);

        Assert.Equal(valor.Trim(), numero.Valor);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void Criar_SemNumero_LancaErroDeNumeroObrigatorio(string? valor)
    {
        AssertErros(valor, ErrosDeTelefone.NumeroObrigatorio);
    }

    [Theory]
    [InlineData("(11) 98765-4321")]
    [InlineData("11 987654321")]
    [InlineData("1198765432a")]
    [InlineData("+5511987654321")]
    [InlineData("١١٩٨٧٦٥٤٣٢١")]
    public void Criar_ComCaracteresQueNaoSaoDigitos_LancaErroDeSomenteDigitos(string valor)
    {
        AssertErros(valor, ErrosDeTelefone.NumeroSomenteComDigitos);
    }

    [Theory]
    [InlineData("113456789")]
    [InlineData("119876543210")]
    public void Criar_ComTamanhoInvalido_LancaErroDeTamanho(string valor)
    {
        AssertErros(valor, ErrosDeTelefone.NumeroComTamanhoInvalido);
    }

    [Theory]
    [InlineData("1034567890")]
    [InlineData("0134567890")]
    [InlineData("10987654321")]
    public void Criar_ComDddForaDoIntervalo_LancaErroDeDdd(string valor)
    {
        AssertErros(valor, ErrosDeTelefone.DddInvalido);
    }

    [Theory]
    [InlineData("11887654321")]
    [InlineData("11087654321")]
    public void Criar_CelularSemNoveNoTerceiroDigito_LancaErroDeCelular(string valor)
    {
        AssertErros(valor, ErrosDeTelefone.CelularSemNoveNoTerceiroDigito);
    }

    [Fact]
    public void Criar_ComVariosProblemas_ReuneTodosOsErros()
    {
        AssertErros("10887654321", ErrosDeTelefone.DddInvalido, ErrosDeTelefone.CelularSemNoveNoTerceiroDigito);
    }

    [Fact]
    public void Criar_ComTamanhoEDddInvalidos_ReuneOsDoisErros()
    {
        AssertErros("0512345", ErrosDeTelefone.NumeroComTamanhoInvalido, ErrosDeTelefone.DddInvalido);
    }

    [Theory]
    [InlineData("11987654321", true)]
    [InlineData("1034567890", false)]
    [InlineData(null, false)]
    public void EhValido_RetornaSeONumeroAtendeAsRegras(string? valor, bool esperado)
    {
        Assert.Equal(esperado, NumeroDeTelefone.EhValido(valor));
    }

    [Fact]
    public void Comparar_ComMesmoValor_SaoIguais()
    {
        Assert.Equal(new NumeroDeTelefone("11987654321"), new NumeroDeTelefone(" 11987654321 "));
    }

    private static void AssertErros(string? valor, params string[] errosEsperados)
    {
        var excecao = Assert.Throws<ValidacaoException>(() => new NumeroDeTelefone(valor));

        Assert.Equal(errosEsperados, excecao.Erros);
    }
}
