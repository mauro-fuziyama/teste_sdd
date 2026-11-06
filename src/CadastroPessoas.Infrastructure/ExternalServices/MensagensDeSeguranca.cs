namespace CadastroPessoas.Infrastructure.ExternalServices;

public static class MensagensDeSeguranca
{
    public const string ChaveInvalida = "A variável de ambiente CHAVE_DE_SEGURANCA deve conter uma chave AES de 256 bits em Base64.";
    public const string SenhaEmFormatoInvalido = "A senha da string de conexão não está no formato criptografado esperado.";
    public const string SenhaNaoDescriptografada = "Não foi possível descriptografar a senha da string de conexão com a chave informada.";
    public const string StringDeConexaoAusente = "A string de conexão CadastroPessoas não foi configurada.";
}
