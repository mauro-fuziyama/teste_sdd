using CadastroPessoas.Application.Interfaces;
using CadastroPessoas.Domain.Interfaces;
using CadastroPessoas.Infrastructure.Persistence.Repositories;
using CadastroPessoas.Infrastructure.Tests.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace CadastroPessoas.Infrastructure.Tests;

[Collection(ColecaoDoBancoDeTeste.Nome)]
public sealed class DependencyInjectionTests(BancoDeTesteFixture banco)
{
    [Fact]
    public void AdicionarInfraestrutura_ResolveEntradaDaAplicacaoERepositorio()
    {
        using var escopo = banco.Provedor.CreateScope();

        Assert.NotNull(escopo.ServiceProvider.GetRequiredService<IPessoaService>());
        Assert.IsType<PessoaRepository>(escopo.ServiceProvider.GetRequiredService<IPessoaRepository>());
    }

    [Fact]
    public void AdicionarInfraestrutura_RegistraRelogioDoSistema()
    {
        Assert.Same(TimeProvider.System, banco.Provedor.GetRequiredService<TimeProvider>());
    }
}
