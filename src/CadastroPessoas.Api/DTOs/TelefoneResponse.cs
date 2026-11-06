using System.Text.Json.Serialization;

namespace CadastroPessoas.Api.DTOs;

public sealed record TelefoneResponse(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("pessoaid")] int PessoaId,
    [property: JsonPropertyName("numero")] string Numero);
