using System.Text.Json.Serialization;

namespace CadastroPessoas.Api.DTOs;

public sealed record CriarEnderecoRequest(
    [property: JsonPropertyName("pessoaid")] int? PessoaId,
    [property: JsonPropertyName("tipo")] int? Tipo,
    [property: JsonPropertyName("logradouro")] string? Logradouro,
    [property: JsonPropertyName("numero")] string? Numero,
    [property: JsonPropertyName("complemento")] string? Complemento,
    [property: JsonPropertyName("bairro")] string? Bairro,
    [property: JsonPropertyName("municipio")] string? Municipio,
    [property: JsonPropertyName("estado")] string? Estado,
    [property: JsonPropertyName("cep")] string? Cep);
