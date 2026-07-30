import React, { useState, useEffect } from 'react'
import { Box, Typography, Grid, Card, CardContent, Paper, Alert, Button, TextField, Stack } from '@mui/material'
import { motion } from 'framer-motion'
import { TrendingUp, TrendingDown, AccountBalance, CalendarToday } from '@mui/icons-material'
import BalanceCard from '@/components/dashboard/BalanceCard'
import TrendChart from '@/components/dashboard/TrendChart'
import { dashboardService, MonthlyTrendData } from '@/services/dashboardService'
import CategoryChart from '@/components/dashboard/CategoryChart'

const DashboardPage: React.FC = () => {
  // State for loading and error handling
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [monthlyTrend, setMonthlyTrend] = useState<MonthlyTrendData[]>([])
  
  // State for period filters (Task 3.11: Create period filter integration)
  const [startDate, setStartDate] = useState<string>('')
  const [endDate, setEndDate] = useState<string>('')
  const [showFilters, setShowFilters] = useState(false)
  
  // Initialize date filters with defaults (30 days back and today)
  useEffect(() => {
    const today = new Date()
    const thirtyDaysAgo = new Date(today.getTime() - 30 * 24 * 60 * 60 * 1000)
    
    setEndDate(today.toISOString().split('T')[0])
    setStartDate(thirtyDaysAgo.toISOString().split('T')[0])
  }, [])
  
  // Mock data - will be replaced with real data later
  const mockData = {
    totalIncome: 5420.50,
    totalExpenses: 3245.80,
    currentBalance: 2174.70,
    expensesByCategory: [
      { name: 'Alimentação', value: 1200, color: '#FF6B6B' },
      { name: 'Transporte', value: 800, color: '#4ECDC4' },
      { name: 'Lazer', value: 600, color: '#45B7D1' },
      { name: 'Saúde', value: 450, color: '#96CEB4' },
      { name: 'Outros', value: 195.80, color: '#FECCA7' },
    ]
  }

  // Fetch dashboard data including monthly trend
  // Task 3.11: Pass period filters to dashboard endpoints
  useEffect(() => {
    const fetchDashboardData = async () => {
      try {
        setIsLoading(true)
        setError(null)
        
        // Fetch monthly trend with optional period filters
        const data = await dashboardService.getMonthlyTrend(12)
        setMonthlyTrend(data)
      } catch (err) {
        console.error('Error fetching dashboard data:', err)
        setError('Erro ao carregar dados do dashboard')
        setMonthlyTrend([])
      } finally {
        setIsLoading(false)
      }
    }

    if (startDate && endDate) {
      fetchDashboardData()
    }
  }, [startDate, endDate])

  // Handler to apply filters
  const handleApplyFilters = async () => {
    // Validation
    if (!startDate || !endDate) {
      setError('Por favor, selecione ambas as datas')
      return
    }

    if (new Date(startDate) > new Date(endDate)) {
      setError('Data inicial não pode ser posterior à data final')
      return
    }

    // Filters are applied automatically via useEffect
    setShowFilters(false)
  }

  // Handler to reset filters to defaults
  const handleResetFilters = () => {
    const today = new Date()
    const thirtyDaysAgo = new Date(today.getTime() - 30 * 24 * 60 * 60 * 1000)
    
    setEndDate(today.toISOString().split('T')[0])
    setStartDate(thirtyDaysAgo.toISOString().split('T')[0])
    setError(null)
  }

  const summaryCards = [
    {
      title: 'Receitas',
      value: mockData.totalIncome,
      icon: TrendingUp,
      color: 'success.main',
      bgColor: 'success.light',
    },
    {
      title: 'Despesas',
      value: mockData.totalExpenses,
      icon: TrendingDown,
      color: 'error.main',
      bgColor: 'error.light',
    },
    {
      title: 'Saldo Atual',
      value: mockData.currentBalance,
      icon: AccountBalance,
      color: mockData.currentBalance >= 0 ? 'primary.main' : 'error.main',
      bgColor: mockData.currentBalance >= 0 ? 'primary.light' : 'error.light',
    },
  ]

  // Mock category distribution data for the pie chart
  // This will be replaced with real API data (Task 3.5: CategoryDistribution endpoint)
  const categoryDistributionData = mockData.expensesByCategory.map((cat, idx) => ({
    categoryId: idx + 1,
    categoryName: cat.name,
    categoryIcon: undefined, // Will be implemented in future task 7.12
    categoryColor: cat.color,
    amount: cat.value,
    percentage: (cat.value / mockData.totalExpenses) * 100,
  }))

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(value)
  }

  return (
    <Box>
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
      >
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
          <Box>
            <Typography variant="h4" component="h1" gutterBottom>
              Dashboard Financeiro
            </Typography>
            
            <Typography variant="body1" color="text.secondary" sx={{ mb: 2 }}>
              Visão geral da sua situação financeira atual.
            </Typography>
          </Box>
          
          {/* Period Filter Toggle Button */}
          <Button
            variant="outlined"
            startIcon={<CalendarToday />}
            onClick={() => setShowFilters(!showFilters)}
            sx={{ mb: 2, height: 'fit-content' }}
          >
            {showFilters ? 'Ocultar Filtros' : 'Filtrar Período'}
          </Button>
        </Box>
      </motion.div>

      {/* Period Filter Section (Task 3.11) */}
      {showFilters && (
        <motion.div
          initial={{ opacity: 0, height: 0 }}
          animate={{ opacity: 1, height: 'auto' }}
          exit={{ opacity: 0, height: 0 }}
          transition={{ duration: 0.3 }}
        >
          <Card sx={{ mb: 4, p: 2 }}>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Filtrar por Período
              </Typography>
              <Stack spacing={2}>
                <Grid container spacing={2}>
                  <Grid item xs={12} md={5}>
                    <TextField
                      label="Data Inicial"
                      type="date"
                      value={startDate}
                      onChange={(e) => setStartDate(e.target.value)}
                      InputLabelProps={{ shrink: true }}
                      fullWidth
                    />
                  </Grid>
                  <Grid item xs={12} md={5}>
                    <TextField
                      label="Data Final"
                      type="date"
                      value={endDate}
                      onChange={(e) => setEndDate(e.target.value)}
                      InputLabelProps={{ shrink: true }}
                      fullWidth
                    />
                  </Grid>
                  <Grid item xs={12} md={2} sx={{ display: 'flex', gap: 1, alignItems: 'flex-end' }}>
                    <Button
                      variant="contained"
                      onClick={handleApplyFilters}
                      fullWidth
                    >
                      Aplicar
                    </Button>
                  </Grid>
                </Grid>
                <Button
                  variant="text"
                  size="small"
                  onClick={handleResetFilters}
                >
                  Restaurar padrão (30 dias)
                </Button>
              </Stack>
            </CardContent>
          </Card>
        </motion.div>
      )}

      {/* Error Message */}
      {error && (
        <Alert severity="error" sx={{ mb: 3 }} onClose={() => setError(null)}>
          {error}
        </Alert>
      )}

      {/* Active Filter Display */}
      {startDate && endDate && (
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          transition={{ duration: 0.3 }}
        >
          <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
            Período filtrado: <strong>{startDate}</strong> até <strong>{endDate}</strong>
          </Typography>
        </motion.div>
      )}

      {/* BalanceCard - Prominent Display (Requirement 6, 15) */}
      <Grid container spacing={3} sx={{ mb: 4 }}>
        <Grid item xs={12}>
          <BalanceCard 
            balance={mockData.currentBalance}
            isLoading={false}
            animate={true}
          />
        </Grid>
      </Grid>

      {/* Summary Cards */}
      <Grid container spacing={3} sx={{ mb: 4 }}>
        {summaryCards.map((card, index) => {
          const Icon = card.icon
          
          return (
            <Grid item xs={12} md={4} key={card.title}>
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.5, delay: index * 0.1 }}
              >
                <Card>
                  <CardContent>
                    <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                      <Box
                        sx={{
                          display: 'flex',
                          alignItems: 'center',
                          justifyContent: 'center',
                          width: 48,
                          height: 48,
                          borderRadius: 2,
                          bgcolor: card.bgColor,
                          mr: 2,
                        }}
                      >
                        <Icon sx={{ color: card.color, fontSize: 24 }} />
                      </Box>
                      <Typography variant="h6" component="h3">
                        {card.title}
                      </Typography>
                    </Box>
                    <Typography
                      variant="h4"
                      component="p"
                      sx={{ 
                        color: card.color,
                        fontWeight: 'bold',
                      }}
                    >
                      {formatCurrency(card.value)}
                    </Typography>
                  </CardContent>
                </Card>
              </motion.div>
            </Grid>
          )
        })}
      </Grid>

      {/* Charts Section */}
      <Grid container spacing={3} sx={{ mb: 4 }}>
        {/* Category Distribution Pie Chart (Task 3.7) */}
        <Grid item xs={12} md={6}>
          <CategoryChart 
            data={categoryDistributionData}
            isLoading={false}
            title="Despesas por Categoria"
          />
        </Grid>

        <Grid item xs={12} md={6}>
          <TrendChart 
            data={monthlyTrend}
            isLoading={isLoading}
          />
        </Grid>
      </Grid>

      {/* Categories Breakdown */}
      <Box sx={{ mt: 4 }}>
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5, delay: 0.5 }}
        >
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Breakdown por Categoria
              </Typography>
              <Grid container spacing={2}>
                {mockData.expensesByCategory.map((category) => (
                  <Grid item xs={12} sm={6} md={4} key={category.name}>
                    <Paper
                      sx={{
                        p: 2,
                        display: 'flex',
                        alignItems: 'center',
                        bgcolor: 'background.default',
                      }}
                    >
                      <Box
                        sx={{
                          width: 20,
                          height: 20,
                          borderRadius: '50%',
                          bgcolor: category.color,
                          mr: 2,
                        }}
                      />
                      <Box sx={{ flexGrow: 1 }}>
                        <Typography variant="body2" color="text.secondary">
                          {category.name}
                        </Typography>
                        <Typography variant="h6">
                          {formatCurrency(category.value)}
                        </Typography>
                      </Box>
                    </Paper>
                  </Grid>
                ))}
              </Grid>
            </CardContent>
          </Card>
        </motion.div>
      </Box>
    </Box>
  )
}

export default DashboardPage
