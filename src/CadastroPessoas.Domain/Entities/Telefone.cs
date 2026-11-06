using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.ValueObjects;

namespace CadastroPessoas.Domain.Entities;

public sealed class Telefone
{
    public int Id { get; }
    public int PessoaId { get; }
    public NumeroDeTelefone Numero { get; private set; }

    private Telefone(int id, int pessoaId, NumeroDeTelefone numero)
    {
        Id = id;
        PessoaId = pessoaId;
        Numero = numero;
    }

    public static IReadOnlyList<string> ValidarCriacao(int? pessoaId, string? numero)
    {
        var erros = NumeroDeTelefone.ObterErros(numero).ToList();

        if (pessoaId is null)
        {
            erros.Add(ErrosDeTelefone.PessoaIdObrigatorio);
        }

        return erros;
    }

    public static Telefone Criar(int? pessoaId, string? numero)
    {
        LancarExcecaoSeHouverErros(ValidarCriacao(pessoaId, numero));

        return new Telefone(0, pessoaId!.Value, new NumeroDeTelefone(numero));
    }

    public static Telefone Reconstituir(int id, int pessoaId, NumeroDeTelefone numero)
    {
        return new Telefone(id, pessoaId, numero);
    }

    public void AtualizarNumero(string? numero)
    {
        LancarExcecaoSeHouverErros(NumeroDeTelefone.ObterErros(numero));

        Numero = new NumeroDeTelefone(numero);
    }

    private static void LancarExcecaoSeHouverErros(IReadOnlyList<string> erros)
    {
        if (erros.Count > 0)
        {
            throw new ValidacaoException(erros);
        }
    }
}
