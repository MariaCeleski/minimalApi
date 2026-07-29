import React, { useState, useEffect } from 'react'
import {
  Box,
  Typography,
  Card,
  CardContent,
  Grid,
  Button,
  TextField,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Alert,
  CircularProgress,
  Divider,
  Tooltip,
} from '@mui/material'
import { motion } from 'framer-motion'
import { TrendingUp, TrendingDown, FileDownload } from '@mui/icons-material'
import { reportService, MonthlyReportResponse, CategoryReportResponse } from '../services/reportService'
import { ErrorResponse } from '../types'

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
   * Format currency
   */
  const formatCurrency = (value: number): string => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(value)
  }

  /**
   * Format percentage
   */
  const formatPercentage = (value: number): string => {
    return `${value.toFixed(2)}%`
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
    <Box>
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
      >
        <Box sx={{ mb: 4 }}>
          <Typography variant="h4" component="h1" gutterBottom>
            Relatórios Financeiros
          </Typography>

          <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
            Visualize relatórios detalhados das suas transações e finanças. Escolha entre
            relatórios mensais ou por categoria.
          </Typography>
        </Box>

        {/* Report Type Selection */}
        <Card sx={{ mb: 4, p: 2 }}>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              Tipo de Relatório
            </Typography>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={6}>
                <Button
                  variant={reportType === 'monthly' ? 'contained' : 'outlined'}
                  fullWidth
                  onClick={() => handleReportTypeChange('monthly')}
                  sx={{ py: 1.5, fontWeight: 'bold' }}
                >
                  Relatório Mensal
                </Button>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Button
                  variant={reportType === 'category' ? 'contained' : 'outlined'}
                  fullWidth
                  onClick={() => handleReportTypeChange('category')}
                  sx={{ py: 1.5, fontWeight: 'bold' }}
                >
                  Relatório por Categoria
                </Button>
              </Grid>
            </Grid>
          </CardContent>
        </Card>

        {/* Error Alert */}
        {error && (
          <Alert severity="error" sx={{ mb: 3 }} onClose={() => setError(null)}>
            <Typography variant="body2" sx={{ fontWeight: 'bold', mb: 0.5 }}>
              {error.message}
            </Typography>
            {error.details && <Typography variant="caption">{error.details}</Typography>}
          </Alert>
        )}

        {/* Monthly Report Section */}
        {reportType === 'monthly' && (
          <motion.div
            key="monthly-section"
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.3 }}
          >
            {/* Filters */}
            <Card sx={{ mb: 4, p: 2 }}>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Filtros
                </Typography>
                <Grid container spacing={2}>
                  <Grid item xs={12} sm={6} md={4}>
                    <TextField
                      label="Ano"
                      type="number"
                      value={selectedYear}
                      onChange={(e) => setSelectedYear(parseInt(e.target.value))}
                      fullWidth
                      inputProps={{ min: 1900, max: new Date().getFullYear() + 1 }}
                    />
                  </Grid>
                  <Grid item xs={12} sm={6} md={4}>
                    <FormControl fullWidth>
                      <InputLabel>Mês</InputLabel>
                      <Select
                        label="Mês"
                        value={selectedMonth}
                        onChange={(e) => setSelectedMonth(e.target.value as number)}
                      >
                        {Array.from({ length: 12 }, (_, i) => i + 1).map((month) => (
                          <MenuItem key={month} value={month}>
                            {getMonthName(month)}
                          </MenuItem>
                        ))}
                      </Select>
                    </FormControl>
                  </Grid>
                  <Grid item xs={12} md={4} sx={{ display: 'flex', alignItems: 'flex-end' }}>
                    <Button
                      variant="contained"
                      fullWidth
                      onClick={handleLoadMonthlyReport}
                      disabled={isLoading}
                      sx={{ height: '56px' }}
                    >
                      {isLoading ? <CircularProgress size={24} /> : 'Gerar Relatório'}
                    </Button>
                  </Grid>
                </Grid>
              </CardContent>
            </Card>

            {/* Monthly Report Data */}
            {monthlyReport && (
              <motion.div
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.3 }}
              >
                {/* Summary Cards */}
                <Grid container spacing={3} sx={{ mb: 4 }}>
                  <Grid item xs={12} md={4}>
                    <motion.div
                      initial={{ opacity: 0, y: 20 }}
                      animate={{ opacity: 1, y: 0 }}
                      transition={{ duration: 0.5, delay: 0 }}
                    >
                      <Card sx={{ bgcolor: 'success.light' }}>
                        <CardContent>
                          <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                            <TrendingUp sx={{ color: 'success.main', mr: 1 }} />
                            <Typography variant="body2" color="text.secondary">
                              Total de Receitas
                            </Typography>
                          </Box>
                          <Typography
                            variant="h5"
                            sx={{ color: 'success.main', fontWeight: 'bold' }}
                          >
                            {formatCurrency(monthlyReport.totalIncome)}
                          </Typography>
                        </CardContent>
                      </Card>
                    </motion.div>
                  </Grid>

                  <Grid item xs={12} md={4}>
                    <motion.div
                      initial={{ opacity: 0, y: 20 }}
                      animate={{ opacity: 1, y: 0 }}
                      transition={{ duration: 0.5, delay: 0.1 }}
                    >
                      <Card sx={{ bgcolor: 'error.light' }}>
                        <CardContent>
                          <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                            <TrendingDown sx={{ color: 'error.main', mr: 1 }} />
                            <Typography variant="body2" color="text.secondary">
                              Total de Despesas
                            </Typography>
                          </Box>
                          <Typography
                            variant="h5"
                            sx={{ color: 'error.main', fontWeight: 'bold' }}
                          >
                            {formatCurrency(monthlyReport.totalExpenses)}
                          </Typography>
                        </CardContent>
                      </Card>
                    </motion.div>
                  </Grid>

                  <Grid item xs={12} md={4}>
                    <motion.div
                      initial={{ opacity: 0, y: 20 }}
                      animate={{ opacity: 1, y: 0 }}
                      transition={{ duration: 0.5, delay: 0.2 }}
                    >
                      <Card
                        sx={{
                          bgcolor: monthlyReport.balance >= 0 ? 'primary.light' : 'warning.light',
                        }}
                      >
                        <CardContent>
                          <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                            Saldo Líquido
                          </Typography>
                          <Typography
                            variant="h5"
                            sx={{
                              color: monthlyReport.balance >= 0 ? 'primary.main' : 'warning.main',
                              fontWeight: 'bold',
                            }}
                          >
                            {formatCurrency(monthlyReport.balance)}
                          </Typography>
                        </CardContent>
                      </Card>
                    </motion.div>
                  </Grid>
                </Grid>

                {/* Report Information */}
                <Card sx={{ mb: 4 }}>
                  <CardContent>
                    <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
                      <Typography variant="h6">
                        {monthlyReport.monthName} - Relatório Detalhado
                      </Typography>
                      <Tooltip title="Exportar relatório (em desenvolvimento)">
                        <Button variant="outlined" size="small" startIcon={<FileDownload />}>
                          Exportar
                        </Button>
                      </Tooltip>
                    </Box>
                    <Divider sx={{ mb: 2 }} />
                    <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                      Total de {monthlyReport.transactionCount} transação
                      {monthlyReport.transactionCount !== 1 ? 's' : ''} registrada
                      {monthlyReport.transactionCount !== 1 ? 's' : ''} neste período.
                    </Typography>

                    {/* Category Breakdown Table */}
                    <Typography variant="subtitle2" sx={{ mt: 3, mb: 2, fontWeight: 'bold' }}>
                      Breakdown por Categoria
                    </Typography>
                    <TableContainer component={Paper} sx={{ bgcolor: 'background.default' }}>
                      <Table>
                        <TableHead>
                          <TableRow sx={{ bgcolor: 'action.hover' }}>
                            <TableCell sx={{ fontWeight: 'bold' }}>Categoria</TableCell>
                            <TableCell align="right" sx={{ fontWeight: 'bold' }}>
                              Valor
                            </TableCell>
                            <TableCell align="right" sx={{ fontWeight: 'bold' }}>
                              Percentual
                            </TableCell>
                            <TableCell align="right" sx={{ fontWeight: 'bold' }}>
                              Transações
                            </TableCell>
                          </TableRow>
                        </TableHead>
                        <TableBody>
                          {monthlyReport.categories.length > 0 ? (
                            monthlyReport.categories.map((category) => (
                              <TableRow key={category.categoryId} hover>
                                <TableCell>
                                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                                    {category.categoryIcon && (
                                      <Box
                                        sx={{
                                          width: 24,
                                          height: 24,
                                          borderRadius: '50%',
                                          bgcolor: category.categoryColor || '#ccc',
                                        }}
                                      />
                                    )}
                                    <Typography variant="body2">
                                      {category.categoryName}
                                    </Typography>
                                  </Box>
                                </TableCell>
                                <TableCell align="right" sx={{ fontWeight: 'bold' }}>
                                  {formatCurrency(category.amount)}
                                </TableCell>
                                <TableCell align="right">
                                  {formatPercentage(category.percentage)}
                                </TableCell>
                                <TableCell align="right">
                                  {category.transactionCount}
                                </TableCell>
                              </TableRow>
                            ))
                          ) : (
                            <TableRow>
                              <TableCell colSpan={4} sx={{ textAlign: 'center', py: 3 }}>
                                <Typography variant="body2" color="text.secondary">
                                  Nenhuma transação registrada neste período
                                </Typography>
                              </TableCell>
                            </TableRow>
                          )}
                        </TableBody>
                      </Table>
                    </TableContainer>
                  </CardContent>
                </Card>
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
            {/* Filters */}
            <Card sx={{ mb: 4, p: 2 }}>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Filtros de Período
                </Typography>
                <Grid container spacing={2}>
                  <Grid item xs={12} sm={6} md={4}>
                    <TextField
                      label="Data Inicial"
                      type="date"
                      value={categoryStartDate}
                      onChange={(e) => setCategoryStartDate(e.target.value)}
                      fullWidth
                      InputLabelProps={{ shrink: true }}
                    />
                  </Grid>
                  <Grid item xs={12} sm={6} md={4}>
                    <TextField
                      label="Data Final"
                      type="date"
                      value={categoryEndDate}
                      onChange={(e) => setCategoryEndDate(e.target.value)}
                      fullWidth
                      InputLabelProps={{ shrink: true }}
                    />
                  </Grid>
                  <Grid item xs={12} md={4} sx={{ display: 'flex', alignItems: 'flex-end' }}>
                    <Button
                      variant="contained"
                      fullWidth
                      onClick={handleLoadCategoryReport}
                      disabled={isLoading}
                      sx={{ height: '56px' }}
                    >
                      {isLoading ? <CircularProgress size={24} /> : 'Gerar Relatório'}
                    </Button>
                  </Grid>
                </Grid>
              </CardContent>
            </Card>

            {/* Category Report Data */}
            {categoryReport && (
              <motion.div
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.3 }}
              >
                {/* Summary Cards */}
                <Grid container spacing={3} sx={{ mb: 4 }}>
                  <Grid item xs={12} md={4}>
                    <motion.div
                      initial={{ opacity: 0, y: 20 }}
                      animate={{ opacity: 1, y: 0 }}
                      transition={{ duration: 0.5, delay: 0 }}
                    >
                      <Card sx={{ bgcolor: 'success.light' }}>
                        <CardContent>
                          <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                            <TrendingUp sx={{ color: 'success.main', mr: 1 }} />
                            <Typography variant="body2" color="text.secondary">
                              Total de Receitas
                            </Typography>
                          </Box>
                          <Typography
                            variant="h5"
                            sx={{ color: 'success.main', fontWeight: 'bold' }}
                          >
                            {formatCurrency(categoryReport.totalIncome)}
                          </Typography>
                        </CardContent>
                      </Card>
                    </motion.div>
                  </Grid>

                  <Grid item xs={12} md={4}>
                    <motion.div
                      initial={{ opacity: 0, y: 20 }}
                      animate={{ opacity: 1, y: 0 }}
                      transition={{ duration: 0.5, delay: 0.1 }}
                    >
                      <Card sx={{ bgcolor: 'error.light' }}>
                        <CardContent>
                          <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                            <TrendingDown sx={{ color: 'error.main', mr: 1 }} />
                            <Typography variant="body2" color="text.secondary">
                              Total de Despesas
                            </Typography>
                          </Box>
                          <Typography
                            variant="h5"
                            sx={{ color: 'error.main', fontWeight: 'bold' }}
                          >
                            {formatCurrency(categoryReport.totalExpenses)}
                          </Typography>
                        </CardContent>
                      </Card>
                    </motion.div>
                  </Grid>

                  <Grid item xs={12} md={4}>
                    <motion.div
                      initial={{ opacity: 0, y: 20 }}
                      animate={{ opacity: 1, y: 0 }}
                      transition={{ duration: 0.5, delay: 0.2 }}
                    >
                      <Card
                        sx={{
                          bgcolor:
                            categoryReport.netBalance >= 0 ? 'primary.light' : 'warning.light',
                        }}
                      >
                        <CardContent>
                          <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                            Saldo Líquido
                          </Typography>
                          <Typography
                            variant="h5"
                            sx={{
                              color:
                                categoryReport.netBalance >= 0 ? 'primary.main' : 'warning.main',
                              fontWeight: 'bold',
                            }}
                          >
                            {formatCurrency(categoryReport.netBalance)}
                          </Typography>
                        </CardContent>
                      </Card>
                    </motion.div>
                  </Grid>
                </Grid>

                {/* Report Information */}
                <Card sx={{ mb: 4 }}>
                  <CardContent>
                    <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
                      <Typography variant="h6">
                        {categoryReport.periodLabel} - Relatório por Categoria
                      </Typography>
                      <Tooltip title="Exportar relatório (em desenvolvimento)">
                        <Button variant="outlined" size="small" startIcon={<FileDownload />}>
                          Exportar
                        </Button>
                      </Tooltip>
                    </Box>
                    <Divider sx={{ mb: 2 }} />
                    <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                      Total de {categoryReport.totalTransactionCount} transação
                      {categoryReport.totalTransactionCount !== 1 ? 's' : ''} registrada
                      {categoryReport.totalTransactionCount !== 1 ? 's' : ''} neste período.
                    </Typography>

                    {/* Category Breakdown Table */}
                    <Typography variant="subtitle2" sx={{ mt: 3, mb: 2, fontWeight: 'bold' }}>
                      Resumo por Categoria
                    </Typography>
                    <TableContainer component={Paper} sx={{ bgcolor: 'background.default' }}>
                      <Table>
                        <TableHead>
                          <TableRow sx={{ bgcolor: 'action.hover' }}>
                            <TableCell sx={{ fontWeight: 'bold' }}>Categoria</TableCell>
                            <TableCell align="right" sx={{ fontWeight: 'bold' }}>
                              Receitas
                            </TableCell>
                            <TableCell align="right" sx={{ fontWeight: 'bold' }}>
                              Despesas
                            </TableCell>
                            <TableCell align="right" sx={{ fontWeight: 'bold' }}>
                              Saldo
                            </TableCell>
                            <TableCell align="right" sx={{ fontWeight: 'bold' }}>
                              %
                            </TableCell>
                            <TableCell align="right" sx={{ fontWeight: 'bold' }}>
                              Transações
                            </TableCell>
                          </TableRow>
                        </TableHead>
                        <TableBody>
                          {categoryReport.categories.length > 0 ? (
                            categoryReport.categories.map((category) => (
                              <TableRow key={category.categoryId} hover>
                                <TableCell>
                                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                                    {category.categoryIcon && (
                                      <Box
                                        sx={{
                                          width: 24,
                                          height: 24,
                                          borderRadius: '50%',
                                          bgcolor: category.categoryColor || '#ccc',
                                        }}
                                      />
                                    )}
                                    <Typography variant="body2">
                                      {category.categoryName}
                                    </Typography>
                                  </Box>
                                </TableCell>
                                <TableCell align="right" sx={{ color: 'success.main' }}>
                                  {formatCurrency(category.incomeAmount)}
                                </TableCell>
                                <TableCell align="right" sx={{ color: 'error.main' }}>
                                  {formatCurrency(category.expenseAmount)}
                                </TableCell>
                                <TableCell
                                  align="right"
                                  sx={{
                                    fontWeight: 'bold',
                                    color: category.netAmount >= 0 ? 'success.main' : 'error.main',
                                  }}
                                >
                                  {formatCurrency(category.netAmount)}
                                </TableCell>
                                <TableCell align="right">{formatPercentage(category.percentage)}</TableCell>
                                <TableCell align="right">{category.transactionCount}</TableCell>
                              </TableRow>
                            ))
                          ) : (
                            <TableRow>
                              <TableCell colSpan={6} sx={{ textAlign: 'center', py: 3 }}>
                                <Typography variant="body2" color="text.secondary">
                                  Nenhuma transação registrada neste período
                                </Typography>
                              </TableCell>
                            </TableRow>
                          )}
                        </TableBody>
                      </Table>
                    </TableContainer>
                  </CardContent>
                </Card>
              </motion.div>
            )}
          </motion.div>
        )}
      </motion.div>
    </Box>
  )
}

export default ReportsPage