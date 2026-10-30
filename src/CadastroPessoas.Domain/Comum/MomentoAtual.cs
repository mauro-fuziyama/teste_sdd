namespace CadastroPessoas.Domain.Comum;

/// <summary>
/// Instante atual em UTC, usado nas datas de auditoria, e a data de hoje no fuso horário
/// configurado da aplicação, usada nas regras que dependem do dia do calendário.
/// </summary>
public sealed record MomentoAtual(DateTimeOffset Utc, DateOnly Hoje);
