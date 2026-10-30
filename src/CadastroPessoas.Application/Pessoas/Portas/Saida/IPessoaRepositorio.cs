using CadastroPessoas.Application.Pessoas.Modelos;
using CadastroPessoas.Domain.Pessoas;
using CadastroPessoas.Domain.Pessoas.ValueObjects;

namespace CadastroPessoas.Application.Pessoas.Portas.Saida;

/// <summary>
/// <see cref="ObterPorIdAsync"/> e <see cref="ListarAsync"/> consideram apenas pessoas ativas;
/// <see cref="ExisteCpfAsync"/> considera ativas e inativas, porque o índice único vale para todos os registros.
/// <see cref="AdicionarAsync"/> e <see cref="AtualizarAsync"/> lançam <see cref="CpfDuplicadoException"/>
/// quando o índice único de CPF é violado por gravações concorrentes.
/// </summary>
public interface IPessoaRepositorio
{
    Task<Pessoa?> ObterPorIdAsync(int id, CancellationToken ct);
    Task<bool> ExisteCpfAsync(Cpf cpf, int? ignorarId, CancellationToken ct);
    Task<int?> ObterIdInativoPorCpfAsync(Cpf cpf, CancellationToken ct);
    Task<(IReadOnlyList<Pessoa> Itens, int Total)> ListarAsync(FiltroPessoas filtro, CancellationToken ct);
    Task<int> AdicionarAsync(Pessoa pessoa, CancellationToken ct);
    Task AtualizarAsync(Pessoa pessoa, CancellationToken ct);
    Task ReativarAsync(Pessoa pessoa, CancellationToken ct);
    Task DesativarAsync(Pessoa pessoa, CancellationToken ct);
}
