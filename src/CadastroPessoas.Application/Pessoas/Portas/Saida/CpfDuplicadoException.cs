namespace CadastroPessoas.Application.Pessoas.Portas.Saida;

/// <summary>
/// Lançada pelo repositório quando a gravação viola o índice único de CPF (gravações concorrentes).
/// </summary>
public sealed class CpfDuplicadoException : Exception
{
    public CpfDuplicadoException()
        : base("Já existe uma pessoa cadastrada com este CPF.")
    {
    }

    public CpfDuplicadoException(string message)
        : base(message)
    {
    }

    public CpfDuplicadoException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
