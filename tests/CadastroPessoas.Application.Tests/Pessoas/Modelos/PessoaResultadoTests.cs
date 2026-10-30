using AwesomeAssertions;
using CadastroPessoas.Application.Pessoas.Modelos;
using CadastroPessoas.Domain.Pessoas;

namespace CadastroPessoas.Application.Tests.Pessoas.Modelos;

public class PessoaResultadoTests
{
    private static readonly DateTimeOffset CriadoEm = new(2026, 10, 30, 12, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset AtualizadoEm = new(2026, 11, 5, 9, 30, 0, TimeSpan.Zero);

    private static readonly Pessoa PessoaGravada = Pessoa.Restaurar(
        7, "Maria da Silva", new DateOnly(1990, 5, 17), "52998224725", "F", CriadoEm, AtualizadoEm);

    [Fact]
    public void DeRetornaCpfComMascara()
    {
        var resultado = PessoaResultado.De(PessoaGravada);

        resultado.Cpf.Should().Be("529.982.247-25");
    }

    [Fact]
    public void DeCopiaDemaisCamposDaPessoa()
    {
        var resultado = PessoaResultado.De(PessoaGravada);

        resultado.Id.Should().Be(7);
        resultado.Nome.Should().Be("Maria da Silva");
        resultado.DataNascimento.Should().Be(new DateOnly(1990, 5, 17));
        resultado.Sexo.Should().Be("F");
        resultado.CriadoEm.Should().Be(CriadoEm);
        resultado.AtualizadoEm.Should().Be(AtualizadoEm);
    }
}
