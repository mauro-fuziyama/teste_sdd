using CadastroPessoas.Application.Comum;
using CadastroPessoas.Application.Pessoas.Portas.Entrada;
using CadastroPessoas.Application.Pessoas.Portas.Saida;
using CadastroPessoas.Domain.Comum;
using CadastroPessoas.Domain.Pessoas;

namespace CadastroPessoas.Application.Pessoas.CasosDeUso;

public sealed class ExcluirPessoa : IExcluirPessoa
{
    private readonly IPessoaRepositorio _repositorio;
    private readonly RelogioAplicacao _relogio;

    public ExcluirPessoa(IPessoaRepositorio repositorio, RelogioAplicacao relogio)
    {
        _repositorio = repositorio;
        _relogio = relogio;
    }

    public async Task<Result<bool>> ExecutarAsync(int id, CancellationToken ct)
    {
        var pessoa = await _repositorio.ObterPorIdAsync(id, ct);
        if (pessoa is null)
            return PessoaErros.NaoEncontrada;

        pessoa.Desativar(_relogio.Agora().Utc);
        await _repositorio.DesativarAsync(pessoa, ct);
        return true;
    }
}
