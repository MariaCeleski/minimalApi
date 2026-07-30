# Implementation Plan: Personal Financial Management Application

## Overview

Este plano implementa a aplicação de gestão financeira pessoal seguindo a arquitetura especificada: ASP.NET Core Minimal API + React + SQLite. A implementação é organizada em 7 fases, com 50+ tasks discretas cobrindo backend (Entity Framework, Services, Endpoints), frontend (React components, integração com Recharts e Framer Motion) e testes.

## Phase 1: Project Foundation & Database Setup

- [x] 1.1 Configure projeto ASP.NET Core com Minimal API
  - Criar projeto com dotnet CLI
  - Adicionar dependências: Entity Framework Core, SQLite, CORS, Logging
  - Configurar Program.cs com services básicos
  - _Requirements: 20_

- [x] 1.2 Setup Entity Framework Core e SQLite
  - Criar DbContexto com DbSet para todas as entidades
  - Configurar conexão SQLite no appsettings.json
  - Criar migrations iniciais
  - _Requirements: 20_

- [x] 1.3 Create domain models (User, Category, Transaction, Goal, Limit)
  - Implementar classes de domínio com validações
  - Configurar relacionamentos no DbContext
  - Adicionar data annotations para constraints
  - _Requirements: 1, 5, 18, 19_

- [x] 1.4 Implement Generic Repository Pattern
  - Criar IRepository<T> interface com CRUD básico
  - Implementar Repository<T> class
  - Adicionar métodos de paginação e filtro
  - _Requirements: 1, 2_

- [x] 1.5 Seed initial categories and default data
  - Criar migration com as 8 categorias predefinidas
  - Adicionar seed data ao DbContext
  - _Requirements: 1, 5_

- [x] 1.6 Implement Global Exception Handler Middleware
  - Criar custom exception classes (ValidationException, NotFoundException)
  - Implementar middleware de tratamento centralizado
  - _Requirements: 1, 8_

- [x] 1.7 Setup React project e TypeScript configuration
  - Criar React app com Vite ou CRA
  - Configurar TypeScript com tsconfig.json
  - Instalar dependências: Axios, TailwindCSS, Recharts, Framer Motion
  - _Requirements: 14, 15, 16_

## Phase 2: Transaction CRUD Operations

- [x] 2.1 Create Transaction DTOs (CreateTransactionDto, UpdateTransactionDto, TransactionResponseDto)
  - Definir estrutura de dados para requisições/respostas
  - Adicionar validações via FluentValidation
  - _Requirements: 1, 2, 7_

- [x] 2.2 Implement TransactionService with validations
  - Criar service para lógica de transações
  - Implementar validação: valor > 0, data não futura, campos obrigatórios
  - Adicionar método de cálculo de saldo
  - _Requirements: 1, 5_

- [x]* 2.3 Write property tests for TransactionService
  - **Property 1: Round-trip consistency (Req 1)**
  - **Property 5: Automatic balance calculation invariant (Req 5)**
  - Testar com values aleatórios, datas válidas, categorias
  - _Requirements: 1, 5_

- [x] 2.4 Create Transaction API endpoints (POST, GET, GET by ID)
  - POST /transactions - criar transação
  - GET /transactions - listar com paginação
  - GET /transactions/{id} - obter por ID
  - _Requirements: 1, 2_

- [x] 2.5 Implement pagination in transaction listing
  - Adicionar parameters: page, pageSize (default 10)
  - Retornar metadados: currentPage, totalPages, totalItems
  - _Requirements: 2_

- [x]* 2.6 Write unit tests for pagination logic
  - Testar page 1, última página, página inválida
  - Verificar metadados de paginação
  - _Requirements: 2_

- [x] 2.7 Create Period Filter endpoint parameter
  - Adicionar startDate, endDate ao filtro
  - Implementar validação: startDate <= endDate
  - Defaults: 30 dias atrás e data atual
  - _Requirements: 3_

- [x]* 2.8 Write property tests for Period Filter
  - **Property 3: Boundary inclusivity (Req 3)**
  - Testar transações na borda do período
  - _Requirements: 3_

- [x] 2.9 Create Category Filter endpoint parameter
  - Adicionar categories[] ao filtro
  - Permitir múltiplas categorias
  - Validar contra lista predefinida
  - _Requirements: 4_

- [x]* 2.10 Write property tests for Category Filter
  - **Property 4: Confluence (Req 4)**
  - Testar ordem de aplicação de filtros
  - _Requirements: 4_

- [x] 2.11 Create PUT endpoint for editing transactions
  - PUT /transactions/{id} - atualizar transação
  - Revalidar todos os campos
  - Recalcular saldo após edição
  - _Requirements: 7_

- [x]* 2.12 Write property tests for transaction editing
  - **Property 7: ID and creation date invariance (Req 7)**
  - _Requirements: 7_

- [x] 2.13 Create DELETE endpoint for removing transactions
  - DELETE /transactions/{id} - deletar transação
  - Validar existência antes de deletar
  - Recalcular saldo
  - _Requirements: 8_

- [x]* 2.14 Write property tests for transaction deletion
  - **Property 8: Balance recalculation invariant (Req 8)**
  - _Requirements: 8_

- [x] 2.15 Checkpoint - Ensure all transaction CRUD tests pass
  - Executar todos os testes de transação
  - Validar integridade de dados

## Phase 3: Dashboard & Balance Calculations

- [x] 3.1 Implement DashboardService with balance calculations
  - Criar método GetBalance() retornando saldo total
  - Implementar Σ(receitas) - Σ(despesas)
  - Precisão de 2 casas decimais
  - _Requirements: 5, 6_

- [x]* 3.2 Write property tests for balance calculations
  - **Property 5: Invariant Saldo = Σ(receitas) - Σ(despesas) (Req 5)**
  - **Property 5: Idempotence (Req 5)**
  - **Property 5: Round-trip with transaction add/remove (Req 5)**
  - _Requirements: 5_

- [x] 3.3 Create GET /dashboard endpoint
  - Retornar saldo total, receitas totais, despesas totais
  - Incluir indicador visual se saldo negativo
  - _Requirements: 6_

- [x] 3.4 Create React BalanceCard component
  - Exibir saldo em destaque (grande, topo)
  - Mostrar cor vermelha se negativo
  - Animar mudanças com Framer Motion
  - _Requirements: 6, 15_

- [x] 3.5 Create CategoryDistribution DTO e endpoint
  - GET /dashboard/category-distribution
  - Retornar totalizado por categoria
  - Incluir percentuais
  - _Requirements: 6_

- [x]* 3.6 Write property tests for category distribution
  - **Property 6: Sum equality (Req 6)**
  - _Requirements: 6_

- [x] 3.7 Create React CategoryChart (pizza) component
  - Integrar com Recharts PieChart
  - Animar entrada com duração 500ms
  - Mostrar labels com percentuais
  - _Requirements: 6, 15, 16_

- [x] 3.8 Create MonthlyTrend DTO e endpoint
  - GET /dashboard/monthly-trend
  - Retornar saldo para cada um dos últimos 12 meses
  - _Requirements: 6_

- [x] 3.9 Create React TrendChart (linha) component
  - Integrar com Recharts LineChart
  - Animar crescimento das linhas
  - Mostrar últimos 12 meses
  - _Requirements: 6, 15_

- [x] 3.10 Implement real-time dashboard updates
  - Adicionar signalR ou polling para atualizações
  - Dashboard recarrega ao adicionar/editar transação
  - _Requirements: 6_

- [x] 3.11 Create period filter integration no dashboard
  - Adicionar startDate, endDate aos parâmetros
  - Gráficos refletem período filtrado
  - _Requirements: 6, 3_

- [x] 3.12 Checkpoint - Ensure all dashboard tests pass
  - Validar cálculos de saldo
  - Testar componentes visuais

## Phase 4: Reports & Export Services

- [x] 4.1 Implement ReportService with monthly aggregations
  - CreateMonthlyReport(year, month): retorna receitas, despesas, saldo, breakdown por categoria
  - Calcular percentuais por categoria
  - _Requirements: 9_

- [x]* 4.2 Write property tests for monthly reports
  - **Property 9: Total income equality (Req 9)**
  - **Property 9: Percentage sum ≈ 100% (Req 9)**
  - _Requirements: 9_

- [x] 4.3 Create GET /reports/monthly endpoint
  - Parâmetros: year, month
  - Retornar ReportDto estruturado
  - _Requirements: 9_

- [x] 4.4 Implement ReportService.CreateCategoryReport()
  - Agregar por categoria no período
  - Incluir percentuais
  - Ordenar por valor descendente
  - _Requirements: 10_

- [x]* 4.5 Write property tests for category reports
  - **Property 10: Sum equality (Req 10)**
  - **Property 10: Confluence (Req 10)**
  - _Requirements: 10_

- [x] 4.6 Create GET /reports/category endpoint
  - Parâmetros: startDate, endDate
  - Retornar CategoryReportDto
  - _Requirements: 10_

- [x] 4.7 Create React ReportPage component
  - Selector para tipo de relatório (mensal, categoria)
  - Mostrar dados em tabelas formatadas
  - _Requirements: 9, 10_

- [x] 4.8 Implement ExportService with CSV export
  - Método ExportTransactionsToCSV(transactions): gera CSV
  - Headers: ID, Data, Tipo, Valor, Categoria, Descrição
  - UTF-8 encoding, escape de caracteres especiais
  - _Requirements: 11_

- [x]* 4.9 Write property tests for CSV export
  - **Property 11: Round-trip parsing (Req 11)**
  - **Property 11: Row count invariant (Req 11)**
  - _Requirements: 11_

- [x] 4.10 Create GET /export/csv endpoint
  - Query params: startDate, endDate, categories
  - Retornar arquivo como download
  - Nome: transacoes_YYYY-MM-DD.csv
  - _Requirements: 11_

- [x] 4.11 Implement ExportService with PDF export
  - Método ExportReportToPDF(report): gera PDF
  - Incluir: título, período, resumo, tabela de transações
  - Cores e formatação visual
  - _Requirements: 12_

- [x]* 4.12 Write property tests for PDF export
  - **Property 12: Data preservation (Req 12)**
  - Verificar caracteres acentuados
  - _Requirements: 12_

- [x] 4.13 Create GET /export/pdf endpoint
  - Query params: startDate, endDate
  - Retornar arquivo PDF como download
  - Nome: relatorio_YYYY-MM-DD.pdf
  - _Requirements: 12_

- [x] 4.14 Create React ExportOptions component
  - Botões para CSV e PDF
  - Mostrar período selecionado
  - Feedback ao usuário após export
  - _Requirements: 11, 12_

- [~] 4.15 Checkpoint - Ensure all report and export tests pass
  - Testar geração de relatórios
  - Validar exports

## Phase 5: Advanced Features (Goals & Limits)

- [-] 5.1 Create Goal DTOs e repository
  - GoalDto, CreateGoalDto com: nome, valor_alvo, data_limite
  - Implementar repository específico
  - _Requirements: 18_

- [-] 5.2 Implement GoalsService with CRUD
  - CreateGoal(), GetGoal(), UpdateGoal(), DeleteGoal()
  - Calcular progresso: (saldo_poupança / valor_alvo) * 100
  - _Requirements: 18_

- [ ]* 5.3 Write property tests for Goals
  - **Property 18: Progress formula invariant (Req 18)**
  - _Requirements: 18_

- [-] 5.4 Create CRUD endpoints para Goals
  - POST /goals, GET /goals, GET /goals/{id}
  - PUT /goals/{id}, DELETE /goals/{id}
  - _Requirements: 18_

- [-] 5.5 Create TransactionLimit DTOs e repository
  - LimitDto com: categoria, limite_valor
  - _Requirements: 19_

- [-] 5.6 Implement TransactionLimitService
  - CreateLimit(), GetLimit(), UpdateLimit(), DeleteLimit()
  - Verificar se gasto excede 80% e 100% após cada transação
  - _Requirements: 19_

- [ ]* 5.7 Write property tests for Limits
  - **Property 19: Threshold invariant (Req 19)**
  - Testar 80% boundary
  - _Requirements: 19_

- [ ] 5.8 Create CRUD endpoints para Limits
  - POST /limits, GET /limits, PUT /limits/{id}, DELETE /limits/{id}
  - _Requirements: 19_

- [~] 5.9 Create NotificationService com notification queue
  - Armazenar notificações em memória ou DB
  - Disparar após verificação de limites
  - _Requirements: 18, 19_

- [~] 5.10 Integrate notifications no TransactionService
  - Após adicionar transação, verificar metas e limites
  - Disparar notificações apropriadas
  - _Requirements: 18, 19_

- [~] 5.11 Create React GoalsOverview component
  - Listar goals com barras de progresso
  - Mostrar verde se atingida, cinza se não
  - _Requirements: 18_

- [~] 5.12 Create React LimitAlerts component
  - Exibir aviso (amarelo) em 80%
  - Exibir alerta (vermelho) em 100%
  - _Requirements: 19_

- [~] 5.13 Implement notification center no React
  - Componente para exibir fila de notificações
  - Toast ou banner visual
  - Desaparecer após 5s
  - _Requirements: 18, 19_

- [~] 5.14 Checkpoint - Ensure goals and limits features work
  - Testar criação e deleção de goals
  - Testar verificação de limites

## Phase 6: Authentication & Theme System

- [~] 6.1 Create User DTOs (RegisterDto, LoginDto, UserResponseDto)
  - Validações: email format, senha min 6 caracteres
  - _Requirements: 17_

- [~] 6.2 Implement AuthenticationService
  - RegisterUser(): validar email único, hash password
  - LoginUser(): validar credenciais, gerar token
  - ValidateToken(): verificar token em requisições
  - _Requirements: 17_

- [ ]* 6.3 Write property tests for authentication
  - **Property 17: Idempotent login (Req 17)**
  - **Property 17: Error handling consistency (Req 17)**
  - _Requirements: 17_

- [~] 6.4 Create JWT token generation e validation
  - Gerar token com claims de usuário
  - Validar assinatura em middleware
  - _Requirements: 17_

- [~] 6.5 Create POST /auth/register endpoint
  - Validar email e senha
  - Retornar sucesso ou erro específico
  - _Requirements: 17_

- [~] 6.6 Create POST /auth/login endpoint
  - Validar credenciais
  - Retornar token de sessão
  - Não revelar qual campo está errado
  - _Requirements: 17_

- [~] 6.7 Add authorization middleware
  - Verificar token em endpoints protegidos
  - Retornar 401 se inválido
  - _Requirements: 17_

- [~] 6.8 Create LoginPage component
  - Form com email e senha
  - Validação client-side
  - Submit ao backend
  - _Requirements: 17_

- [~] 6.9 Create RegisterPage component
  - Form com email, senha, confirmação
  - Mostrar requisitos de senha
  - _Requirements: 17_

- [~] 6.10 Implement localStorage token persistence
  - Salvar token ao fazer login
  - Usar token em headers de requisições
  - Limpar ao fazer logout
  - _Requirements: 17_

- [~] 6.11 Create ThemeProvider context no React
  - Implementar Context API para dark/light mode
  - Hook: useTheme() para componentes
  - _Requirements: 13_

- [~] 6.12 Implement theme switching e persistence
  - Função toggleTheme()
  - Persistir em localStorage
  - Restaurar ao recarregar página
  - _Requirements: 13_

- [ ]* 6.13 Write property tests for theme persistence
  - **Property 13: Idempotence (Req 13)**
  - **Property 13: Persistence round-trip (Req 13)**
  - _Requirements: 13_

- [~] 6.14 Create ThemeToggle component
  - Botão sun/moon icon
  - Animar transição com Framer Motion (300ms)
  - _Requirements: 13, 15_

- [~] 6.15 Apply theme colors em TailwindCSS
  - Configurar paleta: azul, verde, cinza
  - Contraste acessível light/dark
  - _Requirements: 13_

- [~] 6.16 Apply theme colors a todos os componentes
  - Atualizar BalanceCard, Charts, Forms
  - Teste visual em light e dark mode
  - _Requirements: 13_

- [~] 6.17 Checkpoint - Ensure auth and theme systems work
  - Testar login/logout/register
  - Testar alternância de tema

## Phase 7: UI Polish, Animations & Responsiveness

- [~] 7.1 Setup TailwindCSS configuration
  - Instalar e configurar
  - Definir breakpoints: 320px, 768px, 1920px
  - _Requirements: 14_

- [~] 7.2 Create responsive layout system
  - Base layout com header, sidebar, main content
  - Adaptar para mobile/tablet/desktop
  - _Requirements: 14_

- [~] 7.3 Implement mobile-first CSS
  - Componentes em coluna em mobile (320px)
  - Lado a lado em desktop (1920px)
  - _Requirements: 14_

- [~] 7.4 Create TransactionForm component
  - Fields: data, valor, categoria (select), descrição
  - Validação client-side
  - Animar entrada com Framer Motion
  - _Requirements: 1, 15_

- [~] 7.5 Create TransactionList component
  - Renderizar lista com ícones por categoria
  - Paginação integrada
  - Animar new items
  - _Requirements: 2, 16_

- [~] 7.6 Create TransactionFilters component
  - Filtros: período (startDate, endDate), categoria (multi-select)
  - Apply button
  - _Requirements: 3, 4_

- [~] 7.7 Create Dashboard layout responsivo
  - Mobile: BalanceCard, charts empilhados
  - Desktop: BalanceCard, charts lado a lado
  - _Requirements: 6, 14_

- [~] 7.8 Add Framer Motion animations em TransactionList
  - Novas transações: slide in + fade
  - Duração: 300ms
  - Ease: ease-in-out
  - _Requirements: 15_

- [~] 7.9 Add Framer Motion animations em Charts
  - Barras/pizza: grow from 0 com duração 500ms
  - Ease: ease-in-out
  - _Requirements: 15_

- [~] 7.10 Add Framer Motion page transitions
  - Fade + slide ao navegar entre páginas
  - Duração: 300ms
  - _Requirements: 15_

- [~] 7.11 Add Framer Motion theme transition
  - Ao trocar tema, animar cores de fundo/texto
  - Duração: 300ms
  - _Requirements: 13, 15_

- [~] 7.12 Create icon mapping para todas categorias
  - Alimentação: utensil icon
  - Transporte: car icon
  - Lazer: game icon
  - etc (6 mais)
  - _Requirements: 16_

- [~] 7.13 Display icons em TransactionList
  - Mostrar ícone 32px ao lado de cada transação
  - 24px em mobile
  - _Requirements: 16_

- [~] 7.14 Display icons em CategoryChart
  - Mostrar ícone no label da pizza
  - Redimensionar apropriadamente
  - _Requirements: 16_

- [~] 7.15 Implement responsive font sizes
  - Mobile: base, lg em certos lugares
  - Desktop: lg, xl, 2xl
  - _Requirements: 14_

- [~] 7.16 Implement responsive spacing
  - Padding/margin em porcentagens ou rem
  - Mobile-first approach
  - _Requirements: 14_

- [~] 7.17 Test no scroll horizontal em todos breakpoints
  - Viewport 320px, 768px, 1920px
  - Sem overflow-x
  - _Requirements: 14_

- [~] 7.18 Create main navigation component
  - Links: Dashboard, Transações, Relatórios, Goals, Limites, Perfil
  - Responsivo: hamburger em mobile
  - _Requirements: 1, 2, 9, 10, 18, 19_

- [~] 7.19 Checkpoint - Ensure UI is polished and responsive
  - Testar em 3 dispositivos (mobile, tablet, desktop)
  - Verificar animações

## Phase 8: Data Integrity & Testing

- [~] 8.1 Implement data integrity validation service
  - VerifyIntegrity(): saldo_calculado == saldo_armazenado
  - Verificar transações órfãs
  - Registrar erros em log
  - _Requirements: 20_

- [~] 8.2 Add transaction support para operações críticas
  - Usar transaction scope no TransactionService
  - Rollback se erro mid-way
  - _Requirements: 20_

- [~] 8.3 Create GET /health/integrity endpoint
  - Executar VerifyIntegrity()
  - Retornar status e detalhes
  - _Requirements: 20_

- [ ]* 8.4 Write property tests for data integrity
  - **Property 20: Balance invariant (Req 20)**
  - **Property 20: Idempotent validation (Req 20)**
  - _Requirements: 20_

- [ ]* 8.5 Write comprehensive unit tests para TransactionService
  - Testar todas as validações
  - Testar edge cases
  - Coverage > 80%
  - _Requirements: 1, 2, 3, 4, 7, 8_

- [ ]* 8.6 Write comprehensive unit tests para DashboardService
  - Testar cálculos de saldo
  - Testar agregações por categoria
  - _Requirements: 5, 6_

- [ ]* 8.7 Write comprehensive unit tests para ReportService
  - Testar geração de relatórios
  - Testar percentuais
  - _Requirements: 9, 10_

- [ ]* 8.8 Write integration tests para fluxo completo
  - Criar transação, editá-la, deletá-la
  - Verificar atualizações de saldo
  - _Requirements: 1, 2, 5, 7, 8_

- [ ]* 8.9 Write React component tests (Jest)
  - Testar BalanceCard rendering
  - Testar TransactionForm validations
  - _Requirements: 1, 6_

- [ ]* 8.10 Write E2E tests (Cypress/Playwright)
  - Criar transação via UI
  - Verificar atualização no dashboard
  - Fazer login/logout
  - _Requirements: 1, 2, 6, 17_

- [~] 8.11 Checkpoint - Ensure all tests pass
  - Executar full test suite
  - Validar coverage

## Phase 9: Performance & Documentation

- [~] 9.1 Profile backend performance
  - Identificar bottlenecks em queries
  - Otimizar índices no SQLite
  - _Requirements: 6_

- [~] 9.2 Implement database query optimization
  - Adicionar indexes em Transaction (UserId, CategoryId, Date)
  - Usar Include() para eager loading onde apropriado
  - _Requirements: 2, 6_

- [~] 9.3 Implement caching strategy
  - Cache de categorias em memory
  - Cache de dashboard por 5 minutos
  - _Requirements: 6_

- [~] 9.4 Profile React performance
  - Usar React DevTools Profiler
  - Identificar renders desnecessários
  - _Requirements: 6, 14_

- [~] 9.5 Implement React.memo para componentes
  - BalanceCard, CategoryChart, TransactionItem
  - Prevenir rerenders desnecessários
  - _Requirements: 6, 14_

- [~] 9.6 Optimize bundle size
  - Remover dependências não usadas
  - Lazy load pages com React.lazy()
  - _Requirements: 14_

- [~] 9.7 Create API documentation (Swagger)
  - Documentar todos os endpoints
  - Incluir exemplos de requisição/resposta
  - _Requirements: 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 17_

- [~] 9.8 Create user documentation
  - Guia de como usar a aplicação
  - Screenshots e explicações
  - _Requirements: 1, 2, 3, 4, 6, 18, 19_

- [~] 9.9 Create developer documentation
  - Setup local environment
  - Running tests
  - Project structure
  - _Requirements: 1, 17_

- [~] 9.10 Create deployment guide
  - Docker configuration
  - Environment variables
  - Database migrations
  - _Requirements: 1, 2_

- [~] 9.11 Final testing e bug fixes
  - Testar end-to-end toda a aplicação
  - Corrigir bugs identificados
  - _Requirements: 1, 2, 3, 4, 5, 6, 7, 8_

- [~] 9.12 Checkpoint - Application ready for production
  - Todos os testes passando
  - Documentação completa
  - Performance otimizada

## Notes

- Tasks marcadas com `*` são opcionais (testes) e podem ser puladas para MVP mais rápido
- Cada task referencia requisitos específicos para rastreabilidade
- Tasks de integração garantem validação incremental
- Checkpoints entre fases validam progresso
- Seguir ordem de fases: fundação → CRUD → dashboard → relatórios → features avançadas → UI polish → testes → deployment
- Estimativa total: 7 semanas (9 fases, 1 semana por fase com overlaps possíveis)



## Task Dependency Graph

```json
{
  "waves": [
    {
      "id": 0,
      "tasks": ["1.1", "1.2", "1.7"],
      "description": "Project setup: ASP.NET Core, EF Core, React with dependencies"
    },
    {
      "id": 1,
      "tasks": ["1.3", "1.4", "1.5", "1.6"],
      "description": "Database models, repository pattern, exception handling, seed data"
    },
    {
      "id": 2,
      "tasks": ["2.1", "2.2", "2.4", "2.7", "2.9", "2.11", "2.13"],
      "description": "Transaction DTOs, service logic, all CRUD endpoints with filters"
    },
    {
      "id": 3,
      "tasks": ["2.3", "2.6", "2.8", "2.10", "2.12", "2.14"],
      "description": "Property tests and unit tests for transactions (optional)"
    },
    {
      "id": 4,
      "tasks": ["3.1", "3.3", "3.5", "3.8"],
      "description": "Dashboard service and endpoints: balance, category distribution, monthly trend"
    },
    {
      "id": 5,
      "tasks": ["3.2", "3.6"],
      "description": "Property tests for dashboard calculations (optional)"
    },
    {
      "id": 6,
      "tasks": ["3.4", "3.7", "3.9", "3.11"],
      "description": "React dashboard components with period filtering"
    },
    {
      "id": 7,
      "tasks": ["3.10"],
      "description": "Real-time dashboard updates"
    },
    {
      "id": 8,
      "tasks": ["4.1", "4.3", "4.4", "4.6"],
      "description": "Report services and endpoints: monthly and category reports"
    },
    {
      "id": 9,
      "tasks": ["4.2", "4.5"],
      "description": "Property tests for report generation (optional)"
    },
    {
      "id": 10,
      "tasks": ["4.7"],
      "description": "React ReportPage component"
    },
    {
      "id": 11,
      "tasks": ["4.8", "4.10", "4.11", "4.13"],
      "description": "Export services and endpoints: CSV and PDF"
    },
    {
      "id": 12,
      "tasks": ["4.9", "4.12"],
      "description": "Property tests for export functionality (optional)"
    },
    {
      "id": 13,
      "tasks": ["4.14"],
      "description": "React ExportOptions component"
    },
    {
      "id": 14,
      "tasks": ["5.1", "5.2", "5.4", "5.5", "5.6", "5.8"],
      "description": "Goals and Limits CRUD: services and endpoints"
    },
    {
      "id": 15,
      "tasks": ["5.3", "5.7"],
      "description": "Property tests for goals and limits (optional)"
    },
    {
      "id": 16,
      "tasks": ["5.9", "5.10"],
      "description": "Notification system integration"
    },
    {
      "id": 17,
      "tasks": ["5.11", "5.12", "5.13"],
      "description": "React components for goals, limits, and notifications"
    },
    {
      "id": 18,
      "tasks": ["6.1", "6.2", "6.4", "6.5", "6.6", "6.7"],
      "description": "Authentication: JWT, register, login endpoints, middleware"
    },
    {
      "id": 19,
      "tasks": ["6.3"],
      "description": "Property tests for authentication (optional)"
    },
    {
      "id": 20,
      "tasks": ["6.8", "6.9", "6.10"],
      "description": "React authentication pages and token management"
    },
    {
      "id": 21,
      "tasks": ["6.11", "6.12", "6.15"],
      "description": "Theme system: context, persistence, TailwindCSS colors"
    },
    {
      "id": 22,
      "tasks": ["6.13"],
      "description": "Property tests for theme persistence (optional)"
    },
    {
      "id": 23,
      "tasks": ["6.14", "6.16"],
      "description": "Theme toggle component and apply theme to all components"
    },
    {
      "id": 24,
      "tasks": ["7.1", "7.2", "7.3"],
      "description": "Responsive design setup: TailwindCSS, layout system, mobile-first"
    },
    {
      "id": 25,
      "tasks": ["7.4", "7.5", "7.6", "7.18"],
      "description": "Core UI components: forms, lists, filters, navigation"
    },
    {
      "id": 26,
      "tasks": ["7.7"],
      "description": "Dashboard layout responsive"
    },
    {
      "id": 27,
      "tasks": ["7.8", "7.9", "7.10", "7.11"],
      "description": "Framer Motion animations: transactions, charts, page transitions, theme"
    },
    {
      "id": 28,
      "tasks": ["7.12", "7.13", "7.14"],
      "description": "Category icons implementation and display"
    },
    {
      "id": 29,
      "tasks": ["7.15", "7.16", "7.17"],
      "description": "Responsive typography and spacing, overflow testing"
    },
    {
      "id": 30,
      "tasks": ["8.1", "8.2", "8.3"],
      "description": "Data integrity validation and health endpoints"
    },
    {
      "id": 31,
      "tasks": ["8.4"],
      "description": "Property tests for data integrity (optional)"
    },
    {
      "id": 32,
      "tasks": ["8.5", "8.6", "8.7", "8.8"],
      "description": "Unit and integration tests for backend services"
    },
    {
      "id": 33,
      "tasks": ["8.9", "8.10"],
      "description": "React component tests and E2E tests"
    },
    {
      "id": 34,
      "tasks": ["9.1", "9.2", "9.3", "9.4", "9.5", "9.6"],
      "description": "Performance optimization: database, caching, React, bundle size"
    },
    {
      "id": 35,
      "tasks": ["9.7", "9.8", "9.9", "9.10"],
      "description": "Documentation: API, user guide, developer guide, deployment"
    },
    {
      "id": 36,
      "tasks": ["9.11"],
      "description": "Final testing and bug fixes"
    }
  ]
}
```

---

## Execution Guidance

### Wave Scheduling Strategy

The dependency graph organizes 50+ tasks into 37 waves for optimal parallel execution:

1. **Waves 0-1** (Setup): Start here, blocks everything else
2. **Waves 2-4** (Transaction CRUD): Foundation for all features
3. **Waves 5-7** (Dashboard): Depends on transactions working
4. **Waves 8-13** (Reports & Exports): Can run in parallel with dashboard
5. **Waves 14-17** (Advanced Features): Requires transaction infrastructure
6. **Waves 18-23** (Auth & Theme): Independent, can start after wave 2
7. **Waves 24-29** (UI & Animations): Requires components from waves 2-7
8. **Waves 30-33** (Testing): Final validation, depends on all implementation
9. **Waves 34-36** (Performance & Docs): Post-implementation polish

### Estimated Timeline

- **Sequential Execution**: 50+ tasks × avg 2-4 hours = 100-200 hours (2-5 weeks solo)
- **Parallel Execution** (3 developers): Same waves in parallel = ~3 weeks
- **Fast Track** (skip optional tests): Reduce by 20-30%

### Key Milestones

- End of Wave 4: Transaction CRUD fully functional ✓
- End of Wave 7: Dashboard with real-time updates ✓
- End of Wave 13: All reports and exports ✓
- End of Wave 23: Auth and theme system complete ✓
- End of Wave 29: UI fully polished and responsive ✓
- End of Wave 33: All tests passing ✓
- End of Wave 36: Production ready ✓

