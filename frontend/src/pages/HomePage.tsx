import React from 'react'
import { Box, Typography, Grid, Card, CardContent, Button } from '@mui/material'
import { useNavigate } from 'react-router-dom'
import { Dashboard, Receipt, Assessment, TrackChanges } from '@mui/icons-material'
import { motion } from 'framer-motion'

const HomePage: React.FC = () => {
  const navigate = useNavigate()

  const quickActions = [
    {
      title: 'Ver Dashboard',
      description: 'Visualize sua situação financeira atual',
      icon: Dashboard,
      path: '/dashboard',
      color: 'primary',
    },
    {
      title: 'Adicionar Transação',
      description: 'Registre uma nova receita ou despesa',
      icon: Receipt,
      path: '/transactions',
      color: 'success',
    },
    {
      title: 'Gerar Relatório',
      description: 'Analise seus gastos por período',
      icon: Assessment,
      path: '/reports',
      color: 'info',
    },
    {
      title: 'Ver Metas',
      description: 'Acompanhe o progresso das suas metas',
      icon: TrackChanges,
      path: '/goals',
      color: 'warning',
    },
  ]

  return (
    <Box>
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
      >
        <Typography variant="h4" component="h1" gutterBottom>
          Bem-vindo à sua Gestão Financeira
        </Typography>
        
        <Typography variant="body1" color="text.secondary" sx={{ mb: 4 }}>
          Mantenha o controle das suas finanças de forma simples e intuitiva.
        </Typography>
      </motion.div>

      <Grid container spacing={3}>
        {quickActions.map((action, index) => {
          const Icon = action.icon
          
          return (
            <Grid item xs={12} sm={6} md={3} key={action.title}>
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.5, delay: index * 0.1 }}
                whileHover={{ y: -5 }}
              >
                <Card 
                  sx={{ 
                    height: '100%',
                    cursor: 'pointer',
                    transition: 'all 0.2s ease-in-out',
                    '&:hover': {
                      boxShadow: 4,
                    }
                  }}
                  onClick={() => navigate(action.path)}
                >
                  <CardContent sx={{ p: 3, textAlign: 'center' }}>
                    <Icon 
                      sx={{ 
                        fontSize: 48, 
                        mb: 2,
                        color: `${action.color}.main` 
                      }} 
                    />
                    <Typography variant="h6" component="h3" gutterBottom>
                      {action.title}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      {action.description}
                    </Typography>
                  </CardContent>
                </Card>
              </motion.div>
            </Grid>
          )
        })}
      </Grid>

      <Box sx={{ mt: 6 }}>
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5, delay: 0.4 }}
        >
          <Card>
            <CardContent sx={{ p: 4 }}>
              <Typography variant="h5" component="h2" gutterBottom>
                Comece agora mesmo
              </Typography>
              <Typography variant="body1" color="text.secondary" paragraph>
                Esta aplicação permite que você gerencie suas receitas e despesas de forma
                completa, com recursos como:
              </Typography>
              <ul>
                <li>Dashboard com gráficos interativos</li>
                <li>Categorização automática de transações</li>
                <li>Relatórios mensais e por categoria</li>
                <li>Definição e acompanhamento de metas</li>
                <li>Exportação de dados em CSV e PDF</li>
                <li>Tema claro e escuro</li>
              </ul>
              <Button 
                variant="contained" 
                size="large" 
                onClick={() => navigate('/dashboard')}
                sx={{ mt: 2 }}
              >
                Ir para Dashboard
              </Button>
            </CardContent>
          </Card>
        </motion.div>
      </Box>
    </Box>
  )
}

export default HomePage