using System.Security.Cryptography;
using CadastroPessoas.Infrastructure.ExternalServices;
using CadastroPessoas.Infrastructure.Tests.Fakes;

namespace CadastroPessoas.Infrastructure.Tests.ExternalServices;

public sealed class DescriptografadorDeSenhaTests
{
    private readonly string chaveEmBase64 = CriptografiaDeTeste.GerarChaveEmBase64();

    [Theory]
    [InlineData("senha-simples")]
    [InlineData("sênhã com açentos ç €")]
    [InlineData("")]
    public void Descriptografar_ComValorNoFormatoDefinido_RetornaTextoOriginal(string textoOriginal)
    {
        var valorCriptografado = CriptografiaDeTeste.Criptografar(textoOriginal, chaveEmBase64);

        var resultado = new DescriptografadorDeSenha(chaveEmBase64).Descriptografar(valorCriptografado);

        Assert.Equal(textoOriginal, resultado);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("nao-e-base64!")]
    [InlineData("AAAAAAAAAAAAAAAAAAAAAA==")]
    public void Construtor_ComChaveInvalida_LancaExcecao(string? chaveInvalida)
    {
        var excecao = Assert.Throws<InvalidOperationException>(() => new DescriptografadorDeSenha(chaveInvalida));

        Assert.Equal(MensagensDeSeguranca.ChaveInvalida, excecao.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("nao-e-base64!")]
    [InlineData("AAAAAAAAAAAAAAAA")]
    public void Descriptografar_ComValorEmFormatoInvalido_LancaExcecao(string? valorInvalido)
    {
        var descriptografador = new DescriptografadorDeSenha(chaveEmBase64);

        var excecao = Assert.Throws<InvalidOperationException>(() => descriptografador.Descriptografar(valorInvalido));

        Assert.Equal(MensagensDeSeguranca.SenhaEmFormatoInvalido, excecao.Message);
    }

    [Fact]
    public void Descriptografar_ComOutraChave_LancaExcecaoSemExporValor()
    {
        var valorCriptografado = CriptografiaDeTeste.Criptografar("senha-secreta", chaveEmBase64);
        var descriptografador = new DescriptografadorDeSenha(CriptografiaDeTeste.GerarChaveEmBase64());

        var excecao = Assert.Throws<InvalidOperationException>(() => descriptografador.Descriptografar(valorCriptografado));

        Assert.Equal(MensagensDeSeguranca.SenhaNaoDescriptografada, excecao.Message);
    }

    [Fact]
    public void Descriptografar_ComValorAdulterado_LancaExcecao()
    {
        var bytes = Convert.FromBase64String(CriptografiaDeTeste.Criptografar("senha-secreta", chaveEmBase64));
        bytes[^1] ^= 0xFF;
        var descriptografador = new DescriptografadorDeSenha(chaveEmBase64);

        var excecao = Assert.Throws<InvalidOperationException>(() => descriptografador.Descriptografar(Convert.ToBase64String(bytes)));

        Assert.Equal(MensagensDeSeguranca.SenhaNaoDescriptografada, excecao.Message);
    }

    [Fact]
    public void Descriptografar_LeNonceTextoCifradoETagNaOrdemDefinida()
    {
        var chave = RandomNumberGenerator.GetBytes(32);
        var nonce = RandomNumberGenerator.GetBytes(12);
        var textoOriginal = "abc"u8.ToArray();
        var textoCifrado = new byte[textoOriginal.Length];
        var tag = new byte[16];
        using (var aesGcm = new AesGcm(chave, tag.Length))
        {
            aesGcm.Encrypt(nonce, textoOriginal, textoCifrado, tag);
        }
        var valor = Convert.ToBase64String([.. nonce, .. textoCifrado, .. tag]);

        var resultado = new DescriptografadorDeSenha(Convert.ToBase64String(chave)).Descriptografar(valor);

        Assert.Equal("abc", resultado);
    }
}
