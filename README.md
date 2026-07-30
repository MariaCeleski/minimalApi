# Personal Financial Management Application

> Sistema completo de gestão financeira pessoal com interface moderna e relatórios visuais

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18.2-61DAFB?style=flat-square&logo=react)](https://reactjs.org/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.2-3178C6?style=flat-square&logo=typescript)](https://www.typescriptlang.org/)
[![SQLite](https://img.shields.io/badge/SQLite-3-003B57?style=flat-square&logo=sqlite)](https://sqlite.org/)
[![Material-UI](https://img.shields.io/badge/Material--UI-5.14-007FFF?style=flat-square&logo=mui)](https://mui.com/)
[![TailwindCSS](https://img.shields.io/badge/Tailwind-3.3-06B6D4?style=flat-square&logo=tailwindcss)](https://tailwindcss.com/)

---

## 📋 Índice

1. [Descrição do Problema](#-descrição-do-problema)
2. [Objetivo da Aplicação](#-objetivo-da-aplicação)
3. [Arquitetura da Solução](#️-arquitetura-da-solução)
4. [Tecnologias Utilizadas](#-tecnologias-utilizadas)
5. [Features Implementadas](#-features-implementadas)
6. [Estrutura do Projeto](#-estrutura-do-projeto)
7. [Instruções de Execução](#-instruções-de-execução)
8. [Variáveis de Ambiente](#️-variáveis-de-ambiente)
9. [Exemplos de Uso](#-exemplos-de-uso)
10. [Decisões de Design](#-decisões-de-design)
11. [Limitações Conhecidas](#⚠️-limitações-conhecidas)
12. [Roadmap de Melhorias](#-roadmap-de-melhorias)

---

## 💡 Descrição do Problema

A gestão financeira pessoal é um desafio comum onde as pessoas precisam:
- Controlar receitas e despesas de forma organizada
- Visualizar onde o dinheiro está sendo gasto
- Acompanhar evolução do saldo ao longo do tempo
- Gerar relatórios para análise e planejamento
- Ter acesso rápido e intuitivo aos dados financeiros

**Problema atual:** Ferramentas complexas, caras ou que não atendem necessidades básicas de controle financeiro pessoal.

---

## 🎯 Objetivo da Aplicação

Desenvolver um **sistema completo de gestão financeira pessoal** que oferece:

### Objetivos Primários:
- ✅ **CRUD completo** de transações (receitas e despesas)
- ✅ **Dashboard visual** com gráficos intuitivos e saldo atual
- ✅ **Relatórios detalhados** por período e categoria
- ✅ **Exportação de dados** em CSV e PDF
- ✅ **Interface responsiva** para desktop, tablet e mobile

### Objetivos Secundários:
- 🔄 **Dark mode** e tema personalizado
- 🔄 **Animações suaves** para melhor UX
- ⏳ **Metas financeiras** e acompanhamento de progresso
- ⏳ **Notificações de limites** de gastos por categoria
- ⏳ **Autenticação simples** para proteção de dados
---

## 🏗️ Arquitetura da Solução

### Arquitetura em Camadas

```
┌─────────────────────────────────────────────────────────────┐
│                    React Frontend                           │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐           │
│  │ Components  │ │   Hooks     │ │  Services   │           │
│  │             │ │             │ │             │           │
│  │ • Dashboard │ │ • useApi    │ │ • API Client│           │
│  │ • Forms     │ │ • useTheme  │ │ • Trans Svc │           │
│  │ • Charts    │ │ • useLocal  │ │ • Report Svc│           │
│  └─────────────┘ └─────────────┘ └─────────────┘           │
└─────────────────────┬───────────────────────────────────────┘
                      │ HTTP REST API
                      ▼
┌─────────────────────────────────────────────────────────────┐
│                 ASP.NET Core API                            │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐           │
│  │ Endpoints   │ │  Services   │ │Repositories │           │
│  │             │ │             │ │             │           │
│  │ • Minimal   │ │ • Business  │ │ • Generic   │           │
│  │ • REST      │ │ • Logic     │ │ • Specific  │           │
│  │ • Swagger   │ │ • Validation│ │ • EF Core   │           │
│  └─────────────┘ └─────────────┘ └─────────────┘           │
└─────────────────────┬───────────────────────────────────────┘
                      │ Entity Framework Core
                      ▼
┌─────────────────────────────────────────────────────────────┐
│                   SQLite Database                           │
│   User → Categories → Transactions → Goals → Limits        │
└─────────────────────────────────────────────────────────────┘
```

### Fluxo de Dados
1. **Frontend React** → API HTTP requests
2. **ASP.NET Endpoints** → Business Logic Services  
3. **Services** → Repository Pattern
4. **Repositories** → Entity Framework Core
5. **EF Core** → SQLite Database

---

## 💻 Tecnologias Utilizadas

| Categoria | Tecnologia | Versão | Justificativa |
|-----------|------------|--------|---------------|
| **Backend** | ASP.NET Core | 9.0 | Performance, Minimal API, Cross-platform |
| **ORM** | Entity Framework Core | 9.0 | Migrations, Relationships, LINQ |
| **Database** | SQLite | 3.x | Zero-config, portável, sem servidor |
| **Frontend** | React | 18.2 | Ecossistema maduro, hooks, performance |
| **Language** | TypeScript | 5.2 | Type safety, IntelliSense, produtividade |
| **UI Framework** | Material UI | 5.14 | Componentes prontos, acessibilidade |
| **Styling** | TailwindCSS | 3.3 | Utility-first, customização, performance |
| **Charts** | Recharts | 2.8 | React-native, SVG, responsivo |
| **Animations** | Framer Motion | 10.16 | Declarativo, performance, gestos |
| **HTTP Client** | Axios | 1.6 | Interceptors, timeout, error handling |
| **Routing** | React Router | 6.20 | SPA navigation, lazy loading |
| **Forms** | React Hook Form | 7.83 | Performance, validação, menos rerenders |
| **Validation** | Zod | 4.4 | Schema validation, TypeScript integration |
| **Build Tool** | Vite | 5.0 | Fast HMR, ES modules, tree shaking |

---

## 🚀 Features Implementadas

### ✅ **Core Features (Concluídas)**

| Feature | Status | Descrição |
|---------|--------|-----------|
| 🏗️ **Projeto Setup** | ✅ | ASP.NET Core + React configurados |
| 📊 **Domain Models** | ✅ | User, Category, Transaction, Goal, Limit |
| 🗄️ **Database** | ✅ | SQLite + EF Core + Migrations |
| 🎨 **Frontend Base** | ✅ | React + TypeScript + Material UI + TailwindCSS |
| 📋 **Type Safety** | ✅ | DTOs TypeScript alinhados com backend |
| 🔄 **State Management** | ✅ | Context API (Theme, Transaction, App) |
| 📱 **Responsive Layout** | ✅ | Mobile-first design (320px-1920px) |

### 🔄 **Features em Desenvolvimento**

| Feature | Progress | Próximos Passos |
|---------|----------|-----------------|
| 💰 **CRUD Transações** | 🔄 60% | Endpoints + validação + frontend forms |
| 📈 **Dashboard** | 🔄 40% | Gráficos + cálculo de saldo + métricas |
| 🎯 **Filtros** | ⏳ 20% | Por período + categoria + paginação |
| 📊 **Relatórios** | ⏳ 10% | Mensal + categoria + agregações |
| 📤 **Export CSV/PDF** | ⏳ 0% | CsvHelper + QuestPDF integration |
| 🌙 **Dark Mode** | 🔄 70% | TailwindCSS themes + persistence |
| ✨ **Animações** | 🔄 30% | Framer Motion transitions |

### 🎁 **Features Opcionais (Backlog)**

| Feature | Priority | Description |
|---------|----------|-------------|
| 🔐 **Autenticação** | Medium | JWT + login/register simples |
| 🎯 **Metas Financeiras** | Low | CRUD goals + progress tracking |
| 🚨 **Alertas de Limites** | Low | Notificações + thresholds |
| ⚡ **Performance** | High | Caching + query optimization |
| 🧪 **Testes** | High | Unit + integration + E2E |

---

## 📁 Estrutura do Projeto

```
minimalApi/
├── 📋 README.md                     # Este arquivo
├── 📋 docs/                         # Documentação
│   ├── CHECKLIST_GITHUB.md         # Checklist de qualidade
│   └── specs/                       # Especificações técnicas
│       ├── requirements.md          # 20 requisitos detalhados
│       └── tasks.md                 # 122 tasks organizadas
│
├── ⚙️ Backend (ASP.NET Core)/
│   ├── Program.cs                   # Configuração principal + DI
│   ├── minimal-api.csproj           # Dependencies + target framework
│   ├── appsettings*.json            # Configurações ambiente
│   │
│   ├── Dominio/                     # Domain Layer
│   │   └── Entidades/               # Domain Models
│   │       ├── User.cs              # Usuário
│   │       ├── Category.cs          # Categorias (receita/despesa)
│   │       ├── Transaction.cs       # Transações financeiras
│   │       ├── Goal.cs              # Metas financeiras
│   │       └── TransactionLimit.cs  # Limites de gastos
│   │
│   ├── Infraestrutura/              # Infrastructure Layer
│   │   ├── Db/
│   │   │   └── DbContexto.cs        # EF Core Context
│   │   └── Repositorios/            # Repository Pattern
│   │
│   ├── Aplicacao/                   # Application Layer
│   │   ├── Servicos/                # Business Logic Services
│   │   └── Middleware/              # Global Exception Handler
│   │
│   └── Migrations/                  # EF Core Migrations
│
├── 🎨 Frontend (React + TypeScript)/
│   ├── public/                      # Static assets
│   ├── src/
│   │   ├── App.tsx                  # Main component + routing
│   │   ├── main.tsx                 # Entry point
│   │   │
│   │   ├── components/              # Reusable UI Components
│   │   │   └── layout/              # Layout components
│   │   │
│   │   ├── pages/                   # Route Pages
│   │   │   ├── HomePage.tsx         # Landing page
│   │   │   ├── DashboardPage.tsx    # Main dashboard
│   │   │   ├── TransactionsPage.tsx # CRUD transações
│   │   │   ├── ReportsPage.tsx      # Relatórios
│   │   │   ├── GoalsPage.tsx        # Metas financeiras
│   │   │   └── SettingsPage.tsx     # Configurações
│   │   │
│   │   ├── hooks/                   # Custom React Hooks
│   │   │   ├── useApi.ts            # HTTP requests
│   │   │   └── useLocalStorage.ts   # Local persistence
│   │   │
│   │   ├── context/                 # Context API
│   │   │   ├── ThemeContext.tsx     # Dark/Light mode
│   │   │   ├── TransactionContext.tsx # Transaction state
│   │   │   └── AppContext.tsx       # Global app state
│   │   │
│   │   ├── services/                # API Services
│   │   │   ├── api.ts               # Base HTTP client
│   │   │   └── transactionService.ts # Transaction CRUD
│   │   │
│   │   ├── types/                   # TypeScript Definitions
│   │   │   └── index.ts             # All interfaces/types
│   │   │
│   │   └── utils/                   # Utility Functions
│   │       ├── formatters.ts        # Currency, date formatting
│   │       ├── validators.ts        # Form validation
│   │       └── constants.ts         # App constants
│   │
│   ├── package.json                 # Dependencies + scripts
│   ├── tailwind.config.js           # TailwindCSS configuration
│   ├── tsconfig.json                # TypeScript configuration
│   └── vite.config.ts               # Vite build configuration
│
└── 🔧 Configuration/
    ├── .gitignore                   # Git ignore patterns
    ├── .vscode/                     # VS Code settings
    └── logs/                        # Application logs
```
---

## 🚀 Instruções de Execução

### Pré-requisitos

| Software | Versão | Download |
|----------|--------|----------|
| .NET SDK | 9.0+ | [dotnet.microsoft.com](https://dotnet.microsoft.com/download) |
| Node.js | 18+ | [nodejs.org](https://nodejs.org/) |
| Git | Latest | [git-scm.com](https://git-scm.com/) |

### 1️⃣ Clone o Repositório

```bash
git clone https://github.com/[seu-usuario]/minimalApi.git
cd minimalApi
```

### 2️⃣ Setup do Backend (ASP.NET Core)

```bash
# Navegar para pasta do backend
cd minimalApi

# Restaurar dependências
dotnet restore

# Aplicar migrations (criar banco SQLite)
dotnet ef database update

# Executar aplicação
dotnet run

# ✅ Backend estará rodando em: http://localhost:5000
# ✅ Swagger UI disponível em: http://localhost:5000/swagger
```

### 3️⃣ Setup do Frontend (React)

```bash
# Navegar para pasta do frontend (em terminal separado)
cd frontend

# Instalar dependências
npm install

# Executar aplicação de desenvolvimento  
npm run dev

# ✅ Frontend estará rodando em: http://localhost:3000
```

### 4️⃣ Verificação da Instalação

| Serviço | URL | Status Esperado |
|---------|-----|----------------|
| 🔧 Backend API | http://localhost:5000 | "Hello World!" |
| 📋 Swagger UI | http://localhost:5000/swagger | Interface API |
| 🎨 React App | http://localhost:3000 | Dashboard principal |
| 🗄️ Database | `./financialmanagement_dev.db` | Arquivo SQLite criado |

### 5️⃣ Scripts Disponíveis

#### Backend (.NET)
```bash
dotnet run                 # Executar aplicação
dotnet build              # Build da aplicação
dotnet test               # Executar testes
dotnet ef migrations add  # Criar nova migration
dotnet ef database update # Aplicar migrations
```

#### Frontend (React)
```bash
npm run dev       # Servidor de desenvolvimento (Vite)
npm run build     # Build de produção
npm run preview   # Preview do build
npm run lint      # ESLint code analysis
```

---

## ⚙️ Variáveis de Ambiente

### Backend (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=financialmanagement_dev.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000", "http://localhost:5173"]
  }
}
```

### Frontend (.env.local)

```bash
# API Configuration
VITE_API_BASE_URL=http://localhost:5000/api
VITE_APP_TITLE="Personal Finance Manager"

# Feature Flags
VITE_ENABLE_DARK_MODE=true
VITE_ENABLE_ANIMATIONS=true
VITE_ENABLE_EXPORT=true

# Development
VITE_DEBUG_MODE=true
VITE_LOG_LEVEL=debug
```

### Configuração CORS

O backend está configurado para aceitar requisições do frontend:
- **Development**: `http://localhost:3000` (Vite)
- **Alternative**: `http://localhost:5173` (Vite alternative port)

---

## 📸 Exemplos de Uso

### Dashboard Principal
![Dashboard](docs/screenshots/dashboard.png)
*Visão geral com saldo atual, gráfico de despesas por categoria e evolução mensal*

### Cadastro de Transação
![Transaction Form](docs/screenshots/transaction-form.png)
*Formulário responsivo com validação em tempo real*

### Relatórios
![Reports](docs/screenshots/reports.png)
*Relatórios mensais com opção de exportação*

### Dark Mode
![Dark Mode](docs/screenshots/dark-mode.png)
*Tema escuro com transições suaves*

### Exemplos de API (JSON)

#### POST /api/transactions - Criar Transação
```json
{
  "categoryId": "550e8400-e29b-41d4-a716-446655440000",
  "amount": 1500.00,
  "description": "Salário mensal",
  "date": "2024-01-15"
}
```

#### Response - Transação Criada (201)
```json
{
  "id": "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
  "categoryId": "550e8400-e29b-41d4-a716-446655440000",
  "amount": 1500.00,
  "description": "Salário mensal",
  "type": "Income",
  "date": "2024-01-15T00:00:00Z",
  "createdAt": "2024-01-15T14:30:00Z",
  "category": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "name": "Salário",
    "icon": "💰",
    "color": "#22c55e",
    "type": "Income"
  }
}
```

#### GET /api/dashboard - Dashboard Data
```json
{
  "totalIncome": 3500.00,
  "totalExpenses": 2100.00,
  "currentBalance": 1400.00,
  "expensesByCategory": [
    {
      "categoryName": "Alimentação",
      "icon": "🍔",
      "color": "#f59e0b",
      "amount": 800.00,
      "percentage": 38.1
    },
    {
      "categoryName": "Transporte",
      "icon": "🚗",
      "color": "#8b5cf6",
      "amount": 600.00,
      "percentage": 28.6
    }
  ],
  "monthlyTrends": [
    {
      "month": "2024-01",
      "income": 3500.00,
      "expenses": 2100.00,
      "balance": 1400.00
    }
  ]
}
```
---

## 🛠️ Decisões de Design

### Arquiteturais

| Decisão | Justificativa | Alternativa Considerada |
|---------|---------------|-------------------------|
| **Minimal APIs** | Menos boilerplate, performance, modernidade | Controllers tradicionais |
| **Repository Pattern** | Testabilidade, separação de responsabilidades | EF Direct injection |
| **SQLite** | Zero-config, portabilidade, desenvolvimento | PostgreSQL, SQL Server |
| **Context API** | Nativo React, simplicidade do projeto | Redux, Zustand |
| **Material UI + Tailwind** | Componentes prontos + utility classes | Styled Components |
| **TypeScript** | Type safety, produtividade, IntelliSense | JavaScript puro |

### Padrões de Código

| Padrão | Aplicação | Benefício |
|--------|-----------|-----------|
| **SOLID Principles** | Services, Repositories, DTOs | Manutenibilidade |
| **Dependency Injection** | ASP.NET Core nativo | Testabilidade |
| **Convention over Configuration** | EF Core, React Router | Produtividade |
| **Separation of Concerns** | Layers, Components, Hooks | Organização |
| **Single Responsibility** | Services específicos | Clareza |
| **Open/Closed Principle** | Generic Repository | Extensibilidade |

### Frontend

| Decisão | Motivo | Implementação |
|---------|--------|---------------|
| **Mobile-First** | Maioria dos acessos mobile | TailwindCSS breakpoints |
| **Component Composition** | Reutilização, manutenção | Atomic Design |
| **Custom Hooks** | Lógica reutilizável | useApi, useLocalStorage |
| **Error Boundaries** | UX resiliente | React error handling |
| **Lazy Loading** | Performance inicial | React.lazy + Suspense |
| **Accessibility** | Inclusão | ARIA labels, contraste |

---

## ⚠️ Limitações Conhecidas

### Funcionais
- 🔐 **Single-user**: Sistema não suporta múltiplos usuários simultaneamente
- 🗄️ **Local Database**: SQLite não é adequado para alta concorrência
- 🌐 **Sem Deploy**: Aplicação roda apenas localmente
- 📤 **Export Simples**: CSV/PDF básicos, sem templates avançados
- 🔔 **Notificações**: Apenas in-app, sem push notifications
- 📱 **Responsividade**: Otimizado para web, não é app nativo

### Técnicas
- 🧪 **Testes**: Cobertura parcial de testes unitários
- ⚡ **Performance**: Sem otimizações avançadas (caching, CDN)
- 🔒 **Segurança**: Autenticação básica, sem OAuth/2FA
- 📊 **Relatórios**: Limitados a período e categoria
- 🔄 **Sync**: Sem sincronização entre dispositivos
- 🌍 **i18n**: Interface apenas em português

### Infraestrutura
- ☁️ **Cloud**: Sem integração com serviços cloud
- 📈 **Monitoring**: Logs básicos, sem APM
- 🔄 **CI/CD**: Pipeline básico, sem deploy automático
- 🐳 **Containerization**: Docker não configurado
- 🔐 **Secrets**: Configuração local, não produção-ready
- 📦 **Package Management**: Dependências podem estar desatualizadas

---

## 🚀 Roadmap de Melhorias

### Fase 1: Funcionalidades Core (Sprint 1-2)
- [ ] Completar CRUD de transações com validações
- [ ] Dashboard funcional com gráficos Recharts
- [ ] Sistema de filtros (período, categoria, valor)
- [ ] Exportação CSV básica
- [ ] Dark mode completamente funcional

### Fase 2: UX/UI Avançada (Sprint 3-4)  
- [ ] Animações Framer Motion em toda aplicação
- [ ] Interface 100% responsiva (teste em dispositivos)
- [ ] Ícones personalizados por categoria
- [ ] Feedback visual (loading states, toast notifications)
- [ ] Temas customizáveis além de dark/light

### Fase 3: Features Avançadas (Sprint 5-6)
- [ ] Sistema de autenticação com JWT
- [ ] Metas financeiras com progresso visual
- [ ] Alertas e notificações de limites
- [ ] Relatórios avançados com mais visualizações
- [ ] Exportação PDF com templates profissionais

### Fase 4: Qualidade e Performance (Sprint 7-8)
- [ ] Cobertura de testes > 80% (unit + integration)
- [ ] Performance optimization (React.memo, caching)
- [ ] Acessibilidade WCAG AA
- [ ] SEO e meta tags otimizadas
- [ ] Bundle size optimization

### Fase 5: Deploy e Produção (Sprint 9-10)
- [ ] Docker containerization (backend + frontend)
- [ ] CI/CD pipeline (GitHub Actions)
- [ ] Deploy em Azure/AWS/Vercel
- [ ] Monitoring e logs estruturados
- [ ] Database migration para PostgreSQL/SQL Server

### Fase 6: Expansão (Backlog)
- [ ] Multi-tenant architecture 
- [ ] API móvel e aplicativo React Native
- [ ] Integração com bancos (Open Banking)
- [ ] Machine Learning para categorização automática
- [ ] Marketplace de extensões/plugins
- [ ] Internacionalização (i18n)

---

## 📞 Suporte e Contribuição

### 🐛 Reportar Bugs
Encontrou um bug? Abra uma issue com:
- Descrição detalhada do problema
- Steps to reproduce
- Screenshots (se aplicável)
- Environment info (OS, browser, versões)

### 💡 Solicitar Features
Tem uma ideia? Crie uma feature request com:
- Descrição da funcionalidade
- Justificativa e casos de uso
- Mockups ou especificações (se possível)

### 🔧 Contribuir com Código
1. Fork o repositório
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'feat: add amazing feature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

### 📋 Padrões de Contribuição
- Siga o [Conventional Commits](https://www.conventionalcommits.org/)
- Mantenha o [CHECKLIST_GITHUB.md](docs/CHECKLIST_GITHUB.md) atualizado
- Adicione testes para novas funcionalidades
- Documente mudanças no README.md

---

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para detalhes.

---

## 👨‍💻 Autor

**[Seu Nome]**
- GitHub: [@seuusuario](https://github.com/seuusuario)
- LinkedIn: [Seu LinkedIn](https://linkedin.com/in/seulinkedin)
- Email: seu.email@dominio.com

---

## 🙏 Agradecimentos

- [ASP.NET Core Team](https://github.com/dotnet/aspnetcore) pela excelente framework
- [React Team](https://github.com/facebook/react) pela biblioteca revolucionária  
- [Material-UI](https://mui.com/) pelos componentes lindos e acessíveis
- [TailwindCSS](https://tailwindcss.com/) pelo sistema de design utilitário
- [Recharts](https://recharts.org/) pelos gráficos responsivos
- [Framer Motion](https://www.framer.com/motion/) pelas animações fluidas

---

<div align="center">

**⭐ Se este projeto te ajudou, considere dar uma estrela no repositório!**

[![GitHub stars](https://img.shields.io/github/stars/seuusuario/minimalApi?style=social)](https://github.com/seuusuario/minimalApi/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/seuusuario/minimalApi?style=social)](https://github.com/seuusuario/minimalApi/network/members)
[![GitHub issues](https://img.shields.io/github/issues/seuusuario/minimalApi)](https://github.com/seuusuario/minimalApi/issues)

</div>

---

*Última atualização: Janeiro 2024*