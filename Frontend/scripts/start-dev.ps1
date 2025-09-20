# Script PowerShell para iniciar API + Frontend em desenvolvimento
# Executa tanto o backend .NET quanto o frontend Vue.js

Write-Host "🚀 Iniciando Sistema de Controle Financeiro" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green

# Verificar se está no diretório correto
$currentPath = Get-Location
Write-Host "📁 Diretório atual: $currentPath" -ForegroundColor Yellow

# Caminhos dos projetos
$backendPath = "..\..\Backend\ControleFinanceiro.API"
$frontendPath = "."

# Função para verificar se um processo está rodando na porta
function Test-Port {
    param([int]$Port)
    try {
        $connection = Test-NetConnection -ComputerName "localhost" -Port $Port -WarningAction SilentlyContinue
        return $connection.TcpTestSucceeded
    }
    catch {
        return $false
    }
}

# Verificar pré-requisitos
Write-Host "🔍 Verificando pré-requisitos..." -ForegroundColor Yellow

# Verificar .NET
try {
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET SDK: $dotnetVersion" -ForegroundColor Green
}
catch {
    Write-Host "❌ .NET SDK não encontrado. Instale o .NET 8 SDK." -ForegroundColor Red
    exit 1
}

# Verificar Node.js
try {
    $nodeVersion = node --version
    Write-Host "✅ Node.js: $nodeVersion" -ForegroundColor Green
}
catch {
    Write-Host "❌ Node.js não encontrado. Instale o Node.js 18+." -ForegroundColor Red
    exit 1
}

# Verificar se as portas estão livres
if (Test-Port -Port 7123) {
    Write-Host "⚠️  Porta 7123 já está em uso. A API pode já estar rodando." -ForegroundColor Yellow
}

if (Test-Port -Port 5173) {
    Write-Host "⚠️  Porta 5173 já está em uso. O frontend pode já estar rodando." -ForegroundColor Yellow
}

# Iniciar API Backend
Write-Host "🔧 Iniciando API Backend (.NET 8)..." -ForegroundColor Cyan
if (Test-Path $backendPath) {
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$backendPath'; Write-Host '🔥 Executando API Backend...' -ForegroundColor Magenta; dotnet run"
    Write-Host "✅ API Backend iniciada em nova janela" -ForegroundColor Green
    Write-Host "   📍 URL: https://localhost:7123" -ForegroundColor Gray
    Write-Host "   📍 Swagger: https://localhost:7123/swagger" -ForegroundColor Gray
} else {
    Write-Host "❌ Diretório da API não encontrado: $backendPath" -ForegroundColor Red
    exit 1
}

# Aguardar um pouco para a API inicializar
Write-Host "⏳ Aguardando API inicializar..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Instalar dependências do frontend se necessário
if (!(Test-Path "node_modules")) {
    Write-Host "📦 Instalando dependências do frontend..." -ForegroundColor Cyan
    npm install
}

# Iniciar Frontend Vue.js
Write-Host "🎨 Iniciando Frontend Vue.js..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$frontendPath'; Write-Host '🎨 Executando Frontend Vue.js...' -ForegroundColor Magenta; npm run dev"
Write-Host "✅ Frontend iniciado em nova janela" -ForegroundColor Green
Write-Host "   📍 URL: http://localhost:5173" -ForegroundColor Gray

# Aguardar um pouco para o frontend inicializar
Start-Sleep -Seconds 3

# Abrir navegador automaticamente
Write-Host "🌐 Abrindo navegador..." -ForegroundColor Cyan
Start-Process "http://localhost:5173"

Write-Host ""
Write-Host "🎉 Sistema iniciado com sucesso!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host "📍 Frontend: http://localhost:5173" -ForegroundColor White
Write-Host "📍 API: https://localhost:7123" -ForegroundColor White
Write-Host "📍 Swagger: https://localhost:7123/swagger" -ForegroundColor White
Write-Host ""
Write-Host "💡 Para parar os serviços, feche as janelas do PowerShell abertas." -ForegroundColor Yellow
Write-Host "💡 Pressione qualquer tecla para sair deste script..." -ForegroundColor Yellow

# Aguardar input do usuário
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
