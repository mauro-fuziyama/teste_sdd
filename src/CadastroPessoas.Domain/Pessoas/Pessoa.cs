using CadastroPessoas.Domain.Comum;
using CadastroPessoas.Domain.Pessoas.ValueObjects;

namespace CadastroPessoas.Domain.Pessoas;

public sealed class Pessoa
{
    public int Id { get; private set; }
    public Nome Nome { get; private set; }
    public DataNascimento DataNascimento { get; private set; }
    public Cpf Cpf { get; private set; }
    public Sexo Sexo { get; private set; }
    public DateTimeOffset CriadoEm { get; private set; }
    public DateTimeOffset AtualizadoEm { get; private set; }
    public bool Ativo { get; private set; }

    private Pessoa(int id, ValoresValidados valores, DateTimeOffset criadoEm, DateTimeOffset atualizadoEm)
    {
        Id = id;
        Nome = valores.Nome;
        DataNascimento = valores.DataNascimento;
        Cpf = valores.Cpf;
        Sexo = valores.Sexo;
        CriadoEm = criadoEm;
        AtualizadoEm = atualizadoEm;
        Ativo = true;
    }

    public static Result<Pessoa> Criar(CamposPessoa campos, MomentoAtual momento)
    {
        var valores = Validar(campos, momento.Hoje);
        if (!valores.Sucesso)
            return Result<Pessoa>.Falha([.. valores.Erros]);

        return new Pessoa(0, valores.Valor!, momento.Utc, momento.Utc);
    }

    public static Pessoa Restaurar(
        int id,
        string nome,
        DateOnly dataNascimento,
        string cpf,
        string sexo,
        DateTimeOffset criadoEm,
        DateTimeOffset atualizadoEm)
    {
        var valores = new ValoresValidados(
            Nome.Restaurar(nome),
            DataNascimento.Restaurar(dataNascimento),
            Cpf.Restaurar(cpf),
            Sexo.Restaurar(sexo));

        return new Pessoa(id, valores, criadoEm, atualizadoEm);
    }

    public Result<Pessoa> Atualizar(CamposPessoa campos, MomentoAtual momento)
    {
        var valores = Validar(campos, momento.Hoje);
        if (!valores.Sucesso)
            return Result<Pessoa>.Falha([.. valores.Erros]);

        AplicarValores(valores.Valor!);
        AtualizadoEm = momento.Utc;
        return this;
    }

    public void DefinirId(int id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        Id = id;
    }

    public void Desativar(DateTimeOffset agora)
    {
        Ativo = false;
        AtualizadoEm = agora;
    }

    private void AplicarValores(ValoresValidados valores)
    {
        Nome = valores.Nome;
        DataNascimento = valores.DataNascimento;
        Cpf = valores.Cpf;
        Sexo = valores.Sexo;
    }

    private static Result<ValoresValidados> Validar(CamposPessoa campos, DateOnly hoje)
    {
        var nome = Nome.Criar(campos.Nome);
        var dataNascimento = DataNascimento.Criar(campos.DataNascimento, hoje);
        var cpf = Cpf.Criar(campos.Cpf);
        var sexo = Sexo.Criar(campos.Sexo);

        Erro[] erros = [.. nome.Erros, .. dataNascimento.Erros, .. cpf.Erros, .. sexo.Erros];
        if (erros.Length > 0)
            return Result<ValoresValidados>.Falha(erros);

        return new ValoresValidados(nome.Valor!, dataNascimento.Valor!, cpf.Valor!, sexo.Valor!);
    }

    private sealed record ValoresValidados(Nome Nome, DataNascimento DataNascimento, Cpf Cpf, Sexo Sexo);
}
