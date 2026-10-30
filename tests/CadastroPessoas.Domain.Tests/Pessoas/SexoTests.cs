using AwesomeAssertions;
using CadastroPessoas.Domain.Pessoas;
using CadastroPessoas.Domain.Pessoas.ValueObjects;

namespace CadastroPessoas.Domain.Tests.Pessoas;

public class SexoTests
{
    [Theory]
    [InlineData("M", TipoSexo.Masculino)]
    [InlineData("F", TipoSexo.Feminino)]
    [InlineData("O", TipoSexo.Outro)]
    [InlineData("N", TipoSexo.NaoInformado)]
    public void CriarAceitaCodigosValidos(string entrada, TipoSexo tipoEsperado)
    {
        var sexo = Sexo.Criar(entrada).Valor!;

        sexo.Codigo.Should().Be(entrada);
        sexo.Tipo.Should().Be(tipoEsperado);
    }

    // Decisão do usuário (2026-10-30): minúsculas e espaços nas pontas são aceitos e o código é gravado em maiúscula.
    [Theory]
    [InlineData("m", "M")]
    [InlineData("f", "F")]
    [InlineData("o", "O")]
    [InlineData("n", "N")]
    [InlineData(" F ", "F")]
    public void CriarAceitaMinusculasEEspacosENormalizaParaMaiuscula(string entrada, string codigoEsperado)
    {
        var sexo = Sexo.Criar(entrada).Valor!;

        sexo.Codigo.Should().Be(codigoEsperado);
    }

    [Theory]
    [InlineData("X")]
    [InlineData("MF")]
    [InlineData("1")]
    [InlineData("Masculino")]
    public void CriarRejeitaValorForaDaLista(string entrada)
    {
        var resultado = Sexo.Criar(entrada);

        resultado.Erros.Should().ContainSingle().Which.Should().Be(PessoaErros.SexoInvalido);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void CriarRejeitaSexoAusenteComMensagemDeCampoObrigatorio(string? entrada)
    {
        var resultado = Sexo.Criar(entrada);

        resultado.Erros.Should().ContainSingle().Which.Should().Be(PessoaErros.SexoObrigatorio);
    }

    [Fact]
    public void MensagensSaoIguaisAsDaSpec()
    {
        PessoaErros.SexoObrigatorio.Mensagem.Should().Be("O campo sexo é obrigatório.");
        PessoaErros.SexoInvalido.Mensagem.Should().Be("Sexo inválido. Valores aceitos: M, F, O, N.");
    }

    [Fact]
    public void RestaurarReconstroiAPartirDoCodigo()
    {
        var sexo = Sexo.Restaurar("F");

        sexo.Codigo.Should().Be("F");
        sexo.Tipo.Should().Be(TipoSexo.Feminino);
    }
}
