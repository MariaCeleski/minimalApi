import React from 'react'
import { Box, Typography, Grid, Card, CardContent, Paper } from '@mui/material'
import { motion } from 'framer-motion'
import { TrendingUp, TrendingDown, AccountBalance } from '@mui/icons-material'

const DashboardPage: React.FC = () => {
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
        <Typography variant="h4" component="h1" gutterBottom>
          Dashboard Financeiro
        </Typography>
        
        <Typography variant="body1" color="text.secondary" sx={{ mb: 4 }}>
          Visão geral da sua situação financeira atual.
        </Typography>
      </motion.div>

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
      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <motion.div
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ duration: 0.5, delay: 0.3 }}
          >
            <Card>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Despesas por Categoria
                </Typography>
                <Box sx={{ height: 300, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                  <Typography variant="body2" color="text.secondary">
                    Gráfico de pizza será implementado com Recharts
                  </Typography>
                </Box>
              </CardContent>
            </Card>
          </motion.div>
        </Grid>

        <Grid item xs={12} md={6}>
          <motion.div
            initial={{ opacity: 0, x: 20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ duration: 0.5, delay: 0.4 }}
          >
            <Card>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Tendência Mensal
                </Typography>
                <Box sx={{ height: 300, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                  <Typography variant="body2" color="text.secondary">
                    Gráfico de linha será implementado com Recharts
                  </Typography>
                </Box>
              </CardContent>
            </Card>
          </motion.div>
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
                {mockData.expensesByCategory.map((category, index) => (
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