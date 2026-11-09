using CadastroPessoas.Application.Services;
using CadastroPessoas.Application.Tests.Fakes;
using CadastroPessoas.Application.UseCases.Endereco.AtualizarEndereco;
using CadastroPessoas.Application.UseCases.Endereco.CriarEndereco;
using CadastroPessoas.Application.UseCases.Endereco.ExcluirEndereco;
using CadastroPessoas.Application.UseCases.Endereco.PesquisarEndereco;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Enums;
using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.ValueObjects;

namespace CadastroPessoas.Application.Tests.Services;

public class EnderecoServiceTests
{
    private const int PessoaIdInexistente = 999;
    private const int EnderecoIdInexistente = 999;
    private static readonly DateTime MomentoDoCadastro = new(2026, 11, 9, 10, 0, 0);

    private static readonly DadosDoEndereco DadosCompletos = new(
        (int)TipoDeEndereco.Comercial,
        "Avenida Paulista",
        "1000",
        "Sala 12",
        "Bela Vista",
        "São Paulo",
        "SP",
        "01310-100");

    private static readonly DadosDoEndereco DadosSomenteComObrigatorios =
        new(null, null, null, null, null, "Curitiba", "PR", null);

    private readonly PessoaRepositoryEmMemoria pessoaRepository = new();
    private readonly EnderecoRepositoryEmMemoria enderecoRepository = new();
    private readonly EnderecoService servico;

    public EnderecoServiceTests()
    {
        servico = new EnderecoService(
            new CriarEnderecoUseCase(enderecoRepository, pessoaRepository),
            new PesquisarEnderecoUseCase(enderecoRepository),
            new AtualizarEnderecoUseCase(enderecoRepository),
            new ExcluirEnderecoUseCase(enderecoRepository));
    }

    [Fact]
    public async Task Criar_ComDadosValidos_RetornaOIdEGravaOEndereco()
    {
        var pessoaId = await CriarPessoaAsync("52998224725");

        var id = await servico.CriarAsync(new CriarEnderecoInput(pessoaId, DadosCompletos with { Estado = "sp" }));

        var gravado = await enderecoRepository.ObterPorIdAsync(id);
        Assert.Equal(pessoaId, gravado!.PessoaId);
        Assert.Equal(TipoDeEndereco.Comercial, gravado.Tipo);
        Assert.Equal("Avenida Paulista", gravado.Logradouro);
        Assert.Equal("São Paulo", gravado.Municipio);
        Assert.Equal("SP", gravado.Estado.Valor);
        Assert.Equal("01310100", gravado.Cep?.Valor);
    }

    [Fact]
    public async Task Criar_SemTipo_GravaComoNaoInformado()
    {
        var pessoaId = await CriarPessoaAsync("52998224725");

        var id = await servico.CriarAsync(new CriarEnderecoInput(pessoaId, DadosSomenteComObrigatorios));

        Assert.Equal(TipoDeEndereco.NaoInformado, (await enderecoRepository.ObterPorIdAsync(id))!.Tipo);
    }

    [Fact]
    public async Task Criar_ComPessoaInexistente_LancaErroENaoGrava()
    {
        var excecao = await Assert.ThrowsAsync<ValidacaoException>(
            () => servico.CriarAsync(new CriarEnderecoInput(PessoaIdInexistente, DadosCompletos)));

        Assert.Equal([ErrosDeEndereco.PessoaInexistente], excecao.Erros);
        Assert.Equal(0, enderecoRepository.QuantidadeDeRegistros);
    }

    [Fact]
    public async Task Criar_SemPessoaId_LancaErroSemConsultarAPessoa()
    {
        var excecao = await Assert.ThrowsAsync<ValidacaoException>(
            () => servico.CriarAsync(new CriarEnderecoInput(null, DadosCompletos)));

        Assert.Equal([ErrosDeEndereco.PessoaIdObrigatorio], excecao.Erros);
        Assert.Equal(0, pessoaRepository.QuantidadeDeConsultasPorId);
    }

    [Fact]
    public async Task Criar_ComDadosInvalidosEPessoaInexistente_ReuneTodosOsErros()
    {
        var dados = new DadosDoEndereco(9, null, null, null, null, " ", "XX", "0131");

        var excecao = await Assert.ThrowsAsync<ValidacaoException>(
            () => servico.CriarAsync(new CriarEnderecoInput(PessoaIdInexistente, dados)));

        Assert.Equal(
            [
                ErrosDeEndereco.TipoInvalido,
                ErrosDeEndereco.MunicipioObrigatorio,
                ErrosDeEndereco.SiglaDeEstadoInvalida,
                ErrosDeEndereco.CepComTamanhoInvalido,
                ErrosDeEndereco.PessoaInexistente
            ],
            excecao.Erros);
    }

    [Fact]
    public async Task PesquisarPorPessoaId_RetornaSomenteOsEnderecosDaPessoaOrdenadosPeloId()
    {
        var pessoaId = await CriarPessoaAsync("52998224725");
        var outraPessoaId = await CriarPessoaAsync("11144477735");
        var primeiroId = await servico.CriarAsync(new CriarEnderecoInput(pessoaId, DadosCompletos));
        await servico.CriarAsync(new CriarEnderecoInput(outraPessoaId, DadosCompletos));
        var segundoId = await servico.CriarAsync(new CriarEnderecoInput(pessoaId, DadosSomenteComObrigatorios));

        var enderecos = await servico.PesquisarPorPessoaIdAsync(pessoaId);

        Assert.Equal([primeiroId, segundoId], enderecos.Select(endereco => endereco.Id));
    }

    [Fact]
    public async Task PesquisarPorPessoaId_SemEnderecos_RetornaListaVazia()
    {
        var pessoaId = await CriarPessoaAsync("52998224725");

        Assert.Empty(await servico.PesquisarPorPessoaIdAsync(pessoaId));
    }

    [Fact]
    public async Task PesquisarPorPessoaId_ComPessoaInexistente_RetornaListaVazia()
    {
        Assert.Empty(await servico.PesquisarPorPessoaIdAsync(PessoaIdInexistente));
    }

    [Fact]
    public async Task Atualizar_ComDadosValidos_AlteraTodosOsCamposSemAlterarOPessoaId()
    {
        var (id, pessoaId) = await CriarEnderecoCompletoAsync();
        var novosDados = new DadosDoEndereco((int)TipoDeEndereco.Outros, "Rua B", "20", "Casa", "Batel", "Curitiba", "pr", "80420");

        await servico.AtualizarAsync(id, new AtualizarEnderecoInput(novosDados));

        var atualizado = await enderecoRepository.ObterPorIdAsync(id);
        Assert.Equal(pessoaId, atualizado!.PessoaId);
        Assert.Equal(TipoDeEndereco.Outros, atualizado.Tipo);
        Assert.Equal("Rua B", atualizado.Logradouro);
        Assert.Equal("20", atualizado.Numero);
        Assert.Equal("Casa", atualizado.Complemento);
        Assert.Equal("Batel", atualizado.Bairro);
        Assert.Equal("Curitiba", atualizado.Municipio);
        Assert.Equal("PR", atualizado.Estado.Valor);
        Assert.Equal("80420", atualizado.Cep?.Valor);
    }

    [Fact]
    public async Task Atualizar_SemTipo_MantemOTipoAtual()
    {
        var (id, _) = await CriarEnderecoCompletoAsync();

        await servico.AtualizarAsync(id, new AtualizarEnderecoInput(DadosSomenteComObrigatorios));

        Assert.Equal(TipoDeEndereco.Comercial, (await enderecoRepository.ObterPorIdAsync(id))!.Tipo);
    }

    [Fact]
    public async Task Atualizar_SemCamposOpcionais_DeixaOsOpcionaisNulos()
    {
        var (id, _) = await CriarEnderecoCompletoAsync();

        await servico.AtualizarAsync(id, new AtualizarEnderecoInput(DadosSomenteComObrigatorios));

        var atualizado = await enderecoRepository.ObterPorIdAsync(id);
        Assert.Null(atualizado!.Logradouro);
        Assert.Null(atualizado.Numero);
        Assert.Null(atualizado.Complemento);
        Assert.Null(atualizado.Bairro);
        Assert.Null(atualizado.Cep);
    }

    [Fact]
    public async Task Atualizar_SemMunicipioESemEstado_ReuneOsErrosENaoAltera()
    {
        var (id, _) = await CriarEnderecoCompletoAsync();
        var dados = new DadosDoEndereco(null, "Rua B", null, null, null, null, " ", null);

        var excecao = await Assert.ThrowsAsync<ValidacaoException>(
            () => servico.AtualizarAsync(id, new AtualizarEnderecoInput(dados)));

        Assert.Equal([ErrosDeEndereco.MunicipioObrigatorio, ErrosDeEndereco.EstadoObrigatorio], excecao.Erros);
        Assert.Equal("Avenida Paulista", (await enderecoRepository.ObterPorIdAsync(id))!.Logradouro);
    }

    [Fact]
    public async Task Atualizar_EnderecoInexistente_LancaEnderecoNaoEncontrado()
    {
        await Assert.ThrowsAsync<EnderecoNaoEncontradoException>(
            () => servico.AtualizarAsync(EnderecoIdInexistente, new AtualizarEnderecoInput(DadosCompletos)));
    }

    [Fact]
    public async Task Excluir_EnderecoExistente_RemoveORegistro()
    {
        var (id, _) = await CriarEnderecoCompletoAsync();

        await servico.ExcluirAsync(id);

        Assert.Null(await enderecoRepository.ObterPorIdAsync(id));
    }

    [Fact]
    public async Task Excluir_EnderecoInexistente_LancaEnderecoNaoEncontrado()
    {
        await Assert.ThrowsAsync<EnderecoNaoEncontradoException>(
            () => servico.ExcluirAsync(EnderecoIdInexistente));
    }

    private async Task<(int Id, int PessoaId)> CriarEnderecoCompletoAsync()
    {
        var pessoaId = await CriarPessoaAsync("52998224725");
        var id = await servico.CriarAsync(new CriarEnderecoInput(pessoaId, DadosCompletos));

        return (id, pessoaId);
    }

    private Task<int> CriarPessoaAsync(string cpf)
    {
        return pessoaRepository.InserirAsync(Pessoa.Criar("Maria da Silva", cpf, null, MomentoDoCadastro));
    }
}
