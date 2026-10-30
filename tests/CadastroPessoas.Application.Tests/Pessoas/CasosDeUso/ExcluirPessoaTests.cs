using AwesomeAssertions;
using CadastroPessoas.Application.Pessoas.CasosDeUso;
using CadastroPessoas.Domain.Pessoas;

namespace CadastroPessoas.Application.Tests.Pessoas.CasosDeUso;

public class ExcluirPessoaTests
{
    private readonly Cenario _cenario = new();
    private readonly ExcluirPessoa _casoDeUso;

    public ExcluirPessoaTests() => _casoDeUso = new ExcluirPessoa(_cenario.Repositorio, _cenario.Relogio);

    [Fact]
    public async Task ExclusaoDesativaAPessoaSemRemoverORegistro()
    {
        var pessoa = _cenario.Gravar(Cenario.DadosValidos);
        var agora = _cenario.AvancarUmDia();

        var resultado = await _casoDeUso.ExecutarAsync(pessoa.Id, CancellationToken.None);

        resultado.Sucesso.Should().BeTrue();
        var gravado = _cenario.Repositorio.Gravado(pessoa.Id);
        gravado.Should().NotBeNull();
        gravado!.Ativo.Should().BeFalse();
        gravado.AtualizadoEm.Should().Be(agora);
    }

    [Fact]
    public async Task PessoaInexistenteRetornaNaoEncontrada()
    {
        var resultado = await _casoDeUso.ExecutarAsync(99, CancellationToken.None);

        resultado.Erros.Should().Equal(PessoaErros.NaoEncontrada);
    }

    [Fact]
    public async Task PessoaJaExcluidaRetornaNaoEncontrada()
    {
        var pessoa = _cenario.Gravar(Cenario.DadosValidos, ativo: false);

        var resultado = await _casoDeUso.ExecutarAsync(pessoa.Id, CancellationToken.None);

        resultado.Erros.Should().Equal(PessoaErros.NaoEncontrada);
    }
}
