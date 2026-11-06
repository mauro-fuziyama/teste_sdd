using System.Text.Json.Serialization;

namespace CadastroPessoas.Api.DTOs;

public sealed record TelefoneRequest(
    [property: JsonPropertyName("pessoaid")] int? PessoaId,
    [property: JsonPropertyName("numero")] string? Numero);
