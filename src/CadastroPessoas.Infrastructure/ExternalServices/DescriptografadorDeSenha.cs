using System.Security.Cryptography;
using System.Text;

namespace CadastroPessoas.Infrastructure.ExternalServices;

public sealed class DescriptografadorDeSenha
{
    private const int TamanhoDaChaveEmBytes = 32;
    private const int TamanhoDoNonceEmBytes = 12;
    private const int TamanhoDaTagEmBytes = 16;
    private const int TamanhoMinimoDoValorEmBytes = TamanhoDoNonceEmBytes + TamanhoDaTagEmBytes;

    private readonly byte[] chave;

    public DescriptografadorDeSenha(string? chaveEmBase64)
    {
        chave = ObterChave(chaveEmBase64);
    }

    public string Descriptografar(string? valorCriptografado)
    {
        var valor = ObterBytesDoValor(valorCriptografado);
        var nonce = valor.AsSpan(0, TamanhoDoNonceEmBytes);
        var tag = valor.AsSpan(valor.Length - TamanhoDaTagEmBytes);
        var textoCifrado = valor.AsSpan(TamanhoDoNonceEmBytes, valor.Length - TamanhoMinimoDoValorEmBytes);
        var textoOriginal = new byte[textoCifrado.Length];

        try
        {
            using var aesGcm = new AesGcm(chave, TamanhoDaTagEmBytes);
            aesGcm.Decrypt(nonce, textoCifrado, tag, textoOriginal);
        }
        catch (CryptographicException)
        {
            throw new InvalidOperationException(MensagensDeSeguranca.SenhaNaoDescriptografada);
        }

        return Encoding.UTF8.GetString(textoOriginal);
    }

    private static byte[] ObterChave(string? chaveEmBase64)
    {
        var chave = ConverterDeBase64(chaveEmBase64, MensagensDeSeguranca.ChaveInvalida);

        if (chave.Length != TamanhoDaChaveEmBytes)
        {
            throw new InvalidOperationException(MensagensDeSeguranca.ChaveInvalida);
        }

        return chave;
    }

    private static byte[] ObterBytesDoValor(string? valorCriptografado)
    {
        var valor = ConverterDeBase64(valorCriptografado, MensagensDeSeguranca.SenhaEmFormatoInvalido);

        if (valor.Length < TamanhoMinimoDoValorEmBytes)
        {
            throw new InvalidOperationException(MensagensDeSeguranca.SenhaEmFormatoInvalido);
        }

        return valor;
    }

    private static byte[] ConverterDeBase64(string? valorEmBase64, string mensagemDeErro)
    {
        if (string.IsNullOrWhiteSpace(valorEmBase64))
        {
            throw new InvalidOperationException(mensagemDeErro);
        }

        try
        {
            return Convert.FromBase64String(valorEmBase64);
        }
        catch (FormatException)
        {
            throw new InvalidOperationException(mensagemDeErro);
        }
    }
}
