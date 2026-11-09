using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.Interfaces;
using EnderecoEntity = CadastroPessoas.Domain.Entities.Endereco;

namespace CadastroPessoas.Application.UseCases.Endereco.CriarEndereco;

public sealed class CriarEnderecoUseCase(IEnderecoRepository enderecoRepository, IPessoaRepository pessoaRepository)
{
    public async Task<int> ExecutarAsync(CriarEnderecoInput entrada)
    {
        await ValidarAsync(entrada);

        var endereco = EnderecoEntity.Criar(entrada.PessoaId, entrada.Dados);

        return await enderecoRepository.InserirAsync(endereco);
    }

    private async Task ValidarAsync(CriarEnderecoInput entrada)
    {
        var erros = EnderecoEntity.ValidarCriacao(entrada.PessoaId, entrada.Dados).ToList();

        if (entrada.PessoaId is int pessoaId && await PessoaNaoExisteAsync(pessoaId))
        {
            erros.Add(ErrosDeEndereco.PessoaInexistente);
        }

        if (erros.Count > 0)
        {
            throw new ValidacaoException(erros);
        }
    }

    private async Task<bool> PessoaNaoExisteAsync(int pessoaId)
    {
        return await pessoaRepository.ObterPorIdAsync(pessoaId) is null;
    }
}
