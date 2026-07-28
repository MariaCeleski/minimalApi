import React, { createContext, useContext, useState, useCallback } from 'react'
import { Transaction, Category, TransactionFilters } from '../types'

interface TransactionContextType {
  transactions: Transaction[]
  categories: Category[]
  filters: TransactionFilters
  setTransactions: (transactions: Transaction[]) => void
  setCategories: (categories: Category[]) => void
  setFilters: (filters: TransactionFilters) => void
  addTransaction: (transaction: Transaction) => void
  updateTransaction: (id: string, transaction: Transaction) => void
  deleteTransaction: (id: string) => void
  refreshTransactions: () => void
  refreshCategories: () => void
}

const TransactionContext = createContext<TransactionContextType | undefined>(undefined)

export const useTransactions = () => {
  const context = useContext(TransactionContext)
  if (!context) {
    throw new Error('useTransactions must be used within a TransactionContextProvider')
  }
  return context
}

interface Props {
  children: React.ReactNode
}

export const TransactionContextProvider: React.FC<Props> = ({ children }) => {
  const [transactions, setTransactions] = useState<Transaction[]>([])
  const [categories, setCategories] = useState<Category[]>([])
  const [filters, setFilters] = useState<TransactionFilters>({})

  const addTransaction = useCallback((transaction: Transaction) => {
    setTransactions(prev => [...prev, transaction])
  }, [])

  const updateTransaction = useCallback((id: string, updatedTransaction: Transaction) => {
    setTransactions(prev => 
      prev.map(t => t.id === id ? updatedTransaction : t)
    )
  }, [])

  const deleteTransaction = useCallback((id: string) => {
    setTransactions(prev => prev.filter(t => t.id !== id))
  }, [])

  const refreshTransactions = useCallback(() => {
    // This will be implemented when we have the API service
    console.log('Refreshing transactions...')
  }, [])

  const refreshCategories = useCallback(() => {
    // This will be implemented when we have the API service
    console.log('Refreshing categories...')
  }, [])

  const value: TransactionContextType = {
    transactions,
    categories,
    filters,
    setTransactions,
    setCategories,
    setFilters,
    addTransaction,
    updateTransaction,
    deleteTransaction,
    refreshTransactions,
    refreshCategories,
  }

  return (
    <TransactionContext.Provider value={value}>
      {children}
    </TransactionContext.Provider>
  )
}