namespace CadastroPessoas.Domain.Exceptions;

public abstract class RegistroNaoEncontradoException(string erro) : DominioException([erro]);
