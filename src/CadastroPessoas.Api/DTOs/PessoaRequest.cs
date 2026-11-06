using System.Text.Json.Serialization;

namespace CadastroPessoas.Api.DTOs;

public sealed record PessoaRequest(
    [property: JsonPropertyName("nome")] string? Nome,
    [property: JsonPropertyName("cpf")] string? Cpf,
    [property: JsonPropertyName("datadenascimento")] DateOnly? DataDeNascimento);
