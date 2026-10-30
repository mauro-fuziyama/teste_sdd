using CadastroPessoas.Application.Pessoas.Modelos;
using CadastroPessoas.Domain.Comum;

namespace CadastroPessoas.Application.Pessoas.Portas.Entrada;

public interface ICadastrarPessoa
{
    Task<Result<PessoaResultado>> ExecutarAsync(DadosPessoa dados, CancellationToken ct);
}
