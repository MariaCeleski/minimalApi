import React from 'react'
import { Box, Typography, Card, CardContent } from '@mui/material'
import { motion } from 'framer-motion'

const ReportsPage: React.FC = () => {
  return (
    <Box>
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
      >
        <Typography variant="h4" component="h1" gutterBottom>
          Relatórios
        </Typography>
        
        <Typography variant="body1" color="text.secondary" sx={{ mb: 4 }}>
          Visualize relatórios detalhados das suas finanças.
        </Typography>

        <Card>
          <CardContent>
            <Typography variant="body2" color="text.secondary" textAlign="center">
              Página de relatórios será implementada nas próximas tarefas
            </Typography>
          </CardContent>
        </Card>
      </motion.div>
    </Box>
  )
}

export default ReportsPage