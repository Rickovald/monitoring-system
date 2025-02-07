# Сборка проекта
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем .csproj и восстанавливаем зависимости
COPY ./sensor-emulator/*.csproj ./ 
RUN dotnet restore

# Копируем остальные файлы и публикуем проект
COPY ./sensor-emulator ./ 
RUN dotnet publish -c Release -o /app

# Используем минимальный образ для рантайма
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app

# Копируем файлы из стадии сборки
COPY --from=build /app . 

# Запуск приложения
ENTRYPOINT ["dotnet", "sensor-emulator.dll"]
