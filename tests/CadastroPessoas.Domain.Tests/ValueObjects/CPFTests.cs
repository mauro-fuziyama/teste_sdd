using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.ValueObjects;

namespace CadastroPessoas.Domain.Tests.ValueObjects;

public class CPFTests
{
    [Theory]
    [InlineData("52998224725")]
    [InlineData("11144477735")]
    [InlineData("00000000191")]
    public void Criar_ComCpfValido_GuardaOValor(string valor)
    {
        var cpf = new CPF(valor);

        Assert.Equal(valor, cpf.Valor);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("5299822472")]
    [InlineData("529982247250")]
    public void Criar_ComTamanhoInvalido_LancaExcecao(string? valor)
    {
        AssertCpfInvalido(valor);
    }

    [Theory]
    [InlineData("529.982.247-25")]
    [InlineData("5299822472a")]
    [InlineData("5299822472 ")]
    [InlineData("٥٢٩٩٨٢٢٤٧٢٥")]
    public void Criar_ComCaracteresNaoNumericos_LancaExcecao(string valor)
    {
        AssertCpfInvalido(valor);
    }

    [Theory]
    [InlineData("00000000000")]
    [InlineData("11111111111")]
    [InlineData("22222222222")]
    [InlineData("33333333333")]
    [InlineData("44444444444")]
    [InlineData("55555555555")]
    [InlineData("66666666666")]
    [InlineData("77777777777")]
    [InlineData("88888888888")]
    [InlineData("99999999999")]
    public void Criar_ComTodosOsDigitosIguais_LancaExcecao(string valor)
    {
        AssertCpfInvalido(valor);
    }

    [Theory]
    [InlineData("52998224715")]
    [InlineData("52998224724")]
    public void Criar_ComDigitoVerificadorIncorreto_LancaExcecao(string valor)
    {
        AssertCpfInvalido(valor);
    }

    [Fact]
    public void Comparar_ComMesmoValor_SaoIguais()
    {
        Assert.Equal(new CPF("52998224725"), new CPF("52998224725"));
    }

    private static void AssertCpfInvalido(string? valor)
    {
        var excecao = Assert.Throws<ValidacaoException>(() => new CPF(valor));

        Assert.Equal([ErrosDePessoa.CpfInvalido], excecao.Erros);
    }
}
