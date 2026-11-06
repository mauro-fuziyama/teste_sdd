# Imagem da API CadastroPessoas. A aplicação não vai para produção (D-017).
# Segredos não entram na imagem: CHAVE_DE_SEGURANCA e ConnectionStrings__CadastroPessoas
# são informadas por variável de ambiente ao executar o container (D-003).

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/CadastroPessoas.Domain/CadastroPessoas.Domain.csproj src/CadastroPessoas.Domain/
COPY src/CadastroPessoas.Application/CadastroPessoas.Application.csproj src/CadastroPessoas.Application/
COPY src/CadastroPessoas.Infrastructure/CadastroPessoas.Infrastructure.csproj src/CadastroPessoas.Infrastructure/
COPY src/CadastroPessoas.Api/CadastroPessoas.Api.csproj src/CadastroPessoas.Api/
RUN dotnet restore src/CadastroPessoas.Api/CadastroPessoas.Api.csproj

COPY src/ src/
RUN dotnet publish src/CadastroPessoas.Api/CadastroPessoas.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    -p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

# Horário local da aplicação (D-005). A verificação falha o build se os dados de fuso faltarem na imagem.
ENV TZ=America/Sao_Paulo
RUN test -f /usr/share/zoneinfo/America/Sao_Paulo

WORKDIR /app
COPY --from=build /app/publish .

USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "CadastroPessoas.Api.dll"]
