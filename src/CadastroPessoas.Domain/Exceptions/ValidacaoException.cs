namespace CadastroPessoas.Domain.Exceptions;

public sealed class ValidacaoException(IReadOnlyList<string> erros) : DominioException(erros);
