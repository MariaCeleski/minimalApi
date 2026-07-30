import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import ReportsPage from './ReportsPage'

// Mock the reportService module
vi.mock('../services/reportService', () => ({
  reportService: {
    getDefaultDateRange: vi.fn(() => ({
      startDate: '2024-11-09',
      endDate: '2024-12-09',
    })),
    getMonthlyReport: vi.fn(),
    getCategoryReport: vi.fn(),
  },
}))

describe('ReportsPage Component', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should render the ReportsPage component with title', () => {
    render(<ReportsPage />)
    
    expect(screen.getByText('Relatórios Financeiros')).toBeInTheDocument()
    expect(screen.getByText(/Visualize relatórios detalhados/)).toBeInTheDocument()
  })

  it('should render report type selector buttons', () => {
    render(<ReportsPage />)
    
    expect(screen.getByRole('button', { name: /Relatório Mensal/i })).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /Relatório por Categoria/i })).toBeInTheDocument()
  })

  it('should show monthly report filters when monthly tab is active', () => {
    render(<ReportsPage />)
    
    // Monthly tab should be selected by default
    const monthlyButton = screen.getByRole('button', { name: /Relatório Mensal/i })
    expect(monthlyButton).toHaveClass('bg-blue-600')
    
    // Check for year and month inputs
    expect(screen.getByLabelText(/Ano/i)).toBeInTheDocument()
    expect(screen.getByLabelText(/Mês/i)).toBeInTheDocument()
  })

  it('should switch to category report when category tab is clicked', async () => {
    const user = userEvent.setup()
    render(<ReportsPage />)
    
    const categoryButton = screen.getByRole('button', { name: /Relatório por Categoria/i })
    await user.click(categoryButton)
    
    // Check for date inputs
    expect(screen.getByLabelText(/Data Inicial/i)).toBeInTheDocument()
    expect(screen.getByLabelText(/Data Final/i)).toBeInTheDocument()
  })

  it('should initialize category report with default date range', async () => {
    render(<ReportsPage />)
    
    const user = userEvent.setup()
    const categoryButton = screen.getByRole('button', { name: /Relatório por Categoria/i })
    await user.click(categoryButton)
    
    // Check that date inputs have default values
    const startDateInput = screen.getByLabelText(/Data Inicial/i) as HTMLInputElement
    const endDateInput = screen.getByLabelText(/Data Final/i) as HTMLInputElement
    
    expect(startDateInput.value).toBe('2024-11-09')
    expect(endDateInput.value).toBe('2024-12-09')
  })

describe('ReportsPage Component', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should render the ReportsPage component with title', () => {
    render(<ReportsPage />)
    
    expect(screen.getByText('Relatórios Financeiros')).toBeInTheDocument()
    expect(screen.getByText(/Visualize relatórios detalhados/)).toBeInTheDocument()
  })

  it('should render report type selector buttons', () => {
    render(<ReportsPage />)
    
    expect(screen.getByRole('button', { name: /Relatório Mensal/i })).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /Relatório por Categoria/i })).toBeInTheDocument()
  })

  it('should show monthly report filters when monthly tab is active', () => {
    render(<ReportsPage />)
    
    // Monthly tab should be selected by default
    const monthlyButton = screen.getByRole('button', { name: /Relatório Mensal/i })
    expect(monthlyButton).toHaveClass('bg-blue-600')
    
    // Check for year and month inputs
    expect(screen.getByLabelText(/Ano/i)).toBeInTheDocument()
    expect(screen.getByLabelText(/Mês/i)).toBeInTheDocument()
  })

  it('should switch to category report when category tab is clicked', async () => {
    const user = userEvent.setup()
    render(<ReportsPage />)
    
    const categoryButton = screen.getByRole('button', { name: /Relatório por Categoria/i })
    await user.click(categoryButton)
    
    // Check for date inputs
    expect(screen.getByLabelText(/Data Inicial/i)).toBeInTheDocument()
    expect(screen.getByLabelText(/Data Final/i)).toBeInTheDocument()
  })

  it('should initialize category report with default date range', async () => {
    render(<ReportsPage />)
    
    const user = userEvent.setup()
    const categoryButton = screen.getByRole('button', { name: /Relatório por Categoria/i })
    await user.click(categoryButton)
    
    // Check that date inputs have default values
    const startDateInput = screen.getByLabelText(/Data Inicial/i) as HTMLInputElement
    const endDateInput = screen.getByLabelText(/Data Final/i) as HTMLInputElement
    
    expect(startDateInput.value).toBe('2024-11-09')
    expect(endDateInput.value).toBe('2024-12-09')
  })

  it('should display loading state when generating report', async () => {
    const { reportService } = await import('../services/reportService')
    vi.mocked(reportService.getMonthlyReport).mockImplementation(
      () => new Promise(() => {}) // Never resolves
    )
    
    const user = userEvent.setup()
    render(<ReportsPage />)
    
    const generateButton = screen.getByRole('button', { name: /Gerar Relatório/i })
    await user.click(generateButton)
    
    // Look for loading indicator (animated spinner text)
    await waitFor(() => {
      expect(screen.getByText(/Carregando/i)).toBeInTheDocument()
    })
  })

  it('should display error message if report generation fails', async () => {
    const { reportService } = await import('../services/reportService')
    vi.mocked(reportService.getMonthlyReport).mockRejectedValue({
      statusCode: 400,
      message: 'Erro ao buscar relatório',
      details: 'Mês inválido',
      timestamp: new Date().toISOString(),
    })
    
    const user = userEvent.setup()
    render(<ReportsPage />)
    
    const generateButton = screen.getByRole('button', { name: /Gerar Relatório/i })
    await user.click(generateButton)
    
    await waitFor(() => {
      expect(screen.getByText('Erro ao buscar relatório')).toBeInTheDocument()
    })
  })

  it('should format currency correctly', async () => {
    const { reportService } = await import('../services/reportService')
    vi.mocked(reportService.getMonthlyReport).mockResolvedValue({
      year: 2024,
      month: 12,
      monthName: 'Dezembro',
      totalIncome: 5000.50,
      totalExpenses: 3000.25,
      balance: 2000.25,
      transactionCount: 15,
      categories: [
        {
          categoryId: 1,
          categoryName: 'Test',
          amount: 1000,
          percentage: 50,
          transactionCount: 5,
        },
      ],
    })
    
    const user = userEvent.setup()
    render(<ReportsPage />)
    
    const generateButton = screen.getByRole('button', { name: /Gerar Relatório/i })
    await user.click(generateButton)
    
    await waitFor(() => {
      // Look for formatted currency (R$ format in pt-BR)
      expect(screen.getByText(/5\.000/)).toBeInTheDocument()
    })
  })

  it('should display summary cards after report loads', async () => {
    const { reportService } = await import('../services/reportService')
    vi.mocked(reportService.getMonthlyReport).mockResolvedValue({
      year: 2024,
      month: 12,
      monthName: 'Dezembro',
      totalIncome: 5000,
      totalExpenses: 3000,
      balance: 2000,
      transactionCount: 10,
      categories: [
        {
          categoryId: 1,
          categoryName: 'Alimentação',
          categoryIcon: undefined,
          categoryColor: '#FF6B6B',
          amount: 1500,
          percentage: 50,
          transactionCount: 5,
        },
      ],
    })
    
    const user = userEvent.setup()
    render(<ReportsPage />)
    
    const generateButton = screen.getByRole('button', { name: /Gerar Relatório/i })
    await user.click(generateButton)
    
    await waitFor(() => {
      expect(screen.getByText(/Total de Receitas/i)).toBeInTheDocument()
      expect(screen.getByText(/Total de Despesas/i)).toBeInTheDocument()
      expect(screen.getByText(/Saldo Líquido/i)).toBeInTheDocument()
    })
  })

  it('should display category breakdown table', async () => {
    const { reportService } = await import('../services/reportService')
    vi.mocked(reportService.getMonthlyReport).mockResolvedValue({
      year: 2024,
      month: 12,
      monthName: 'Dezembro',
      totalIncome: 5000,
      totalExpenses: 3000,
      balance: 2000,
      transactionCount: 10,
      categories: [
        {
          categoryId: 1,
          categoryName: 'Alimentação',
          categoryIcon: undefined,
          categoryColor: '#FF6B6B',
          amount: 1500,
          percentage: 50,
          transactionCount: 5,
        },
      ],
    })
    
    const user = userEvent.setup()
    render(<ReportsPage />)
    
    const generateButton = screen.getByRole('button', { name: /Gerar Relatório/i })
    await user.click(generateButton)
    
    await waitFor(() => {
      expect(screen.getByText('Alimentação')).toBeInTheDocument()
      expect(screen.getByText(/50\.0%/)).toBeInTheDocument()
    })
  })

  it('should display no transactions message when list is empty', async () => {
    const { reportService } = await import('../services/reportService')
    vi.mocked(reportService.getMonthlyReport).mockResolvedValue({
      year: 2024,
      month: 12,
      monthName: 'Dezembro',
      totalIncome: 0,
      totalExpenses: 0,
      balance: 0,
      transactionCount: 0,
      categories: [],
    })
    
    const user = userEvent.setup()
    render(<ReportsPage />)
    
    const generateButton = screen.getByRole('button', { name: /Gerar Relatório/i })
    await user.click(generateButton)
    
    await waitFor(() => {
      expect(screen.getByText(/Nenhuma transação registrada/i)).toBeInTheDocument()
    })
  })
})
})
