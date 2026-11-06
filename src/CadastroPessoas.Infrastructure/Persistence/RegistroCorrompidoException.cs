namespace CadastroPessoas.Infrastructure.Persistence;

public sealed class RegistroCorrompidoException(int id)
    : Exception($"O registro de pessoa com id {id} tem um CPF inválido no banco de dados.");
