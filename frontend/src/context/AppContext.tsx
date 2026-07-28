import React, { createContext, useContext, useState } from 'react'

interface AppContextType {
  isLoading: boolean
  setIsLoading: (loading: boolean) => void
  error: string | null
  setError: (error: string | null) => void
  success: string | null
  setSuccess: (success: string | null) => void
}

const AppContext = createContext<AppContextType | undefined>(undefined)

export const useApp = () => {
  const context = useContext(AppContext)
  if (!context) {
    throw new Error('useApp must be used within an AppContextProvider')
  }
  return context
}

interface Props {
  children: React.ReactNode
}

export const AppContextProvider: React.FC<Props> = ({ children }) => {
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)

  const value: AppContextType = {
    isLoading,
    setIsLoading,
    error,
    setError,
    success,
    setSuccess,
  }

  return (
    <AppContext.Provider value={value}>
      {children}
    </AppContext.Provider>
  )
}