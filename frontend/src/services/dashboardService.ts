import axios from 'axios'
import { ErrorResponse } from '../types'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5209/api'

export interface MonthlyTrendData {
  month: string
  income: number
  expenses: number
  balance: number
}

export interface DashboardData {
  totalIncome: number
  totalExpenses: number
  currentBalance: number
  monthlyTrends: MonthlyTrendData[]
}

class DashboardService {
  async getDashboardData(): Promise<DashboardData> {
    try {
      const response = await axios.get<DashboardData>(`${API_BASE_URL}/dashboard`)
      return response.data
    } catch (error) {
      throw this.handleError(error)
    }
  }

  async getMonthlyTrend(monthsBack: number = 12): Promise<MonthlyTrendData[]> {
    try {
      const response = await axios.get<{ [key: string]: { income: number; expenses: number; balance: number } }>(
        `${API_BASE_URL}/dashboard/monthly-trend`,
        {
          params: { monthsBack }
        }
      )
      
      // Transform the response into MonthlyTrendData array
      return Object.entries(response.data).map(([month, data]) => ({
        month,
        ...data
      }))
    } catch (error) {
      throw this.handleError(error)
    }
  }

  private handleError(error: unknown): ErrorResponse {
    if (axios.isAxiosError(error)) {
      const response = error.response?.data
      if (response) {
        return response as ErrorResponse
      }
      return {
        statusCode: error.response?.status || 500,
        message: error.message || 'Erro ao buscar dados do dashboard',
        details: error.response?.statusText,
        timestamp: new Date().toISOString(),
      }
    }

    return {
      statusCode: 500,
      message: 'Erro desconhecido',
      details: String(error),
      timestamp: new Date().toISOString(),
    }
  }
}

export const dashboardService = new DashboardService()
