using AwesomeAssertions;
using CadastroPessoas.Domain.Comum;
using CadastroPessoas.Domain.Pessoas;

namespace CadastroPessoas.Domain.Tests.Pessoas;

public class PessoaErrosTests
{
    [Fact]
    public void FormatoDeDataInvalidoTemMensagemDaSpec()
    {
        PessoaErros.DataNascimentoFormatoInvalido.Should().Be(
            new Erro("dataNascimento", "A data de nascimento deve estar no formato AAAA-MM-DD.", TipoErro.Validacao));
    }

    [Fact]
    public void CpfDuplicadoEhConflito()
    {
        PessoaErros.CpfDuplicado.Should().Be(
            new Erro("cpf", "Já existe uma pessoa cadastrada com este CPF.", TipoErro.Conflito));
    }

    [Fact]
    public void NaoEncontradaEhNaoEncontrado()
    {
        PessoaErros.NaoEncontrada.Should().Be(
            new Erro("id", "Pessoa não encontrada.", TipoErro.NaoEncontrado));
    }

    [Fact]
    public void CpfInvalidoTemMensagemDaSpec()
    {
        PessoaErros.CpfInvalido.Mensagem.Should().Be("CPF inválido.");
        PessoaErros.CpfObrigatorio.Mensagem.Should().Be("O campo cpf é obrigatório.");
    }
}
