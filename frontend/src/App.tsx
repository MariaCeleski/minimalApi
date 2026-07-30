import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import { CssBaseline } from '@mui/material'
import { ThemeContextProvider } from './context/ThemeContext'
import { TransactionContextProvider } from './context/TransactionContext'
import { AppContextProvider } from './context/AppContext'
import Layout from './components/layout/Layout'
import HomePage from './pages/HomePage'
import TransactionsPage from './pages/TransactionsPage'
import DashboardPage from './pages/DashboardPage'
import ReportsPage from './pages/ReportsPage'
import GoalsPage from './pages/GoalsPage'
import SettingsPage from './pages/SettingsPage'

function App() {
  return (
    <AppContextProvider>
      <ThemeContextProvider>
        <TransactionContextProvider>
          <Router>
            <CssBaseline />
            <Layout>
              <Routes>
                <Route path="/" element={<HomePage />} />
                <Route path="/dashboard" element={<DashboardPage />} />
                <Route path="/transactions" element={<TransactionsPage />} />
                <Route path="/reports" element={<ReportsPage />} />
                <Route path="/goals" element={<GoalsPage />} />
                <Route path="/settings" element={<SettingsPage />} />
              </Routes>
            </Layout>
          </Router>
        </TransactionContextProvider>
      </ThemeContextProvider>
    </AppContextProvider>
  )
}

export default App