namespace CadastroPessoas.Domain.Exceptions;

public static class ErrosDeTelefone
{
    public const string NumeroObrigatorio = "O número do telefone é obrigatório.";
    public const string NumeroSomenteComDigitos = "O número do telefone deve conter apenas dígitos.";
    public const string NumeroComTamanhoInvalido = "O número do telefone deve ter 10 dígitos (fixo) ou 11 dígitos (celular), incluindo o DDD.";
    public const string DddInvalido = "O DDD deve estar entre 11 e 99.";
    public const string CelularSemNoveNoTerceiroDigito = "O celular com 11 dígitos deve ter o dígito 9 logo após o DDD.";
    public const string PessoaIdObrigatorio = "O pessoaid é obrigatório.";
    public const string PessoaInexistente = "A pessoa informada não existe.";
    public const string NumeroJaCadastradoParaAPessoa = "O número informado já está cadastrado para esta pessoa.";
    public const string TelefoneNaoEncontrado = "O telefone informado não foi encontrado.";
}
