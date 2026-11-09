using CadastroPessoas.Domain.Exceptions;

namespace CadastroPessoas.Domain.ValueObjects;

public sealed record Cep
{
    private const int QuantidadeDeDigitosDoCepCurto = 5;
    private const int QuantidadeDeDigitosDoCepCompleto = 8;
    private const string Hifen = "-";

    public string Valor { get; }

    public Cep(string? valor)
    {
        var erros = ObterErros(valor);

        if (erros.Count > 0)
        {
            throw new ValidacaoException(erros);
        }

        Valor = Normalizar(valor);
    }

    public static bool EhValido(string? valor)
    {
        return ObterErros(valor).Count == 0;
    }

    public static IReadOnlyList<string> ObterErros(string? valor)
    {
        var cep = Normalizar(valor);
        var erros = new List<string>();

        if (!cep.All(char.IsAsciiDigit))
        {
            erros.Add(ErrosDeEndereco.CepSomenteComDigitos);
        }

        if (cep.Length is not (QuantidadeDeDigitosDoCepCurto or QuantidadeDeDigitosDoCepCompleto))
        {
            erros.Add(ErrosDeEndereco.CepComTamanhoInvalido);
        }

        return erros;
    }

    private static string Normalizar(string? valor)
    {
        return valor?.Trim().Replace(Hifen, string.Empty) ?? string.Empty;
    }
}
