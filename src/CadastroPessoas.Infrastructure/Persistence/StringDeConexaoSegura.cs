using CadastroPessoas.Infrastructure.ExternalServices;
using MySqlConnector;

namespace CadastroPessoas.Infrastructure.Persistence;

public static class StringDeConexaoSegura
{
    public static string ObterComSenhaDescriptografada(
        string? stringDeConexao,
        DescriptografadorDeSenha descriptografador)
    {
        if (string.IsNullOrWhiteSpace(stringDeConexao))
        {
            throw new InvalidOperationException(MensagensDeSeguranca.StringDeConexaoAusente);
        }

        var construtor = new MySqlConnectionStringBuilder(stringDeConexao);
        construtor.Password = descriptografador.Descriptografar(construtor.Password);

        return construtor.ConnectionString;
    }
}
