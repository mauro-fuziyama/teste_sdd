using System.Text.Json.Serialization;

namespace CadastroPessoas.Api.DTOs;

public sealed record PessoaResponse(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("nome")] string Nome,
    [property: JsonPropertyName("cpf")] string Cpf,
    [property: JsonPropertyName("datadenascimento")] DateOnly? DataDeNascimento,
    [property: JsonPropertyName("datadecadastro")] DateTime DataDeCadastro,
    [property: JsonPropertyName("dataatualizacao")] DateTime DataAtualizacao);
