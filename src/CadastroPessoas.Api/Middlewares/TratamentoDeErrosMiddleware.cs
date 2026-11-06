using CadastroPessoas.Api.DTOs;
using CadastroPessoas.Domain.Exceptions;

namespace CadastroPessoas.Api.Middlewares;

public sealed class TratamentoDeErrosMiddleware(RequestDelegate proximo, ILogger<TratamentoDeErrosMiddleware> registrador)
{
    private const string FalhaDeServidor = "Ocorreu uma falha interna ao processar a requisição.";

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await proximo(contexto);
        }
        catch (RegistroNaoEncontradoException excecao)
        {
            await EscreverErrosAsync(contexto, StatusCodes.Status404NotFound, excecao.Erros);
        }
        catch (DominioException excecao)
        {
            await EscreverErrosAsync(contexto, StatusCodes.Status400BadRequest, excecao.Erros);
        }
        catch (Exception excecao)
        {
            registrador.LogError(excecao, "Falha não tratada ao processar a requisição.");
            await EscreverErrosAsync(contexto, StatusCodes.Status500InternalServerError, [FalhaDeServidor]);
        }
    }

    private static Task EscreverErrosAsync(HttpContext contexto, int codigoDeStatus, IReadOnlyList<string> erros)
    {
        contexto.Response.StatusCode = codigoDeStatus;

        return contexto.Response.WriteAsJsonAsync(new ErrosResponse(erros));
    }
}
