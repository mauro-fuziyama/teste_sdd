using CadastroPessoas.Domain.Comum;

namespace CadastroPessoas.Domain.Pessoas.ValueObjects;

public sealed record Nome
{
    public const int TamanhoMinimo = 3;
    public const int TamanhoMaximo = 150;

    public string Valor { get; }

    private Nome(string valor) => Valor = valor;

    public static Result<Nome> Criar(string? entrada)
    {
        if (string.IsNullOrWhiteSpace(entrada))
            return PessoaErros.NomeObrigatorio;

        var nome = Normalizar(entrada);

        if (!TemTamanhoValido(nome))
            return PessoaErros.NomeTamanhoInvalido;

        return new Nome(nome);
    }

    public static Nome Restaurar(string valor) => new(valor);

    private static string Normalizar(string entrada) =>
        string.Join(' ', entrada.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));

    private static bool TemTamanhoValido(string nome) =>
        nome.Length is >= TamanhoMinimo and <= TamanhoMaximo;
}
