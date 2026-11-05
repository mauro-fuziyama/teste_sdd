using System.Text.Json.Serialization;

namespace CadastroPessoas.Api.DTOs;

public sealed record PessoaCriadaResponse([property: JsonPropertyName("id")] int Id);
