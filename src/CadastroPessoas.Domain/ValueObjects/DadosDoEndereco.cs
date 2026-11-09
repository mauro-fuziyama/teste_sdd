namespace CadastroPessoas.Domain.ValueObjects;

public sealed record DadosDoEndereco(
    int? Tipo,
    string? Logradouro,
    string? Numero,
    string? Complemento,
    string? Bairro,
    string? Municipio,
    string? Estado,
    string? Cep);
