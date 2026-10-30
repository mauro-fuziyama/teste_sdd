using AwesomeAssertions;
using CadastroPessoas.Domain.Pessoas;
using CadastroPessoas.Domain.Pessoas.ValueObjects;

namespace CadastroPessoas.Domain.Tests.Pessoas;

public class NomeTests
{
    [Fact]
    public void CriarRemoveEspacosNasPontas()
    {
        var resultado = Nome.Criar("   Maria da Silva   ");

        resultado.Valor!.Valor.Should().Be("Maria da Silva");
    }

    [Fact]
    public void CriarReduzEspacosInternosRepetidos()
    {
        var resultado = Nome.Criar("Maria    da     Silva");

        resultado.Valor!.Valor.Should().Be("Maria da Silva");
    }

    [Theory]
    [InlineData(3)]
    [InlineData(150)]
    public void CriarAceitaTamanhoNosLimites(int tamanho)
    {
        var resultado = Nome.Criar(new string('a', tamanho));

        resultado.Sucesso.Should().BeTrue();
    }

    [Theory]
    [InlineData(2)]
    [InlineData(151)]
    public void CriarRejeitaTamanhoForaDosLimites(int tamanho)
    {
        var resultado = Nome.Criar(new string('a', tamanho));

        resultado.Erros.Should().ContainSingle().Which.Should().Be(PessoaErros.NomeTamanhoInvalido);
    }

    [Fact]
    public void CriarMedeTamanhoDepoisDeNormalizar()
    {
        var resultado = Nome.Criar("  ab  ");

        resultado.Erros.Should().ContainSingle().Which.Should().Be(PessoaErros.NomeTamanhoInvalido);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void CriarRejeitaNomeAusenteComMensagemDeCampoObrigatorio(string? entrada)
    {
        var resultado = Nome.Criar(entrada);

        resultado.Erros.Should().ContainSingle().Which.Should().Be(PessoaErros.NomeObrigatorio);
    }

    [Fact]
    public void MensagensSaoIguaisAsDaSpec()
    {
        PessoaErros.NomeObrigatorio.Mensagem.Should().Be("O campo nome é obrigatório.");
        PessoaErros.NomeTamanhoInvalido.Mensagem.Should().Be("O nome deve ter entre 3 e 150 caracteres.");
    }

    [Fact]
    public void RestaurarReconstroiSemValidar()
    {
        Nome.Restaurar("Jo").Valor.Should().Be("Jo");
    }
}
