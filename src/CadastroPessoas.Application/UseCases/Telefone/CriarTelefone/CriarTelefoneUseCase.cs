using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.Interfaces;
using CadastroPessoas.Domain.ValueObjects;
using TelefoneEntity = CadastroPessoas.Domain.Entities.Telefone;

namespace CadastroPessoas.Application.UseCases.Telefone.CriarTelefone;

public sealed class CriarTelefoneUseCase(ITelefoneRepository telefoneRepository, IPessoaRepository pessoaRepository)
{
    public async Task<int> ExecutarAsync(CriarTelefoneInput dados)
    {
        await ValidarAsync(dados);

        var telefone = TelefoneEntity.Criar(dados.PessoaId, dados.Numero);

        return await telefoneRepository.InserirAsync(telefone);
    }

    private async Task ValidarAsync(CriarTelefoneInput dados)
    {
        var erros = TelefoneEntity.ValidarCriacao(dados.PessoaId, dados.Numero).ToList();

        if (dados.PessoaId is int pessoaId)
        {
            erros.AddRange(await ObterErrosDaPessoaAsync(pessoaId, dados.Numero));
        }

        if (erros.Count > 0)
        {
            throw new ValidacaoException(erros);
        }
    }

    private async Task<IReadOnlyList<string>> ObterErrosDaPessoaAsync(int pessoaId, string? numero)
    {
        if (await pessoaRepository.ObterPorIdAsync(pessoaId) is null)
        {
            return [ErrosDeTelefone.PessoaInexistente];
        }

        return await NumeroJaCadastradoAsync(pessoaId, numero)
            ? [ErrosDeTelefone.NumeroJaCadastradoParaAPessoa]
            : [];
    }

    private async Task<bool> NumeroJaCadastradoAsync(int pessoaId, string? numero)
    {
        return NumeroDeTelefone.EhValido(numero)
            && await telefoneRepository.NumeroJaCadastradoParaPessoaAsync(pessoaId, new NumeroDeTelefone(numero));
    }
}
