namespace CadastroPessoas.Domain.Exceptions;

public abstract class DominioException(IReadOnlyList<string> erros) : Exception(string.Join(" ", erros))
{
    public IReadOnlyList<string> Erros { get; } = erros;
}
