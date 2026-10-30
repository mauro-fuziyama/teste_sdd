using CadastroPessoas.Domain.Comum;

namespace CadastroPessoas.Domain.Pessoas;

public static class PessoaErros
{
    public static readonly Erro CpfObrigatorio =
        new("cpf", "O campo cpf é obrigatório.", TipoErro.Validacao);

    public static readonly Erro CpfInvalido =
        new("cpf", "CPF inválido.", TipoErro.Validacao);
}
