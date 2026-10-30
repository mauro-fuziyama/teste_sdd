using CadastroPessoas.Domain.Comum;

namespace CadastroPessoas.Application.Comum;

/// <summary>
/// Combina o instante UTC do <see cref="TimeProvider"/> com a data de hoje no fuso horário configurado da
/// aplicação, para que a validação da data de nascimento siga o calendário do usuário, e não o de UTC.
/// </summary>
public sealed class RelogioAplicacao
{
    private readonly TimeProvider _relogio;
    private readonly TimeZoneInfo _fusoHorario;

    public RelogioAplicacao(TimeProvider relogio, TimeZoneInfo fusoHorario)
    {
        _relogio = relogio;
        _fusoHorario = fusoHorario;
    }

    public MomentoAtual Agora()
    {
        var utc = _relogio.GetUtcNow();
        var local = TimeZoneInfo.ConvertTime(utc, _fusoHorario);
        return new MomentoAtual(utc, DateOnly.FromDateTime(local.DateTime));
    }
}
