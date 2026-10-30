using AwesomeAssertions;
using CadastroPessoas.Application.Pessoas.CasosDeUso;
using CadastroPessoas.Domain.Pessoas;

namespace CadastroPessoas.Application.Tests.Pessoas.CasosDeUso;

public class AtualizarPessoaTests
{
    private readonly Cenario _cenario = new();
    private readonly AtualizarPessoa _casoDeUso;

    public AtualizarPessoaTests() => _casoDeUso = new AtualizarPessoa(_cenario.Repositorio, _cenario.Relogio);

    [Fact]
    public async Task AtualizacaoValidaGravaNovosDadosEMantemIdECriadoEm()
    {
        var pessoa = _cenario.Gravar(Cenario.DadosValidos);
        var agora = _cenario.AvancarUmDia();

        var resultado = await _casoDeUso.ExecutarAsync(pessoa.Id, Cenario.OutrosDadosValidos, CancellationToken.None);

        resultado.Valor!.Id.Should().Be(pessoa.Id);
        resultado.Valor.Nome.Should().Be("João Souza");
        resultado.Valor.Cpf.Should().Be("111.444.777-35");
        resultado.Valor.CriadoEm.Should().Be(Cenario.Inicio);
        resultado.Valor.AtualizadoEm.Should().Be(agora);
        _cenario.Repositorio.Gravado(pessoa.Id)!.Nome.Should().Be("João Souza");
    }

    [Fact]
    public async Task PessoaInexistenteRetornaNaoEncontrada()
    {
        var resultado = await _casoDeUso.ExecutarAsync(99, Cenario.DadosValidos, CancellationToken.None);

        resultado.Erros.Should().Equal(PessoaErros.NaoEncontrada);
    }

    [Fact]
    public async Task DadosInvalidosRetornamErrosSemGravar()
    {
        var pessoa = _cenario.Gravar(Cenario.DadosValidos);
        var gravadoAntes = _cenario.Repositorio.Gravado(pessoa.Id);

        var resultado = await _casoDeUso.ExecutarAsync(
            pessoa.Id, Cenario.DadosValidos with { Nome = "ab" }, CancellationToken.None);

        resultado.Erros.Should().Equal(PessoaErros.NomeTamanhoInvalido);
        _cenario.Repositorio.Gravado(pessoa.Id).Should().Be(gravadoAntes);
    }

    [Fact]
    public async Task CpfDeOutraPessoaRetornaCpfDuplicadoSemAlterarNada()
    {
        var pessoa = _cenario.Gravar(Cenario.DadosValidos);
        _cenario.Gravar(Cenario.OutrosDadosValidos);
        var gravadoAntes = _cenario.Repositorio.Gravado(pessoa.Id);

        var resultado = await _casoDeUso.ExecutarAsync(
            pessoa.Id, Cenario.DadosValidos with { Nome = "Outro Nome", Cpf = "111.444.777-35" }, CancellationToken.None);

        resultado.Erros.Should().Equal(PessoaErros.CpfDuplicado);
        _cenario.Repositorio.Gravado(pessoa.Id).Should().Be(gravadoAntes);
    }

    [Fact]
    public async Task ManterOProprioCpfNaoContaComoDuplicado()
    {
        var pessoa = _cenario.Gravar(Cenario.DadosValidos);

        var resultado = await _casoDeUso.ExecutarAsync(
            pessoa.Id, Cenario.DadosValidos with { Nome = "Maria Souza" }, CancellationToken.None);

        resultado.Sucesso.Should().BeTrue();
        resultado.Valor!.Nome.Should().Be("Maria Souza");
    }

    [Fact]
    public async Task CpfDePessoaInativaRetornaCpfDuplicado()
    {
        var pessoa = _cenario.Gravar(Cenario.DadosValidos);
        _cenario.Gravar(Cenario.OutrosDadosValidos, ativo: false);

        var resultado = await _casoDeUso.ExecutarAsync(
            pessoa.Id, Cenario.DadosValidos with { Cpf = "111.444.777-35" }, CancellationToken.None);

        resultado.Erros.Should().Equal(PessoaErros.CpfDuplicado);
    }

    [Fact]
    public async Task CpfDuplicadoExceptionNaGravacaoViraCpfDuplicado()
    {
        var pessoa = _cenario.Gravar(Cenario.DadosValidos);
        _cenario.Repositorio.LancarCpfDuplicadoNaGravacao = true;

        var resultado = await _casoDeUso.ExecutarAsync(pessoa.Id, Cenario.OutrosDadosValidos, CancellationToken.None);

        resultado.Erros.Should().Equal(PessoaErros.CpfDuplicado);
    }

    [Fact]
    public async Task DadosNulosLancamArgumentNullException()
    {
        var acao = () => _casoDeUso.ExecutarAsync(1, null!, CancellationToken.None);

        await acao.Should().ThrowAsync<ArgumentNullException>();
    }
}
