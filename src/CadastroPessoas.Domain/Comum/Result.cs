using System.Diagnostics.CodeAnalysis;

namespace CadastroPessoas.Domain.Comum;

[SuppressMessage(
    "Design",
    "CA1000:Do not declare static members on generic types",
    Justification = "Result<T>.Ok e Result<T>.Falha são a API de criação definida no plano (seção 4.1).")]
public sealed class Result<T>
{
    public T? Valor { get; }
    public IReadOnlyList<Erro> Erros { get; }
    public bool Sucesso => Erros.Count == 0;

    private Result(T? valor, IReadOnlyList<Erro> erros)
    {
        Valor = valor;
        Erros = erros;
    }

    public static Result<T> Ok(T valor) => new(valor, []);
    public static Result<T> Falha(params Erro[] erros) => new(default, erros);

    public static implicit operator Result<T>(T valor) => Ok(valor);
    public static implicit operator Result<T>(Erro erro) => Falha(erro);
}
