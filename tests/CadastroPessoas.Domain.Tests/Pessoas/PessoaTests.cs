using AwesomeAssertions;
using CadastroPessoas.Domain.Comum;
using CadastroPessoas.Domain.Pessoas;

namespace CadastroPessoas.Domain.Tests.Pessoas;

public class PessoaTests
{
    private static readonly MomentoAtual Criacao =
        new(new DateTimeOffset(2026, 10, 30, 12, 0, 0, TimeSpan.Zero), new DateOnly(2026, 10, 30));

    private static readonly MomentoAtual Alteracao =
        new(new DateTimeOffset(2026, 11, 5, 9, 30, 0, TimeSpan.Zero), new DateOnly(2026, 11, 5));

    private static readonly CamposPessoa CamposValidos =
        new("  Maria   da Silva ", new DateOnly(1990, 5, 17), "529.982.247-25", "f");

    private static readonly CamposPessoa OutrosCamposValidos =
        new("João Souza", new DateOnly(1985, 1, 2), "111.444.777-35", "M");

    [Fact]
    public void CriarComCamposValidosPreencheDadosEDatas()
    {
        var pessoa = Pessoa.Criar(CamposValidos, Criacao).Valor!;

        pessoa.Id.Should().Be(0);
        pessoa.Ativo.Should().BeTrue();
        pessoa.Nome.Valor.Should().Be("Maria da Silva");
        pessoa.DataNascimento.Valor.Should().Be(new DateOnly(1990, 5, 17));
        pessoa.Cpf.Numero.Should().Be("52998224725");
        pessoa.Sexo.Codigo.Should().Be("F");
        pessoa.CriadoEm.Should().Be(Criacao.Utc);
        pessoa.AtualizadoEm.Should().Be(Criacao.Utc);
    }

    [Fact]
    public void CriarComQuatroCamposInvalidosRetornaQuatroErrosNaOrdem()
    {
        var campos = new CamposPessoa("ab", Criacao.Hoje.AddDays(1), "529.982.247-26", "X");

        var resultado = Pessoa.Criar(campos, Criacao);

        resultado.Sucesso.Should().BeFalse();
        resultado.Erros.Should().Equal(
            PessoaErros.NomeTamanhoInvalido,
            PessoaErros.DataNascimentoFutura,
            PessoaErros.CpfInvalido,
            PessoaErros.SexoInvalido);
    }

    [Fact]
    public void CriarSemNenhumCampoRetornaErroDeObrigatorioParaTodos()
    {
        var resultado = Pessoa.Criar(new CamposPessoa(null, null, null, null), Criacao);

        resultado.Erros.Should().Equal(
            PessoaErros.NomeObrigatorio,
            PessoaErros.DataNascimentoObrigatoria,
            PessoaErros.CpfObrigatorio,
            PessoaErros.SexoObrigatorio);
    }

    [Fact]
    public void CriarValidaDataDeNascimentoPeloDiaLocalENaoPeloDiaUtc()
    {
        // 30/10 às 22h em Brasília já é 31/10 em UTC: o dia 31 continua sendo futuro.
        var momento = new MomentoAtual(new DateTimeOffset(2026, 10, 31, 1, 0, 0, TimeSpan.Zero), new DateOnly(2026, 10, 30));
        var campos = CamposValidos with { DataNascimento = new DateOnly(2026, 10, 31) };

        var resultado = Pessoa.Criar(campos, momento);

        resultado.Erros.Should().ContainSingle().Which.Should().Be(PessoaErros.DataNascimentoFutura);
    }

    [Fact]
    public void AtualizarComCamposValidosAlteraDadosEAtualizadoEmMantendoIdECriadoEm()
    {
        var pessoa = PessoaGravada();

        var resultado = pessoa.Atualizar(OutrosCamposValidos, Alteracao);

        resultado.Sucesso.Should().BeTrue();
        pessoa.Id.Should().Be(7);
        pessoa.CriadoEm.Should().Be(Criacao.Utc);
        pessoa.AtualizadoEm.Should().Be(Alteracao.Utc);
        pessoa.Nome.Valor.Should().Be("João Souza");
        pessoa.DataNascimento.Valor.Should().Be(new DateOnly(1985, 1, 2));
        pessoa.Cpf.Numero.Should().Be("11144477735");
        pessoa.Sexo.Codigo.Should().Be("M");
    }

    [Fact]
    public void AtualizarComCampoInvalidoNaoAlteraNenhumCampo()
    {
        var pessoa = PessoaGravada();
        var campos = OutrosCamposValidos with { Cpf = "123" };

        var resultado = pessoa.Atualizar(campos, Alteracao);

        resultado.Erros.Should().ContainSingle().Which.Should().Be(PessoaErros.CpfInvalido);
        pessoa.Nome.Valor.Should().Be("Maria da Silva");
        pessoa.DataNascimento.Valor.Should().Be(new DateOnly(1990, 5, 17));
        pessoa.Cpf.Numero.Should().Be("52998224725");
        pessoa.Sexo.Codigo.Should().Be("F");
        pessoa.AtualizadoEm.Should().Be(Criacao.Utc);
    }

    [Fact]
    public void RestaurarReconstroiPessoaAtivaSemValidarESemGerarId()
    {
        var atualizadoEm = Criacao.Utc.AddDays(3);

        var pessoa = Pessoa.Restaurar(42, "Jo", new DateOnly(1990, 5, 17), "52998224725", "N", Criacao.Utc, atualizadoEm);

        pessoa.Id.Should().Be(42);
        pessoa.Nome.Valor.Should().Be("Jo");
        pessoa.Cpf.Formatado().Should().Be("529.982.247-25");
        pessoa.Sexo.Codigo.Should().Be("N");
        pessoa.CriadoEm.Should().Be(Criacao.Utc);
        pessoa.AtualizadoEm.Should().Be(atualizadoEm);
        pessoa.Ativo.Should().BeTrue();
    }

    [Fact]
    public void DefinirIdAtribuiIdGeradoPeloBanco()
    {
        var pessoa = Pessoa.Criar(CamposValidos, Criacao).Valor!;

        pessoa.DefinirId(15);

        pessoa.Id.Should().Be(15);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void DefinirIdRejeitaIdNaoPositivo(int id)
    {
        var pessoa = Pessoa.Criar(CamposValidos, Criacao).Valor!;

        var acao = () => pessoa.DefinirId(id);

        acao.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void DesativarMarcaComoInativaEAtualizaAtualizadoEm()
    {
        var pessoa = PessoaGravada();

        pessoa.Desativar(Alteracao.Utc);

        pessoa.Ativo.Should().BeFalse();
        pessoa.AtualizadoEm.Should().Be(Alteracao.Utc);
        pessoa.CriadoEm.Should().Be(Criacao.Utc);
    }

    private static Pessoa PessoaGravada()
    {
        var pessoa = Pessoa.Criar(CamposValidos, Criacao).Valor!;
        pessoa.DefinirId(7);
        return pessoa;
    }
}
