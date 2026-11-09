using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.ValueObjects;

namespace CadastroPessoas.Domain.Tests.ValueObjects;

public class SiglaDeEstadoTests
{
    [Theory]
    [InlineData("AC")]
    [InlineData("AL")]
    [InlineData("AP")]
    [InlineData("AM")]
    [InlineData("BA")]
    [InlineData("CE")]
    [InlineData("DF")]
    [InlineData("ES")]
    [InlineData("GO")]
    [InlineData("MA")]
    [InlineData("MT")]
    [InlineData("MS")]
    [InlineData("MG")]
    [InlineData("PA")]
    [InlineData("PB")]
    [InlineData("PR")]
    [InlineData("PE")]
    [InlineData("PI")]
    [InlineData("RJ")]
    [InlineData("RN")]
    [InlineData("RS")]
    [InlineData("RO")]
    [InlineData("RR")]
    [InlineData("SC")]
    [InlineData("SP")]
    [InlineData("SE")]
    [InlineData("TO")]
    public void Criar_ComSiglaValida_GuardaOValor(string valor)
    {
        var sigla = new SiglaDeEstado(valor);

        Assert.Equal(valor, sigla.Valor);
    }

    [Theory]
    [InlineData("sp")]
    [InlineData("Sp")]
    [InlineData("sP")]
    [InlineData("  sp  ")]
    public void Criar_ComMinusculasOuEspacos_GuardaEmMaiusculasSemOsEspacos(string valor)
    {
        var sigla = new SiglaDeEstado(valor);

        Assert.Equal("SP", sigla.Valor);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_SemValor_LancaErroDeEstadoObrigatorio(string? valor)
    {
        var excecao = Assert.Throws<ValidacaoException>(() => new SiglaDeEstado(valor));

        Assert.Equal([ErrosDeEndereco.EstadoObrigatorio], excecao.Erros);
    }

    [Theory]
    [InlineData("S")]
    [InlineData("SPA")]
    public void Criar_ComTamanhoDiferenteDeDois_LancaErroDeTamanho(string valor)
    {
        var excecao = Assert.Throws<ValidacaoException>(() => new SiglaDeEstado(valor));

        Assert.Equal([ErrosDeEndereco.EstadoComTamanhoInvalido], excecao.Erros);
    }

    [Theory]
    [InlineData("XX")]
    [InlineData("xx")]
    [InlineData("12")]
    public void Criar_ComSiglaInexistente_LancaErroDeSiglaInvalida(string valor)
    {
        var excecao = Assert.Throws<ValidacaoException>(() => new SiglaDeEstado(valor));

        Assert.Equal([ErrosDeEndereco.SiglaDeEstadoInvalida], excecao.Erros);
    }

    [Theory]
    [InlineData("rj", true)]
    [InlineData("XX", false)]
    [InlineData(null, false)]
    public void EhValido_RetornaSeOValorAtendeAsRegras(string? valor, bool resultadoEsperado)
    {
        Assert.Equal(resultadoEsperado, SiglaDeEstado.EhValido(valor));
    }
}
