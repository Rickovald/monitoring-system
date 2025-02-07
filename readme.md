# Система мониторинга данных с виртуальных датчиков

## Описание
Данное приложение состоит из 4 компонентов:
- **Backend (API)**: ASP.NET Core приложение для работы с данными и валидации XML.
- **Sensor Emulator**: Сервис эмуляции данных с 3 виртуальных датчиков.
- **Frontend**: React приложение (с использованием Vite, TypeScript, Material-UI) для отображения данных и загрузки XML.
- **База данных**: PostgreSQL с инициализацией таблицы для хранения данных.

## Запуск с использованием Docker

### 1. Предварительные условия
- Установлен и запущен [Docker Desktop для Windows](https://www.docker.com/products/docker-desktop).

### 2. Запуск
В корне проекта выполните:
```bash
docker-compose up --build
```
Это запустит все 4 контейнера:

PostgreSQL на порту 5432
Backend на порту 5177
Frontend на порту 3000
Sensor Emulator
### 3. Проверка работы
API Swagger: http://localhost:5177/swagger
Frontend: http://localhost:3000


## Запуск без Docker
### 1. PostgreSQL
Установите PostgreSQL локально и создайте базу данных monitoring_system с пользователем postgres и паролем postgres. Выполните скрипт db-init/init.sql для создания таблицы.

### 2. Backend
Откройте папку backend в IDE. Выполните команды:
```bash
dotnet restore
dotnet run
```
По умолчанию API будет доступен по https://localhost:5177.

### 3. Sensor Emulator
Откройте папку sensor-emulator  в IDE и выполните:
```bash
dotnet restore
dotnet run
```

### 4. Frontend
Откройте папку frontend в IDE:
```bash
npm install
npm run dev
```
Приложение запустится на http://localhost:3000.

### Дополнительная информация
- Документация API доступна через Swagger.
- Логика эмулятора: каждую секунду для каждого из 3 датчиков отправляется POST-запрос с случайным значением.
- Фронтенд: реализовано автообновление данных каждые 5 секунд, возможность загрузки XML и отображение результатов в таблице и на графике.