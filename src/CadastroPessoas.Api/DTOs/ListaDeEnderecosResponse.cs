using System.Text.Json.Serialization;

namespace CadastroPessoas.Api.DTOs;

public sealed record ListaDeEnderecosResponse(
    [property: JsonPropertyName("Registros")] IReadOnlyList<EnderecoResponse> Registros);
