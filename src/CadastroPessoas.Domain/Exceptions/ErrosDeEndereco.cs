namespace CadastroPessoas.Domain.Exceptions;

public static class ErrosDeEndereco
{
    public const string PessoaIdObrigatorio = "O pessoaid é obrigatório.";
    public const string PessoaInexistente = "A pessoa informada não existe.";
    public const string TipoInvalido = "O tipo do endereço deve ser 0 (não informado), 1 (residencial), 2 (comercial) ou 3 (outros).";
    public const string MunicipioObrigatorio = "O município é obrigatório.";
    public const string EstadoObrigatorio = "O estado é obrigatório.";
    public const string EstadoComTamanhoInvalido = "O estado deve ter exatamente 2 caracteres.";
    public const string SiglaDeEstadoInvalida = "O estado deve ser a sigla de uma unidade federativa brasileira.";
    public const string CepSomenteComDigitos = "O CEP deve conter apenas dígitos.";
    public const string CepComTamanhoInvalido = "O CEP deve ter 5 ou 8 dígitos.";
    public const string LogradouroComTamanhoInvalido = "O logradouro deve ter no máximo 100 caracteres.";
    public const string NumeroComTamanhoInvalido = "O número do endereço deve ter no máximo 20 caracteres.";
    public const string ComplementoComTamanhoInvalido = "O complemento deve ter no máximo 100 caracteres.";
    public const string BairroComTamanhoInvalido = "O bairro deve ter no máximo 100 caracteres.";
    public const string MunicipioComTamanhoInvalido = "O município deve ter no máximo 100 caracteres.";
    public const string EnderecoNaoEncontrado = "O endereço informado não foi encontrado.";
}
