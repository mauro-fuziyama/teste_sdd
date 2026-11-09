using CadastroPessoas.Application.Services;
using CadastroPessoas.Application.Tests.Fakes;
using CadastroPessoas.Application.UseCases.Telefone.AtualizarTelefone;
using CadastroPessoas.Application.UseCases.Telefone.CriarTelefone;
using CadastroPessoas.Application.UseCases.Telefone.ExcluirTelefone;
using CadastroPessoas.Application.UseCases.Telefone.PesquisarTelefone;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Exceptions;

namespace CadastroPessoas.Application.Tests.Services;

public class TelefoneServiceTests
{
    private const string Celular = "11987654321";
    private const string Fixo = "1134567890";
    private const int PessoaIdInexistente = 999;
    private const int TelefoneIdInexistente = 999;
    private static readonly DateTime MomentoDoCadastro = new(2026, 11, 6, 10, 0, 0);

    private readonly PessoaRepositoryEmMemoria pessoaRepository = new();
    private readonly TelefoneRepositoryEmMemoria telefoneRepository = new();
    private readonly TelefoneService servico;

    public TelefoneServiceTests()
    {
        servico = new TelefoneService(
            new CriarTelefoneUseCase(telefoneRepository, pessoaRepository),
            new PesquisarTelefoneUseCase(telefoneRepository),
            new AtualizarTelefoneUseCase(telefoneRepository),
            new ExcluirTelefoneUseCase(telefoneRepository));
    }

    [Fact]
    public async Task Criar_ComDadosValidos_RetornaOIdEGravaOTelefone()
    {
        var pessoaId = await CriarPessoaAsync("52998224725");

        var id = await servico.CriarAsync(new CriarTelefoneInput(pessoaId, $" {Celular} "));

        var gravado = await telefoneRepository.ObterPorIdAsync(id);
        Assert.Equal(pessoaId, gravado!.PessoaId);
        Assert.Equal(Celular, gravado.Numero.Valor);
    }

    [Fact]
    public async Task Criar_ComPessoaInexistente_LancaErroDePessoaInexistente()
    {
        var excecao = await Assert.ThrowsAsync<ValidacaoException>(
            () => servico.CriarAsync(new CriarTelefoneInput(PessoaIdInexistente, Celular)));

        Assert.Equal([ErrosDeTelefone.PessoaInexistente], excecao.Erros);
    }

    [Fact]
    public async Task Criar_SemPessoaId_LancaErroDePessoaIdObrigatorio()
    {
        var excecao = await Assert.ThrowsAsync<ValidacaoException>(
            () => servico.CriarAsync(new CriarTelefoneInput(null, Celular)));

        Assert.Equal([ErrosDeTelefone.PessoaIdObrigatorio], excecao.Erros);
    }

    [Fact]
    public async Task Criar_ComNumeroJaCadastradoParaAMesmaPessoa_LancaErroDeNumeroRepetido()
    {
        var pessoaId = await CriarPessoaAsync("52998224725");
        await servico.CriarAsync(new CriarTelefoneInput(pessoaId, Celular));

        var excecao = await Assert.ThrowsAsync<ValidacaoException>(
            () => servico.CriarAsync(new CriarTelefoneInput(pessoaId, $"  {Celular}")));

        Assert.Equal([ErrosDeTelefone.NumeroJaCadastradoParaAPessoa], excecao.Erros);
    }

    [Fact]
    public async Task Criar_ComMesmoNumeroParaPessoasDiferentes_Aceita()
    {
        var primeiraPessoaId = await CriarPessoaAsync("52998224725");
        var segundaPessoaId = await CriarPessoaAsync("11144477735");
        await servico.CriarAsync(new CriarTelefoneInput(primeiraPessoaId, Celular));

        var id = await servico.CriarAsync(new CriarTelefoneInput(segundaPessoaId, Celular));

        Assert.True(id > 0);
    }

    [Fact]
    public async Task Criar_ComNumeroInvalidoEPessoaInexistente_ReuneTodosOsErros()
    {
        var excecao = await Assert.ThrowsAsync<ValidacaoException>(
            () => servico.CriarAsync(new CriarTelefoneInput(PessoaIdInexistente, "10887654321")));

        Assert.Equal(
            [ErrosDeTelefone.DddInvalido, ErrosDeTelefone.CelularSemNoveNoTerceiroDigito, ErrosDeTelefone.PessoaInexistente],
            excecao.Erros);
    }

    [Fact]
    public async Task Criar_SemNumeroESemPessoaId_ReuneOsDoisErros()
    {
        var excecao = await Assert.ThrowsAsync<ValidacaoException>(
            () => servico.CriarAsync(new CriarTelefoneInput(null, null)));

        Assert.Equal([ErrosDeTelefone.NumeroObrigatorio, ErrosDeTelefone.PessoaIdObrigatorio], excecao.Erros);
    }

    [Fact]
    public async Task PesquisarPorPessoaId_RetornaSomenteOsTelefonesDaPessoaOrdenadosPeloId()
    {
        var pessoaId = await CriarPessoaAsync("52998224725");
        var outraPessoaId = await CriarPessoaAsync("11144477735");
        var primeiroId = await servico.CriarAsync(new CriarTelefoneInput(pessoaId, Fixo));
        await servico.CriarAsync(new CriarTelefoneInput(outraPessoaId, Fixo));
        var segundoId = await servico.CriarAsync(new CriarTelefoneInput(pessoaId, Celular));

        var telefones = await servico.PesquisarPorPessoaIdAsync(pessoaId);

        Assert.Equal([primeiroId, segundoId], telefones.Select(telefone => telefone.Id));
    }

    [Fact]
    public async Task PesquisarPorPessoaId_SemTelefones_RetornaListaVazia()
    {
        var pessoaId = await CriarPessoaAsync("52998224725");

        Assert.Empty(await servico.PesquisarPorPessoaIdAsync(pessoaId));
    }

    [Fact]
    public async Task Atualizar_ComNumeroValido_AlteraSomenteONumero()
    {
        var pessoaId = await CriarPessoaAsync("52998224725");
        var id = await servico.CriarAsync(new CriarTelefoneInput(pessoaId, Celular));

        await servico.AtualizarAsync(id, new AtualizarTelefoneInput(Fixo));

        var atualizado = await telefoneRepository.ObterPorIdAsync(id);
        Assert.Equal(Fixo, atualizado!.Numero.Valor);
        Assert.Equal(pessoaId, atualizado.PessoaId);
    }

    [Fact]
    public async Task Atualizar_MantendoOProprioNumero_Aceita()
    {
        var pessoaId = await CriarPessoaAsync("52998224725");
        var id = await servico.CriarAsync(new CriarTelefoneInput(pessoaId, Celular));

        await servico.AtualizarAsync(id, new AtualizarTelefoneInput(Celular));

        var atualizado = await telefoneRepository.ObterPorIdAsync(id);
        Assert.Equal(Celular, atualizado!.Numero.Valor);
    }

    [Fact]
    public async Task Atualizar_ParaNumeroDeOutroTelefoneDaMesmaPessoa_LancaErroDeNumeroRepetido()
    {
        var pessoaId = await CriarPessoaAsync("52998224725");
        await servico.CriarAsync(new CriarTelefoneInput(pessoaId, Celular));
        var id = await servico.CriarAsync(new CriarTelefoneInput(pessoaId, Fixo));

        var excecao = await Assert.ThrowsAsync<ValidacaoException>(
            () => servico.AtualizarAsync(id, new AtualizarTelefoneInput(Celular)));

        Assert.Equal([ErrosDeTelefone.NumeroJaCadastradoParaAPessoa], excecao.Erros);
    }

    [Theory]
    [InlineData(null, ErrosDeTelefone.NumeroObrigatorio)]
    [InlineData("   ", ErrosDeTelefone.NumeroObrigatorio)]
    [InlineData("1034567890", ErrosDeTelefone.DddInvalido)]
    public async Task Atualizar_ComNumeroInvalido_LancaErroENaoAltera(string? numero, string erroEsperado)
    {
        var pessoaId = await CriarPessoaAsync("52998224725");
        var id = await servico.CriarAsync(new CriarTelefoneInput(pessoaId, Celular));

        var excecao = await Assert.ThrowsAsync<ValidacaoException>(
            () => servico.AtualizarAsync(id, new AtualizarTelefoneInput(numero)));

        Assert.Equal([erroEsperado], excecao.Erros);
        Assert.Equal(Celular, (await telefoneRepository.ObterPorIdAsync(id))!.Numero.Valor);
    }

    [Fact]
    public async Task Atualizar_TelefoneInexistente_LancaTelefoneNaoEncontrado()
    {
        await Assert.ThrowsAsync<TelefoneNaoEncontradoException>(
            () => servico.AtualizarAsync(TelefoneIdInexistente, new AtualizarTelefoneInput(Celular)));
    }

    [Fact]
    public async Task Excluir_TelefoneExistente_RemoveORegistro()
    {
        var pessoaId = await CriarPessoaAsync("52998224725");
        var id = await servico.CriarAsync(new CriarTelefoneInput(pessoaId, Celular));

        await servico.ExcluirAsync(id);

        Assert.Null(await telefoneRepository.ObterPorIdAsync(id));
    }

    [Fact]
    public async Task Excluir_TelefoneInexistente_LancaTelefoneNaoEncontrado()
    {
        await Assert.ThrowsAsync<TelefoneNaoEncontradoException>(
            () => servico.ExcluirAsync(TelefoneIdInexistente));
    }

    private Task<int> CriarPessoaAsync(string cpf)
    {
        return pessoaRepository.InserirAsync(Pessoa.Criar("Maria da Silva", cpf, null, MomentoDoCadastro));
    }
}
