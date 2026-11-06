using CadastroPessoas.Domain.Interfaces;
using CadastroPessoas.Domain.ValueObjects;
using PessoaEntity = CadastroPessoas.Domain.Entities.Pessoa;

namespace CadastroPessoas.Application.UseCases.Pessoa.ConsultarPessoa;

public sealed class ConsultarPessoaUseCase(IPessoaRepository repositorio)
{
    public Task<PessoaEntity?> ExecutarPorIdAsync(int id)
    {
        return repositorio.ObterPorIdAsync(id);
    }

    public Task<PessoaEntity?> ExecutarPorCpfAsync(string? cpf)
    {
        return repositorio.ObterPorCpfAsync(new CPF(cpf));
    }
}
