using CadastroPessoas.Api.DTOs;
using CadastroPessoas.Api.Mappers;
using CadastroPessoas.Api.Middlewares;
using CadastroPessoas.Infrastructure;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(opcoes => opcoes.InvalidModelStateResponseFactory = contexto =>
        new BadRequestObjectResult(new ErrosResponse(ErrosDeRequisicaoMapper.ParaErros(contexto.ModelState))));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AdicionarInfraestrutura(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<TratamentoDeErrosMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
