using System.Text.Json.Serialization;

namespace CadastroPessoas.Api.DTOs;

public sealed record TelefoneCriadoResponse([property: JsonPropertyName("id")] int Id);
