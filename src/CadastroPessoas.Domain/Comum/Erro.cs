namespace CadastroPessoas.Domain.Comum;

public sealed record Erro(string Campo, string Mensagem, TipoErro Tipo);
