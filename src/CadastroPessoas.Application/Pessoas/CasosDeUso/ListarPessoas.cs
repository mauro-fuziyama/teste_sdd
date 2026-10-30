using CadastroPessoas.Application.Comum;
using CadastroPessoas.Application.Pessoas.Modelos;
using CadastroPessoas.Application.Pessoas.Portas.Entrada;
using CadastroPessoas.Application.Pessoas.Portas.Saida;
using CadastroPessoas.Domain.Pessoas.ValueObjects;

namespace CadastroPessoas.Application.Pessoas.CasosDeUso;

public sealed class ListarPessoas : IListarPessoas
{
    private readonly IPessoaRepositorio _repositorio;

    public ListarPessoas(IPessoaRepositorio repositorio) => _repositorio = repositorio;

    public async Task<ResultadoPaginado<PessoaResultado>> ExecutarAsync(FiltroPessoas filtro, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(filtro);

        var (pessoas, total) = await _repositorio.ListarAsync(Normalizar(filtro), ct);
        var itens = pessoas.Select(PessoaResultado.De).ToList();

        return new ResultadoPaginado<PessoaResultado>(itens, filtro.Pagina, filtro.TamanhoPagina, total);
    }

    private static FiltroPessoas Normalizar(FiltroPessoas filtro) =>
        filtro with { Nome = TextoOuNulo(filtro.Nome), Cpf = CpfSemMascara(filtro.Cpf) };

    private static string? CpfSemMascara(string? cpf) =>
        TextoOuNulo(cpf) is { } texto ? Cpf.RemoverMascara(texto) : null;

    private static string? TextoOuNulo(string? texto) =>
        string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
}
