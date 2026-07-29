# 📊 FUNCIONALIDADES DOS COMPONENTES

## 🎯 Visão Geral do Projeto

**Financial Management App** é um sistema completo de gestão financeira pessoal com:
- Backend API robusta em .NET Core
- Frontend interativo em React
- Banco de dados SQLite
- Arquitetura profissional e escalável

---

## 🔙 BACKEND - Financial.Api

### 📌 Funcionalidades Principais

#### **1. Autenticação e Autorização**
```
Responsabilidade: Gerenciar acesso de usuários
├─ Login/Register
├─ Token JWT
├─ Refresh Token
└─ Validação de credenciais
```

#### **2. Dashboard**
```
Responsabilidade: Fornecer visão geral financeira
├─ Saldo total
├─ Receitas vs Despesas
├─ Categorias principais
├─ Últimas transações
└─ Gráficos de tendências
```

#### **3. Transações**
```
Responsabilidade: CRUD de transações financeiras
├─ Criar transação (receita/despesa)
├─ Listar transações
├─ Filtrar por data/categoria
├─ Atualizar transação
├─ Deletar transação
└─ Validar limites
```

#### **4. Relatórios**
```
Responsabilidade: Gerar análises financeiras
├─ Relatório por período
├─ Análise por categoria
├─ Comparativo mês anterior
├─ Tendências de gastos
└─ Previsão de saldo
```

#### **5. Exportação de Dados**
```
Responsabilidade: Exportar dados em diferentes formatos
├─ Export para CSV
├─ Export para PDF
├─ Export para Excel
└─ Configurar período
```

#### **6. Metas Financeiras**
```
Responsabilidade: Gerenciar objetivos financeiros
├─ Criar meta
├─ Definir valor alvo
├─ Acompanhar progresso
├─ Alertas de atingimento
└─ Histórico de metas
```

#### **7. Limites de Gastos**
```
Responsabilidade: Controlar gastos por categoria
├─ Definir limite por categoria
├─ Monitorar gastos
├─ Alertas de ultrapassagem
├─ Histórico de limites
└─ Sugestões de redução
```

---

## 🟪 BACKEND - Financial.Domain

### 📌 Modelos de Domínio (Entities)

#### **User**
```csharp
Propriedades:
├─ Id (PK)
├─ Email (Único)
├─ NomeCompleto
├─ Senha (Hash)
├─ DataCriacao
├─ Ativo
└─ Relacionamentos
   ├─ Transactions
   ├─ Goals
   └─ Categories
```

#### **Transaction**
```csharp
Propriedades:
├─ Id (PK)
├─ UserId (FK)
├─ Descricao
├─ Valor
├─ Tipo (Receita/Despesa)
├─ Data
├─ CategoryId (FK)
├─ Status
└─ Validações
   ├─ Valor > 0
   ├─ Descrição obrigatória
   └─ Tipo válido
```

#### **Category**
```csharp
Propriedades:
├─ Id (PK)
├─ UserId (FK)
├─ Nome
├─ Cor (Hex)
├─ Icone
└─ Transações relacionadas
```

#### **Goal**
```csharp
Propriedades:
├─ Id (PK)
├─ UserId (FK)
├─ Titulo
├─ ValorAlvo
├─ ValorAtual
├─ DataInicio
├─ DataFim
├─ Status
└─ Progresso %
```

#### **TransactionLimit**
```csharp
Propriedades:
├─ Id (PK)
├─ UserId (FK)
├─ CategoryId (FK)
├─ LimiteValor
├─ GastoAtual
├─ Mes/Ano
└─ Alertas
```

---

## 🟩 BACKEND - Financial.Application

### 📌 Services (Casos de Uso)

#### **TransactionService**
```
Métodos Principais:
├─ CreateAsync(dto) → Criar transação
├─ GetByIdAsync(id) → Buscar por ID
├─ GetAllAsync() → Listar todas
├─ UpdateAsync(id, dto) → Atualizar
├─ DeleteAsync(id) → Deletar
├─ GetByDateRangeAsync(start, end) → Filtro período
├─ GetByCategoryAsync(categoryId) → Filtro categoria
└─ ValidateTransactionAsync() → Validar regras
```

#### **DashboardService**
```
Métodos Principais:
├─ GetBalanceAsync() → Saldo total
├─ GetIncomesVsExpensesAsync() → Receitas vs Despesas
├─ GetTopCategoriesAsync() → Categorias principais
├─ GetRecentTransactionsAsync() → Últimas transações
├─ GetTrendsAsync() → Análise de tendências
└─ GetMonthlyDataAsync() → Dados do mês
```

#### **ReportService**
```
Métodos Principais:
├─ GenerateMonthlyReportAsync() → Relatório mensal
├─ GenerateByCategoryAsync() → Análise por categoria
├─ GenerateComparisonAsync() → Comparativo período
├─ CalculateTrendsAsync() → Calcular tendências
├─ GenerateForecastAsync() → Previsão de saldo
└─ GetStatisticsAsync() → Estatísticas gerais
```

#### **ExportService**
```
Métodos Principais:
├─ ExportToCsvAsync(data) → Exportar CSV
├─ ExportToPdfAsync(data) → Exportar PDF
├─ ExportToExcelAsync(data) → Exportar Excel
├─ FilterByDateAsync(start, end) → Filtrar por período
└─ FormatReportAsync(data) → Formatar dados
```

#### **GoalService**
```
Métodos Principais:
├─ CreateGoalAsync(dto) → Criar meta
├─ GetGoalAsync(id) → Buscar meta
├─ UpdateProgressAsync(id) → Atualizar progresso
├─ CheckGoalAsync() → Validar atingimento
├─ GetAllGoalsAsync() → Listar metas
└─ DeleteGoalAsync(id) → Remover meta
```

### 📌 DTOs (Data Transfer Objects)

```
TransactionDto
├─ Id, Descricao, Valor
├─ Tipo, Data, CategoryId
└─ Status

CreateTransactionDto
├─ Descricao, Valor
├─ Tipo, Data, CategoryId
└─ (sem Id, criado automaticamente)

DashboardDto
├─ Saldo, Receitas, Despesas
├─ CategoriasPrincipais, TransaçõesRecentes
└─ TendênciasGráficos
```

---

## 🟥 BACKEND - Financial.Infrastructure

### 📌 Repositórios (Acesso a Dados)

#### **TransactionRepository**
```
Métodos:
├─ GetByIdAsync(id)
├─ GetAllAsync(userId)
├─ GetByDateRangeAsync(start, end)
├─ GetByCategoryAsync(categoryId)
├─ AddAsync(entity)
├─ UpdateAsync(entity)
├─ DeleteAsync(id)
└─ GetTotalByTypeAsync() → Soma receitas/despesas
```

#### **CategoryRepository**
```
Métodos:
├─ GetAllUserCategoriesAsync(userId)
├─ GetByIdAsync(id)
├─ AddAsync(entity)
├─ UpdateAsync(entity)
├─ DeleteAsync(id)
└─ GetMostUsedAsync() → Mais usadas
```

#### **UserRepository**
```
Métodos:
├─ GetByIdAsync(id)
├─ GetByEmailAsync(email)
├─ CreateAsync(entity)
├─ UpdateAsync(entity)
├─ DeleteAsync(id)
└─ ValidateCredentialsAsync() → Verificar login
```

#### **GoalRepository**
```
Métodos:
├─ GetAllUserGoalsAsync(userId)
├─ GetByIdAsync(id)
├─ AddAsync(entity)
├─ UpdateAsync(entity)
├─ DeleteAsync(id)
└─ GetActiveGoalsAsync() → Apenas ativas
```

#### **TransactionLimitRepository**
```
Métodos:
├─ GetLimitAsync(categoryId, mes)
├─ SetLimitAsync(entity)
├─ GetCurrentSpendingAsync(categoryId)
├─ CheckLimitAsync() → Validar limite
└─ GetAlertsAsync() → Buscar alertas
```

### 📌 Banco de Dados (EF Core)

```
DbContexto
├─ DbSet<User>
├─ DbSet<Transaction>
├─ DbSet<Category>
├─ DbSet<Goal>
├─ DbSet<TransactionLimit>
└─ OnModelCreating()
   ├─ Relacionamentos
   ├─ Índices
   └─ Validações
```

---

## 💻 FRONTEND - React Components

### 🎨 **Common Components** (Reutilizáveis)

```
Button/
├─ Props: label, onClick, variant, disabled
└─ Uso: Todos os formulários e ações

Card/
├─ Props: title, children, icon
└─ Uso: Exibir dados em cards

Modal/
├─ Props: isOpen, onClose, title, children
└─ Uso: Formulários e confirmações

Input/
├─ Props: label, type, value, onChange, error
└─ Uso: Campos de entrada em formulários

Spinner/
├─ Props: size, color
└─ Uso: Indicador de carregamento

Navigation/
├─ Props: activeTab
└─ Uso: Menu de navegação principal
```

### 📊 **Dashboard Feature**

#### **BalanceCard**
```
Funcionalidade:
├─ Exibir saldo total
├─ Mostrar saldo anterior
├─ Diferença do período
├─ Cor verde/vermelho
└─ Atualizar em tempo real
```

#### **CategoryChart**
```
Funcionalidade:
├─ Gráfico de pizza
├─ Distribuição por categoria
├─ % de cada categoria
├─ Top 5 categorias
└─ Interativo (click para detalhe)
```

#### **TrendChart**
```
Funcionalidade:
├─ Gráfico de linha
├─ Histórico últimos 12 meses
├─ Receitas vs Despesas
├─ Saldo acumulado
└─ Previsão para próximo mês
```

### 💳 **Transactions Feature**

#### **TransactionForm**
```
Funcionalidade:
├─ Criar nova transação
├─ Campo: Descrição
├─ Campo: Valor
├─ Campo: Tipo (Receita/Despesa)
├─ Campo: Data
├─ Campo: Categoria (Select)
├─ Validação de campos
└─ Enviar para API
```

#### **TransactionList**
```
Funcionalidade:
├─ Tabela com transações
├─ Colunas: Data, Descrição, Categoria, Valor, Tipo
├─ Ações: Editar, Deletar
├─ Paginação
├─ Ordenação
└─ Busca/Filtro
```

#### **TransactionFilter**
```
Funcionalidade:
├─ Filtrar por data (range)
├─ Filtrar por categoria (select)
├─ Filtrar por tipo (radio)
├─ Botão: Aplicar filtro
└─ Botão: Limpar filtro
```

### 📈 **Reports Feature**

#### **ReportTable**
```
Funcionalidade:
├─ Exibir relatório tabular
├─ Colunas: Período, Receita, Despesa, Saldo
├─ Total geral por coluna
├─ Exportar dados
└─ Imprimir relatório
```

#### **ReportChart**
```
Funcionalidade:
├─ Gráfico de barras
├─ Comparativo mês a mês
├─ Análise por categoria
├─ Legenda com cores
└─ Interativo
```

#### **ExportButton**
```
Funcionalidade:
├─ Botão: Export para CSV
├─ Botão: Export para PDF
├─ Botão: Export para Excel
├─ Menu dropdown
└─ Confirmar período
```

### 🎯 **Goals Feature**

#### **GoalForm**
```
Funcionalidade:
├─ Criar nova meta
├─ Campo: Título
├─ Campo: Valor alvo
├─ Campo: Data fim
├─ Validação de dados
└─ Enviar para API
```

#### **GoalList**
```
Funcionalidade:
├─ Listar todas as metas
├─ Status: Ativa/Completa/Expirada
├─ Ações: Editar, Deletar, Completar
├─ Ordenar por data
└─ Filtrar por status
```

#### **GoalProgress**
```
Funcionalidade:
├─ Progress bar visual
├─ % concluído
├─ Valor atual vs alvo
├─ Tempo restante
└─ Data de conclusão
```

### 🔐 **Auth Feature**

#### **LoginForm**
```
Funcionalidade:
├─ Campo: Email
├─ Campo: Senha
├─ Lembrar-me (checkbox)
├─ Link: Esqueci senha
├─ Link: Registrar
└─ Enviar credenciais
```

#### **RegisterForm**
```
Funcionalidade:
├─ Campo: Nome completo
├─ Campo: Email
├─ Campo: Senha
├─ Campo: Confirmar senha
├─ Validação de força de senha
└─ Link: Já tenho conta
```

#### **LogoutButton**
```
Funcionalidade:
├─ Botão no header
├─ Confirmar logout
├─ Limpar token
└─ Redirecionar login
```

---

## 🔧 FRONTEND - Serviços e Utilitários

### 📡 **Services/API**

```
transactionService
├─ createTransaction(data)
├─ getTransactions(filters)
├─ updateTransaction(id, data)
├─ deleteTransaction(id)
└─ getTransactionById(id)

dashboardService
├─ getBalance()
├─ getIncomesVsExpenses()
├─ getTopCategories()
├─ getRecentTransactions()
└─ getTrends()

reportService
├─ generateMonthlyReport()
├─ generateByCategoryReport()
├─ generateComparison()
├─ getStatistics()
└─ generateForecast()

exportService
├─ exportToCsv(data)
├─ exportToPdf(data)
├─ exportToExcel(data)
└─ filterByDate(start, end)

authService
├─ login(email, password)
├─ register(userData)
├─ logout()
├─ refreshToken()
└─ getCurrentUser()

goalService
├─ createGoal(data)
├─ getGoals()
├─ updateGoal(id, data)
├─ deleteGoal(id)
└─ updateProgress(id, value)
```

### 🪝 **Custom Hooks**

```
useApi()
├─ Gerenciar chamadas API
├─ Loading, error, data
└─ Tratamento de erros

useAuth()
├─ Autenticação do usuário
├─ Token gerenciamento
└─ Estado login

useTransaction()
├─ CRUD de transações
├─ Filtros aplicados
└─ Refresh automático

usePagination()
├─ Paginação de listas
├─ Page, limit, total
└─ Navegar páginas

useLocalStorage()
├─ Persistência local
├─ Salvar dados
└─ Recuperar dados
```

### 🎨 **Utilitários**

```
formatting
├─ formatCurrency(value)
├─ formatDate(date)
├─ formatPercentage(value)
└─ formatNumber(value)

validation
├─ validateEmail(email)
├─ validatePassword(password)
├─ validateCurrency(value)
└─ validateDateRange(start, end)

date
├─ getMonthName(month)
├─ getLastDayOfMonth(date)
├─ formatDateRange(start, end)
└─ getDaysDifference(date1, date2)

currency
├─ convertCurrency(value, from, to)
├─ formatBRL(value)
├─ roundCurrency(value)
└─ calculatePercentage(value, total)
```

---

## 📊 Fluxo de Dados

```
User Input (Form)
         ↓
  Hook (useTransaction)
         ↓
  Service (transactionService)
         ↓
  API Request (axios)
         ↓
  Backend Endpoint
         ↓
  Application Service (TransactionService)
         ↓
  Repository (TransactionRepository)
         ↓
  Database (EF Core)
         ↓
  Response JSON
         ↓
  Context/State Update
         ↓
  Component Re-render
         ↓
  UI Updated
```

---

## 🎯 Mapeamento de Funcionalidades

| Feature | Backend | Frontend | Banco |
|---------|---------|----------|-------|
| **Transações** | ✅ CRUD API | ✅ Form/List | ✅ Transaction |
| **Dashboard** | ✅ Agregação | ✅ Gráficos | ✅ Queries |
| **Relatórios** | ✅ Análise | ✅ Tabelas | ✅ Reports |
| **Metas** | ✅ Gerenciar | ✅ Progress | ✅ Goal |
| **Limites** | ✅ Validar | ✅ Alertas | ✅ Limit |
| **Autenticação** | ✅ JWT | ✅ Login Form | ✅ User |
| **Exportação** | ✅ Gerar | ✅ Download | ✅ Export |
| **Categorias** | ✅ Gerenciar | ✅ Seletor | ✅ Category |

---

## ✅ Funcionalidades Prontas

- [x] Autenticação com JWT
- [x] CRUD de Transações
- [x] Dashboard com gráficos
- [x] Relatórios customizados
- [x] Metas financeiras
- [x] Limites de gastos
- [x] Exportação de dados
- [x] Categorias personalizadas
- [x] Filtros avançados
- [x] Validações completas

---

**Versão:** 1.0  
**Data:** Julho 2026  
**Status:** ✅ Completo
