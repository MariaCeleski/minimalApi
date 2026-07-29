# Proposta de Reorganização do Projeto - Estrutura Profissional

## 📋 Visão Geral

Sua aplicação é um **Financial Management App** com backend .NET (C# Minimal API), frontend React (TypeScript) e banco de dados SQLite. A proposta é organizar tudo seguindo padrões profissionais de indústria.

---

## 🎯 Estrutura Proposta

### Raiz do Projeto
```
financial-management-app/
├── backend/                          # Backend .NET Core
├── frontend/                         # Frontend React + TypeScript
├── database/                         # Scripts e configurações de BD
├── docs/                             # Documentação do projeto
├── .github/                          # GitHub workflows e templates
├── .gitignore
├── README.md
└── docker-compose.yml                # (Futuro) Orquestração de containers
```

---

## 📁 Backend (.NET Core)

### Estrutura Recomendada
```
backend/
├── src/
│   ├── Financial.Api/                # Projeto principal (API)
│   │   ├── Controllers/              # Não usado (Minimal API)
│   │   ├── Endpoints/                # Endpoints do Minimal API
│   │   │   ├── DashboardEndpoints.cs
│   │   │   ├── ReportEndpoints.cs
│   │   │   ├── TransactionEndpoints.cs
│   │   │   ├── AuthEndpoints.cs
│   │   │   └── HealthCheckEndpoints.cs
│   │   ├── Middleware/               # Middlewares customizados
│   │   │   ├── GlobalExceptionMiddleware.cs
│   │   │   ├── LoggingMiddleware.cs
│   │   │   └── AuthenticationMiddleware.cs
│   │   ├── Program.cs                # Configuração da aplicação
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   ├── appsettings.Production.json
│   │   └── Financial.Api.csproj
│   │
│   ├── Financial.Domain/             # Projeto Domain (Lógica de Negócio)
│   │   ├── Entities/                 # Modelos de domínio
│   │   │   ├── User.cs
│   │   │   ├── Transaction.cs
│   │   │   ├── Category.cs
│   │   │   ├── Goal.cs
│   │   │   └── TransactionLimit.cs
│   │   ├── Enums/                    # Enumerações
│   │   │   ├── TransactionType.cs
│   │   │   ├── GoalStatus.cs
│   │   │   └── UserRole.cs
│   │   ├── Exceptions/               # Exceções de domínio
│   │   │   ├── BusinessRuleException.cs
│   │   │   ├── NotFoundException.cs
│   │   │   ├── UnauthorizedException.cs
│   │   │   └── ValidationException.cs
│   │   ├── Interfaces/               # Contratos/Interfaces
│   │   │   ├── Repositories/
│   │   │   │   ├── IRepository.cs
│   │   │   │   ├── ITransactionRepository.cs
│   │   │   │   ├── ICategoryRepository.cs
│   │   │   │   ├── IUserRepository.cs
│   │   │   │   ├── IGoalRepository.cs
│   │   │   │   └── ITransactionLimitRepository.cs
│   │   │   └── Services/
│   │   │       ├── ITransactionService.cs
│   │   │       ├── IDashboardService.cs
│   │   │       ├── IReportService.cs
│   │   │       ├── IExportService.cs
│   │   │       └── IAuthService.cs
│   │   ├── Validators/               # Validadores de negócio
│   │   │   ├── TransactionValidator.cs
│   │   │   ├── UserValidator.cs
│   │   │   └── GoalValidator.cs
│   │   ├── Specifications/           # Specifications (DDD pattern)
│   │   └── Financial.Domain.csproj
│   │
│   ├── Financial.Application/        # Projeto Application (Casos de Uso)
│   │   ├── DTOs/                     # Data Transfer Objects
│   │   │   ├── User/
│   │   │   │   ├── CreateUserDto.cs
│   │   │   │   ├── UpdateUserDto.cs
│   │   │   │   └── UserResponseDto.cs
│   │   │   ├── Transaction/
│   │   │   │   ├── CreateTransactionDto.cs
│   │   │   │   ├── UpdateTransactionDto.cs
│   │   │   │   ├── TransactionResponseDto.cs
│   │   │   │   └── TransactionFilterDto.cs
│   │   │   ├── Dashboard/
│   │   │   │   └── DashboardDtos.cs
│   │   │   ├── Report/
│   │   │   │   └── ReportDtos.cs
│   │   │   └── Auth/
│   │   │       ├── LoginDto.cs
│   │   │       ├── RegisterDto.cs
│   │   │       └── TokenResponseDto.cs
│   │   ├── Services/                 # Implementação de Services
│   │   │   ├── TransactionService.cs
│   │   │   ├── DashboardService.cs
│   │   │   ├── ReportService.cs
│   │   │   ├── ExportService.cs
│   │   │   ├── AuthService.cs
│   │   │   └── UserService.cs
│   │   ├── Mappers/                  # AutoMapper profiles
│   │   │   ├── TransactionMapper.cs
│   │   │   ├── UserMapper.cs
│   │   │   └── DashboardMapper.cs
│   │   ├── Extensions/               # Métodos de extensão
│   │   │   └── ServiceCollectionExtensions.cs
│   │   └── Financial.Application.csproj
│   │
│   └── Financial.Infrastructure/     # Projeto Infrastructure (Persistência)
│       ├── Persistence/
│       │   ├── DbContext.cs
│       │   ├── DesignTimeDbContextFactory.cs
│       │   └── SeedData.cs
│       ├── Repositories/             # Implementação de Repositories
│       │   ├── Repository.cs         # Genérico
│       │   ├── TransactionRepository.cs
│       │   ├── CategoryRepository.cs
│       │   ├── UserRepository.cs
│       │   ├── GoalRepository.cs
│       │   └── TransactionLimitRepository.cs
│       ├── Configuration/            # Configurações do EF Core
│       │   ├── UserConfiguration.cs
│       │   ├── TransactionConfiguration.cs
│       │   ├── CategoryConfiguration.cs
│       │   ├── GoalConfiguration.cs
│       │   └── TransactionLimitConfiguration.cs
│       ├── Migrations/               # Migrações do EF Core
│       │   └── [Migrations...]
│       ├── Extensions/
│       │   └── ServiceCollectionExtensions.cs
│       └── Financial.Infrastructure.csproj
│
├── tests/
│   ├── Financial.Domain.Tests/       # Testes de domínio (unitários)
│   │   ├── Entities/
│   │   ├── Validators/
│   │   └── Financial.Domain.Tests.csproj
│   │
│   ├── Financial.Application.Tests/  # Testes de aplicação (integração)
│   │   ├── Services/
│   │   ├── Mappers/
│   │   └── Financial.Application.Tests.csproj
│   │
│   └── Financial.Api.Tests/          # Testes de API (end-to-end)
│       ├── Endpoints/
│       ├── Integration/
│       └── Financial.Api.Tests.csproj
│
├── Financial.sln                     # Solution única
└── README.md                         # Documentação backend
```

### Explicação da Estrutura Backend

| Camada | Responsabilidade | Exemplos |
|--------|-----------------|----------|
| **API** | Endpoints HTTP, middleware | Controllers/Endpoints, autenticação |
| **Domain** | Lógica de negócio, regras | Entities, Exceptions, Validators |
| **Application** | Casos de uso, orquestração | Services, DTOs, Mappers |
| **Infrastructure** | Persistência, dados | Repositories, DbContext, Migrations |

---

## 💻 Frontend (React + TypeScript)

### Estrutura Recomendada
```
frontend/
├── public/
│   ├── favicon.ico
│   ├── index.html
│   └── assets/
│       ├── icons/
│       ├── images/
│       └── fonts/
│
├── src/
│   ├── components/
│   │   ├── common/                   # Componentes reutilizáveis
│   │   │   ├── Button/
│   │   │   │   ├── Button.tsx
│   │   │   │   ├── Button.module.css
│   │   │   │   └── Button.test.tsx
│   │   │   ├── Card/
│   │   │   ├── Modal/
│   │   │   ├── Input/
│   │   │   ├── Spinner/
│   │   │   └── Navigation/
│   │   │
│   │   ├── layout/                   # Componentes de layout
│   │   │   ├── Header/
│   │   │   │   ├── Header.tsx
│   │   │   │   ├── Header.module.css
│   │   │   │   └── Header.test.tsx
│   │   │   ├── Sidebar/
│   │   │   ├── Footer/
│   │   │   └── Layout.tsx
│   │   │
│   │   ├── dashboard/                # Feature: Dashboard
│   │   │   ├── BalanceCard.tsx
│   │   │   ├── CategoryChart.tsx
│   │   │   ├── TrendChart.tsx
│   │   │   ├── RecentTransactions.tsx
│   │   │   └── __tests__/
│   │   │
│   │   ├── transactions/             # Feature: Transactions
│   │   │   ├── TransactionForm.tsx
│   │   │   ├── TransactionList.tsx
│   │   │   ├── TransactionFilter.tsx
│   │   │   ├── TransactionModal.tsx
│   │   │   └── __tests__/
│   │   │
│   │   ├── reports/                  # Feature: Reports
│   │   │   ├── ReportTable.tsx
│   │   │   ├── ReportChart.tsx
│   │   │   ├── ExportButton.tsx
│   │   │   └── __tests__/
│   │   │
│   │   ├── goals/                    # Feature: Goals
│   │   │   ├── GoalForm.tsx
│   │   │   ├── GoalList.tsx
│   │   │   ├── GoalProgress.tsx
│   │   │   └── __tests__/
│   │   │
│   │   └── auth/                     # Feature: Authentication
│   │       ├── LoginForm.tsx
│   │       ├── RegisterForm.tsx
│   │       ├── LogoutButton.tsx
│   │       └── __tests__/
│   │
│   ├── pages/
│   │   ├── DashboardPage.tsx
│   │   ├── TransactionsPage.tsx
│   │   ├── ReportsPage.tsx
│   │   ├── GoalsPage.tsx
│   │   ├── LoginPage.tsx
│   │   ├── RegisterPage.tsx
│   │   ├── NotFoundPage.tsx
│   │   └── ErrorPage.tsx
│   │
│   ├── services/                     # Serviços de API
│   │   ├── api/
│   │   │   ├── client.ts             # Configuração do axios/fetch
│   │   │   ├── endpoints.ts          # URLs das APIs
│   │   │   └── interceptors.ts       # Request/Response interceptors
│   │   ├── auth.service.ts
│   │   ├── transaction.service.ts
│   │   ├── dashboard.service.ts
│   │   ├── report.service.ts
│   │   ├── export.service.ts
│   │   └── goal.service.ts
│   │
│   ├── hooks/                        # Custom React hooks
│   │   ├── useApi.ts
│   │   ├── useAuth.ts
│   │   ├── useLocalStorage.ts
│   │   ├── useTransaction.ts
│   │   ├── usePagination.ts
│   │   ├── useNotification.ts
│   │   ├── useForm.ts
│   │   └── index.ts
│   │
│   ├── context/                      # Context API
│   │   ├── AppContext.tsx
│   │   ├── AuthContext.tsx
│   │   ├── ThemeContext.tsx
│   │   ├── TransactionContext.tsx
│   │   ├── NotificationContext.tsx
│   │   └── index.ts
│   │
│   ├── types/                        # TypeScript types
│   │   ├── index.ts                  # Tipos globais
│   │   ├── api.types.ts              # Tipos de respostas da API
│   │   ├── entities.types.ts         # Tipos de entidades
│   │   ├── forms.types.ts            # Tipos de formulários
│   │   └── enums.ts                  # Enumerações
│   │
│   ├── utils/                        # Funções utilitárias
│   │   ├── formatting.ts             # Formatação de dados
│   │   ├── validation.ts             # Validações
│   │   ├── date.ts                   # Funções de data
│   │   ├── currency.ts               # Funções de moeda
│   │   ├── storage.ts                # LocalStorage helpers
│   │   └── constants.ts              # Constantes da aplicação
│   │
│   ├── styles/
│   │   ├── global.css
│   │   ├── variables.css
│   │   ├── animations.css
│   │   └── responsive.css
│   │
│   ├── App.tsx
│   ├── main.tsx
│   ├── index.css
│   └── vite-env.d.ts
│
├── tests/
│   ├── setup.ts                      # Configuração de testes
│   ├── mocks/
│   │   ├── handlers.ts               # MSW handlers
│   │   └── server.ts
│   └── __tests__/                    # Testes de integração
│
├── .env.example
├── .env.local
├── .eslintrc.cjs
├── .gitignore
├── package.json
├── tsconfig.json
├── tsconfig.node.json
├── vite.config.ts
├── vitest.config.ts
├── tailwind.config.js
├── postcss.config.js
├── README.md
└── index.html
```

---

## 📊 Database

### Estrutura Recomendada
```
database/
├── scripts/
│   ├── init.sql                      # Script inicial
│   ├── seed.sql                      # Dados de teste
│   └── backup.sql                    # Backup de exemplo
├── migrations/
│   ├── [EF Core migrations]          # Migração automática do EF
│   └── manual/                       # Migrações manuais (se houver)
├── schemas/
│   ├── users.sql
│   ├── transactions.sql
│   ├── categories.sql
│   ├── goals.sql
│   └── views.sql
├── backups/                          # Backups da BD
└── README.md
```

---

## 📚 Documentação

### Estrutura Recomendada
```
docs/
├── README.md                         # Documentação principal
├── ARCHITECTURE.md                   # Arquitetura do projeto
├── API.md                            # Documentação da API (OpenAPI/Swagger)
├── SETUP.md                          # Como configurar o ambiente
├── CONTRIBUTING.md                   # Guia de contribuição
├── DEPLOYMENT.md                     # Como fazer deploy
│
├── guides/
│   ├── getting-started.md
│   ├── development.md
│   ├── testing.md
│   ├── code-style.md
│   └── performance.md
│
├── api/
│   ├── swagger.json                  # OpenAPI spec
│   └── endpoints.md
│
└── specs/
    ├── requirements.md
    ├── design.md
    └── tasks.md
```

---

## 🔧 Configurações na Raiz

```
/
├── .github/
│   ├── workflows/
│   │   ├── ci-backend.yml            # CI/CD Backend
│   │   ├── ci-frontend.yml           # CI/CD Frontend
│   │   ├── deploy.yml                # Deploy
│   │   └── codeql.yml                # Análise de segurança
│   └── ISSUE_TEMPLATE/
│
├── .gitignore
├── .editorconfig                     # Configuração de editor
├── docker-compose.yml                # Orquestração
├── Dockerfile                        # Container da API
├── docker-compose.dev.yml            # Dev environment
│
├── README.md                         # Raiz do projeto
├── CONTRIBUTING.md
├── LICENSE
└── .env.example
```

---

## 🎨 Padrões de Naming

### Backend
- **Pastas**: PascalCase (`Services/`, `Repositories/`, `Entities/`)
- **Arquivos**: PascalCase (`UserService.cs`, `IRepository.cs`)
- **Classes**: PascalCase (`UserService`, `Transaction`)
- **Interfaces**: Começa com `I` (`IRepository`, `IUserService`)
- **Métodos**: PascalCase (`GetUserById`, `CreateTransaction`)
- **Propriedades**: PascalCase (`UserId`, `TransactionDate`)

### Frontend
- **Pastas**: kebab-case (`dashboard/`, `common/`, `auth/`)
- **Componentes**: PascalCase (`Button.tsx`, `UserProfile.tsx`)
- **Hooks**: camelCase, começa com `use` (`useAuth.ts`, `usePagination.ts`)
- **Services**: camelCase com sufixo `.service.ts` (`auth.service.ts`)
- **Types**: PascalCase em `.types.ts` ou `.d.ts`
- **Utils**: camelCase (`formatting.ts`, `validation.ts`)
- **Constantes**: UPPER_SNAKE_CASE (`API_BASE_URL`, `DEFAULT_PAGE_SIZE`)

---

## 📦 Dependências Sugeridas

### Backend (.NET 9)
- **Principais**: `Entity Framework Core 9.0`, `Minimal API`
- **Validação**: `FluentValidation`
- **Logging**: `Serilog`
- **Mapeamento**: `AutoMapper`
- **Autenticação**: `System.IdentityModel.Tokens.Jwt`
- **Documentação**: `Swashbuckle.AspNetCore` (Swagger)

### Frontend (React 18)
- **Build**: `Vite`
- **Routing**: `React Router v6`
- **State**: `React Context` + `Zustand` (alternativa)
- **HTTP Client**: `Axios`
- **UI**: `Tailwind CSS` + `shadcn/ui`
- **Formulários**: `React Hook Form`
- **Validação**: `Zod`
- **Charts**: `Recharts`
- **Testing**: `Vitest` + `React Testing Library`
- **Linting**: `ESLint` + `Prettier`

---

## ✅ Checklist de Migração

- [ ] Criar estrutura de pastas backend
- [ ] Mover arquivos do backend para novas pastas
- [ ] Atualizar referências de `using` e imports
- [ ] Reorganizar frontend (já bem organizado)
- [ ] Atualizar `.gitignore` para nova estrutura
- [ ] Criar documentação (ARCHITECTURE.md, SETUP.md)
- [ ] Atualizar GitHub workflows
- [ ] Testar build da solução
- [ ] Testar aplicação completa
- [ ] Criar docker-compose.yml
- [ ] Documentar processo de setup no README

---

## 💡 Próximos Passos

1. **Aprovação da estrutura**: Confirme se esta proposta está ok
2. **Implementação**: Vou criar os arquivos e mover tudo
3. **Atualização de referências**: Atualizarei imports e configurações
4. **Testes**: Verificarei se tudo funciona
5. **Documentação**: Criarei guias de setup e desenvolvimento

