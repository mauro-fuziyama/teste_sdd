using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.Interfaces;
using CadastroPessoas.Domain.ValueObjects;
using TelefoneEntity = CadastroPessoas.Domain.Entities.Telefone;

namespace CadastroPessoas.Application.UseCases.Telefone.AtualizarTelefone;

public sealed class AtualizarTelefoneUseCase(ITelefoneRepository telefoneRepository)
{
    public async Task ExecutarAsync(int id, AtualizarTelefoneInput dados)
    {
        var telefone = await telefoneRepository.ObterPorIdAsync(id) ?? throw new TelefoneNaoEncontradoException();

        await ValidarAsync(telefone, dados.Numero);
        telefone.AtualizarNumero(dados.Numero);

        await telefoneRepository.AtualizarAsync(telefone);
    }

    private async Task ValidarAsync(TelefoneEntity telefone, string? numero)
    {
        var erros = NumeroDeTelefone.ObterErros(numero).ToList();

        if (erros.Count == 0 && await NumeroJaCadastradoEmOutroTelefoneAsync(telefone, numero))
        {
            erros.Add(ErrosDeTelefone.NumeroJaCadastradoParaAPessoa);
        }

        if (erros.Count > 0)
        {
            throw new ValidacaoException(erros);
        }
    }

    private Task<bool> NumeroJaCadastradoEmOutroTelefoneAsync(TelefoneEntity telefone, string? numero)
    {
        return telefoneRepository.NumeroJaCadastradoParaPessoaAsync(
            telefone.PessoaId,
            new NumeroDeTelefone(numero),
            idDoTelefoneIgnorado: telefone.Id);
    }
}
