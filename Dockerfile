
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Clean.Web/Clean.Web.csproj", "Clean.Web/"]
COPY ["Clean.Application/Clean.Application.csproj", "Clean.Application/"]
COPY ["Clean.Domain/Clean.Domain.csproj", "Clean.Domain/"]
COPY ["Clean.Infrastructure/Clean.Infrastructure.csproj", "Clean.Infrastructure/"]
COPY ["loginForm.sln", "./"]

RUN dotnet restore "loginForm.sln"


COPY . .
RUN dotnet publish "Clean.Web/Clean.Web.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Use the PORT provided by Railway
ENV ASPNETCORE_URLS=http://+:${PORT}
ENTRYPOINT ["dotnet", "Clean.Web.dll"]