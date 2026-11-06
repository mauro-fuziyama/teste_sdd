using System.Text.Json.Serialization;

namespace CadastroPessoas.Api.DTOs;

public sealed record ListaDeTelefonesResponse(
    [property: JsonPropertyName("Registros")] IReadOnlyList<TelefoneResponse> Registros);
