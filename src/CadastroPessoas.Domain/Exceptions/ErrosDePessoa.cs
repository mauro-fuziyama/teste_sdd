namespace CadastroPessoas.Domain.Exceptions;

public static class ErrosDePessoa
{
    public const string NomeInvalido = "O nome é obrigatório e deve ter entre 3 e 100 caracteres.";
    public const string CpfInvalido = "O CPF é obrigatório e deve ser válido, com 11 dígitos numéricos.";
    public const string CpfNaoPodeSerAlterado = "O CPF não pode ser alterado.";
    public const string CpfJaCadastrado = "O CPF informado já está cadastrado.";
    public const string DataDeNascimentoInvalida = "A data de nascimento deve ser posterior a 1910-01-01 e anterior a hoje.";
    public const string PessoaNaoEncontrada = "A pessoa informada não foi encontrada.";
    public const string NomeDaPesquisaObrigatorio = "O nome da pesquisa é obrigatório.";
}
