using CadastroPessoas.Application.Pessoas.Modelos;
using CadastroPessoas.Application.Pessoas.Portas.Entrada;
using CadastroPessoas.Application.Pessoas.Portas.Saida;
using CadastroPessoas.Domain.Comum;
using CadastroPessoas.Domain.Pessoas;

namespace CadastroPessoas.Application.Pessoas.CasosDeUso;

public sealed class ConsultarPessoa : IConsultarPessoa
{
    private readonly IPessoaRepositorio _repositorio;

    public ConsultarPessoa(IPessoaRepositorio repositorio) => _repositorio = repositorio;

    public async Task<Result<PessoaResultado>> ExecutarAsync(int id, CancellationToken ct)
    {
        var pessoa = await _repositorio.ObterPorIdAsync(id, ct);
        if (pessoa is null)
            return PessoaErros.NaoEncontrada;

        return PessoaResultado.De(pessoa);
    }
}
