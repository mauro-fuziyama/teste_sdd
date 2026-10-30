using CadastroPessoas.Application.Pessoas.Modelos;
using CadastroPessoas.Domain.Comum;

namespace CadastroPessoas.Application.Pessoas.Portas.Entrada;

public interface IAtualizarPessoa
{
    Task<Result<PessoaResultado>> ExecutarAsync(int id, DadosPessoa dados, CancellationToken ct);
}
