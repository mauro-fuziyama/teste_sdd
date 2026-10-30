using AwesomeAssertions;
using CadastroPessoas.Application.Pessoas.Modelos;
using CadastroPessoas.Domain.Pessoas;

namespace CadastroPessoas.Application.Tests.Pessoas.Modelos;

public class DadosPessoaTests
{
    [Fact]
    public void ParaCamposRepassaOsValoresSemAlteracao()
    {
        var dados = new DadosPessoa(" Maria ", new DateOnly(1990, 5, 17), "529.982.247-25", "f");

        var campos = dados.ParaCampos();

        campos.Should().Be(new CamposPessoa(" Maria ", new DateOnly(1990, 5, 17), "529.982.247-25", "f"));
    }
}
