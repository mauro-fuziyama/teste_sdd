using AwesomeAssertions;
using CadastroPessoas.Application.Pessoas.CasosDeUso;
using CadastroPessoas.Application.Pessoas.Modelos;
using CadastroPessoas.Domain.Pessoas;

namespace CadastroPessoas.Application.Tests.Pessoas.CasosDeUso;

public class CadastrarPessoaTests
{
    private readonly Cenario _cenario = new();
    private readonly CadastrarPessoa _casoDeUso;

    public CadastrarPessoaTests() => _casoDeUso = new CadastrarPessoa(_cenario.Repositorio, _cenario.Relogio);

    [Fact]
    public async Task CadastroValidoGravaERetornaAPessoa()
    {
        var resultado = await _casoDeUso.ExecutarAsync(Cenario.DadosValidos, CancellationToken.None);

        resultado.Sucesso.Should().BeTrue();
        resultado.Valor!.Id.Should().BePositive();
        resultado.Valor.Cpf.Should().Be("529.982.247-25");
        resultado.Valor.CriadoEm.Should().Be(Cenario.Inicio);
        resultado.Valor.AtualizadoEm.Should().Be(Cenario.Inicio);

        var gravado = _cenario.Repositorio.Gravado(resultado.Valor.Id);
        gravado!.Cpf.Should().Be("52998224725");
        gravado.Ativo.Should().BeTrue();
    }

    [Fact]
    public async Task DadosInvalidosRetornamErrosSemGravar()
    {
        var dados = new DadosPessoa("ab", null, "529.982.247-26", "X");

        var resultado = await _casoDeUso.ExecutarAsync(dados, CancellationToken.None);

        resultado.Erros.Should().Equal(
            PessoaErros.NomeTamanhoInvalido,
            PessoaErros.DataNascimentoObrigatoria,
            PessoaErros.CpfInvalido,
            PessoaErros.SexoInvalido);
        _cenario.Repositorio.Gravado(1).Should().BeNull();
    }

    [Fact]
    public async Task CpfDePessoaAtivaRetornaCpfDuplicado()
    {
        _cenario.Gravar(Cenario.DadosValidos);
        var dados = Cenario.OutrosDadosValidos with { Cpf = "52998224725" };

        var resultado = await _casoDeUso.ExecutarAsync(dados, CancellationToken.None);

        resultado.Erros.Should().Equal(PessoaErros.CpfDuplicado);
    }

    [Fact]
    public async Task CpfDePessoaInativaReaproveitaOMesmoRegistro()
    {
        var inativa = _cenario.Gravar(Cenario.DadosValidos, ativo: false);
        var agora = _cenario.AvancarUmDia();
        var dados = Cenario.OutrosDadosValidos with { Cpf = "529.982.247-25" };

        var resultado = await _casoDeUso.ExecutarAsync(dados, CancellationToken.None);

        resultado.Valor!.Id.Should().Be(inativa.Id);
        resultado.Valor.Nome.Should().Be("João Souza");
        resultado.Valor.CriadoEm.Should().Be(agora);
        resultado.Valor.AtualizadoEm.Should().Be(agora);
        _cenario.Repositorio.Gravado(inativa.Id)!.Ativo.Should().BeTrue();
    }

    [Fact]
    public async Task CpfDuplicadoExceptionNaGravacaoViraCpfDuplicado()
    {
        _cenario.Repositorio.LancarCpfDuplicadoNaGravacao = true;

        var resultado = await _casoDeUso.ExecutarAsync(Cenario.DadosValidos, CancellationToken.None);

        resultado.Erros.Should().Equal(PessoaErros.CpfDuplicado);
    }

    [Fact]
    public async Task DataDeAmanhaNoFusoConfiguradoEhFuturaMesmoQuandoEmUtcJaEhHoje()
    {
        _cenario.Tempo.SetUtcNow(new DateTimeOffset(2026, 10, 31, 1, 0, 0, TimeSpan.Zero));
        var dataFuturaEmBrasilia = Cenario.DadosValidos with { DataNascimento = new DateOnly(2026, 10, 31) };

        var resultado = await _casoDeUso.ExecutarAsync(dataFuturaEmBrasilia, CancellationToken.None);

        resultado.Erros.Should().Equal(PessoaErros.DataNascimentoFutura);
    }
}
