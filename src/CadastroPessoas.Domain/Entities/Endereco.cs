using System.Diagnostics.CodeAnalysis;
using CadastroPessoas.Domain.Enums;
using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.ValueObjects;

namespace CadastroPessoas.Domain.Entities;

public sealed class Endereco
{
    private const int TamanhoMaximoDoLogradouro = 100;
    private const int TamanhoMaximoDoNumero = 20;
    private const int TamanhoMaximoDoComplemento = 100;
    private const int TamanhoMaximoDoBairro = 100;
    private const int TamanhoMaximoDoMunicipio = 100;

    public int Id { get; }
    public int PessoaId { get; }
    public TipoDeEndereco Tipo { get; private set; }
    public string? Logradouro { get; private set; }
    public string? Numero { get; private set; }
    public string? Complemento { get; private set; }
    public string? Bairro { get; private set; }
    public string Municipio { get; private set; }
    public SiglaDeEstado Estado { get; private set; }
    public Cep? Cep { get; private set; }

    private Endereco(int id, int pessoaId, TipoDeEndereco tipo, DadosDoEndereco dados)
    {
        Id = id;
        PessoaId = pessoaId;
        Preencher(tipo, dados);
    }

    public static IReadOnlyList<string> ValidarCriacao(int? pessoaId, DadosDoEndereco dados)
    {
        var erros = ObterErrosDosDados(dados);

        if (pessoaId is null)
        {
            erros.Add(ErrosDeEndereco.PessoaIdObrigatorio);
        }

        return erros;
    }

    public static Endereco Criar(int? pessoaId, DadosDoEndereco dados)
    {
        LancarExcecaoSeHouverErros(ValidarCriacao(pessoaId, dados));

        return new Endereco(0, pessoaId!.Value, ObterTipoOuPadrao(dados.Tipo, TipoDeEndereco.NaoInformado), dados);
    }

    public static Endereco Reconstituir(int id, int pessoaId, DadosDoEndereco dados)
    {
        return new Endereco(id, pessoaId, ObterTipoOuPadrao(dados.Tipo, TipoDeEndereco.NaoInformado), dados);
    }

    public void Atualizar(DadosDoEndereco dados)
    {
        LancarExcecaoSeHouverErros(ObterErrosDosDados(dados));

        Preencher(ObterTipoOuPadrao(dados.Tipo, Tipo), dados);
    }

    [MemberNotNull(nameof(Municipio), nameof(Estado))]
    private void Preencher(TipoDeEndereco tipo, DadosDoEndereco dados)
    {
        Tipo = tipo;
        Logradouro = ObterTextoOuNulo(dados.Logradouro);
        Numero = ObterTextoOuNulo(dados.Numero);
        Complemento = ObterTextoOuNulo(dados.Complemento);
        Bairro = ObterTextoOuNulo(dados.Bairro);
        Municipio = dados.Municipio!.Trim();
        Estado = new SiglaDeEstado(dados.Estado);
        Cep = CriarCepSeInformado(dados.Cep);
    }

    private static List<string> ObterErrosDosDados(DadosDoEndereco dados)
    {
        var erros = new List<string>();

        AdicionarErroDeTipo(dados.Tipo, erros);
        AdicionarErrosDeTamanhoDosCamposOpcionais(dados, erros);
        AdicionarErrosDeMunicipio(dados.Municipio, erros);
        erros.AddRange(SiglaDeEstado.ObterErros(dados.Estado));
        AdicionarErrosDeCep(dados.Cep, erros);

        return erros;
    }

    private static void AdicionarErroDeTipo(int? tipo, List<string> erros)
    {
        if (tipo is not null && !Enum.IsDefined((TipoDeEndereco)tipo.Value))
        {
            erros.Add(ErrosDeEndereco.TipoInvalido);
        }
    }

    private static void AdicionarErrosDeTamanhoDosCamposOpcionais(DadosDoEndereco dados, List<string> erros)
    {
        AdicionarErroSeExcederTamanho(dados.Logradouro, TamanhoMaximoDoLogradouro, ErrosDeEndereco.LogradouroComTamanhoInvalido, erros);
        AdicionarErroSeExcederTamanho(dados.Numero, TamanhoMaximoDoNumero, ErrosDeEndereco.NumeroComTamanhoInvalido, erros);
        AdicionarErroSeExcederTamanho(dados.Complemento, TamanhoMaximoDoComplemento, ErrosDeEndereco.ComplementoComTamanhoInvalido, erros);
        AdicionarErroSeExcederTamanho(dados.Bairro, TamanhoMaximoDoBairro, ErrosDeEndereco.BairroComTamanhoInvalido, erros);
    }

    private static void AdicionarErrosDeMunicipio(string? municipio, List<string> erros)
    {
        if (string.IsNullOrWhiteSpace(municipio))
        {
            erros.Add(ErrosDeEndereco.MunicipioObrigatorio);
            return;
        }

        AdicionarErroSeExcederTamanho(municipio, TamanhoMaximoDoMunicipio, ErrosDeEndereco.MunicipioComTamanhoInvalido, erros);
    }

    private static void AdicionarErrosDeCep(string? cep, List<string> erros)
    {
        if (!string.IsNullOrWhiteSpace(cep))
        {
            erros.AddRange(Cep.ObterErros(cep));
        }
    }

    private static void AdicionarErroSeExcederTamanho(string? valor, int tamanhoMaximo, string erro, List<string> erros)
    {
        if (valor?.Trim().Length > tamanhoMaximo)
        {
            erros.Add(erro);
        }
    }

    private static TipoDeEndereco ObterTipoOuPadrao(int? tipo, TipoDeEndereco tipoPadrao)
    {
        return tipo is null ? tipoPadrao : (TipoDeEndereco)tipo.Value;
    }

    private static string? ObterTextoOuNulo(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }

    private static Cep? CriarCepSeInformado(string? cep)
    {
        return string.IsNullOrWhiteSpace(cep) ? null : new Cep(cep);
    }

    private static void LancarExcecaoSeHouverErros(IReadOnlyList<string> erros)
    {
        if (erros.Count > 0)
        {
            throw new ValidacaoException(erros);
        }
    }
}
