using AwesomeAssertions;
using CadastroPessoas.Application.Pessoas.CasosDeUso;
using CadastroPessoas.Application.Pessoas.Modelos;

namespace CadastroPessoas.Application.Tests.Pessoas.CasosDeUso;

public class ListarPessoasTests
{
    private readonly Cenario _cenario = new();
    private readonly ListarPessoas _casoDeUso;

    public ListarPessoasTests() => _casoDeUso = new ListarPessoas(_cenario.Repositorio);

    [Fact]
    public async Task PaginaRetornaItensOrdenadosPorNomeETotais()
    {
        GravarPessoas("Carla Dias", "Ana Lima", "Bruno Reis", "Davi Melo", "Eva Rocha");

        var resultado = await _casoDeUso.ExecutarAsync(new FiltroPessoas(null, null, 2, 2), CancellationToken.None);

        resultado.Itens.Select(p => p.Nome).Should().Equal("Carla Dias", "Davi Melo");
        resultado.Pagina.Should().Be(2);
        resultado.TamanhoPagina.Should().Be(2);
        resultado.TotalItens.Should().Be(5);
        resultado.TotalPaginas.Should().Be(3);
    }

    [Fact]
    public async Task ItensRetornamCpfFormatado()
    {
        _cenario.Gravar(Cenario.DadosValidos);

        var resultado = await _casoDeUso.ExecutarAsync(new FiltroPessoas(null, null, 1, 20), CancellationToken.None);

        resultado.Itens.Single().Cpf.Should().Be("529.982.247-25");
    }

    [Fact]
    public async Task FiltroDeCpfComMascaraEhNormalizado()
    {
        _cenario.Gravar(Cenario.DadosValidos);
        _cenario.Gravar(Cenario.OutrosDadosValidos);

        var resultado = await _casoDeUso.ExecutarAsync(
            new FiltroPessoas(null, " 529.982.247-25 ", 1, 20), CancellationToken.None);

        _cenario.Repositorio.UltimoFiltro!.Cpf.Should().Be("52998224725");
        resultado.Itens.Select(p => p.Nome).Should().Equal("Maria da Silva");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task FiltrosVaziosSaoIgnorados(string? filtroVazio)
    {
        await _casoDeUso.ExecutarAsync(new FiltroPessoas(filtroVazio, filtroVazio, 1, 20), CancellationToken.None);

        _cenario.Repositorio.UltimoFiltro!.Nome.Should().BeNull();
        _cenario.Repositorio.UltimoFiltro.Cpf.Should().BeNull();
    }

    [Fact]
    public async Task PessoasInativasNaoAparecem()
    {
        _cenario.Gravar(Cenario.DadosValidos, ativo: false);

        var resultado = await _casoDeUso.ExecutarAsync(new FiltroPessoas(null, null, 1, 20), CancellationToken.None);

        resultado.Itens.Should().BeEmpty();
        resultado.TotalItens.Should().Be(0);
        resultado.TotalPaginas.Should().Be(0);
    }

    private void GravarPessoas(params string[] nomes)
    {
        string[] cpfs = ["52998224725", "11144477735", "12345678909", "39053344705", "71428793860"];

        for (var i = 0; i < nomes.Length; i++)
            _cenario.Gravar(Cenario.DadosValidos with { Nome = nomes[i], Cpf = cpfs[i] });
    }
}
