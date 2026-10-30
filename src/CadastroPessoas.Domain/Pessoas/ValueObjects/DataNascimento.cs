using CadastroPessoas.Domain.Comum;

namespace CadastroPessoas.Domain.Pessoas.ValueObjects;

public sealed record DataNascimento
{
    public const int IdadeMaximaAnos = 130;

    public DateOnly Valor { get; }

    private DataNascimento(DateOnly valor) => Valor = valor;

    public static Result<DataNascimento> Criar(DateOnly? entrada, DateOnly hoje)
    {
        if (entrada is null)
            return PessoaErros.DataNascimentoObrigatoria;

        var data = entrada.Value;

        if (data > hoje)
            return PessoaErros.DataNascimentoFutura;

        if (data < DataMinima(hoje))
            return PessoaErros.IdadeAcimaDoLimite;

        return new DataNascimento(data);
    }

    public static DataNascimento Restaurar(DateOnly valor) => new(valor);

    private static DateOnly DataMinima(DateOnly hoje) => hoje.AddYears(-IdadeMaximaAnos);
}
