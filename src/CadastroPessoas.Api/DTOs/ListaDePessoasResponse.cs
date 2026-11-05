using System.Text.Json.Serialization;

namespace CadastroPessoas.Api.DTOs;

public sealed record ListaDePessoasResponse(
    [property: JsonPropertyName("Registros")] IReadOnlyList<PessoaResponse> Registros);
