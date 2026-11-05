using System.Text.Json.Serialization;

namespace CadastroPessoas.Api.DTOs;

public sealed record ErrosResponse([property: JsonPropertyName("Erros")] IReadOnlyList<string> Erros);
