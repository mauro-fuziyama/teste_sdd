using System.Security.Cryptography;

namespace CadastroPessoas.Infrastructure.Tests.Persistence;

internal static class GeradorDeCpf
{
    private const int QuantidadeDeDigitosBase = 9;

    public static string Gerar()
    {
        var digitos = Enumerable
            .Range(0, QuantidadeDeDigitosBase)
            .Select(_ => RandomNumberGenerator.GetInt32(10))
            .ToList();

        digitos.Add(CalcularDigitoVerificador(digitos));
        digitos.Add(CalcularDigitoVerificador(digitos));

        return string.Concat(digitos);
    }

    private static int CalcularDigitoVerificador(IReadOnlyList<int> digitos)
    {
        var pesoInicial = digitos.Count + 1;
        var soma = digitos.Select((digito, posicao) => digito * (pesoInicial - posicao)).Sum();
        var resto = soma % 11;

        return resto < 2 ? 0 : 11 - resto;
    }
}
