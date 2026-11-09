using CadastroPessoas.Api.DTOs;
using CadastroPessoas.Api.Mappers;
using CadastroPessoas.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CadastroPessoas.Api.Controllers;

[ApiController]
[Route("api/endereco")]
[ProducesResponseType<ErrosResponse>(StatusCodes.Status400BadRequest)]
[ProducesResponseType<ErrosResponse>(StatusCodes.Status500InternalServerError)]
public sealed class EnderecoController(IEnderecoService enderecoService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<EnderecoCriadoResponse>(StatusCodes.Status200OK)]
    public async Task<EnderecoCriadoResponse> CriarAsync(CriarEnderecoRequest dados)
    {
        var id = await enderecoService.CriarAsync(EnderecoMapper.ParaCriarEnderecoInput(dados));

        return new EnderecoCriadoResponse(id);
    }

    [HttpGet("{pessoaId}")]
    [ProducesResponseType<ListaDeEnderecosResponse>(StatusCodes.Status200OK)]
    public async Task<ListaDeEnderecosResponse> PesquisarPorPessoaIdAsync(int pessoaId)
    {
        return EnderecoMapper.ParaListaDeEnderecosResponse(await enderecoService.PesquisarPorPessoaIdAsync(pessoaId));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrosResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarAsync(int id, AtualizarEnderecoRequest dados)
    {
        await enderecoService.AtualizarAsync(id, EnderecoMapper.ParaAtualizarEnderecoInput(dados));

        return Ok();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrosResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExcluirAsync(int id)
    {
        await enderecoService.ExcluirAsync(id);

        return Ok();
    }
}
