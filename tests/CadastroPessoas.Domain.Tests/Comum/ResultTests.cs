using AwesomeAssertions;
using CadastroPessoas.Domain.Comum;

namespace CadastroPessoas.Domain.Tests.Comum;

public class ResultTests
{
    private static readonly Erro ErroNome = new("nome", "O nome é obrigatório.", TipoErro.Validacao);
    private static readonly Erro ErroCpf = new("cpf", "CPF inválido.", TipoErro.Validacao);
    private static readonly Erro ErroSexo = new("sexo", "Sexo inválido.", TipoErro.Validacao);

    [Fact]
    public void OkIndicaSucessoComValorESemErros()
    {
        var resultado = Result<string>.Ok("valor");

        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().Be("valor");
        resultado.Erros.Should().BeEmpty();
    }

    [Fact]
    public void FalhaComVariosErrosMantemTodosNaOrdem()
    {
        var resultado = Result<string>.Falha(ErroNome, ErroCpf, ErroSexo);

        resultado.Sucesso.Should().BeFalse();
        resultado.Valor.Should().BeNull();
        resultado.Erros.Should().Equal(ErroNome, ErroCpf, ErroSexo);
    }

    [Fact]
    public void ConversaoImplicitaDeValorGeraSucesso()
    {
        Result<int> resultado = 42;

        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().Be(42);
    }

    [Fact]
    public void ConversaoImplicitaDeErroGeraFalha()
    {
        Result<int> resultado = ErroCpf;

        resultado.Sucesso.Should().BeFalse();
        resultado.Erros.Should().ContainSingle().Which.Should().Be(ErroCpf);
    }
}
