import { apiService } from './api'
import {
  Transaction,
  CreateTransactionRequest,
  UpdateTransactionRequest,
  TransactionFilters
} from '../types'

export class TransactionService {
  private readonly endpoint = '/api/transactions'

  async getAll(userId: string, filters?: TransactionFilters): Promise<Transaction[]> {
    const params = {
      userId,
      ...filters
    }
    return apiService.get<Transaction[]>(this.endpoint, params)
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