// Transaction Types
export enum TransactionType {
  Income = 0,
  Expense = 1
}

export interface Transaction {
  id: string;
  userId: string;
  categoryId: string;
  amount: number;
  description?: string;
  type: TransactionType;
  date: string;
  createdAt: string;
  updatedAt: string;
  category?: Category;
}

export interface CreateTransactionRequest {
  categoryId: string;
  amount: number;
  description?: string;
  date: string;
}

export interface UpdateTransactionRequest {
  categoryId: string;
  amount: number;
  description?: string;
  date: string;
}

// Category Types
export interface Category {
  id: string;
  userId: string;
  name: string;
  icon?: string;
  color?: string;
  type: TransactionType;
  createdAt: string;
  updatedAt: string;
}

export interface CreateCategoryRequest {
  name: string;
  icon?: string;
  color?: string;
  type: TransactionType;
}

// Dashboard Types
export interface DashboardData {
  totalIncome: number;
  totalExpenses: number;
  currentBalance: number;
  expensesByCategory: CategoryBreakdown[];
  monthlyTrends: MonthlyTrend[];
  activeGoals: GoalProgress[];
  limitAlerts: LimitAlert[];
}

export interface CategoryBreakdown {
  categoryName: string;
  icon?: string;
  color?: string;
  amount: number;
  percentage: number;
}

export interface MonthlyTrend {
  month: string;
  income: number;
  expenses: number;
  balance: number;
}

// Goal Types
export enum GoalStatus {
  Active = 0,
  Completed = 1,
  Cancelled = 2
}

export interface Goal {
  id: string;
  userId: string;
  name: string;
  targetAmount: number;
  currentAmount: number;
  deadline?: string;
  status: GoalStatus;
  createdAt: string;
  updatedAt: string;
}

export interface GoalProgress {
  id: string;
  name: string;
  targetAmount: number;
  currentAmount: number;
  progressPercentage: number;
  deadline?: string;
}

export interface CreateGoalRequest {
  name: string;
  targetAmount: number;
  deadline?: string;
}

// Limit Types
export enum LimitPeriod {
  Daily = 0,
  Weekly = 1,
  Monthly = 2
}

export interface TransactionLimit {
  id: string;
  userId: string;
  categoryId?: string;
  limitAmount: number;
  period: LimitPeriod;
  alertThreshold: number;
  createdAt: string;
  updatedAt: string;
}

export interface LimitAlert {
  id: string;
  categoryName: string;
  limitAmount: number;
  currentSpending: number;
  percentageUsed: number;
  isExceeded: boolean;
}

export interface CreateLimitRequest {
  categoryId?: string;
  limitAmount: number;
  period: LimitPeriod;
  alertThreshold: number;
}

// Report Types
export interface ReportData {
  startDate: string;
  endDate: string;
  totalIncome: number;
  totalExpenses: number;
  balance: number;
  categoryBreakdown: CategoryReport[];
  transactions: Transaction[];
}

export interface CategoryReport {
  categoryName: string;
  amount: number;
  percentage: number;
  transactionCount: number;
  averageTransaction: number;
}

// API Response Types
export interface ApiResponse<T> {
  data: T;
  success: boolean;
  message?: string;
}

export interface ErrorResponse {
  statusCode: number;
  message: string;
  details?: string;
  timestamp: string;
}

// Filter Types
export interface TransactionFilters {
  startDate?: string;
  endDate?: string;
  categoryId?: string;
  type?: TransactionType;
}

// Pagination Types
export interface PaginatedResponse<T> {
  data: T[];
  currentPage: number;
  totalPages: number;
  totalItems: number;
  pageSize: number;
}

export interface PaginationParams {
  page?: number;
  pageSize?: number;
}

// Theme Types
export type ThemeMode = 'light' | 'dark';

export interface ThemeContextType {
  mode: ThemeMode;
  toggleTheme: () => void;
}