import axios, { AxiosInstance, AxiosResponse } from 'axios'
import { ApiResponse, ErrorResponse } from '../types'

class ApiService {
  private client: AxiosInstance

  constructor() {
    this.client = axios.create({
      baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000',
      timeout: 10000,
      headers: {
        'Content-Type': 'application/json',
      },
    })

    this.setupInterceptors()
  }

  private setupInterceptors() {
    // Request interceptor
    this.client.interceptors.request.use(
      (config) => {
        // Add auth token if available
        const token = localStorage.getItem('auth-token')
        if (token) {
          config.headers.Authorization = `Bearer ${token}`
        }
        return config
      },
      (error) => Promise.reject(error)
    )

    // Response interceptor
    this.client.interceptors.response.use(
      (response: AxiosResponse) => response,
      (error) => {
        if (error.response) {
          const errorResponse: ErrorResponse = {
            statusCode: error.response.status,
            message: error.response.data?.message || 'Erro na requisição',
            details: error.response.data?.details,
            timestamp: new Date().toISOString(),
          }
          return Promise.reject(errorResponse)
        }
        
        return Promise.reject({
          statusCode: 0,
          message: 'Erro de conexão com o servidor',
          timestamp: new Date().toISOString(),
        } as ErrorResponse)
      }
    )
  }

  async get<T>(url: string, params?: object): Promise<T> {
    const response = await this.client.get<ApiResponse<T>>(url, { params })
    return response.data.data
  }

  async post<T>(url: string, data?: object): Promise<T> {
    const response = await this.client.post<ApiResponse<T>>(url, data)
    return response.data.data
  }

  async put<T>(url: string, data?: object): Promise<T> {
    const response = await this.client.put<ApiResponse<T>>(url, data)
    return response.data.data
  }

  async delete<T>(url: string): Promise<T> {
    const response = await this.client.delete<ApiResponse<T>>(url)
    return response.data.data
  }

  // File download methods
  async downloadFile(url: string, params?: object): Promise<Blob> {
    const response = await this.client.get(url, {
      params,
      responseType: 'blob',
    })
    return response.data
  }
}

export const apiService = new ApiService()
export default apiService