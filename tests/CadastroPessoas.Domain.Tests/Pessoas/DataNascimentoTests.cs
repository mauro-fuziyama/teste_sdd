using AwesomeAssertions;
using CadastroPessoas.Domain.Pessoas;
using CadastroPessoas.Domain.Pessoas.ValueObjects;

namespace CadastroPessoas.Domain.Tests.Pessoas;

public class DataNascimentoTests
{
    private static readonly DateOnly Hoje = new(2026, 10, 30);

    [Fact]
    public void CriarAceitaHoje()
    {
        var resultado = DataNascimento.Criar(Hoje, Hoje);

        resultado.Valor!.Valor.Should().Be(Hoje);
    }

    [Fact]
    public void CriarAceitaDataPassada()
    {
        var resultado = DataNascimento.Criar(new DateOnly(1990, 5, 17), Hoje);

        resultado.Sucesso.Should().BeTrue();
    }

    [Fact]
    public void CriarRejeitaAmanha()
    {
        var resultado = DataNascimento.Criar(Hoje.AddDays(1), Hoje);

        resultado.Erros.Should().ContainSingle().Which.Should().Be(PessoaErros.DataNascimentoFutura);
    }

    [Fact]
    public void CriarAceitaExatamente130Anos()
    {
        var resultado = DataNascimento.Criar(Hoje.AddYears(-130), Hoje);

        resultado.Sucesso.Should().BeTrue();
    }

    [Fact]
    public void CriarRejeita130AnosEUmDia()
    {
        var resultado = DataNascimento.Criar(Hoje.AddYears(-130).AddDays(-1), Hoje);

        resultado.Erros.Should().ContainSingle().Which.Should().Be(PessoaErros.IdadeAcimaDoLimite);
    }

    [Fact]
    public void CriarRejeitaNuloComMensagemDeCampoObrigatorio()
    {
        var resultado = DataNascimento.Criar(null, Hoje);

        resultado.Erros.Should().ContainSingle().Which.Should().Be(PessoaErros.DataNascimentoObrigatoria);
    }

    [Fact]
    public void MensagensSaoIguaisAsDaSpec()
    {
        PessoaErros.DataNascimentoObrigatoria.Mensagem.Should().Be("O campo dataNascimento é obrigatório.");
        PessoaErros.DataNascimentoFutura.Mensagem.Should().Be("A data de nascimento não pode ser futura.");
        PessoaErros.IdadeAcimaDoLimite.Mensagem.Should().Be("A data de nascimento indica uma idade superior a 130 anos.");
    }

    [Fact]
    public void RestaurarReconstroiSemValidar()
    {
        var futura = Hoje.AddDays(10);

        DataNascimento.Restaurar(futura).Valor.Should().Be(futura);
    }
}
