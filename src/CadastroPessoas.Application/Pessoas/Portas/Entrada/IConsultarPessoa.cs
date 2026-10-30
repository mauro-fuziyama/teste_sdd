using CadastroPessoas.Application.Pessoas.Modelos;
using CadastroPessoas.Domain.Comum;

namespace CadastroPessoas.Application.Pessoas.Portas.Entrada;

public interface IConsultarPessoa
{
    Task<Result<PessoaResultado>> ExecutarAsync(int id, CancellationToken ct);
}
