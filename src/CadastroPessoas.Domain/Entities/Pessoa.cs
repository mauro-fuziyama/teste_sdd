using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.ValueObjects;

namespace CadastroPessoas.Domain.Entities;

public sealed class Pessoa
{
    private const int TamanhoMinimoDoNome = 3;
    private const int TamanhoMaximoDoNome = 100;
    private static readonly DateOnly LimiteInferiorDaDataDeNascimento = new(1910, 1, 1);

    public int Id { get; }
    public string Nome { get; private set; }
    public CPF Cpf { get; }
    public DateOnly? DataDeNascimento { get; private set; }
    public DateTime DataDeCadastro { get; }
    public DateTime DataAtualizacao { get; private set; }

    private Pessoa(
        int id,
        string nome,
        CPF cpf,
        DateOnly? dataDeNascimento,
        DateTime dataDeCadastro,
        DateTime dataAtualizacao)
    {
        Id = id;
        Nome = nome;
        Cpf = cpf;
        DataDeNascimento = dataDeNascimento;
        DataDeCadastro = dataDeCadastro;
        DataAtualizacao = dataAtualizacao;
    }

    public static IReadOnlyList<string> ValidarCriacao(
        string? nome,
        string? cpf,
        DateOnly? dataDeNascimento,
        DateTime momentoAtual)
    {
        var erros = ObterErrosDeNomeEData(nome, dataDeNascimento, momentoAtual);

        if (!CPF.EhValido(cpf))
        {
            erros.Add(ErrosDePessoa.CpfInvalido);
        }

        return erros;
    }

    public static Pessoa Criar(string? nome, string? cpf, DateOnly? dataDeNascimento, DateTime momentoAtual)
    {
        LancarExcecaoSeHouverErros(ValidarCriacao(nome, cpf, dataDeNascimento, momentoAtual));

        return new Pessoa(0, RemoverEspacosDasPontas(nome), new CPF(cpf), dataDeNascimento, momentoAtual, momentoAtual);
    }

    public static Pessoa Reconstituir(
        int id,
        string nome,
        CPF cpf,
        DateOnly? dataDeNascimento,
        DateTime dataDeCadastro,
        DateTime dataAtualizacao)
    {
        return new Pessoa(id, nome, cpf, dataDeNascimento, dataDeCadastro, dataAtualizacao);
    }

    public void Atualizar(string? nome, string? cpf, DateOnly? dataDeNascimento, DateTime momentoAtual)
    {
        LancarExcecaoSeHouverErros(ValidarAtualizacao(nome, cpf, dataDeNascimento, momentoAtual));

        Nome = RemoverEspacosDasPontas(nome);
        DataDeNascimento = dataDeNascimento;
        DataAtualizacao = momentoAtual;
    }

    private List<string> ValidarAtualizacao(
        string? nome,
        string? cpf,
        DateOnly? dataDeNascimento,
        DateTime momentoAtual)
    {
        var erros = ObterErrosDeNomeEData(nome, dataDeNascimento, momentoAtual);

        if (string.IsNullOrWhiteSpace(cpf))
        {
            erros.Add(ErrosDePessoa.CpfInvalido);
        }
        else if (cpf != Cpf.Valor)
        {
            erros.Add(ErrosDePessoa.CpfNaoPodeSerAlterado);
        }

        return erros;
    }

    private static List<string> ObterErrosDeNomeEData(string? nome, DateOnly? dataDeNascimento, DateTime momentoAtual)
    {
        var erros = new List<string>();

        if (!NomeEhValido(nome))
        {
            erros.Add(ErrosDePessoa.NomeInvalido);
        }

        if (!DataDeNascimentoEhValida(dataDeNascimento, momentoAtual))
        {
            erros.Add(ErrosDePessoa.DataDeNascimentoInvalida);
        }

        return erros;
    }

    private static bool NomeEhValido(string? nome)
    {
        return RemoverEspacosDasPontas(nome).Length is >= TamanhoMinimoDoNome and <= TamanhoMaximoDoNome;
    }

    private static bool DataDeNascimentoEhValida(DateOnly? dataDeNascimento, DateTime momentoAtual)
    {
        if (dataDeNascimento is null)
        {
            return true;
        }

        var hoje = DateOnly.FromDateTime(momentoAtual);

        return dataDeNascimento > LimiteInferiorDaDataDeNascimento && dataDeNascimento < hoje;
    }

    private static string RemoverEspacosDasPontas(string? nome)
    {
        return nome?.Trim() ?? string.Empty;
    }

    private static void LancarExcecaoSeHouverErros(IReadOnlyList<string> erros)
    {
        if (erros.Count > 0)
        {
            throw new ValidacaoException(erros);
        }
    }
}
