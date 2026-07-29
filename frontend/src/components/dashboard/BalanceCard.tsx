import React from 'react'

interface BalanceCardProps {
  balance?: number
  title?: string
  isLoading?: boolean
  animate?: boolean
  trend?: 'up' | 'down'
  icon?: React.ReactNode
}

const BalanceCard: React.FC<BalanceCardProps> = ({ balance = 0, title = 'Saldo', isLoading = false }) => {
  return (
    <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
      <div className="flex justify-between items-start">
        <div>
          <p className="text-sm font-medium text-gray-600 dark:text-gray-400">{title}</p>
          <p className="text-3xl font-bold text-gray-900 dark:text-white mt-2">
            {isLoading ? 'Carregando...' : new Intl.NumberFormat('pt-BR', {
              style: 'currency',
              currency: 'BRL',
            }).format(balance)}
          </p>
        </div>
      </div>
    </div>
  )
}

export default BalanceCard
