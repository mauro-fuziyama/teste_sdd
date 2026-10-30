using AwesomeAssertions;
using CadastroPessoas.Application.Comum;

namespace CadastroPessoas.Application.Tests.Comum;

public class ResultadoPaginadoTests
{
    [Theory]
    [InlineData(0, 20, 0)]
    [InlineData(1, 20, 1)]
    [InlineData(20, 20, 1)]
    [InlineData(21, 20, 2)]
    [InlineData(100, 10, 10)]
    public void TotalPaginasArredondaParaCima(int totalItens, int tamanhoPagina, int esperado)
    {
        var resultado = new ResultadoPaginado<string>([], 1, tamanhoPagina, totalItens);

        resultado.TotalPaginas.Should().Be(esperado);
    }

    [Fact]
    public void TotalPaginasEhZeroQuandoTamanhoPaginaEhZero()
    {
        var resultado = new ResultadoPaginado<string>([], 1, 0, 5);

        resultado.TotalPaginas.Should().Be(0);
    }
}
