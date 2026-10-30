using CadastroPessoas.Application.Comum;
using CadastroPessoas.Application.Pessoas.Modelos;
using CadastroPessoas.Application.Pessoas.Portas.Entrada;
using CadastroPessoas.Application.Pessoas.Portas.Saida;
using CadastroPessoas.Domain.Comum;
using CadastroPessoas.Domain.Pessoas;

namespace CadastroPessoas.Application.Pessoas.CasosDeUso;

public sealed class CadastrarPessoa : ICadastrarPessoa
{
    private readonly IPessoaRepositorio _repositorio;
    private readonly RelogioAplicacao _relogio;

    public CadastrarPessoa(IPessoaRepositorio repositorio, RelogioAplicacao relogio)
    {
        _repositorio = repositorio;
        _relogio = relogio;
    }

    public async Task<Result<PessoaResultado>> ExecutarAsync(DadosPessoa dados, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dados);

        var criacao = Pessoa.Criar(dados.ParaCampos(), _relogio.Agora());
        if (!criacao.Sucesso)
            return Result<PessoaResultado>.Falha([.. criacao.Erros]);

        return await GravarAsync(criacao.Valor!, ct);
    }

    private async Task<Result<PessoaResultado>> GravarAsync(Pessoa pessoa, CancellationToken ct)
    {
        var idInativo = await _repositorio.ObterIdInativoPorCpfAsync(pessoa.Cpf, ct);
        if (idInativo is not null)
            return await ReativarAsync(pessoa, idInativo.Value, ct);

        if (await CpfJaCadastradoAsync(pessoa, ct))
            return PessoaErros.CpfDuplicado;

        return await AdicionarAsync(pessoa, ct);
    }

    private Task<bool> CpfJaCadastradoAsync(Pessoa pessoa, CancellationToken ct) =>
        _repositorio.ExisteCpfAsync(pessoa.Cpf, ignorarId: null, ct);

    private async Task<Result<PessoaResultado>> ReativarAsync(Pessoa pessoa, int idInativo, CancellationToken ct)
    {
        pessoa.DefinirId(idInativo);
        await _repositorio.ReativarAsync(pessoa, ct);
        return PessoaResultado.De(pessoa);
    }

    private async Task<Result<PessoaResultado>> AdicionarAsync(Pessoa pessoa, CancellationToken ct)
    {
        try
        {
            pessoa.DefinirId(await _repositorio.AdicionarAsync(pessoa, ct));
            return PessoaResultado.De(pessoa);
        }
        catch (CpfDuplicadoException)
        {
            return PessoaErros.CpfDuplicado;
        }
    }
}
