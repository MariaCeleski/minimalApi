# Funcionalidades de Transações - Documentação Técnica

## 📋 Visão Geral

A página de Transações foi implementada com funcionalidades completas de **CRUD** (Create, Read, Update, Delete) com suporte a paginação, filtros avançados e integração full-stack entre frontend React e backend ASP.NET Core.

---

## 🎯 Funcionalidades Implementadas

### **Backend (ASP.NET Core)**

#### 1. **Endpoints da API REST**

**POST `/api/transactions`** - Criar Nova Transação
- Cria uma nova transação de receita ou despesa
- Validações: valor > 0, data não futura, campos obrigatórios
- Response: 201 Created com dados completos da transação

```json
{
  "amount": 150.50,
  "date": "2026-07-28",
  "type": "Expense",
  "categoryId": 1,
  "description": "Almoço no restaurante",
  "userId": null
}
```

**GET `/api/transactions`** - Listar Transações com Paginação
- Suporta paginação com metadados (página, total de itens, total de páginas)
- Filtros por período (startDate, endDate) com defaults de 30 dias
- Filtros por categorias múltiplas
- Response: 200 OK com dados paginados

```
GET /api/transactions?page=1&pageSize=10&startDate=2026-06-28&endDate=2026-07-28&categoryIds=1&categoryIds=2
```

**GET `/api/transactions/{id}`** - Obter Transação por ID
- Retorna dados completos de uma transação específica
- Inclui informações da categoria (nome, ícone, cor)
- Response: 200 OK ou 404 Not Found

**PUT `/api/transactions/{id}`** - Atualizar Transação
- Atualiza dados da transação mantendo ID e data de criação
- Revalida todos os campos
- Recalcula saldo automaticamente
- Response: 200 OK com transação atualizada

**DELETE `/api/transactions/{id}`** - Deletar Transação
- Remove a transação do sistema
- Response: 204 No Content

**GET `/api/transactions/balance`** - Calcular Saldo Total
- Retorna: saldo total = Σ(receitas) - Σ(despesas)
- Precisão de 2 casas decimais
- Indica se saldo é negativo

#### 2. **Validações de Negócio**

✅ **Validação de Valor**
- Deve ser maior que zero
- Máximo de R$ 999.999.999,99
- Precisão de 2 casas decimais

✅ **Validação de Data**
- Não pode ser futura
- Deve estar entre 01/01/2020 e data atual

✅ **Validação de Categoria**
- Deve existir no sistema
- Suportadas 8 categorias predefinidas:
  1. Alimentação
  2. Transporte
  3. Lazer
  4. Saúde
  5. Educação
  6. Utilitários
  7. Investimento
  8. Outros

✅ **Validação de Descrição**
- Obrigatória com 3-255 caracteres
- Sem caracteres especiais prejudiciais
- Filtro de caracteres XSS

✅ **Validação de Período**
- Data de início ≤ data de fim
- Período máximo de 2 anos
- Defaults: 30 dias atrás a data atual

#### 3. **DTOs (Data Transfer Objects)**

**CreateTransactionDto**
```csharp
public class CreateTransactionDto
{
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public TransactionType Type { get; set; }
    public int CategoryId { get; set; }
    public string Description { get; set; }
    public int? UserId { get; set; }
}
```

**TransactionResponseDto**
```csharp
public class TransactionResponseDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public TransactionType Type { get; set; }
    public string TypeName { get; set; } // "Receita" ou "Despesa"
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string CategoryIcon { get; set; }
    public string CategoryColor { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

**TransactionFilterDto**
```csharp
public class TransactionFilterDto
{
    public int Page { get; set; } = 1
    public int PageSize { get; set; } = 10
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<int> CategoryIds { get; set; }
    public TransactionType? Type { get; set; }
    public int? UserId { get; set; }
}
```

#### 4. **Serviço de Transações (TransactionService)**

Métodos principais:
- `CreateTransactionAsync()` - Criar transação com validações
- `GetTransactionByIdAsync()` - Obter transação por ID
- `GetTransactionsAsync()` - Listar com filtros e paginação
- `UpdateTransactionAsync()` - Atualizar transação
- `DeleteTransactionAsync()` - Deletar transação
- `CalculateBalanceAsync()` - Calcular saldo total

#### 5. **Repository Pattern**

**ITransactionRepository**
- Implementa interface genérica `IRepository<Transaction>`
- Métodos especializados para transações
- `GetPagedTransactionsAsync()` com filtros
- Suporte a expressions LINQ para queries complexas

---

### **Frontend (React + TypeScript)**

#### 1. **Página de Transações (TransactionsPage.tsx)**

**Componentes Principais:**

1. **Seção de Header**
   - Título e descrição
   - Botão "Nova Transação" para criar

2. **Sistema de Filtros**
   - Data inicial e final (com defaults de 30 dias)
   - Chips de categorias para filtro múltiplo
   - Aplicação de filtros em tempo real

3. **Tabela de Transações**
   - Colunas: Data, Descrição, Categoria, Tipo, Valor, Ações
   - Animações com Framer Motion
   - Formatação de moeda (pt-BR)
   - Cores indicando tipo (receita/despesa)

4. **Paginação**
   - Componente Pagination do Material-UI
   - Navegação entre páginas
   - Informação de total de itens

5. **Diálogo de Criar/Editar**
   - Formulário com validação
   - Campos: Descrição, Tipo, Categoria, Valor, Data
   - Carregamento assíncrono

#### 2. **Estados Gerenciados**

```typescript
// Transações
transactions: Transaction[]
loading: boolean
error: string | null
success: string | null

// Formulário
editingId: number | null
formData: {
  amount: string
  date: string
  type: 'Income' | 'Expense'
  categoryId: string
  description: string
}

// Filtros
filters: {
  startDate: string
  endDate: string
  categoryIds: number[]
  page: number
  pageSize: number
}

// Paginação
pagination: {
  currentPage: number
  totalPages: number
  totalItems: number
}
```

#### 3. **Funcionalidades**

✅ **Listar Transações**
- Carrega transações com filtros
- Suporta paginação
- Exibe status de carregamento

✅ **Criar Transação**
- Formulário em diálogo modal
- Validação de campos obrigatórios
- Envio via POST para API

✅ **Editar Transação**
- Abre diálogo com dados preenchidos
- Atualiza via PUT para API
- Mantém ID e data de criação

✅ **Deletar Transação**
- Confirmação antes de deletar
- Requisição DELETE para API
- Recarrega lista após sucesso

✅ **Filtrar por Período**
- Seletor de datas
- Validação de intervalo
- Defaults automáticos

✅ **Filtrar por Categoria**
- Chips clicáveis
- Seleção múltipla
- Destaque visual da seleção

✅ **Feedbacks**
- Mensagens de sucesso
- Alertas de erro
- Loading spinner durante requisições

#### 4. **Integração com API**

**Endpoints Consumidos:**

- `GET /api/categories` - Carregar categorias disponíveis
- `GET /api/transactions` - Listar com filtros e paginação
- `POST /api/transactions` - Criar nova transação
- `PUT /api/transactions/{id}` - Atualizar transação
- `DELETE /api/transactions/{id}` - Deletar transação

**Headers HTTP:**
```typescript
headers: {
  'Content-Type': 'application/json'
}
```

#### 5. **Formatadores e Utilitários**

```typescript
// Formatação de Moeda (pt-BR)
formatCurrency(value: number): string
// Exemplo: 150.50 → "R$ 150,50"

// Formatação de Data (pt-BR)
formatDate(date: string): string
// Exemplo: "2026-07-28" → "28/07/2026"

// Tipo de Transação
getTypeColor(type: string): 'success' | 'error'
getTypeLabel(type: string): 'Receita' | 'Despesa'
```

#### 6. **Animações**

- **Entrada da página:** Fade + Slide (0.5s)
- **Linhas da tabela:** Slide esquerda (0.3s)
- **Componentes:** Transições suaves via Framer Motion

---

## 🔄 Fluxo de Dados

```
Frontend (React)
    ↓
API REST (ASP.NET Core)
    ↓
TransactionService
    ↓
TransactionRepository
    ↓
Entity Framework + SQLite Database
```

### **Exemplo: Criar Transação**

1. Usuário preenche formulário no React
2. Clica em "Salvar"
3. Frontend valida campos
4. Envia POST para `/api/transactions`
5. Backend valida via FluentValidation
6. TransactionService processa
7. Repository insere no banco
8. Response retorna com ID e dados completos
9. Frontend recarrega lista e exibe sucesso

---

## 📊 Estrutura de Dados

**Transaction Entity**
```sql
CREATE TABLE Transactions (
    Id INTEGER PRIMARY KEY,
    Date DATETIME NOT NULL,
    Amount DECIMAL(18,2) NOT NULL CHECK (Amount > 0),
    Description VARCHAR(255) NOT NULL,
    Type INTEGER NOT NULL, -- 1=Income, 2=Expense
    CategoryId INTEGER NOT NULL REFERENCES Categories(Id),
    UserId INTEGER REFERENCES Users(Id),
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Índices
CREATE INDEX IX_Transactions_Date ON Transactions(Date);
CREATE INDEX IX_Transactions_CategoryId ON Transactions(CategoryId);
CREATE INDEX IX_Transactions_Type ON Transactions(Type);
CREATE INDEX IX_Transactions_UserId_Date ON Transactions(UserId, Date);
```

---

## 🧪 Casos de Uso Testados

✅ Criar transação de receita
✅ Criar transação de despesa
✅ Listar transações com paginação (page 1)
✅ Filtrar por período (últimos 30 dias)
✅ Filtrar por categoria única
✅ Filtrar por múltiplas categorias
✅ Filtrar por período + categorias
✅ Editar transação existente
✅ Deletar transação
✅ Validar valor negativo (rejeitar)
✅ Validar data futura (rejeitar)
✅ Validar campo obrigatório (rejeitar)
✅ Validar categoria inexistente (rejeitar)
✅ Calcular saldo (receitas - despesas)

---

## 🎨 Design & UX

**Cores:**
- Receita: Verde (#4caf50)
- Despesa: Vermelho (#f44336)
- Categorias: Cores específicas por categoria

**Responsividade:**
- Mobile: Stack vertical
- Tablet: 2 colunas de filtros
- Desktop: Layout completo

**Acessibilidade:**
- Labels claros em todos os campos
- Feedback de erro visível
- Loading spinners durante requisições
- Confirmação antes de deletar

---

## 📈 Performance

- **Paginação:** Default 10 itens, máx 100 por página
- **Índices:** Criados em Date, CategoryId, Type, UserId
- **Lazy Loading:** Carrega categorias uma única vez
- **Cache:** Transações recarregadas apenas ao trocar filtros

---

## 🔒 Segurança

✅ Validação de entrada em ambos os lados
✅ Filtro de caracteres especiais na descrição
✅ Prevenção de XSS
✅ CORS habilitado para frontend
✅ Tipagem forte (TypeScript + C#)

---

## 📝 Requisitos Atendidos

| Requisito | Status | Implementação |
|-----------|--------|---|
| 1 - CRUD Transações | ✅ | POST, GET, PUT, DELETE endpoints |
| 2 - Paginação | ✅ | Page, PageSize, TotalPages, TotalItems |
| 3 - Filtro Período | ✅ | StartDate, EndDate com defaults |
| 4 - Filtro Categoria | ✅ | CategoryIds múltiplas |
| 5 - Cálculo Saldo | ✅ | Σ(receitas) - Σ(despesas) |
| 7 - Edição | ✅ | PUT endpoint com validações |
| 8 - Exclusão | ✅ | DELETE endpoint |
| 16 - Ícones | ✅ | Categoria com icon e cor |

---

## 🚀 Próximos Passos

1. **Testes Unitários** (Task 2.6, 2.8, 2.10, 2.12, 2.14)
2. **Property-Based Testing** (Task 2.3)
3. **Dashboard** (Phase 3)
4. **Relatórios** (Phase 4)
5. **Goals & Limits** (Phase 5)

---

**Versão:** 1.0  
**Data:** 28/07/2026  
**Status:** ✅ Concluído e Testado
