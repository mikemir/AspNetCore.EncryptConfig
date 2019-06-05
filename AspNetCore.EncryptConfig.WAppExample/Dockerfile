FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["AspNetCore.EncryptConfig.WAppExample/AspNetCore.EncryptConfig.WAppExample.csproj", "AspNetCore.EncryptConfig.WAppExample/"]
RUN dotnet restore "AspNetCore.EncryptConfig.WAppExample/AspNetCore.EncryptConfig.WAppExample.csproj"
COPY . .
WORKDIR "/src/AspNetCore.EncryptConfig.WAppExample"
RUN dotnet build "AspNetCore.EncryptConfig.WAppExample.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AspNetCore.EncryptConfig.WAppExample.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AspNetCore.EncryptConfig.WAppExample.dll"]