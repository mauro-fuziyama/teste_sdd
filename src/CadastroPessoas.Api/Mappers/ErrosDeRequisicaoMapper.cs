using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CadastroPessoas.Api.Mappers;

public static class ErrosDeRequisicaoMapper
{
    public const string IdInvalido = "O id deve ser um número inteiro válido.";
    public const string PessoaIdInvalido = "O pessoaid deve ser um número inteiro válido.";
    public const string TipoInvalido = "O tipo deve ser um número inteiro válido.";
    public const string FormatoDaDataInvalido = "A data de nascimento deve ser uma data válida no formato AAAA-MM-DD.";
    public const string CorpoInvalido = "O corpo da requisição é obrigatório e deve ser um JSON válido.";

    private const string PrefixoDeCampoDoJson = "$";
    private const string ParametroDeId = "id";
    private const string CampoDoPessoaId = "pessoaid";
    private const string CampoDoTipo = "tipo";
    private const string CampoDaDataDeNascimento = "datadenascimento";

    public static IReadOnlyList<string> ParaErros(ModelStateDictionary estadoDaRequisicao)
    {
        return ObterCamposRelevantes(estadoDaRequisicao)
            .Select(ObterMensagem)
            .Distinct()
            .ToList();
    }

    private static List<string> ObterCamposRelevantes(ModelStateDictionary estadoDaRequisicao)
    {
        var camposComErro = estadoDaRequisicao
            .Where(campo => campo.Value?.Errors.Count > 0)
            .Select(campo => campo.Key)
            .ToList();
        var camposDoJson = camposComErro
            .Where(campo => campo.StartsWith(PrefixoDeCampoDoJson, StringComparison.Ordinal))
            .ToList();

        return camposDoJson.Count > 0 ? camposDoJson : camposComErro;
    }

    private static string ObterMensagem(string campo)
    {
        if (campo.Equals(ParametroDeId, StringComparison.OrdinalIgnoreCase))
        {
            return IdInvalido;
        }

        if (campo.EndsWith(CampoDoPessoaId, StringComparison.OrdinalIgnoreCase))
        {
            return PessoaIdInvalido;
        }

        if (campo.EndsWith(CampoDoTipo, StringComparison.OrdinalIgnoreCase))
        {
            return TipoInvalido;
        }

        return campo.EndsWith(CampoDaDataDeNascimento, StringComparison.OrdinalIgnoreCase)
            ? FormatoDaDataInvalido
            : CorpoInvalido;
    }
}
