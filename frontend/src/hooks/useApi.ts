import { useState, useEffect } from 'react'
import { ErrorResponse } from '../types'

interface UseApiOptions {
  immediate?: boolean
}

interface UseApiState<T> {
  data: T | null
  loading: boolean
  error: ErrorResponse | null
}

export function useApi<T>(
  apiCall: () => Promise<T>,
  options: UseApiOptions = { immediate: true }
): UseApiState<T> & { refetch: () => Promise<void> } {
  const [state, setState] = useState<UseApiState<T>>({
    data: null,
    loading: false,
    error: null,
  })

  const fetchData = async () => {
    setState(prev => ({ ...prev, loading: true, error: null }))
    
    try {
      const result = await apiCall()
      setState({ data: result, loading: false, error: null })
    } catch (error) {
      setState({
        data: null,
        loading: false,
        error: error as ErrorResponse,
      })
    }
  }

  useEffect(() => {
    if (options.immediate) {
      fetchData()
    }
  }, [])

  return {
    ...state,
    refetch: fetchData,
  }
}