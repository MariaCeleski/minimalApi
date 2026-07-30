import React, { useState, useEffect } from 'react'
import { motion } from 'framer-motion'
import { reportService, MonthlyReportResponse, CategoryReportResponse } from '@/services/reportService'
import { ErrorResponse } from '@/types'

const ReportsPage: React.FC = () => {
  // Report type selection
  const [reportType, setReportType] = useState<'monthly' | 'category'>('monthly')

  // Monthly report state
  const [selectedYear, setSelectedYear] = useState<number>(new Date().getFullYear())
  const [selectedMonth, setSelectedMonth] = useState<number>(new Date().getMonth() + 1)
  const [monthlyReport, setMonthlyReport] = useState<MonthlyReportResponse | null>(null)

  // Category report state
  const [categoryStartDate, setCategoryStartDate] = useState<string>('')
  const [categoryEndDate, setCategoryEndDate] = useState<string>('')
  const [categoryReport, setCategoryReport] = useState<CategoryReportResponse | null>(null)

  // UI state
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<ErrorResponse | null>(null)

  // Initialize default dates for category report
  useEffect(() => {
    const { startDate, endDate } = reportService.getDefaultDateRange()
    setCategoryStartDate(startDate)
    setCategoryEndDate(endDate)
  }, [])

  /**
   * Load monthly report data
   * Requirements: 9 (Relatório Mensal)
   */
  const handleLoadMonthlyReport = async () => {
    setIsLoading(true)
    setError(null)

    try {
      // Validate month and year
      if (selectedMonth < 1 || selectedMonth > 12) {
        setError({
          statusCode: 400,
          message: 'Mês inválido',
          details: 'O mês deve estar entre 1 e 12',
          timestamp: new Date().toISOString(),
        })
        setIsLoading(false)
        return
      }

      if (selectedYear < 1900 || selectedYear > new Date().getFullYear() + 1) {
        setError({
          statusCode: 400,
          message: 'Ano inválido',
          details: 'O ano deve ser válido',
          timestamp: new Date().toISOString(),
        })
        setIsLoading(false)
        return
      }

      const report = await reportService.getMonthlyReport(selectedYear, selectedMonth)
      setMonthlyReport(report)
    } catch (err) {
      setError(err as ErrorResponse)
      setMonthlyReport(null)
    } finally {
      setIsLoading(false)
    }
  }

  /**
   * Load category report data
   * Requirements: 10 (Relatório por Categoria)
   */
  const handleLoadCategoryReport = async () => {
    setIsLoading(true)
    setError(null)

    try {
      // Validate dates
      if (!categoryStartDate || !categoryEndDate) {
        setError({
          statusCode: 400,
          message: 'Datas inválidas',
          details: 'Ambas as datas devem ser preenchidas',
          timestamp: new Date().toISOString(),
        })
        setIsLoading(false)
        return
      }

      if (new Date(categoryStartDate) > new Date(categoryEndDate)) {
        setError({
          statusCode: 400,
          message: 'Intervalo inválido',
          details: 'A data inicial não pode ser posterior à data final',
          timestamp: new Date().toISOString(),
        })
        setIsLoading(false)
        return
      }

      const report = await reportService.getCategoryReport(categoryStartDate, categoryEndDate)
      setCategoryReport(report)
    } catch (err) {
      setError(err as ErrorResponse)
      setCategoryReport(null)
    } finally {
      setIsLoading(false)
    }
  }

  /**
   * Handle report type change
   */
  const handleReportTypeChange = (type: 'monthly' | 'category') => {
    setReportType(type)
    setError(null)
    setMonthlyReport(null)
    setCategoryReport(null)
  }

  /**
   * Format currency with 2 decimal places
   * Requirements: 4 (Formatação - valores monetários)
   */
  const formatCurrency = (value: number): string => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    }).format(value)
  }

  /**
   * Format percentage with 1 decimal place
   * Requirements: 4 (Formatação - percentuais)
   */
  const formatPercentage = (value: number): string => {
    return `${value.toFixed(1)}%`
  }

  /**
   * Get month name
   */
  const getMonthName = (month: number): string => {
    const months = [
      'Janeiro',
      'Fevereiro',
      'Março',
      'Abril',
      'Maio',
      'Junho',
      'Julho',
      'Agosto',
      'Setembro',
      'Outubro',
      'Novembro',
      'Dezembro',
    ]
    return months[month - 1] || ''
  }

  return (
    <div className="min-h-screen bg-white dark:bg-gray-900">
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
      >
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-4xl font-bold text-gray-900 dark:text-white mb-2">
            Relatórios Financeiros
          </h1>
          <p className="text-lg text-gray-600 dark:text-gray-400">
            Visualize relatórios detalhados das suas transações e finanças. Escolha entre
            relatórios mensais ou por categoria.
          </p>
        </div>

        {/* Report Type Selection - Task 4.7 Requirement 1 */}
        <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6 mb-6">
          <h2 className="text-xl font-semibold text-gray-900 dark:text-white mb-4">
            Tipo de Relatório
          </h2>
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <button
              onClick={() => handleReportTypeChange('monthly')}
              className={`py-3 px-4 rounded-lg font-semibold transition-colors ${
                reportType === 'monthly'
                  ? 'bg-blue-600 text-white'
                  : 'bg-gray-200 dark:bg-gray-700 text-gray-900 dark:text-white hover:bg-gray-300 dark:hover:bg-gray-600'
              }`}
            >
              Relatório Mensal
            </button>
            <button
              onClick={() => handleReportTypeChange('category')}
              className={`py-3 px-4 rounded-lg font-semibold transition-colors ${
                reportType === 'category'
                  ? 'bg-blue-600 text-white'
                  : 'bg-gray-200 dark:bg-gray-700 text-gray-900 dark:text-white hover:bg-gray-300 dark:hover:bg-gray-600'
              }`}
            >
              Relatório por Categoria
            </button>
          </div>
        </div>

        {/* Error Alert - Task 4.7 Requirement 4 */}
        {error && (
          <motion.div
            initial={{ opacity: 0, y: -10 }}
            animate={{ opacity: 1, y: 0 }}
            className="mb-6 p-4 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-700 rounded-lg"
          >
            <div className="flex justify-between items-start">
              <div>
                <h3 className="font-bold text-red-800 dark:text-red-200 mb-1">
                  {error.message}
                </h3>
                {error.details && (
                  <p className="text-sm text-red-700 dark:text-red-300">
                    {error.details}
                  </p>
                )}
              </div>
              <button
                onClick={() => setError(null)}
                className="text-red-600 dark:text-red-400 hover:text-red-800 dark:hover:text-red-200"
              >
                ✕
              </button>
            </div>
          </motion.div>
        )}

        {/* Monthly Report Section */}
        {reportType === 'monthly' && (
          <motion.div
            key="monthly-section"
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.3 }}
          >
            {/* Filters Card */}
            <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6 mb-6">
              <h2 className="text-xl font-semibold text-gray-900 dark:text-white mb-4">
                Filtros
              </h2>
              <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
                {/* Year Input */}
                <div>
                  <label htmlFor="year-input" className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                    Ano
                  </label>
                  <input
                    id="year-input"
                    type="number"
                    value={selectedYear}
                    onChange={(e) => setSelectedYear(parseInt(e.target.value))}
                    min="1900"
                    max={new Date().getFullYear() + 1}
                    className="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500 outline-none"
                  />
                </div>

                {/* Month Select */}
                <div>
                  <label htmlFor="month-input" className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                    Mês
                  </label>
                  <select
                    id="month-input"
                    value={selectedMonth}
                    onChange={(e) => setSelectedMonth(parseInt(e.target.value))}
                    className="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500 outline-none"
                  >
                    {Array.from({ length: 12 }, (_, i) => i + 1).map((month) => (
                      <option key={month} value={month}>
                        {getMonthName(month)}
                      </option>
                    ))}
                  </select>
                </div>

                {/* Generate Button */}
                <div className="flex items-end">
                  <button
                    onClick={handleLoadMonthlyReport}
                    disabled={isLoading}
                    className="w-full px-4 py-2 bg-blue-600 hover:bg-blue-700 disabled:bg-gray-400 text-white font-medium rounded-lg transition-colors"
                  >
                    {isLoading ? (
                      <span className="flex items-center justify-center">
                        <svg className="animate-spin h-5 w-5 mr-2" viewBox="0 0 24 24">
                          <circle
                            className="opacity-25"
                            cx="12"
                            cy="12"
                            r="10"
                            stroke="currentColor"
                            strokeWidth="4"
                            fill="none"
                          />
                          <path
                            className="opacity-75"
                            fill="currentColor"
                            d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                          />
                        </svg>
                        Carregando...
                      </span>
                    ) : (
                      'Gerar Relatório'
                    )}
                  </button>
                </div>
              </div>
            </div>

            {/* Monthly Report Data */}
            {monthlyReport && (
              <motion.div
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.3 }}
              >
                {/* Summary Cards - Task 4.7 Requirement 2 */}
                <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
                  {/* Total Receitas */}
                  <motion.div
                    initial={{ opacity: 0, y: 20 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ duration: 0.5, delay: 0 }}
                  >
                    <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6 border-l-4 border-green-500">
                      <p className="text-sm font-medium text-gray-600 dark:text-gray-400 mb-2">
                        Total de Receitas
                      </p>
                      <p className="text-3xl font-bold text-green-600 dark:text-green-400">
                        {formatCurrency(monthlyReport.totalIncome)}
                      </p>
                    </div>
                  </motion.div>

                  {/* Total Despesas */}
                  <motion.div
                    initial={{ opacity: 0, y: 20 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ duration: 0.5, delay: 0.1 }}
                  >
                    <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6 border-l-4 border-red-500">
                      <p className="text-sm font-medium text-gray-600 dark:text-gray-400 mb-2">
                        Total de Despesas
                      </p>
                      <p className="text-3xl font-bold text-red-600 dark:text-red-400">
                        {formatCurrency(monthlyReport.totalExpenses)}
                      </p>
                    </div>
                  </motion.div>

                  {/* Saldo Líquido */}
                  <motion.div
                    initial={{ opacity: 0, y: 20 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ duration: 0.5, delay: 0.2 }}
                  >
                    <div
                      className={`rounded-lg shadow-md p-6 border-l-4 ${
                        monthlyReport.balance >= 0
                          ? 'bg-white dark:bg-gray-800 border-blue-500'
                          : 'bg-white dark:bg-gray-800 border-yellow-500'
                      }`}
                    >
                      <p className="text-sm font-medium text-gray-600 dark:text-gray-400 mb-2">
                        Saldo Líquido
                      </p>
                      <p
                        className={`text-3xl font-bold ${
                          monthlyReport.balance >= 0
                            ? 'text-blue-600 dark:text-blue-400'
                            : 'text-yellow-600 dark:text-yellow-400'
                        }`}
                      >
                        {formatCurrency(monthlyReport.balance)}
                      </p>
                    </div>
                  </motion.div>
                </div>

                {/* Report Details Card */}
                <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
                  <div className="flex justify-between items-center mb-4">
                    <h3 className="text-xl font-semibold text-gray-900 dark:text-white">
                      {monthlyReport.monthName} - Relatório Detalhado
                    </h3>
                  </div>

                  <div className="border-t border-gray-200 dark:border-gray-700 pt-4 mb-4">
                    <p className="text-sm text-gray-600 dark:text-gray-400">
                      Total de {monthlyReport.transactionCount} transação{monthlyReport.transactionCount !== 1 ? 's' : ''} registrada
                      {monthlyReport.transactionCount !== 1 ? 's' : ''} neste período.
                    </p>
                  </div>

                  {/* Breakdown Table - Task 4.7 Requirement 2 */}
                  <h4 className="text-lg font-semibold text-gray-900 dark:text-white mb-4 mt-6">
                    Breakdown por Categoria
                  </h4>

                  <div className="overflow-x-auto">
                    <table className="w-full text-sm">
                      <thead>
                        <tr className="border-b-2 border-gray-300 dark:border-gray-600">
                          <th className="text-left px-4 py-3 font-semibold text-gray-900 dark:text-white">
                            Categoria
                          </th>
                          <th className="text-right px-4 py-3 font-semibold text-gray-900 dark:text-white">
                            Valor
                          </th>
                          <th className="text-right px-4 py-3 font-semibold text-gray-900 dark:text-white">
                            Percentual
                          </th>
                          <th className="text-right px-4 py-3 font-semibold text-gray-900 dark:text-white">
                            Transações
                          </th>
                        </tr>
                      </thead>
                      <tbody>
                        {monthlyReport.categories.length > 0 ? (
                          monthlyReport.categories.map((category: any) => (
                            <tr
                              key={category.categoryId}
                              className="border-b border-gray-200 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-700/50 transition-colors"
                            >
                              <td className="px-4 py-3">
                                <div className="flex items-center gap-2">
                                  {category.categoryIcon && (
                                    <div
                                      className="w-6 h-6 rounded-full"
                                      style={{
                                        backgroundColor: category.categoryColor || '#ccc',
                                      }}
                                    />
                                  )}
                                  <span className="text-gray-900 dark:text-white">
                                    {category.categoryName}
                                  </span>
                                </div>
                              </td>
                              <td className="px-4 py-3 text-right font-semibold text-gray-900 dark:text-white">
                                {formatCurrency(category.amount)}
                              </td>
                              <td className="px-4 py-3 text-right text-gray-600 dark:text-gray-400">
                                {formatPercentage(category.percentage)}
                              </td>
                              <td className="px-4 py-3 text-right text-gray-600 dark:text-gray-400">
                                {category.transactionCount}
                              </td>
                            </tr>
                          ))
                        ) : (
                          <tr>
                            <td colSpan={4} className="px-4 py-6 text-center text-gray-600 dark:text-gray-400">
                              Nenhuma transação registrada neste período
                            </td>
                          </tr>
                        )}
                      </tbody>
                    </table>
                  </div>
                </div>
              </motion.div>
            )}
          </motion.div>
        )}

        {/* Category Report Section */}
        {reportType === 'category' && (
          <motion.div
            key="category-section"
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.3 }}
          >
            {/* Filters Card */}
            <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6 mb-6">
              <h2 className="text-xl font-semibold text-gray-900 dark:text-white mb-4">
                Filtros de Período
              </h2>
              <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
                {/* Start Date Input */}
                <div>
                  <label htmlFor="start-date-input" className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                    Data Inicial
                  </label>
                  <input
                    id="start-date-input"
                    type="date"
                    value={categoryStartDate}
                    onChange={(e) => setCategoryStartDate(e.target.value)}
                    className="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500 outline-none"
                  />
                </div>

                {/* End Date Input */}
                <div>
                  <label htmlFor="end-date-input" className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                    Data Final
                  </label>
                  <input
                    id="end-date-input"
                    type="date"
                    value={categoryEndDate}
                    onChange={(e) => setCategoryEndDate(e.target.value)}
                    className="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500 outline-none"
                  />
                </div>

                {/* Generate Button */}
                <div className="flex items-end">
                  <button
                    onClick={handleLoadCategoryReport}
                    disabled={isLoading}
                    className="w-full px-4 py-2 bg-blue-600 hover:bg-blue-700 disabled:bg-gray-400 text-white font-medium rounded-lg transition-colors"
                  >
                    {isLoading ? (
                      <span className="flex items-center justify-center">
                        <svg className="animate-spin h-5 w-5 mr-2" viewBox="0 0 24 24">
                          <circle
                            className="opacity-25"
                            cx="12"
                            cy="12"
                            r="10"
                            stroke="currentColor"
                            strokeWidth="4"
                            fill="none"
                          />
                          <path
                            className="opacity-75"
                            fill="currentColor"
                            d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                          />
                        </svg>
                        Carregando...
                      </span>
                    ) : (
                      'Gerar Relatório'
                    )}
                  </button>
                </div>
              </div>
            </div>

            {/* Category Report Data */}
            {categoryReport && (
              <motion.div
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.3 }}
              >
                {/* Summary Cards - Task 4.7 Requirement 3 */}
                <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
                  {/* Total Receitas */}
                  <motion.div
                    initial={{ opacity: 0, y: 20 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ duration: 0.5, delay: 0 }}
                  >
                    <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6 border-l-4 border-green-500">
                      <p className="text-sm font-medium text-gray-600 dark:text-gray-400 mb-2">
                        Total de Receitas
                      </p>
                      <p className="text-3xl font-bold text-green-600 dark:text-green-400">
                        {formatCurrency(categoryReport.totalIncome)}
                      </p>
                    </div>
                  </motion.div>

                  {/* Total Despesas */}
                  <motion.div
                    initial={{ opacity: 0, y: 20 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ duration: 0.5, delay: 0.1 }}
                  >
                    <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6 border-l-4 border-red-500">
                      <p className="text-sm font-medium text-gray-600 dark:text-gray-400 mb-2">
                        Total de Despesas
                      </p>
                      <p className="text-3xl font-bold text-red-600 dark:text-red-400">
                        {formatCurrency(categoryReport.totalExpenses)}
                      </p>
                    </div>
                  </motion.div>

                  {/* Saldo Líquido */}
                  <motion.div
                    initial={{ opacity: 0, y: 20 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ duration: 0.5, delay: 0.2 }}
                  >
                    <div
                      className={`rounded-lg shadow-md p-6 border-l-4 ${
                        categoryReport.netBalance >= 0
                          ? 'bg-white dark:bg-gray-800 border-blue-500'
                          : 'bg-white dark:bg-gray-800 border-yellow-500'
                      }`}
                    >
                      <p className="text-sm font-medium text-gray-600 dark:text-gray-400 mb-2">
                        Saldo Líquido
                      </p>
                      <p
                        className={`text-3xl font-bold ${
                          categoryReport.netBalance >= 0
                            ? 'text-blue-600 dark:text-blue-400'
                            : 'text-yellow-600 dark:text-yellow-400'
                        }`}
                      >
                        {formatCurrency(categoryReport.netBalance)}
                      </p>
                    </div>
                  </motion.div>
                </div>

                {/* Report Details Card */}
                <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
                  <div className="flex justify-between items-center mb-4">
                    <h3 className="text-xl font-semibold text-gray-900 dark:text-white">
                      {categoryReport.periodLabel} - Relatório por Categoria
                    </h3>
                  </div>

                  <div className="border-t border-gray-200 dark:border-gray-700 pt-4 mb-4">
                    <p className="text-sm text-gray-600 dark:text-gray-400">
                      Total de {categoryReport.totalTransactionCount} transação
                      {categoryReport.totalTransactionCount !== 1 ? 's' : ''} registrada
                      {categoryReport.totalTransactionCount !== 1 ? 's' : ''} neste período.
                    </p>
                  </div>

                  {/* Breakdown Table - Task 4.7 Requirement 3 */}
                  <h4 className="text-lg font-semibold text-gray-900 dark:text-white mb-4 mt-6">
                    Resumo por Categoria
                  </h4>

                  <div className="overflow-x-auto">
                    <table className="w-full text-sm">
                      <thead>
                        <tr className="border-b-2 border-gray-300 dark:border-gray-600">
                          <th className="text-left px-4 py-3 font-semibold text-gray-900 dark:text-white">
                            Categoria
                          </th>
                          <th className="text-right px-4 py-3 font-semibold text-gray-900 dark:text-white">
                            Receitas
                          </th>
                          <th className="text-right px-4 py-3 font-semibold text-gray-900 dark:text-white">
                            Despesas
                          </th>
                          <th className="text-right px-4 py-3 font-semibold text-gray-900 dark:text-white">
                            Saldo
                          </th>
                          <th className="text-right px-4 py-3 font-semibold text-gray-900 dark:text-white">
                            %
                          </th>
                          <th className="text-right px-4 py-3 font-semibold text-gray-900 dark:text-white">
                            Transações
                          </th>
                        </tr>
                      </thead>
                      <tbody>
                        {categoryReport.categories.length > 0 ? (
                          categoryReport.categories.map((category: any) => (
                            <tr
                              key={category.categoryId}
                              className="border-b border-gray-200 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-700/50 transition-colors"
                            >
                              <td className="px-4 py-3">
                                <div className="flex items-center gap-2">
                                  {category.categoryIcon && (
                                    <div
                                      className="w-6 h-6 rounded-full"
                                      style={{
                                        backgroundColor: category.categoryColor || '#ccc',
                                      }}
                                    />
                                  )}
                                  <span className="text-gray-900 dark:text-white">
                                    {category.categoryName}
                                  </span>
                                </div>
                              </td>
                              <td className="px-4 py-3 text-right text-green-600 dark:text-green-400 font-medium">
                                {formatCurrency(category.incomeAmount)}
                              </td>
                              <td className="px-4 py-3 text-right text-red-600 dark:text-red-400 font-medium">
                                {formatCurrency(category.expenseAmount)}
                              </td>
                              <td
                                className={`px-4 py-3 text-right font-semibold ${
                                  category.netAmount >= 0
                                    ? 'text-green-600 dark:text-green-400'
                                    : 'text-red-600 dark:text-red-400'
                                }`}
                              >
                                {formatCurrency(category.netAmount)}
                              </td>
                              <td className="px-4 py-3 text-right text-gray-600 dark:text-gray-400">
                                {formatPercentage(category.percentage)}
                              </td>
                              <td className="px-4 py-3 text-right text-gray-600 dark:text-gray-400">
                                {category.transactionCount}
                              </td>
                            </tr>
                          ))
                        ) : (
                          <tr>
                            <td colSpan={6} className="px-4 py-6 text-center text-gray-600 dark:text-gray-400">
                              Nenhuma transação registrada neste período
                            </td>
                          </tr>
                        )}
                      </tbody>
                    </table>
                  </div>
                </div>
              </motion.div>
            )}
          </motion.div>
        )}
      </motion.div>
    </div>
  )
}

export default ReportsPage
