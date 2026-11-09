using CadastroPessoas.Domain.Exceptions;

namespace CadastroPessoas.Domain.ValueObjects;

public sealed record SiglaDeEstado
{
    private const int QuantidadeDeCaracteres = 2;

    private static readonly HashSet<string> SiglasDasUnidadesFederativas =
    [
        "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO",
        "MA", "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI",
        "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO"
    ];

    public string Valor { get; }

    public SiglaDeEstado(string? valor)
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
        var sigla = Normalizar(valor);

        if (sigla.Length == 0)
        {
            return [ErrosDeEndereco.EstadoObrigatorio];
        }

        if (sigla.Length != QuantidadeDeCaracteres)
        {
            return [ErrosDeEndereco.EstadoComTamanhoInvalido];
        }

        return SiglasDasUnidadesFederativas.Contains(sigla) ? [] : [ErrosDeEndereco.SiglaDeEstadoInvalida];
    }

    private static string Normalizar(string? valor)
    {
        return valor?.Trim().ToUpperInvariant() ?? string.Empty;
    }
}
