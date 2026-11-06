using CadastroPessoas.Api.DTOs;
using CadastroPessoas.Api.Mappers;
using CadastroPessoas.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CadastroPessoas.Api.Controllers;

[ApiController]
[Route("api/telefone")]
[ProducesResponseType<ErrosResponse>(StatusCodes.Status400BadRequest)]
[ProducesResponseType<ErrosResponse>(StatusCodes.Status500InternalServerError)]
public sealed class TelefoneController(ITelefoneService telefoneService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<TelefoneCriadoResponse>(StatusCodes.Status200OK)]
    public async Task<TelefoneCriadoResponse> CriarAsync(TelefoneRequest dados)
    {
        var id = await telefoneService.CriarAsync(TelefoneMapper.ParaCriarTelefoneInput(dados));

        return new TelefoneCriadoResponse(id);
    }

    [HttpGet("{pessoaId}")]
    [ProducesResponseType<ListaDeTelefonesResponse>(StatusCodes.Status200OK)]
    public async Task<ListaDeTelefonesResponse> PesquisarPorPessoaIdAsync(int pessoaId)
    {
        return TelefoneMapper.ParaListaDeTelefonesResponse(await telefoneService.PesquisarPorPessoaIdAsync(pessoaId));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrosResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarAsync(int id, TelefoneRequest dados)
    {
        await telefoneService.AtualizarAsync(id, TelefoneMapper.ParaAtualizarTelefoneInput(dados));

        return Ok();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrosResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExcluirAsync(int id)
    {
        await telefoneService.ExcluirAsync(id);

        return Ok();
    }
}
