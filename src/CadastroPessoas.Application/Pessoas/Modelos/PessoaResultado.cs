using CadastroPessoas.Domain.Pessoas;

namespace CadastroPessoas.Application.Pessoas.Modelos;

public sealed record PessoaResultado(
    int Id,
    string Nome,
    DateOnly DataNascimento,
    string Cpf,
    string Sexo,
    DateTimeOffset CriadoEm,
    DateTimeOffset AtualizadoEm)
{
    public static PessoaResultado De(Pessoa pessoa)
    {
        ArgumentNullException.ThrowIfNull(pessoa);

        return new PessoaResultado(
            pessoa.Id,
            pessoa.Nome.Valor,
            pessoa.DataNascimento.Valor,
            pessoa.Cpf.Formatado(),
            pessoa.Sexo.Codigo,
            pessoa.CriadoEm,
            pessoa.AtualizadoEm);
    }
}
