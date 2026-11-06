using CadastroPessoas.Api.DTOs;
using CadastroPessoas.Api.Mappers;
using CadastroPessoas.Application.Interfaces;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CadastroPessoas.Api.Controllers;

[ApiController]
[Route("api/pessoa")]
[ProducesResponseType<ErrosResponse>(StatusCodes.Status400BadRequest)]
[ProducesResponseType<ErrosResponse>(StatusCodes.Status500InternalServerError)]
public sealed class PessoaController(IPessoaService servico) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<PessoaCriadaResponse>(StatusCodes.Status200OK)]
    public async Task<PessoaCriadaResponse> CriarAsync(PessoaRequest dados)
    {
        var id = await servico.CriarAsync(PessoaMapper.ParaCriarPessoaInput(dados));

        return new PessoaCriadaResponse(id);
    }

    [HttpGet("{id}")]
    [ProducesResponseType<PessoaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrosResponse>(StatusCodes.Status404NotFound)]
    public async Task<PessoaResponse> ConsultarPorIdAsync(int id)
    {
        return ParaRespostaOuNaoEncontrada(await servico.ConsultarPorIdAsync(id));
    }

    [HttpGet("cpf/{valor}")]
    [ProducesResponseType<PessoaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrosResponse>(StatusCodes.Status404NotFound)]
    public async Task<PessoaResponse> ConsultarPorCpfAsync(string? valor)
    {
        return ParaRespostaOuNaoEncontrada(await servico.ConsultarPorCpfAsync(valor));
    }

    [HttpGet("nome/{valor}")]
    [ProducesResponseType<ListaDePessoasResponse>(StatusCodes.Status200OK)]
    public async Task<ListaDePessoasResponse> PesquisarPorNomeAsync(string? valor)
    {
        return PessoaMapper.ParaListaDePessoasResponse(await servico.PesquisarPorNomeAsync(valor));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrosResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarAsync(int id, PessoaRequest dados)
    {
        await servico.AtualizarAsync(id, PessoaMapper.ParaAtualizarPessoaInput(dados));

        return Ok();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrosResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExcluirAsync(int id)
    {
        await servico.ExcluirAsync(id);

        return Ok();
    }

    private static PessoaResponse ParaRespostaOuNaoEncontrada(Pessoa? pessoa)
    {
        return pessoa is null
            ? throw new PessoaNaoEncontradaException()
            : PessoaMapper.ParaPessoaResponse(pessoa);
    }
}
