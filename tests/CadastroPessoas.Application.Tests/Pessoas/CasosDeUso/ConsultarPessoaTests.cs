using AwesomeAssertions;
using CadastroPessoas.Application.Pessoas.CasosDeUso;
using CadastroPessoas.Domain.Pessoas;

namespace CadastroPessoas.Application.Tests.Pessoas.CasosDeUso;

public class ConsultarPessoaTests
{
    private readonly Cenario _cenario = new();
    private readonly ConsultarPessoa _casoDeUso;

    public ConsultarPessoaTests() => _casoDeUso = new ConsultarPessoa(_cenario.Repositorio);

    [Fact]
    public async Task PessoaEncontradaRetornaDadosComCpfFormatado()
    {
        var pessoa = _cenario.Gravar(Cenario.DadosValidos);

        var resultado = await _casoDeUso.ExecutarAsync(pessoa.Id, CancellationToken.None);

        resultado.Valor!.Id.Should().Be(pessoa.Id);
        resultado.Valor.Nome.Should().Be("Maria da Silva");
        resultado.Valor.Cpf.Should().Be("529.982.247-25");
    }

    [Fact]
    public async Task PessoaInexistenteRetornaNaoEncontrada()
    {
        var resultado = await _casoDeUso.ExecutarAsync(99, CancellationToken.None);

        resultado.Erros.Should().Equal(PessoaErros.NaoEncontrada);
    }

    [Fact]
    public async Task PessoaInativaRetornaNaoEncontrada()
    {
        var pessoa = _cenario.Gravar(Cenario.DadosValidos, ativo: false);

        var resultado = await _casoDeUso.ExecutarAsync(pessoa.Id, CancellationToken.None);

        resultado.Erros.Should().Equal(PessoaErros.NaoEncontrada);
    }
}
