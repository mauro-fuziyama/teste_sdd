namespace CadastroPessoas.Application.Pessoas.Modelos;

public sealed record FiltroPessoas(string? Nome, string? Cpf, int Pagina, int TamanhoPagina);
