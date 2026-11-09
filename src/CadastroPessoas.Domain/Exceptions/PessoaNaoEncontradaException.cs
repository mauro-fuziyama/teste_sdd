namespace CadastroPessoas.Domain.Exceptions;

public sealed class PessoaNaoEncontradaException() : RegistroNaoEncontradoException(ErrosDePessoa.PessoaNaoEncontrada);
