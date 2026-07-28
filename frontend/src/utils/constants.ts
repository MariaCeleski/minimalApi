// Application constants

export const API_ENDPOINTS = {
  TRANSACTIONS: '/api/transactions',
  CATEGORIES: '/api/categories', 
  GOALS: '/api/goals',
  LIMITS: '/api/limits',
  DASHBOARD: '/api/dashboard',
  REPORTS: '/api/reports',
} as const

export const TRANSACTION_TYPES = {
  INCOME: 0,
  EXPENSE: 1,
} as const

export const GOAL_STATUS = {
  ACTIVE: 0,
  COMPLETED: 1,
  CANCELLED: 2,
} as const

export const LIMIT_PERIODS = {
  DAILY: 0,
  WEEKLY: 1,
  MONTHLY: 2,
} as const

export const DEFAULT_CATEGORY_COLORS = [
  '#3B82F6', '#EF4444', '#10B981', '#F59E0B',
  '#8B5CF6', '#EC4899', '#06B6D4', '#84CC16'
] as const

export const CATEGORY_ICONS = {
  FOOD: '🍽️',
  TRANSPORT: '🚗',
  ENTERTAINMENT: '🎬',
  SHOPPING: '🛍️',
  HEALTH: '🏥',
  EDUCATION: '📚',
  SALARY: '💰',
  FREELANCE: '💼',
  INVESTMENT: '📈',
  OTHER: '📋',
} as const