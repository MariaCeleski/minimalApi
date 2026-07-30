import React, { useState, useEffect } from 'react'
import {
  Box,
  Typography,
  Card,
  CardContent,
  TextField,
  Button,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Grid,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  IconButton,
  Chip,
  Alert,
  CircularProgress,
  Pagination,
} from '@mui/material'
import { motion } from 'framer-motion'
import EditIcon from '@mui/icons-material/Edit'
import DeleteIcon from '@mui/icons-material/Delete'
import AddIcon from '@mui/icons-material/Add'
import FilterListIcon from '@mui/icons-material/FilterList'

interface Transaction {
  id: number
  amount: decimal
  date: string
  type: 'Income' | 'Expense'
  categoryId: number
  categoryName: string
  categoryIcon: string
  categoryColor: string
  description: string
  createdAt: string
}

interface Category {
  id: number
  name: string
  iconName: string
  color: string
}

interface PaginatedResponse {
  data: Transaction[]
  currentPage: number
  pageSize: number
  totalItems: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

const TransactionsPage: React.FC = () => {
  // State
  const [transactions, setTransactions] = useState<Transaction[]>([])
  const [categories, setCategories] = useState<Category[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)

  // Formulário
  const [openDialog, setOpenDialog] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [formData, setFormData] = useState({
    amount: '',
    date: new Date().toISOString().split('T')[0],
    type: 'Expense' as 'Income' | 'Expense',
    categoryId: '',
    description: '',
  })

  // Filtros
  const [filters, setFilters] = useState({
    startDate: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
    endDate: new Date().toISOString().split('T')[0],
    categoryIds: [] as number[],
    page: 1,
    pageSize: 10,
  })

  const [pagination, setPagination] = useState({
    currentPage: 1,
    totalPages: 1,
    totalItems: 0,
  })

  // Carregar categorias
  useEffect(() => {
    const loadCategories = async () => {
      try {
        const response = await fetch('/api/categories')
        const data = await response.json()
        setCategories(data)
      } catch (err) {
        console.error('Erro ao carregar categorias:', err)
      }
    }
    loadCategories()
  }, [])

  // Carregar transações
  useEffect(() => {
    loadTransactions()
  }, [filters])

  const loadTransactions = async () => {
    setLoading(true)
    setError(null)
    try {
      const params = new URLSearchParams({
        page: filters.page.toString(),
        pageSize: filters.pageSize.toString(),
        startDate: filters.startDate,
        endDate: filters.endDate,
      })

      if (filters.categoryIds.length > 0) {
        filters.categoryIds.forEach(id => params.append('categoryIds', id.toString()))
      }

      const response = await fetch(`/api/transactions?${params}`)
      const data: PaginatedResponse = await response.json()

      setTransactions(data.data)
      setPagination({
        currentPage: data.currentPage,
        totalPages: data.totalPages,
        totalItems: data.totalItems,
      })
    } catch (err) {
      setError('Erro ao carregar transações')
      console.error(err)
    } finally {
      setLoading(false)
    }
  }

  // Handlers
  const handleOpenDialog = (transaction?: Transaction) => {
    if (transaction) {
      setEditingId(transaction.id)
      setFormData({
        amount: transaction.amount.toString(),
        date: transaction.date.split('T')[0],
        type: transaction.type,
        categoryId: transaction.categoryId.toString(),
        description: transaction.description,
      })
    } else {
      setEditingId(null)
      setFormData({
        amount: '',
        date: new Date().toISOString().split('T')[0],
        type: 'Expense',
        categoryId: '',
        description: '',
      })
    }
    setOpenDialog(true)
  }

  const handleCloseDialog = () => {
    setOpenDialog(false)
    setEditingId(null)
  }

  const handleSaveTransaction = async () => {
    if (!formData.amount || !formData.categoryId || !formData.description) {
      setError('Preencha todos os campos obrigatórios')
      return
    }

    setLoading(true)
    try {
      const url = editingId ? `/api/transactions/${editingId}` : '/api/transactions'
      const method = editingId ? 'PUT' : 'POST'

      const response = await fetch(url, {
        method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          ...formData,
          amount: parseFloat(formData.amount),
          categoryId: parseInt(formData.categoryId),
        }),
      })

      if (!response.ok) throw new Error('Erro ao salvar transação')

      setSuccess(editingId ? 'Transação atualizada!' : 'Transação criada!')
      handleCloseDialog()
      loadTransactions()
    } catch (err) {
      setError('Erro ao salvar transação')
      console.error(err)
    } finally {
      setLoading(false)
    }
  }

  const handleDeleteTransaction = async (id: number) => {
    if (!window.confirm('Tem certeza que deseja deletar esta transação?')) return

    setLoading(true)
    try {
      const response = await fetch(`/api/transactions/${id}`, { method: 'DELETE' })
      if (!response.ok) throw new Error('Erro ao deletar')

      setSuccess('Transação deletada!')
      loadTransactions()
    } catch (err) {
      setError('Erro ao deletar transação')
      console.error(err)
    } finally {
      setLoading(false)
    }
  }

  const handleFilterChange = (key: string, value: any) => {
    setFilters(prev => ({ ...prev, [key]: value, page: 1 }))
  }

  const handleCategoryFilterToggle = (categoryId: number) => {
    setFilters(prev => {
      const ids = prev.categoryIds.includes(categoryId)
        ? prev.categoryIds.filter(id => id !== categoryId)
        : [...prev.categoryIds, categoryId]
      return { ...prev, categoryIds: ids, page: 1 }
    })
  }

  // Formatadores
  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(value)
  }

  const formatDate = (date: string) => {
    return new Date(date).toLocaleDateString('pt-BR')
  }

  const getTypeColor = (type: string) => {
    return type === 'Income' ? 'success' : 'error'
  }

  const getTypeLabel = (type: string) => {
    return type === 'Income' ? 'Receita' : 'Despesa'
  }

  return (
    <Box>
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
      >
        {/* Header */}
        <Box sx={{ mb: 4, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Box>
            <Typography variant="h4" component="h1" gutterBottom>
              Transações
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Gerencie suas receitas e despesas.
            </Typography>
          </Box>
          <Button
            variant="contained"
            color="primary"
            startIcon={<AddIcon />}
            onClick={() => handleOpenDialog()}
          >
            Nova Transação
          </Button>
        </Box>

        {/* Alerts */}
        {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>{error}</Alert>}
        {success && <Alert severity="success" sx={{ mb: 2 }} onClose={() => setSuccess(null)}>{success}</Alert>}

        {/* Filtros */}
        <Card sx={{ mb: 3 }}>
          <CardContent>
            <Typography variant="h6" sx={{ mb: 2 }}>
              <FilterListIcon sx={{ mr: 1, verticalAlign: 'middle' }} />
              Filtros
            </Typography>

            <Grid container spacing={2}>
              <Grid item xs={12} sm={6} md={3}>
                <TextField
                  label="Data Inicial"
                  type="date"
                  value={filters.startDate}
                  onChange={e => handleFilterChange('startDate', e.target.value)}
                  fullWidth
                  InputLabelProps={{ shrink: true }}
                />
              </Grid>

              <Grid item xs={12} sm={6} md={3}>
                <TextField
                  label="Data Final"
                  type="date"
                  value={filters.endDate}
                  onChange={e => handleFilterChange('endDate', e.target.value)}
                  fullWidth
                  InputLabelProps={{ shrink: true }}
                />
              </Grid>

              <Grid item xs={12} md={6}>
                <Typography variant="body2" sx={{ mb: 1 }}>
                  Categorias
                </Typography>
                <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap' }}>
                  {categories.map(cat => (
                    <Chip
                      key={cat.id}
                      label={cat.name}
                      onClick={() => handleCategoryFilterToggle(cat.id)}
                      color={filters.categoryIds.includes(cat.id) ? 'primary' : 'default'}
                      variant={filters.categoryIds.includes(cat.id) ? 'filled' : 'outlined'}
                      sx={{ bgcolor: filters.categoryIds.includes(cat.id) ? cat.color : 'transparent' }}
                    />
                  ))}
                </Box>
              </Grid>
            </Grid>
          </CardContent>
        </Card>

        {/* Tabela de Transações */}
        <Card>
          <CardContent sx={{ p: 0 }}>
            {loading ? (
              <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
                <CircularProgress />
              </Box>
            ) : (
              <>
                <TableContainer>
                  <Table>
                    <TableHead sx={{ bgcolor: '#f5f5f5' }}>
                      <TableRow>
                        <TableCell>Data</TableCell>
                        <TableCell>Descrição</TableCell>
                        <TableCell>Categoria</TableCell>
                        <TableCell>Tipo</TableCell>
                        <TableCell align="right">Valor</TableCell>
                        <TableCell align="center">Ações</TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {transactions.length > 0 ? (
                        transactions.map(transaction => (
                          <motion.tr
                            key={transaction.id}
                            initial={{ opacity: 0, x: -20 }}
                            animate={{ opacity: 1, x: 0 }}
                            transition={{ duration: 0.3 }}
                          >
                            <TableRow>
                              <TableCell>{formatDate(transaction.date)}</TableCell>
                              <TableCell>{transaction.description}</TableCell>
                              <TableCell>
                                <Chip
                                  label={transaction.categoryName}
                                  size="small"
                                  sx={{ bgcolor: transaction.categoryColor, color: '#fff' }}
                                />
                              </TableCell>
                              <TableCell>
                                <Chip
                                  label={getTypeLabel(transaction.type)}
                                  size="small"
                                  color={getTypeColor(transaction.type) as any}
                                  variant="outlined"
                                />
                              </TableCell>
                              <TableCell align="right">
                                <Typography
                                  sx={{
                                    color: transaction.type === 'Income' ? '#4caf50' : '#f44336',
                                    fontWeight: 'bold',
                                  }}
                                >
                                  {transaction.type === 'Income' ? '+' : '-'}
                                  {formatCurrency(transaction.amount)}
                                </Typography>
                              </TableCell>
                              <TableCell align="center">
                                <IconButton
                                  size="small"
                                  color="primary"
                                  onClick={() => handleOpenDialog(transaction)}
                                >
                                  <EditIcon fontSize="small" />
                                </IconButton>
                                <IconButton
                                  size="small"
                                  color="error"
                                  onClick={() => handleDeleteTransaction(transaction.id)}
                                >
                                  <DeleteIcon fontSize="small" />
                                </IconButton>
                              </TableCell>
                            </TableRow>
                          </motion.tr>
                        ))
                      ) : (
                        <TableRow>
                          <TableCell colSpan={6} align="center" sx={{ py: 4 }}>
                            <Typography color="text.secondary">
                              Nenhuma transação encontrada
                            </Typography>
                          </TableCell>
                        </TableRow>
                      )}
                    </TableBody>
                  </Table>
                </TableContainer>

                {/* Paginação */}
                {pagination.totalPages > 1 && (
                  <Box sx={{ display: 'flex', justifyContent: 'center', p: 2 }}>
                    <Pagination
                      count={pagination.totalPages}
                      page={pagination.currentPage}
                      onChange={(_, page) => handleFilterChange('page', page)}
                    />
                  </Box>
                )}
              </>
            )}
          </CardContent>
        </Card>
      </motion.div>

      {/* Dialog de Criar/Editar */}
      <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
        <DialogTitle>
          {editingId ? 'Editar Transação' : 'Nova Transação'}
        </DialogTitle>
        <DialogContent sx={{ pt: 3 }}>
          <Grid container spacing={2}>
            <Grid item xs={12}>
              <TextField
                label="Descrição"
                value={formData.description}
                onChange={e => setFormData({ ...formData, description: e.target.value })}
                fullWidth
                multiline
                rows={2}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControl fullWidth>
                <InputLabel>Tipo</InputLabel>
                <Select
                  value={formData.type}
                  onChange={e => setFormData({ ...formData, type: e.target.value as any })}
                >
                  <MenuItem value="Income">Receita</MenuItem>
                  <MenuItem value="Expense">Despesa</MenuItem>
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControl fullWidth>
                <InputLabel>Categoria</InputLabel>
                <Select
                  value={formData.categoryId}
                  onChange={e => setFormData({ ...formData, categoryId: e.target.value })}
                >
                  {categories.map(cat => (
                    <MenuItem key={cat.id} value={cat.id}>
                      {cat.name}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                label="Valor"
                type="number"
                value={formData.amount}
                onChange={e => setFormData({ ...formData, amount: e.target.value })}
                fullWidth
                inputProps={{ step: '0.01', min: '0' }}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                label="Data"
                type="date"
                value={formData.date}
                onChange={e => setFormData({ ...formData, date: e.target.value })}
                fullWidth
                InputLabelProps={{ shrink: true }}
              />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>Cancelar</Button>
          <Button onClick={handleSaveTransaction} variant="contained" color="primary" disabled={loading}>
            {loading ? <CircularProgress size={24} /> : 'Salvar'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

export default TransactionsPage