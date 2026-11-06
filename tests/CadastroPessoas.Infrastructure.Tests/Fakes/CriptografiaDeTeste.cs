using System.Security.Cryptography;
using System.Text;

namespace CadastroPessoas.Infrastructure.Tests.Fakes;

internal static class CriptografiaDeTeste
{
    private const int TamanhoDaChaveEmBytes = 32;
    private const int TamanhoDoNonceEmBytes = 12;
    private const int TamanhoDaTagEmBytes = 16;

    public static string GerarChaveEmBase64()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(TamanhoDaChaveEmBytes));
    }

    public static string Criptografar(string textoOriginal, string chaveEmBase64)
    {
        var bytesOriginais = Encoding.UTF8.GetBytes(textoOriginal);
        var nonce = RandomNumberGenerator.GetBytes(TamanhoDoNonceEmBytes);
        var textoCifrado = new byte[bytesOriginais.Length];
        var tag = new byte[TamanhoDaTagEmBytes];

        using var aesGcm = new AesGcm(Convert.FromBase64String(chaveEmBase64), TamanhoDaTagEmBytes);
        aesGcm.Encrypt(nonce, bytesOriginais, textoCifrado, tag);

        return Convert.ToBase64String([.. nonce, .. textoCifrado, .. tag]);
    }
}
