namespace CadastroPessoas.Infrastructure.Persistence;

public sealed class RegistroCorrompidoException(string tipoDoRegistro, int id, string campoInvalido)
    : Exception($"O registro de {tipoDoRegistro} com id {id} tem o campo {campoInvalido} inválido no banco de dados.");
