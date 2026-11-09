using CadastroPessoas.Domain.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace CadastroPessoas.Infrastructure.Tests.Persistence;

public sealed class BancoDeTesteFixture : IAsyncLifetime
{
    private readonly List<string> cpfsCriados = [];
    private ServiceProvider? provedor;

    public IServiceProvider Provedor => provedor!;

    public MySqlDataSource FonteDeDados => Provedor.GetRequiredService<MySqlDataSource>();

    public IPessoaRepository Repositorio => Provedor.GetRequiredService<IPessoaRepository>();

    public ITelefoneRepository TelefoneRepositorio => Provedor.GetRequiredService<ITelefoneRepository>();

    public IEnderecoRepository EnderecoRepositorio => Provedor.GetRequiredService<IEnderecoRepository>();

    public Task InitializeAsync()
    {
        var configuracao = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        provedor = new ServiceCollection()
            .AdicionarInfraestrutura(configuracao)
            .BuildServiceProvider();

        return Task.CompletedTask;
    }

    public string GerarCpfParaRemoverAoFinal()
    {
        var cpf = GeradorDeCpf.Gerar();

        lock (cpfsCriados)
        {
            cpfsCriados.Add(cpf);
        }

        return cpf;
    }

    public string RegistrarCpfParaRemoverAoFinal(string cpf)
    {
        lock (cpfsCriados)
        {
            cpfsCriados.Add(cpf);
        }

        return cpf;
    }

    public async Task DisposeAsync()
    {
        if (provedor is null)
        {
            return;
        }

        await using var conexao = await FonteDeDados.OpenConnectionAsync();
        var parametros = new { Cpfs = cpfsCriados };

        await conexao.ExecuteAsync(
            "DELETE endereco FROM endereco JOIN pessoa ON pessoa.id = endereco.pessoaid WHERE pessoa.cpf IN @Cpfs",
            parametros);
        await conexao.ExecuteAsync(
            "DELETE telefone FROM telefone JOIN pessoa ON pessoa.id = telefone.pessoaid WHERE pessoa.cpf IN @Cpfs",
            parametros);
        await conexao.ExecuteAsync("DELETE FROM pessoa WHERE cpf IN @Cpfs", parametros);
        await provedor.DisposeAsync();
    }
}

[CollectionDefinition(Nome)]
public sealed class ColecaoDoBancoDeTeste : ICollectionFixture<BancoDeTesteFixture>
{
    public const string Nome = "Banco de teste";
}
