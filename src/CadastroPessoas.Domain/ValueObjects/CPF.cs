using CadastroPessoas.Domain.Exceptions;

namespace CadastroPessoas.Domain.ValueObjects;

public sealed record CPF
{
    private const int QuantidadeDeDigitos = 11;
    private const int QuantidadeDeDigitosBase = 9;

    public string Valor { get; }

    public CPF(string? valor)
    {
        if (!EhValido(valor))
        {
            throw new ValidacaoException([ErrosDePessoa.CpfInvalido]);
        }

        Valor = valor!;
    }

    public static bool EhValido(string? valor)
    {
        return valor is { Length: QuantidadeDeDigitos }
            && valor.All(char.IsAsciiDigit)
            && !TemTodosOsDigitosIguais(valor)
            && TemDigitosVerificadoresCorretos(valor);
    }

    private static bool TemTodosOsDigitosIguais(string valor)
    {
        return valor.All(digito => digito == valor[0]);
    }

    private static bool TemDigitosVerificadoresCorretos(string valor)
    {
        return ObterValorDoDigito(valor[QuantidadeDeDigitosBase]) == CalcularDigitoVerificador(valor, QuantidadeDeDigitosBase)
            && ObterValorDoDigito(valor[QuantidadeDeDigitosBase + 1]) == CalcularDigitoVerificador(valor, QuantidadeDeDigitosBase + 1);
    }

    private static int CalcularDigitoVerificador(string valor, int quantidadeDeDigitosConsiderados)
    {
        var pesoInicial = quantidadeDeDigitosConsiderados + 1;
        var soma = valor
            .Take(quantidadeDeDigitosConsiderados)
            .Select((digito, posicao) => ObterValorDoDigito(digito) * (pesoInicial - posicao))
            .Sum();
        var resto = soma % 11;

        return resto < 2 ? 0 : 11 - resto;
    }

    private static int ObterValorDoDigito(char digito)
    {
        return digito - '0';
    }
}
