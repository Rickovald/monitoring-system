# Используем официальный образ .NET 8 SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем .csproj и восстанавливаем зависимости
COPY backend/*.csproj ./ 
RUN dotnet restore

# Копируем остальные файлы и публикуем проект
COPY backend/. ./
RUN dotnet publish -c Release -o /app

# Образ для запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Копируем файлы из стадии сборки
COPY --from=build /app . 

# Открываем порты 80 и 443
EXPOSE 8080
EXPOSE 443

# Запуск приложения
ENTRYPOINT ["dotnet", "backend.dll"]
