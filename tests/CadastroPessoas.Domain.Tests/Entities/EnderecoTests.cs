using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Enums;
using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.ValueObjects;

namespace CadastroPessoas.Domain.Tests.Entities;

public class EnderecoTests
{
    private const int PessoaIdValido = 7;
    private const int IdExistente = 15;

    private static readonly DadosDoEndereco DadosValidos = new(
        (int)TipoDeEndereco.Comercial,
        "Avenida Paulista",
        "1000",
        "Sala 12",
        "Bela Vista",
        "São Paulo",
        "SP",
        "01310100");

    private static readonly DadosDoEndereco DadosSomenteComObrigatorios =
        new(null, null, null, null, null, "Curitiba", "PR", null);

    [Fact]
    public void Criar_ComDadosValidos_PreencheOsCampos()
    {
        var endereco = Endereco.Criar(PessoaIdValido, DadosValidos);

        Assert.Equal(0, endereco.Id);
        Assert.Equal(PessoaIdValido, endereco.PessoaId);
        Assert.Equal(TipoDeEndereco.Comercial, endereco.Tipo);
        Assert.Equal("Avenida Paulista", endereco.Logradouro);
        Assert.Equal("1000", endereco.Numero);
        Assert.Equal("Sala 12", endereco.Complemento);
        Assert.Equal("Bela Vista", endereco.Bairro);
        Assert.Equal("São Paulo", endereco.Municipio);
        Assert.Equal("SP", endereco.Estado.Valor);
        Assert.Equal("01310100", endereco.Cep?.Valor);
    }

    [Fact]
    public void Criar_SomenteComObrigatorios_DeixaOsOpcionaisNulosEOTipoNaoInformado()
    {
        var endereco = Endereco.Criar(PessoaIdValido, DadosSomenteComObrigatorios);

        Assert.Equal(TipoDeEndereco.NaoInformado, endereco.Tipo);
        AssertCamposOpcionaisNulos(endereco);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_ComOpcionaisEmBranco_TrataComoNaoInformados(string valorEmBranco)
    {
        var dados = DadosSomenteComObrigatorios with
        {
            Logradouro = valorEmBranco,
            Numero = valorEmBranco,
            Complemento = valorEmBranco,
            Bairro = valorEmBranco,
            Cep = valorEmBranco
        };

        var endereco = Endereco.Criar(PessoaIdValido, dados);

        AssertCamposOpcionaisNulos(endereco);
    }

    [Fact]
    public void Criar_ComEspacosNasPontas_GuardaOsTextosSemOsEspacos()
    {
        var dados = new DadosDoEndereco(null, " Rua A ", " 10 ", " Fundos ", " Centro ", " Curitiba ", " pr ", " 80010 ");

        var endereco = Endereco.Criar(PessoaIdValido, dados);

        Assert.Equal("Rua A", endereco.Logradouro);
        Assert.Equal("10", endereco.Numero);
        Assert.Equal("Fundos", endereco.Complemento);
        Assert.Equal("Centro", endereco.Bairro);
        Assert.Equal("Curitiba", endereco.Municipio);
        Assert.Equal("PR", endereco.Estado.Valor);
        Assert.Equal("80010", endereco.Cep?.Valor);
    }

    [Fact]
    public void Criar_ComEstadoEmMinusculas_GuardaEmMaiusculas()
    {
        var endereco = Endereco.Criar(PessoaIdValido, DadosValidos with { Estado = "sp" });

        Assert.Equal("SP", endereco.Estado.Valor);
    }

    [Fact]
    public void Criar_ComCepComHifen_GuardaOCepSemOHifen()
    {
        var endereco = Endereco.Criar(PessoaIdValido, DadosValidos with { Cep = "01310-100" });

        Assert.Equal("01310100", endereco.Cep?.Valor);
    }

    [Theory]
    [InlineData(0, TipoDeEndereco.NaoInformado)]
    [InlineData(1, TipoDeEndereco.Residencial)]
    [InlineData(2, TipoDeEndereco.Comercial)]
    [InlineData(3, TipoDeEndereco.Outros)]
    public void Criar_ComTipoDoEnum_GuardaOTipo(int tipo, TipoDeEndereco tipoEsperado)
    {
        var endereco = Endereco.Criar(PessoaIdValido, DadosValidos with { Tipo = tipo });

        Assert.Equal(tipoEsperado, endereco.Tipo);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    public void Criar_ComTipoForaDoEnum_LancaErroDeTipo(int tipoInvalido)
    {
        AssertErrosNaCriacao(DadosValidos with { Tipo = tipoInvalido }, ErrosDeEndereco.TipoInvalido);
    }

    [Fact]
    public void Criar_SemPessoaId_LancaErroDePessoaIdObrigatorio()
    {
        var excecao = Assert.Throws<ValidacaoException>(() => Endereco.Criar(null, DadosValidos));

        Assert.Equal([ErrosDeEndereco.PessoaIdObrigatorio], excecao.Erros);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_SemMunicipio_LancaErroDeMunicipioObrigatorio(string? municipio)
    {
        AssertErrosNaCriacao(DadosValidos with { Municipio = municipio }, ErrosDeEndereco.MunicipioObrigatorio);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_SemEstado_LancaErroDeEstadoObrigatorio(string? estado)
    {
        AssertErrosNaCriacao(DadosValidos with { Estado = estado }, ErrosDeEndereco.EstadoObrigatorio);
    }

    [Theory]
    [InlineData("0131", ErrosDeEndereco.CepComTamanhoInvalido)]
    [InlineData("0131a", ErrosDeEndereco.CepSomenteComDigitos)]
    public void Criar_ComCepInvalido_LancaErroDeCep(string cep, string erroEsperado)
    {
        AssertErrosNaCriacao(DadosValidos with { Cep = cep }, erroEsperado);
    }

    [Theory]
    [InlineData(nameof(DadosDoEndereco.Logradouro), 100)]
    [InlineData(nameof(DadosDoEndereco.Numero), 20)]
    [InlineData(nameof(DadosDoEndereco.Complemento), 100)]
    [InlineData(nameof(DadosDoEndereco.Bairro), 100)]
    [InlineData(nameof(DadosDoEndereco.Municipio), 100)]
    public void Criar_ComTextoNoTamanhoMaximo_NaoRetornaErros(string campo, int tamanhoMaximo)
    {
        var dados = DadosComCampo(campo, new string('a', tamanhoMaximo));

        Assert.Empty(Endereco.ValidarCriacao(PessoaIdValido, dados));
    }

    [Theory]
    [InlineData(nameof(DadosDoEndereco.Logradouro), 100, ErrosDeEndereco.LogradouroComTamanhoInvalido)]
    [InlineData(nameof(DadosDoEndereco.Numero), 20, ErrosDeEndereco.NumeroComTamanhoInvalido)]
    [InlineData(nameof(DadosDoEndereco.Complemento), 100, ErrosDeEndereco.ComplementoComTamanhoInvalido)]
    [InlineData(nameof(DadosDoEndereco.Bairro), 100, ErrosDeEndereco.BairroComTamanhoInvalido)]
    [InlineData(nameof(DadosDoEndereco.Municipio), 100, ErrosDeEndereco.MunicipioComTamanhoInvalido)]
    public void Criar_ComTextoAcimaDoTamanhoMaximo_LancaErroDeTamanho(string campo, int tamanhoMaximo, string erroEsperado)
    {
        AssertErrosNaCriacao(DadosComCampo(campo, new string('a', tamanhoMaximo + 1)), erroEsperado);
    }

    [Fact]
    public void Criar_ComTextoNoLimiteDepoisDeRemoverEspacos_NaoRetornaErros()
    {
        var dados = DadosComCampo(nameof(DadosDoEndereco.Numero), $"  {new string('1', 20)}  ");

        Assert.Empty(Endereco.ValidarCriacao(PessoaIdValido, dados));
    }

    [Fact]
    public void Criar_ComVariosCamposInvalidos_ReuneTodosOsErros()
    {
        var dados = new DadosDoEndereco(9, new string('a', 101), null, null, null, " ", "XX", "abc");

        var excecao = Assert.Throws<ValidacaoException>(() => Endereco.Criar(null, dados));

        Assert.Equal(
            [
                ErrosDeEndereco.TipoInvalido,
                ErrosDeEndereco.LogradouroComTamanhoInvalido,
                ErrosDeEndereco.MunicipioObrigatorio,
                ErrosDeEndereco.SiglaDeEstadoInvalida,
                ErrosDeEndereco.CepSomenteComDigitos,
                ErrosDeEndereco.CepComTamanhoInvalido,
                ErrosDeEndereco.PessoaIdObrigatorio
            ],
            excecao.Erros);
    }

    [Fact]
    public void ValidarCriacao_ComDadosValidos_NaoRetornaErros()
    {
        Assert.Empty(Endereco.ValidarCriacao(PessoaIdValido, DadosValidos));
    }

    [Fact]
    public void Reconstituir_PreencheOsCampos()
    {
        var endereco = Endereco.Reconstituir(IdExistente, PessoaIdValido, DadosValidos);

        Assert.Equal(IdExistente, endereco.Id);
        Assert.Equal(PessoaIdValido, endereco.PessoaId);
        Assert.Equal(TipoDeEndereco.Comercial, endereco.Tipo);
        Assert.Equal("São Paulo", endereco.Municipio);
        Assert.Equal("SP", endereco.Estado.Valor);
        Assert.Equal("01310100", endereco.Cep?.Valor);
    }

    [Fact]
    public void Atualizar_ComDadosValidos_AlteraOsCamposSemAlterarIdEPessoaId()
    {
        var endereco = CriarEnderecoExistente();
        var novosDados = new DadosDoEndereco((int)TipoDeEndereco.Outros, "Rua B", "20", "Casa", "Batel", "Curitiba", "pr", "80420");

        endereco.Atualizar(novosDados);

        Assert.Equal(IdExistente, endereco.Id);
        Assert.Equal(PessoaIdValido, endereco.PessoaId);
        Assert.Equal(TipoDeEndereco.Outros, endereco.Tipo);
        Assert.Equal("Rua B", endereco.Logradouro);
        Assert.Equal("20", endereco.Numero);
        Assert.Equal("Casa", endereco.Complemento);
        Assert.Equal("Batel", endereco.Bairro);
        Assert.Equal("Curitiba", endereco.Municipio);
        Assert.Equal("PR", endereco.Estado.Valor);
        Assert.Equal("80420", endereco.Cep?.Valor);
    }

    [Fact]
    public void Atualizar_SemTipo_MantemOTipoAtual()
    {
        var endereco = CriarEnderecoExistente();

        endereco.Atualizar(DadosSomenteComObrigatorios);

        Assert.Equal(TipoDeEndereco.Comercial, endereco.Tipo);
    }

    [Fact]
    public void Atualizar_ComTipoNaoInformado_AlteraOTipo()
    {
        var endereco = CriarEnderecoExistente();

        endereco.Atualizar(DadosValidos with { Tipo = (int)TipoDeEndereco.NaoInformado });

        Assert.Equal(TipoDeEndereco.NaoInformado, endereco.Tipo);
    }

    [Fact]
    public void Atualizar_SemCamposOpcionais_DeixaOsOpcionaisNulos()
    {
        var endereco = CriarEnderecoExistente();

        endereco.Atualizar(DadosSomenteComObrigatorios);

        AssertCamposOpcionaisNulos(endereco);
    }

    [Theory]
    [InlineData(null, "SP", ErrosDeEndereco.MunicipioObrigatorio)]
    [InlineData(" ", "SP", ErrosDeEndereco.MunicipioObrigatorio)]
    [InlineData("São Paulo", null, ErrosDeEndereco.EstadoObrigatorio)]
    [InlineData("São Paulo", " ", ErrosDeEndereco.EstadoObrigatorio)]
    [InlineData("São Paulo", "XX", ErrosDeEndereco.SiglaDeEstadoInvalida)]
    public void Atualizar_ComMunicipioOuEstadoInvalido_LancaErroENaoAlteraOsDados(
        string? municipio,
        string? estado,
        string erroEsperado)
    {
        var endereco = CriarEnderecoExistente();
        var dados = new DadosDoEndereco(null, "Rua B", null, null, null, municipio, estado, null);

        var excecao = Assert.Throws<ValidacaoException>(() => endereco.Atualizar(dados));

        Assert.Equal([erroEsperado], excecao.Erros);
        Assert.Equal("Avenida Paulista", endereco.Logradouro);
        Assert.Equal("São Paulo", endereco.Municipio);
        Assert.Equal("SP", endereco.Estado.Valor);
    }

    private static Endereco CriarEnderecoExistente()
    {
        return Endereco.Reconstituir(IdExistente, PessoaIdValido, DadosValidos);
    }

    private static DadosDoEndereco DadosComCampo(string campo, string valor)
    {
        return campo switch
        {
            nameof(DadosDoEndereco.Logradouro) => DadosValidos with { Logradouro = valor },
            nameof(DadosDoEndereco.Numero) => DadosValidos with { Numero = valor },
            nameof(DadosDoEndereco.Complemento) => DadosValidos with { Complemento = valor },
            nameof(DadosDoEndereco.Bairro) => DadosValidos with { Bairro = valor },
            nameof(DadosDoEndereco.Municipio) => DadosValidos with { Municipio = valor },
            _ => throw new ArgumentOutOfRangeException(nameof(campo), campo, "Campo sem limite de tamanho.")
        };
    }

    private static void AssertErrosNaCriacao(DadosDoEndereco dados, params string[] errosEsperados)
    {
        var excecao = Assert.Throws<ValidacaoException>(() => Endereco.Criar(PessoaIdValido, dados));

        Assert.Equal(errosEsperados, excecao.Erros);
    }

    private static void AssertCamposOpcionaisNulos(Endereco endereco)
    {
        Assert.Null(endereco.Logradouro);
        Assert.Null(endereco.Numero);
        Assert.Null(endereco.Complemento);
        Assert.Null(endereco.Bairro);
        Assert.Null(endereco.Cep);
    }
}
