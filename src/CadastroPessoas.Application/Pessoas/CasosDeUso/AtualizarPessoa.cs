using CadastroPessoas.Application.Comum;
using CadastroPessoas.Application.Pessoas.Modelos;
using CadastroPessoas.Application.Pessoas.Portas.Entrada;
using CadastroPessoas.Application.Pessoas.Portas.Saida;
using CadastroPessoas.Domain.Comum;
using CadastroPessoas.Domain.Pessoas;

namespace CadastroPessoas.Application.Pessoas.CasosDeUso;

public sealed class AtualizarPessoa : IAtualizarPessoa
{
    private readonly IPessoaRepositorio _repositorio;
    private readonly RelogioAplicacao _relogio;

    public AtualizarPessoa(IPessoaRepositorio repositorio, RelogioAplicacao relogio)
    {
        _repositorio = repositorio;
        _relogio = relogio;
    }

    public async Task<Result<PessoaResultado>> ExecutarAsync(int id, DadosPessoa dados, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dados);

        var pessoa = await _repositorio.ObterPorIdAsync(id, ct);
        if (pessoa is null)
            return PessoaErros.NaoEncontrada;

        var atualizacao = pessoa.Atualizar(dados.ParaCampos(), _relogio.Agora());
        if (!atualizacao.Sucesso)
            return Result<PessoaResultado>.Falha([.. atualizacao.Erros]);

        return await GravarAsync(pessoa, ct);
    }

    private async Task<Result<PessoaResultado>> GravarAsync(Pessoa pessoa, CancellationToken ct)
    {
        if (await CpfPertenceAOutraPessoaAsync(pessoa, ct))
            return PessoaErros.CpfDuplicado;

        try
        {
            await _repositorio.AtualizarAsync(pessoa, ct);
            return PessoaResultado.De(pessoa);
        }
        catch (CpfDuplicadoException)
        {
            return PessoaErros.CpfDuplicado;
        }
    }

    private Task<bool> CpfPertenceAOutraPessoaAsync(Pessoa pessoa, CancellationToken ct) =>
        _repositorio.ExisteCpfAsync(pessoa.Cpf, ignorarId: pessoa.Id, ct);
}
