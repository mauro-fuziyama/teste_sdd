namespace CadastroPessoas.Application.Tests.Fakes;

public sealed class ProvedorDeTempoControlado(DateTime momentoInicial) : TimeProvider
{
    public DateTime MomentoAtual { get; set; } = momentoInicial;

    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;

    public override DateTimeOffset GetUtcNow()
    {
        return new DateTimeOffset(MomentoAtual, TimeSpan.Zero);
    }
}
