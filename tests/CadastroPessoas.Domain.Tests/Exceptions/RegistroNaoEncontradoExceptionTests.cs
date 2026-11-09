using CadastroPessoas.Domain.Exceptions;

namespace CadastroPessoas.Domain.Tests.Exceptions;

public class RegistroNaoEncontradoExceptionTests
{
    [Fact]
    public void PessoaNaoEncontrada_EhRegistroNaoEncontradoComAMensagemDePessoa()
    {
        RegistroNaoEncontradoException excecao = new PessoaNaoEncontradaException();

        Assert.Equal([ErrosDePessoa.PessoaNaoEncontrada], excecao.Erros);
    }

    [Fact]
    public void TelefoneNaoEncontrado_EhRegistroNaoEncontradoComAMensagemDeTelefone()
    {
        RegistroNaoEncontradoException excecao = new TelefoneNaoEncontradoException();

        Assert.Equal([ErrosDeTelefone.TelefoneNaoEncontrado], excecao.Erros);
    }

    [Fact]
    public void EnderecoNaoEncontrado_EhRegistroNaoEncontradoComAMensagemDeEndereco()
    {
        RegistroNaoEncontradoException excecao = new EnderecoNaoEncontradoException();

        Assert.Equal([ErrosDeEndereco.EnderecoNaoEncontrado], excecao.Erros);
    }

    [Fact]
    public void RegistroNaoEncontrado_EhDominioException()
    {
        Assert.IsAssignableFrom<DominioException>(new TelefoneNaoEncontradoException());
    }
}
