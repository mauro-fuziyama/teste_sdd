using CadastroPessoas.Application.Comum;
using CadastroPessoas.Application.Pessoas.Modelos;

namespace CadastroPessoas.Application.Pessoas.Portas.Entrada;

public interface IListarPessoas
{
    Task<ResultadoPaginado<PessoaResultado>> ExecutarAsync(FiltroPessoas filtro, CancellationToken ct);
}
