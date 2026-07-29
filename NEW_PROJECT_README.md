# Financial Management App - Estrutura Profissional

## 📍 Localização do Projeto

O projeto foi reorganizado para a seguinte estrutura:

```
c:\Aluracord\minimalApi\financial-management-app\
```

> ⚠️ O projeto antigo em `c:\Aluracord\minimalApi\minimalApi\` pode ser usado como referência/backup

---

## 🏗️ Arquitetura do Projeto

### 🔙 Backend (.NET Core 9)

**Padrão:** Clean Architecture + DDD

```
backend/
├── src/
│   ├── Financial.Api              ← API REST (Endpoints, Middleware)
│   ├── Financial.Domain           ← Lógica de Negócio (Entities, Rules)
│   ├── Financial.Application      ← Casos de Uso (Services, DTOs)
│   └── Financial.Infrastructure   ← Persistência (DB, Repositories)
└── tests/                          ← Suite de Testes
```

**Camadas:**
1. **API Layer** - Endpoints HTTP, Middleware, Validações HTTP
2. **Domain Layer** - Entidades, Interfaces, Regras de Negócio
3. **Application Layer** - Serviços, DTOs, Mapeadores, Casos de Uso
4. **Infrastructure Layer** - Repositories, DbContext, Migrations

### 💻 Frontend (React + TypeScript)

**Padrão:** Component-Based + Feature-Driven

```
frontend/
├── src/
│   ├── components/     ← Componentes reutilizáveis e por feature
│   ├── pages/          ← Páginas da aplicação
│   ├── services/       ← Chamadas de API
│   ├── hooks/          ← Custom React Hooks
│   ├── context/        ← Context API (State Management)
│   ├── types/          ← TypeScript Types
│   ├── utils/          ← Funções utilitárias
│   └── styles/         ← CSS Global e Variáveis
└── tests/              ← Testes de Componentes
```

### 📊 Database

```
database/
├── scripts/      ← Scripts de inicialização
├── migrations/   ← Migrações EF Core
├── schemas/      ← Schemas SQL
└── backups/      ← Backups de dados
```

### 📚 Documentação

```
docs/
├── guides/       ← Guias de desenvolvimento
├── api/          ← Documentação de API
└── specs/        ← Especificações do projeto
```

---

## 🚀 Como Começar

### Pré-requisitos

- .NET 9 SDK
- Node.js 18+
- npm ou yarn
- Git
- Visual Studio 2022 ou VS Code

### Instalação do Backend

```bash
cd backend

# Restaurar dependências
dotnet restore

# Compilar solução
dotnet build

# Executar migrações (se houver database.db)
dotnet ef database update

# Executar testes
dotnet test

# Iniciar API
dotnet run --project src/Financial.Api/Financial.Api.csproj
```

API estará disponível em: `https://localhost:5001`
Swagger: `https://localhost:5001/swagger`

### Instalação do Frontend

```bash
cd frontend

# Instalar dependências
npm install

# Iniciar servidor de desenvolvimento
npm run dev

# Executar testes
npm run test

# Build para produção
npm run build
```

Frontend estará disponível em: `http://localhost:5173`

---

## 📦 Estrutura de Pastas Detalhada

### Backend - src/Financial.Api/

```
Financial.Api/
├── Endpoints/              ← Endpoints da API
│   ├── DashboardEndpoints.cs
│   ├── TransactionEndpoints.cs
│   ├── ReportEndpoints.cs
│   └── ...
├── Middleware/             ← Middlewares
│   ├── GlobalExceptionMiddleware.cs
│   └── ...
├── Program.cs              ← Configuração da aplicação
├── appsettings.json        ← Configurações
├── appsettings.Development.json
└── Financial.Api.csproj
```

### Backend - src/Financial.Domain/

```
Financial.Domain/
├── Entities/               ← Modelos de domínio
│   ├── User.cs
│   ├── Transaction.cs
│   ├── Category.cs
│   └── ...
├── Exceptions/             ← Exceções de negócio
│   ├── BusinessRuleException.cs
│   └── ...
├── Interfaces/             ← Contratos
│   ├── Repositories/
│   │   └── IRepository.cs
│   └── Services/
│       └── ITransactionService.cs
├── Validators/             ← Validadores
│   └── TransactionValidator.cs
└── Financial.Domain.csproj
```

### Backend - src/Financial.Application/

```
Financial.Application/
├── DTOs/                   ← Data Transfer Objects
│   ├── User/
│   ├── Transaction/
│   ├── Dashboard/
│   ├── Report/
│   └── Auth/
├── Services/               ← Implementação de serviços
│   ├── TransactionService.cs
│   ├── DashboardService.cs
│   └── ...
├── Mappers/                ← AutoMapper profiles
│   └── TransactionMapper.cs
├── Extensions/             ← Extensões de serviços
│   └── ServiceCollectionExtensions.cs
└── Financial.Application.csproj
```

### Backend - src/Financial.Infrastructure/

```
Financial.Infrastructure/
├── Persistence/            ← Contexto de banco de dados
│   └── DbContexto.cs
├── Repositories/           ← Implementações de repositórios
│   ├── Repository.cs (genérico)
│   ├── TransactionRepository.cs
│   └── ...
├── Configuration/          ← Configuração de entidades EF Core
│   └── TransactionConfiguration.cs
├── Migrations/             ← Migrações de banco de dados
│   └── [...]
├── Extensions/             ← Extensões de configuração
│   └── ServiceCollectionExtensions.cs
└── Financial.Infrastructure.csproj
```

### Frontend - src/

```
src/
├── components/
│   ├── common/             ← Componentes reutilizáveis
│   │   ├── Button/
│   │   ├── Card/
│   │   ├── Modal/
│   │   └── ...
│   ├── layout/             ← Componentes de layout
│   │   ├── Header/
│   │   ├── Sidebar/
│   │   └── Layout.tsx
│   ├── dashboard/          ← Feature: Dashboard
│   │   ├── BalanceCard.tsx
│   │   ├── CategoryChart.tsx
│   │   └── ...
│   ├── transactions/       ← Feature: Transações
│   ├── reports/            ← Feature: Relatórios
│   ├── goals/              ← Feature: Metas
│   └── auth/               ← Feature: Autenticação
├── pages/                  ← Páginas (roteadas)
│   ├── DashboardPage.tsx
│   ├── TransactionsPage.tsx
│   ├── ReportsPage.tsx
│   └── ...
├── services/               ← Serviços de API
│   ├── api/
│   │   ├── client.ts       ← Configuração HTTP
│   │   ├── endpoints.ts    ← URLs das APIs
│   │   └── interceptors.ts
│   ├── transaction.service.ts
│   ├── dashboard.service.ts
│   └── ...
├── hooks/                  ← Custom React Hooks
│   ├── useApi.ts
│   ├── useAuth.ts
│   ├── usePagination.ts
│   └── ...
├── context/                ← Context API
│   ├── AuthContext.tsx
│   ├── ThemeContext.tsx
│   ├── TransactionContext.tsx
│   └── ...
├── types/                  ← TypeScript Types
│   ├── index.ts
│   ├── api.types.ts
│   └── entities.types.ts
├── utils/                  ← Funções utilitárias
│   ├── formatting.ts
│   ├── validation.ts
│   ├── date.ts
│   └── currency.ts
└── styles/                 ← CSS Global
    ├── global.css
    ├── variables.css
    └── animations.css
```

---

## 🔄 Fluxo de Dados

```
Frontend (React)
    ↓
HTTP Request (Axios)
    ↓
Backend API (Endpoints)
    ↓
Application Services (DTOs)
    ↓
Domain Services (Business Logic)
    ↓
Infrastructure Repositories
    ↓
Database (SQLite)
```

---

## 📝 Padrões de Código

### Backend (C#)

**Namespace:**
```csharp
namespace Financial.Domain.Entities;
namespace Financial.Application.Services;
namespace Financial.Infrastructure.Repositories;
```

**Classe de Serviço:**
```csharp
public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repository;
    
    public TransactionService(ITransactionRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<TransactionDto> GetByIdAsync(int id)
    {
        var transaction = await _repository.GetByIdAsync(id);
        return transaction;
    }
}
```

**Endpoint:**
```csharp
app.MapGet("/api/transactions/{id}", GetTransactionById);

async Task<IResult> GetTransactionById(int id, ITransactionService service)
{
    var transaction = await service.GetByIdAsync(id);
    return transaction is null ? Results.NotFound() : Results.Ok(transaction);
}
```

### Frontend (TypeScript/React)

**Componente:**
```typescript
import React from 'react';

interface Props {
  title: string;
  onSubmit: (data: any) => void;
}

export const TransactionForm: React.FC<Props> = ({ title, onSubmit }) => {
  return (
    <div>
      <h2>{title}</h2>
      {/* conteúdo */}
    </div>
  );
};
```

**Hook:**
```typescript
export const useTransaction = () => {
  const [transactions, setTransactions] = React.useState([]);
  const [loading, setLoading] = React.useState(false);

  const fetchTransactions = async () => {
    setLoading(true);
    try {
      const data = await transactionService.getAll();
      setTransactions(data);
    } finally {
      setLoading(false);
    }
  };

  return { transactions, loading, fetchTransactions };
};
```

---

## 🧪 Testes

### Backend
```bash
# Executar todos os testes
dotnet test

# Com cobertura
dotnet test /p:CollectCoverage=true

# Teste específico
dotnet test --filter "FullyQualifiedName~TransactionServiceTests"
```

### Frontend
```bash
# Executar testes
npm run test

# Com cobertura
npm run test -- --coverage

# Watch mode
npm run test -- --watch
```

---

## 🚢 Deployment

### Docker (Opcional)

```bash
# Build da imagem
docker build -t financial-app:latest .

# Executar container
docker run -p 5001:5001 financial-app:latest
```

### GitHub Actions (CI/CD)

Workflows estão em `.github/workflows/`:
- `ci-backend.yml` - Build e testes do backend
- `ci-frontend.yml` - Build e testes do frontend
- `deploy.yml` - Deploy automático

---

## 📖 Documentação Adicional

- **[SETUP.md](./docs/guides/setup.md)** - Guia de configuração
- **[ARCHITECTURE.md](./docs/ARCHITECTURE.md)** - Detalhes de arquitetura
- **[API.md](./docs/api/endpoints.md)** - Documentação de endpoints
- **[CONTRIBUTING.md](./CONTRIBUTING.md)** - Guia de contribuição

---

## 🛠️ Tecnologias

### Backend
- **.NET 9** - Framework
- **Entity Framework Core** - ORM
- **SQLite** - Banco de dados
- **Serilog** - Logging
- **AutoMapper** - Mapeamento de objetos
- **FluentValidation** - Validação

### Frontend
- **React 18** - UI Framework
- **TypeScript** - Tipagem
- **Vite** - Build tool
- **Tailwind CSS** - Styling
- **React Router v6** - Roteamento
- **Axios** - HTTP Client
- **Vitest** - Testing

---

## 📞 Contato & Suporte

- Documentação: Ver pasta `docs/`
- Issues: GitHub Issues
- Discussões: GitHub Discussions

---

**Versão:** 1.0.0  
**Data:** Julho 2026  
**Status:** ✅ Pronto para Desenvolvimento

