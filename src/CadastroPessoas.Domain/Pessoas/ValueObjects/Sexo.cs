using CadastroPessoas.Domain.Comum;

namespace CadastroPessoas.Domain.Pessoas.ValueObjects;

public sealed record Sexo
{
    public const string CodigosAceitos = "M, F, O, N";

    private static readonly Dictionary<string, TipoSexo> TiposPorCodigo = new(StringComparer.Ordinal)
    {
        ["M"] = TipoSexo.Masculino,
        ["F"] = TipoSexo.Feminino,
        ["O"] = TipoSexo.Outro,
        ["N"] = TipoSexo.NaoInformado
    };

    public string Codigo { get; }
    public TipoSexo Tipo { get; }

    private Sexo(string codigo, TipoSexo tipo)
    {
        Codigo = codigo;
        Tipo = tipo;
    }

    public static Result<Sexo> Criar(string? entrada)
    {
        if (string.IsNullOrWhiteSpace(entrada))
            return PessoaErros.SexoObrigatorio;

        var codigo = entrada.Trim().ToUpperInvariant();

        if (!TiposPorCodigo.TryGetValue(codigo, out var tipo))
            return PessoaErros.SexoInvalido;

        return new Sexo(codigo, tipo);
    }

    public static Sexo Restaurar(string codigo) => new(codigo, TiposPorCodigo[codigo]);
}
