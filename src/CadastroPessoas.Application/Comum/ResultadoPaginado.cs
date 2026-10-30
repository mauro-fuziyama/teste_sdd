namespace CadastroPessoas.Application.Comum;

public sealed record ResultadoPaginado<T>(IReadOnlyList<T> Itens, int Pagina, int TamanhoPagina, int TotalItens)
{
    public int TotalPaginas => TamanhoPagina > 0 ? (int)Math.Ceiling(TotalItens / (double)TamanhoPagina) : 0;
}
