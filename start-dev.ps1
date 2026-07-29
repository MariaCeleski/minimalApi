# Script para iniciar projeto em modo desenvolvimento
# Backend: ASP.NET Core Minimal API
# Frontend: React com Vite

param(
    [switch]$Backend,
    [switch]$Frontend,
    [switch]$Both = $true
)

Write-Host "=== Personal Financial Management App ===" -ForegroundColor Cyan
Write-Host "Iniciando servidor(s) de desenvolvimento..." -ForegroundColor Yellow

# Verificar se está no diretório correto
if (-not (Test-Path "minimal-api.csproj")) {
    Write-Host "Erro: Arquivo minimal-api.csproj não encontrado." -ForegroundColor Red
    Write-Host "Execute este script no diretório raiz do projeto." -ForegroundColor Red
    exit 1
}

# Iniciar backend se solicitado
if ($Backend -or $Both) {
    Write-Host "`n[Backend] Iniciando ASP.NET Core..." -ForegroundColor Green
    Write-Host "URL: http://localhost:5209" -ForegroundColor Cyan
    Write-Host "Swagger: http://localhost:5209/swagger" -ForegroundColor Cyan
    
    $backendProcess = Start-Process powershell -ArgumentList "-NoExit -Command `"cd '$PWD'; dotnet run --configuration Debug`"" -PassThru
    Write-Host "Backend iniciado com PID: $($backendProcess.Id)" -ForegroundColor Green
}

# Aguardar um pouco antes de iniciar frontend
Start-Sleep -Seconds 3

# Iniciar frontend se solicitado
if ($Frontend -or $Both) {
    Write-Host "`n[Frontend] Iniciando React (Vite)..." -ForegroundColor Green
    Write-Host "URL: http://localhost:5173" -ForegroundColor Cyan
    
    $frontendProcess = Start-Process powershell -ArgumentList "-NoExit -Command `"cd '$PWD\frontend'; npm run dev`"" -PassThru
    Write-Host "Frontend iniciado com PID: $($frontendProcess.Id)" -ForegroundColor Green
}

Write-Host "`n=== Servidores iniciados com sucesso ===" -ForegroundColor Green
Write-Host "Backend:  http://localhost:5209" -ForegroundColor Cyan
Write-Host "Frontend: http://localhost:5173" -ForegroundColor Cyan
Write-Host "Swagger:  http://localhost:5209/swagger" -ForegroundColor Cyan
Write-Host "`nPressione Ctrl+C para parar os servidores." -ForegroundColor Yellow

# Manter o script rodando
while ($true) {
    Start-Sleep -Seconds 1
}
