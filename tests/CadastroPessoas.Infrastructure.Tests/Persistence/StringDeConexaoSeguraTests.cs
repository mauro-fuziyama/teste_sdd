using CadastroPessoas.Infrastructure.ExternalServices;
using CadastroPessoas.Infrastructure.Persistence;
using CadastroPessoas.Infrastructure.Tests.Fakes;
using MySqlConnector;

namespace CadastroPessoas.Infrastructure.Tests.Persistence;

public sealed class StringDeConexaoSeguraTests
{
    private readonly string chaveEmBase64 = CriptografiaDeTeste.GerarChaveEmBase64();

    [Fact]
    public void ObterComSenhaDescriptografada_TrocaSomenteASenha()
    {
        var senhaCriptografada = CriptografiaDeTeste.Criptografar("senha-de-teste", chaveEmBase64);
        var stringOriginal = $"Server=localhost;Port=3306;Database=banco_de_teste;User=usuario_de_teste;Password={senhaCriptografada}";

        var resultado = StringDeConexaoSegura.ObterComSenhaDescriptografada(
            stringOriginal,
            new DescriptografadorDeSenha(chaveEmBase64));

        var construtor = new MySqlConnectionStringBuilder(resultado);
        Assert.Equal("senha-de-teste", construtor.Password);
        Assert.Equal("localhost", construtor.Server);
        Assert.Equal("banco_de_teste", construtor.Database);
        Assert.Equal("usuario_de_teste", construtor.UserID);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ObterComSenhaDescriptografada_SemStringDeConexao_LancaExcecao(string? stringDeConexao)
    {
        var excecao = Assert.Throws<InvalidOperationException>(() => StringDeConexaoSegura.ObterComSenhaDescriptografada(
            stringDeConexao,
            new DescriptografadorDeSenha(chaveEmBase64)));

        Assert.Equal(MensagensDeSeguranca.StringDeConexaoAusente, excecao.Message);
    }

    [Fact]
    public void ObterComSenhaDescriptografada_ComSenhaVazia_LancaExcecao()
    {
        var excecao = Assert.Throws<InvalidOperationException>(() => StringDeConexaoSegura.ObterComSenhaDescriptografada(
            "Server=localhost;User=usuario_de_teste;Password=",
            new DescriptografadorDeSenha(chaveEmBase64)));

        Assert.Equal(MensagensDeSeguranca.SenhaEmFormatoInvalido, excecao.Message);
    }
}
