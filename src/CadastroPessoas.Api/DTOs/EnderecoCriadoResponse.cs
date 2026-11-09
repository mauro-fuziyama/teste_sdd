using System.Text.Json.Serialization;

namespace CadastroPessoas.Api.DTOs;

public sealed record EnderecoCriadoResponse([property: JsonPropertyName("id")] int Id);
