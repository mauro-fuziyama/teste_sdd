namespace CadastroPessoas.Domain.Exceptions;

public sealed class TelefoneNaoEncontradoException() : RegistroNaoEncontradoException(ErrosDeTelefone.TelefoneNaoEncontrado);
