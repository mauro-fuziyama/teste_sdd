using AwesomeAssertions;
using CadastroPessoas.Domain.Pessoas;
using CadastroPessoas.Domain.Pessoas.ValueObjects;

namespace CadastroPessoas.Domain.Tests.Pessoas;

public class CpfTests
{
    private const string CpfValido = "52998224725";

    [Theory]
    [InlineData("529.982.247-25")]
    [InlineData("52998224725")]
    public void CriarAceitaCpfValidoComOuSemMascara(string entrada)
    {
        var resultado = Cpf.Criar(entrada);

        resultado.Sucesso.Should().BeTrue();
        resultado.Valor!.Numero.Should().Be(CpfValido);
    }

    [Theory]
    [InlineData("123.456.789-09")]
    [InlineData("12345678909")]
    public void CriarAceitaCpfComDigitoVerificadorZero(string entrada)
    {
        var resultado = Cpf.Criar(entrada);

        resultado.Valor!.Numero.Should().Be("12345678909");
    }

    [Theory]
    [InlineData("529.982.247-15")]
    [InlineData("529.982.247-26")]
    [InlineData("5299822472")]
    [InlineData("529.982.247-2A")]
    [InlineData("529982247250")]
    public void CriarRejeitaCpfInvalido(string entrada)
    {
        var resultado = Cpf.Criar(entrada);

        resultado.Sucesso.Should().BeFalse();
        resultado.Erros.Should().ContainSingle().Which.Should().Be(PessoaErros.CpfInvalido);
    }

    [Theory]
    [InlineData("000.000.000-00")]
    [InlineData("111.111.111-11")]
    [InlineData("222.222.222-22")]
    [InlineData("333.333.333-33")]
    [InlineData("444.444.444-44")]
    [InlineData("555.555.555-55")]
    [InlineData("666.666.666-66")]
    [InlineData("777.777.777-77")]
    [InlineData("888.888.888-88")]
    [InlineData("99999999999")]
    public void CriarRejeitaCpfComTodosDigitosIguais(string entrada)
    {
        var resultado = Cpf.Criar(entrada);

        resultado.Erros.Should().ContainSingle().Which.Should().Be(PessoaErros.CpfInvalido);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CriarRejeitaCpfAusenteComMensagemDeCampoObrigatorio(string? entrada)
    {
        var resultado = Cpf.Criar(entrada);

        resultado.Sucesso.Should().BeFalse();
        resultado.Erros.Should().ContainSingle().Which.Should().Be(PessoaErros.CpfObrigatorio);
    }

    [Fact]
    public void CriarIgnoraEspacosNasPontas()
    {
        var resultado = Cpf.Criar("  529.982.247-25  ");

        resultado.Valor!.Numero.Should().Be(CpfValido);
    }

    [Fact]
    public void NumeroSempreTemOnzeDigitosSemMascara()
    {
        var cpf = Cpf.Criar("529.982.247-25").Valor!;

        cpf.Numero.Should().HaveLength(11).And.MatchRegex("^[0-9]{11}$");
    }

    [Fact]
    public void FormatadoAplicaMascara()
    {
        var cpf = Cpf.Criar(CpfValido).Valor!;

        cpf.Formatado().Should().Be("529.982.247-25");
    }

    [Fact]
    public void MascaradoOcultaInicioEDigitosVerificadores()
    {
        var cpf = Cpf.Criar(CpfValido).Valor!;

        cpf.Mascarado().Should().Be("***.982.247-**");
    }

    [Fact]
    public void CpfsComMesmoNumeroSaoIguais()
    {
        var comMascara = Cpf.Criar("529.982.247-25").Valor!;
        var semMascara = Cpf.Criar(CpfValido).Valor!;

        comMascara.Should().Be(semMascara);
    }

    [Fact]
    public void RestaurarReconstroiSemValidar()
    {
        var cpf = Cpf.Restaurar(CpfValido);

        cpf.Numero.Should().Be(CpfValido);
        cpf.Formatado().Should().Be("529.982.247-25");
    }
}
