using CadastroPessoas.Domain.Comum;
using CadastroPessoas.Domain.Pessoas.ValueObjects;

namespace CadastroPessoas.Domain.Pessoas;

public static class PessoaErros
{
    public static readonly Erro NomeObrigatorio = CampoObrigatorio("nome");

    public static readonly Erro NomeTamanhoInvalido =
        Validacao("nome", $"O nome deve ter entre {Nome.TamanhoMinimo} e {Nome.TamanhoMaximo} caracteres.");

    public static readonly Erro DataNascimentoObrigatoria = CampoObrigatorio("dataNascimento");

    public static readonly Erro DataNascimentoFutura =
        Validacao("dataNascimento", "A data de nascimento não pode ser futura.");

    public static readonly Erro IdadeAcimaDoLimite =
        Validacao("dataNascimento", $"A data de nascimento indica uma idade superior a {DataNascimento.IdadeMaximaAnos} anos.");

    public static readonly Erro CpfObrigatorio = CampoObrigatorio("cpf");

    public static readonly Erro CpfInvalido = Validacao("cpf", "CPF inválido.");

    public static readonly Erro SexoObrigatorio = CampoObrigatorio("sexo");

    public static readonly Erro SexoInvalido =
        Validacao("sexo", $"Sexo inválido. Valores aceitos: {Sexo.CodigosAceitos}.");

    private static Erro CampoObrigatorio(string campo) =>
        Validacao(campo, $"O campo {campo} é obrigatório.");

    private static Erro Validacao(string campo, string mensagem) =>
        new(campo, mensagem, TipoErro.Validacao);
}
