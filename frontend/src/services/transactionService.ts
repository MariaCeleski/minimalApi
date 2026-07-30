import { apiService } from './api'
import {
  Transaction,
  CreateTransactionRequest,
  UpdateTransactionRequest,
  TransactionFilters
} from '../types'

// Pagination Response Types
export interface PaginatedResponse<T> {
  data: T[]
  currentPage: number
  totalPages: number
  totalItems: number
  pageSize: number
}

export interface PaginationParams {
  page?: number
  pageSize?: number
}

export class TransactionService {
  private readonly endpoint = '/api/transactions'

  async getAll(userId: string, filters?: TransactionFilters): Promise<Transaction[]> {
    const params = {
      userId,
      ...filters
    }
    return apiService.get<Transaction[]>(this.endpoint, params)
  }

  async getPaginated(
    userId: string, 
    pagination?: PaginationParams,
    filters?: TransactionFilters
  ): Promise<PaginatedResponse<Transaction>> {
    const params = {
      userId,
      page: pagination?.page || 1,
      pageSize: pagination?.pageSize || 10,
      ...filters
    }
    return apiService.get<PaginatedResponse<Transaction>>(this.endpoint, params)
  }

  async getById(id: string): Promise<Transaction> {
    return apiService.get<Transaction>(`${this.endpoint}/${id}`)
  }

  async create(userId: string, data: CreateTransactionRequest): Promise<Transaction> {
    return apiService.post<Transaction>(this.endpoint, { userId, ...data })
  }

  async update(id: string, data: UpdateTransactionRequest): Promise<Transaction> {
    return apiService.put<Transaction>(`${this.endpoint}/${id}`, data)
  }

  async delete(id: string): Promise<void> {
    return apiService.delete<void>(`${this.endpoint}/${id}`)
  }

  async getBalance(userId: string): Promise<number> {
    const response = await apiService.get<{ currentBalance: number }>(
      '/api/dashboard/balance',
      { userId }
    )
    return response.currentBalance
  }
}

export const transactionService = new TransactionService()