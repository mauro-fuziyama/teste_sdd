using CadastroPessoas.Application.Interfaces;
using CadastroPessoas.Application.Services;
using CadastroPessoas.Application.UseCases.Endereco.AtualizarEndereco;
using CadastroPessoas.Application.UseCases.Endereco.CriarEndereco;
using CadastroPessoas.Application.UseCases.Endereco.ExcluirEndereco;
using CadastroPessoas.Application.UseCases.Endereco.PesquisarEndereco;
using CadastroPessoas.Application.UseCases.Pessoa.AtualizarPessoa;
using CadastroPessoas.Application.UseCases.Pessoa.ConsultarPessoa;
using CadastroPessoas.Application.UseCases.Pessoa.CriarPessoa;
using CadastroPessoas.Application.UseCases.Pessoa.ExcluirPessoa;
using CadastroPessoas.Application.UseCases.Pessoa.PesquisarPessoa;
using CadastroPessoas.Application.UseCases.Telefone.AtualizarTelefone;
using CadastroPessoas.Application.UseCases.Telefone.CriarTelefone;
using CadastroPessoas.Application.UseCases.Telefone.ExcluirTelefone;
using CadastroPessoas.Application.UseCases.Telefone.PesquisarTelefone;
using CadastroPessoas.Domain.Interfaces;
using CadastroPessoas.Infrastructure.ExternalServices;
using CadastroPessoas.Infrastructure.Persistence;
using CadastroPessoas.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace CadastroPessoas.Infrastructure;

public static class DependencyInjection
{
    private const string NomeDaStringDeConexao = "CadastroPessoas";
    private const string NomeDaVariavelDaChave = "CHAVE_DE_SEGURANCA";

    public static IServiceCollection AdicionarInfraestrutura(
        this IServiceCollection servicos,
        IConfiguration configuracao)
    {
        servicos.AddSingleton(TimeProvider.System);
        servicos.AddSingleton(_ => CriarFonteDeDados(configuracao));
        servicos.AddScoped<IPessoaRepository, PessoaRepository>();
        servicos.AddScoped<ITelefoneRepository, TelefoneRepository>();
        servicos.AddScoped<IEnderecoRepository, EnderecoRepository>();

        AdicionarAplicacaoDePessoa(servicos);
        AdicionarAplicacaoDeTelefone(servicos);
        AdicionarAplicacaoDeEndereco(servicos);

        return servicos;
    }

    private static void AdicionarAplicacaoDePessoa(IServiceCollection servicos)
    {
        servicos.AddScoped<CriarPessoaUseCase>();
        servicos.AddScoped<ConsultarPessoaUseCase>();
        servicos.AddScoped<PesquisarPessoaUseCase>();
        servicos.AddScoped<AtualizarPessoaUseCase>();
        servicos.AddScoped<ExcluirPessoaUseCase>();
        servicos.AddScoped<IPessoaService, PessoaService>();
    }

    private static void AdicionarAplicacaoDeTelefone(IServiceCollection servicos)
    {
        servicos.AddScoped<CriarTelefoneUseCase>();
        servicos.AddScoped<PesquisarTelefoneUseCase>();
        servicos.AddScoped<AtualizarTelefoneUseCase>();
        servicos.AddScoped<ExcluirTelefoneUseCase>();
        servicos.AddScoped<ITelefoneService, TelefoneService>();
    }

    private static void AdicionarAplicacaoDeEndereco(IServiceCollection servicos)
    {
        servicos.AddScoped<CriarEnderecoUseCase>();
        servicos.AddScoped<PesquisarEnderecoUseCase>();
        servicos.AddScoped<AtualizarEnderecoUseCase>();
        servicos.AddScoped<ExcluirEnderecoUseCase>();
        servicos.AddScoped<IEnderecoService, EnderecoService>();
    }

    private static MySqlDataSource CriarFonteDeDados(IConfiguration configuracao)
    {
        var descriptografador = new DescriptografadorDeSenha(Environment.GetEnvironmentVariable(NomeDaVariavelDaChave));
        var stringDeConexao = StringDeConexaoSegura.ObterComSenhaDescriptografada(
            configuracao.GetConnectionString(NomeDaStringDeConexao),
            descriptografador);

        return new MySqlDataSource(stringDeConexao);
    }
}
