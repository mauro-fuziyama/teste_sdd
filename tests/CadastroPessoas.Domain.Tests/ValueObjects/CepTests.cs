using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.ValueObjects;

namespace CadastroPessoas.Domain.Tests.ValueObjects;

public class CepTests
{
    [Theory]
    [InlineData("01310")]
    [InlineData("01310100")]
    public void Criar_ComCepValido_GuardaOValor(string valor)
    {
        var cep = new Cep(valor);

        Assert.Equal(valor, cep.Valor);
    }

    [Theory]
    [InlineData("  01310100  ")]
    [InlineData("\t01310 ")]
    public void Criar_ComEspacosNasPontas_GuardaOValorSemOsEspacos(string valor)
    {
        var cep = new Cep(valor);

        Assert.Equal(valor.Trim(), cep.Valor);
    }

    [Theory]
    [InlineData("0131")]
    [InlineData("013101")]
    [InlineData("0131010")]
    [InlineData("013101000")]
    public void Criar_ComQuantidadeDeDigitosInvalida_LancaErroDeTamanho(string valor)
    {
        var excecao = Assert.Throws<ValidacaoException>(() => new Cep(valor));

        Assert.Equal([ErrosDeEndereco.CepComTamanhoInvalido], excecao.Erros);
    }

    [Theory]
    [InlineData("0131a")]
    [InlineData("0131010b")]
    [InlineData("١٢٣٤٥")]
    public void Criar_ComCaracteresQueNaoSaoDigitos_LancaErroDeDigitos(string valor)
    {
        var excecao = Assert.Throws<ValidacaoException>(() => new Cep(valor));

        Assert.Equal([ErrosDeEndereco.CepSomenteComDigitos], excecao.Erros);
    }

    [Theory]
    [InlineData("01310-100", "01310100")]
    [InlineData(" 01310-100 ", "01310100")]
    [InlineData("01310-", "01310")]
    [InlineData("0-1-3-1-0", "01310")]
    public void Criar_ComHifen_GuardaOValorSemOHifen(string valor, string valorEsperado)
    {
        var cep = new Cep(valor);

        Assert.Equal(valorEsperado, cep.Valor);
    }

    [Theory]
    [InlineData("-")]
    [InlineData("0131-")]
    [InlineData("01310-1")]
    public void Criar_ComHifenETamanhoInvalidoSemOHifen_LancaErroDeTamanho(string valor)
    {
        var excecao = Assert.Throws<ValidacaoException>(() => new Cep(valor));

        Assert.Equal([ErrosDeEndereco.CepComTamanhoInvalido], excecao.Erros);
    }

    [Fact]
    public void Criar_ComHifenELetras_LancaErroDeDigitos()
    {
        var excecao = Assert.Throws<ValidacaoException>(() => new Cep("0131a-100"));

        Assert.Equal([ErrosDeEndereco.CepSomenteComDigitos], excecao.Erros);
    }

    [Fact]
    public void Criar_ComLetrasETamanhoInvalido_ReuneOsDoisErros()
    {
        var excecao = Assert.Throws<ValidacaoException>(() => new Cep("0131a1"));

        Assert.Equal([ErrosDeEndereco.CepSomenteComDigitos, ErrosDeEndereco.CepComTamanhoInvalido], excecao.Erros);
    }

    [Theory]
    [InlineData("01310", true)]
    [InlineData("01310-100", true)]
    [InlineData("0131", false)]
    [InlineData("abcde", false)]
    public void EhValido_RetornaSeOValorAtendeAsRegras(string valor, bool resultadoEsperado)
    {
        Assert.Equal(resultadoEsperado, Cep.EhValido(valor));
    }
}
