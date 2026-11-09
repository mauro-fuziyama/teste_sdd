using CadastroPessoas.Domain.Exceptions;

namespace CadastroPessoas.Domain.ValueObjects;

public sealed record NumeroDeTelefone
{
    private const int QuantidadeDeDigitosDoFixo = 10;
    private const int QuantidadeDeDigitosDoCelular = 11;
    private const int QuantidadeDeDigitosDoDdd = 2;
    private const int MenorDdd = 11;
    private const int MaiorDdd = 99;
    private const char DigitoInicialDoCelular = '9';

    public string Valor { get; }

    public NumeroDeTelefone(string? valor)
    {
        var erros = ObterErros(valor);

        if (erros.Count > 0)
        {
            throw new ValidacaoException(erros);
        }

        Valor = RemoverEspacosDasPontas(valor);
    }

    public static bool EhValido(string? valor)
    {
        return ObterErros(valor).Count == 0;
    }

    public static IReadOnlyList<string> ObterErros(string? valor)
    {
        var numero = RemoverEspacosDasPontas(valor);

        if (numero.Length == 0)
        {
            return [ErrosDeTelefone.NumeroObrigatorio];
        }

        if (!numero.All(char.IsAsciiDigit))
        {
            return [ErrosDeTelefone.NumeroSomenteComDigitos];
        }

        return ObterErrosDeFormato(numero);
    }

    private static List<string> ObterErrosDeFormato(string numero)
    {
        var erros = new List<string>();

        if (numero.Length is not (QuantidadeDeDigitosDoFixo or QuantidadeDeDigitosDoCelular))
        {
            erros.Add(ErrosDeTelefone.NumeroComTamanhoInvalido);
        }

        if (!DddEhValido(numero))
        {
            erros.Add(ErrosDeTelefone.DddInvalido);
        }

        if (EhCelularSemNoveNoTerceiroDigito(numero))
        {
            erros.Add(ErrosDeTelefone.CelularSemNoveNoTerceiroDigito);
        }

        return erros;
    }

    private static bool DddEhValido(string numero)
    {
        return numero.Length >= QuantidadeDeDigitosDoDdd
            && int.Parse(numero[..QuantidadeDeDigitosDoDdd]) is >= MenorDdd and <= MaiorDdd;
    }

    private static bool EhCelularSemNoveNoTerceiroDigito(string numero)
    {
        return numero.Length == QuantidadeDeDigitosDoCelular
            && numero[QuantidadeDeDigitosDoDdd] != DigitoInicialDoCelular;
    }

    private static string RemoverEspacosDasPontas(string? valor)
    {
        return valor?.Trim() ?? string.Empty;
    }
}
