FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TodoList.MVC.API/TodoList.MVC.API.csproj", "TodoList.MVC.API/"]
RUN dotnet restore "TodoList.MVC.API/TodoList.MVC.API.csproj"
COPY . .
WORKDIR "/src/TodoList.MVC.API"
RUN dotnet build "TodoList.MVC.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TodoList.MVC.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TodoList.MVC.API.dll"]
