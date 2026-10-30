using CadastroPessoas.Application.Comum;
using CadastroPessoas.Application.Pessoas.Modelos;
using CadastroPessoas.Application.Tests.Pessoas.Fakes;
using CadastroPessoas.Domain.Comum;
using CadastroPessoas.Domain.Pessoas;

using Microsoft.Extensions.Time.Testing;

namespace CadastroPessoas.Application.Tests.Pessoas.CasosDeUso;

/// <summary>
/// Dados e dependências compartilhados pelos testes dos casos de uso.
/// </summary>
internal sealed class Cenario
{
    public static readonly DateTimeOffset Inicio = new(2026, 10, 30, 12, 0, 0, TimeSpan.Zero);

    public static readonly TimeZoneInfo FusoBrasilia =
        TimeZoneInfo.CreateCustomTimeZone("Teste/Brasilia", TimeSpan.FromHours(-3), "Brasília", "Brasília");

    public static readonly DadosPessoa DadosValidos =
        new("Maria da Silva", new DateOnly(1990, 5, 17), "529.982.247-25", "F");

    public static readonly DadosPessoa OutrosDadosValidos =
        new("João Souza", new DateOnly(1985, 1, 2), "111.444.777-35", "M");

    public Cenario()
    {
        Tempo = new FakeTimeProvider(Inicio);
        Relogio = new RelogioAplicacao(Tempo, FusoBrasilia);
    }

    public FakeTimeProvider Tempo { get; }
    public RelogioAplicacao Relogio { get; }
    public PessoaRepositorioEmMemoria Repositorio { get; } = new();

    public Pessoa Gravar(DadosPessoa dados, bool ativo = true)
    {
        var pessoa = Pessoa.Criar(dados.ParaCampos(), new MomentoAtual(Inicio, DateOnly.FromDateTime(Inicio.Date)));
        return Repositorio.Inserir(pessoa.Valor!, ativo);
    }

    public DateTimeOffset AvancarUmDia()
    {
        Tempo.Advance(TimeSpan.FromDays(1));
        return Tempo.GetUtcNow();
    }
}
