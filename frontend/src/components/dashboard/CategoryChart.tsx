import React from 'react'

interface CategoryChartProps {
  data?: any[]
  title?: string
  isLoading?: boolean
}

const CategoryChart: React.FC<CategoryChartProps> = ({ title, isLoading = false }) => {
  return (
    <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
      {title && <h3 className="text-lg font-semibold text-gray-900 dark:text-white mb-4">{title}</h3>}
      <div className="w-full h-64 flex items-center justify-center text-gray-500">
        {isLoading ? <p>Carregando...</p> : <p>Chart Placeholder - Category Chart</p>}
      </div>
    </div>
  )
}

export default CategoryChart
