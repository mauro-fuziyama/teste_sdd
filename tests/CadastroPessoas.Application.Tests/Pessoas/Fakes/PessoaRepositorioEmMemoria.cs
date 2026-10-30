using System.Globalization;

using CadastroPessoas.Application.Pessoas.Modelos;
using CadastroPessoas.Application.Pessoas.Portas.Saida;
using CadastroPessoas.Domain.Pessoas;
using CadastroPessoas.Domain.Pessoas.ValueObjects;

namespace CadastroPessoas.Application.Tests.Pessoas.Fakes;

/// <summary>
/// Guarda cópias dos dados, como o banco faria: alterar uma <see cref="Pessoa"/> obtida daqui
/// não muda o que está gravado até que um método de gravação seja chamado.
/// </summary>
internal sealed class PessoaRepositorioEmMemoria : IPessoaRepositorio
{
    private readonly Dictionary<int, Registro> _registros = [];
    private int _ultimoId;

    public bool LancarCpfDuplicadoNaGravacao { get; set; }

    public FiltroPessoas? UltimoFiltro { get; private set; }

    public Pessoa Inserir(Pessoa pessoa, bool ativo = true)
    {
        pessoa.DefinirId(++_ultimoId);
        _registros[pessoa.Id] = Registro.De(pessoa, ativo);
        return pessoa;
    }

    public Registro? Gravado(int id) => _registros.GetValueOrDefault(id);

    public Task<Pessoa?> ObterPorIdAsync(int id, CancellationToken ct) =>
        Task.FromResult(Ativos().FirstOrDefault(r => r.Id == id)?.ParaPessoa());

    public Task<bool> ExisteCpfAsync(Cpf cpf, int? ignorarId, CancellationToken ct) =>
        Task.FromResult(_registros.Values.Any(r => r.Cpf == cpf.Numero && r.Id != ignorarId));

    public Task<int?> ObterIdInativoPorCpfAsync(Cpf cpf, CancellationToken ct) =>
        Task.FromResult(_registros.Values.FirstOrDefault(r => r.Cpf == cpf.Numero && !r.Ativo)?.Id);

    public Task<(IReadOnlyList<Pessoa> Itens, int Total)> ListarAsync(FiltroPessoas filtro, CancellationToken ct)
    {
        UltimoFiltro = filtro;
        var encontrados = Ativos().Where(r => AtendeFiltro(r, filtro)).OrderBy(r => r.Nome, StringComparer.Ordinal).ToList();
        var pagina = encontrados.Skip((filtro.Pagina - 1) * filtro.TamanhoPagina).Take(filtro.TamanhoPagina);

        IReadOnlyList<Pessoa> itens = [.. pagina.Select(r => r.ParaPessoa())];
        return Task.FromResult((itens, encontrados.Count));
    }

    public Task<int> AdicionarAsync(Pessoa pessoa, CancellationToken ct)
    {
        LancarSeConfigurado();
        var id = ++_ultimoId;
        _registros[id] = Registro.De(pessoa, ativo: true) with { Id = id };
        return Task.FromResult(id);
    }

    public Task AtualizarAsync(Pessoa pessoa, CancellationToken ct)
    {
        LancarSeConfigurado();
        _registros[pessoa.Id] = Registro.De(pessoa, ativo: true) with { CriadoEm = _registros[pessoa.Id].CriadoEm };
        return Task.CompletedTask;
    }

    public Task ReativarAsync(Pessoa pessoa, CancellationToken ct)
    {
        _registros[pessoa.Id] = Registro.De(pessoa, ativo: true);
        return Task.CompletedTask;
    }

    public Task DesativarAsync(Pessoa pessoa, CancellationToken ct)
    {
        _registros[pessoa.Id] = _registros[pessoa.Id] with { Ativo = false, AtualizadoEm = pessoa.AtualizadoEm };
        return Task.CompletedTask;
    }

    private IEnumerable<Registro> Ativos() => _registros.Values.Where(r => r.Ativo);

    private static bool AtendeFiltro(Registro registro, FiltroPessoas filtro) =>
        (filtro.Cpf is null || registro.Cpf == filtro.Cpf)
        && (filtro.Nome is null || ContemSemAcento(registro.Nome, filtro.Nome));

    private static bool ContemSemAcento(string texto, string trecho) =>
        CultureInfo.InvariantCulture.CompareInfo.IndexOf(
            texto, trecho, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0;

    private void LancarSeConfigurado()
    {
        if (LancarCpfDuplicadoNaGravacao)
            throw new CpfDuplicadoException();
    }

    internal sealed record Registro(
        int Id,
        string Nome,
        DateOnly DataNascimento,
        string Cpf,
        string Sexo,
        DateTimeOffset CriadoEm,
        DateTimeOffset AtualizadoEm,
        bool Ativo)
    {
        public static Registro De(Pessoa pessoa, bool ativo) => new(
            pessoa.Id,
            pessoa.Nome.Valor,
            pessoa.DataNascimento.Valor,
            pessoa.Cpf.Numero,
            pessoa.Sexo.Codigo,
            pessoa.CriadoEm,
            pessoa.AtualizadoEm,
            ativo);

        public Pessoa ParaPessoa() =>
            Pessoa.Restaurar(Id, Nome, DataNascimento, Cpf, Sexo, CriadoEm, AtualizadoEm);
    }
}
