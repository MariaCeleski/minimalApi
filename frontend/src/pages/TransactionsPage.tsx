import React from 'react'
import { Box, Typography, Card, CardContent } from '@mui/material'
import { motion } from 'framer-motion'

const TransactionsPage: React.FC = () => {
  return (
    <Box>
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
      >
        <Typography variant="h4" component="h1" gutterBottom>
          Transações
        </Typography>
        
        <Typography variant="body1" color="text.secondary" sx={{ mb: 4 }}>
          Gerencie suas receitas e despesas.
        </Typography>

        <Card>
          <CardContent>
            <Typography variant="body2" color="text.secondary" textAlign="center">
              Página de transações será implementada nas próximas tarefas
            </Typography>
          </CardContent>
        </Card>
      </motion.div>
    </Box>
  )
}

export default TransactionsPage