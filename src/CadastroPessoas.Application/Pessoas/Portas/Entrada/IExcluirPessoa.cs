using CadastroPessoas.Domain.Comum;

namespace CadastroPessoas.Application.Pessoas.Portas.Entrada;

public interface IExcluirPessoa
{
    Task<Result<bool>> ExecutarAsync(int id, CancellationToken ct);
}
