import axios, { AxiosError } from 'axios'
import { ErrorResponse } from '../types'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5209/api'

/**
 * Monthly Report Response DTO
 * Task 4.3: Response from GET /reports/monthly endpoint
 */
export interface MonthlyReportResponse {
  year: number
  month: number
  monthName: string
  totalIncome: number
  totalExpenses: number
  balance: number
  transactionCount: number
  categories: CategoryBreakdown[]
}

/**
 * Category Breakdown in Monthly Report
 */
export interface CategoryBreakdown {
  categoryId: number
  categoryName: string
  categoryIcon?: string
  categoryColor?: string
  amount: number
  percentage: number
  transactionCount: number
}

/**
 * Category Report Response DTO
 * Task 4.6: Response from GET /reports/category endpoint
 */
export interface CategoryReportResponse {
  periodLabel: string
  startDate: string
  endDate: string
  totalIncome: number
  totalExpenses: number
  netBalance: number
  totalTransactionCount: number
  categories: CategoryReportBreakdown[]
}

/**
 * Category Breakdown in Category Report
 */
export interface CategoryReportBreakdown {
  categoryId: number
  categoryName: string
  categoryIcon?: string
  categoryColor?: string
  incomeAmount: number
  expenseAmount: number
  netAmount: number
  percentage: number
  transactionCount: number
}

/**
 * Report Service
 * Provides methods to fetch reports from backend API
 * Task 4.3: GET /reports/monthly endpoint
 * Task 4.6: GET /reports/category endpoint
 */
class ReportService {
  /**
   * Get monthly report for a specific year and month
   * Task 4.3: Create GET /reports/monthly endpoint
   * Requirement 9: Relatório Mensal
   * 
   * @param year - Year (e.g., 2024)
   * @param month - Month (1-12)
   * @returns Monthly report data
   */
  async getMonthlyReport(year: number, month: number): Promise<MonthlyReportResponse> {
    try {
      const response = await axios.get<MonthlyReportResponse>(
        `${API_BASE_URL}/reports/monthly`,
        {
          params: {
            year,
            month,
          },
        }
      )
      return response.data
    } catch (error) {
      throw this.handleError(error)
    }
  }

  /**
   * Get category report for a date range
   * Task 4.6: Create GET /reports/category endpoint
   * Requirement 10: Relatório por Categoria
   * 
   * @param startDate - Start date (ISO 8601 format: YYYY-MM-DD)
   * @param endDate - End date (ISO 8601 format: YYYY-MM-DD)
   * @returns Category report data
   */
  async getCategoryReport(startDate: string, endDate: string): Promise<CategoryReportResponse> {
    try {
      const response = await axios.get<CategoryReportResponse>(
        `${API_BASE_URL}/reports/category`,
        {
          params: {
            startDate,
            endDate,
          },
        }
      )
      return response.data
    } catch (error) {
      throw this.handleError(error)
    }
  }

  /**
   * Get default date range (last 30 days)
   * Used for category report filter initialization
   * 
   * @returns Object with startDate and endDate in ISO format
   */
  getDefaultDateRange(): { startDate: string; endDate: string } {
    const endDate = new Date()
    const startDate = new Date()
    startDate.setDate(startDate.getDate() - 30)

    return {
      startDate: this.formatDateToISO(startDate),
      endDate: this.formatDateToISO(endDate),
    }
  }

  /**
   * Format date to ISO 8601 format (YYYY-MM-DD)
   * 
   * @param date - Date to format
   * @returns Formatted date string
   */
  private formatDateToISO(date: Date): string {
    const year = date.getFullYear()
    const month = String(date.getMonth() + 1).padStart(2, '0')
    const day = String(date.getDate()).padStart(2, '0')
    return `${year}-${month}-${day}`
  }

  /**
   * Handle API errors
   * 
   * @param error - Error from axios
   * @returns Formatted error response
   */
  private handleError(error: unknown): ErrorResponse {
    if (axios.isAxiosError(error)) {
      const axiosError = error as AxiosError<ErrorResponse>
      
      if (axiosError.response?.data) {
        return axiosError.response.data
      }

      return {
        statusCode: axiosError.response?.status || 500,
        message: axiosError.message || 'Erro ao buscar relatório',
        details: axiosError.response?.statusText,
        timestamp: new Date().toISOString(),
      }
    }

    return {
      statusCode: 500,
      message: 'Erro desconhecido ao buscar relatório',
      details: String(error),
      timestamp: new Date().toISOString(),
    }
  }
}

export const reportService = new ReportService()
