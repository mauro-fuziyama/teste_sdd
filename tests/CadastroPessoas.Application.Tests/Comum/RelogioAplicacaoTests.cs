using AwesomeAssertions;
using CadastroPessoas.Application.Comum;

using Microsoft.Extensions.Time.Testing;

namespace CadastroPessoas.Application.Tests.Comum;

public class RelogioAplicacaoTests
{
    private static readonly TimeZoneInfo FusoBrasilia =
        TimeZoneInfo.CreateCustomTimeZone("Teste/Brasilia", TimeSpan.FromHours(-3), "Brasília", "Brasília");

    [Fact]
    public void AgoraRetornaInstanteUtcSemConversao()
    {
        var instante = new DateTimeOffset(2026, 10, 30, 15, 45, 0, TimeSpan.Zero);
        var relogio = new RelogioAplicacao(new FakeTimeProvider(instante), FusoBrasilia);

        relogio.Agora().Utc.Should().Be(instante);
    }

    [Theory]
    [InlineData(2, 59, 30)]
    [InlineData(3, 0, 31)]
    [InlineData(23, 59, 31)]
    public void HojeSegueOCalendarioDoFusoConfigurado(int horaUtc, int minutoUtc, int diaEsperado)
    {
        var instante = new DateTimeOffset(2026, 10, 31, horaUtc, minutoUtc, 0, TimeSpan.Zero);
        var relogio = new RelogioAplicacao(new FakeTimeProvider(instante), FusoBrasilia);

        relogio.Agora().Hoje.Should().Be(new DateOnly(2026, 10, diaEsperado));
    }
}
