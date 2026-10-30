using CadastroPessoas.Domain.Comum;

namespace CadastroPessoas.Domain.Pessoas.ValueObjects;

public sealed record Cpf
{
    private const int QuantidadeDigitos = 11;
    private const int PosicaoPrimeiroVerificador = 9;
    private const int PosicaoSegundoVerificador = 10;

    public string Numero { get; }

    private Cpf(string numero) => Numero = numero;

    public static Result<Cpf> Criar(string? entrada)
    {
        if (string.IsNullOrWhiteSpace(entrada))
            return PessoaErros.CpfObrigatorio;

        var digitos = RemoverMascara(entrada);

        if (!EhValido(digitos))
            return PessoaErros.CpfInvalido;

        return new Cpf(digitos);
    }

    public static Cpf Restaurar(string numero) => new(numero);

    public string Formatado() =>
        $"{Numero[..3]}.{Numero[3..6]}.{Numero[6..9]}-{Numero[9..]}";

    public string Mascarado() =>
        $"***.{Numero[3..6]}.{Numero[6..9]}-**";

    public static string RemoverMascara(string entrada)
    {
        ArgumentNullException.ThrowIfNull(entrada);
        return entrada.Trim().Replace(".", "", StringComparison.Ordinal).Replace("-", "", StringComparison.Ordinal);
    }


    private static bool EhValido(string digitos) =>
        TemFormatoCorreto(digitos)
        && !TemTodosDigitosIguais(digitos)
        && DigitosVerificadoresConferem(digitos);

    private static bool TemFormatoCorreto(string digitos) =>
        digitos.Length == QuantidadeDigitos && digitos.All(char.IsAsciiDigit);

    private static bool TemTodosDigitosIguais(string digitos) =>
        digitos.Distinct().Count() == 1;

    private static bool DigitosVerificadoresConferem(string digitos) =>
        DigitoConfere(digitos, PosicaoPrimeiroVerificador)
        && DigitoConfere(digitos, PosicaoSegundoVerificador);

    private static bool DigitoConfere(string digitos, int posicao) =>
        ValorDoDigito(digitos[posicao]) == CalcularDigitoVerificador(digitos, posicao);

    private static int CalcularDigitoVerificador(string digitos, int quantidadeBase)
    {
        var pesoInicial = quantidadeBase + 1;
        var soma = digitos
            .Take(quantidadeBase)
            .Select((digito, indice) => ValorDoDigito(digito) * (pesoInicial - indice))
            .Sum();

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    private static int ValorDoDigito(char digito) => digito - '0';
}
