import React from 'react'
import { useNavigate, useLocation } from 'react-router-dom'
import {
  Drawer,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Box,
  useTheme,
  useMediaQuery,
} from '@mui/material'
import {
  Dashboard,
  Receipt,
  Assessment,
  TrackChanges,
  Settings,
  Home,
} from '@mui/icons-material'

interface Props {
  open: boolean
  onClose: () => void
}

const menuItems = [
  { path: '/', label: 'Início', icon: Home },
  { path: '/dashboard', label: 'Dashboard', icon: Dashboard },
  { path: '/transactions', label: 'Transações', icon: Receipt },
  { path: '/reports', label: 'Relatórios', icon: Assessment },
  { path: '/goals', label: 'Metas', icon: TrackChanges },
  { path: '/settings', label: 'Configurações', icon: Settings },
]

const drawerWidth = 240

const Sidebar: React.FC<Props> = ({ open, onClose }) => {
  const navigate = useNavigate()
  const location = useLocation()
  const theme = useTheme()
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'))

  const handleNavigation = (path: string) => {
    navigate(path)
    if (isMobile) {
      onClose()
    }
  }

  const drawer = (
    <Box>
      <Box sx={{ height: 64 }} /> {/* AppBar height spacer */}
      <List>
        {menuItems.map((item) => {
          const Icon = item.icon
          const isActive = location.pathname === item.path
          
          return (
            <ListItem key={item.path} disablePadding>
              <ListItemButton
                onClick={() => handleNavigation(item.path)}
                selected={isActive}
                sx={{
                  mx: 1,
                  borderRadius: 1,
                  '&.Mui-selected': {
                    bgcolor: 'primary.main',
                    color: 'primary.contrastText',
                    '&:hover': {
                      bgcolor: 'primary.dark',
                    },
                    '& .MuiListItemIcon-root': {
                      color: 'primary.contrastText',
                    },
                  },
                }}
              >
                <ListItemIcon>
                  <Icon />
                </ListItemIcon>
                <ListItemText primary={item.label} />
              </ListItemButton>
            </ListItem>
          )
        })}
      </List>
    </Box>
  )

  return (
    <Drawer
      variant={isMobile ? 'temporary' : 'permanent'}
      open={isMobile ? open : true}
      onClose={onClose}
      sx={{
        width: drawerWidth,
        flexShrink: 0,
        '& .MuiDrawer-paper': {
          width: drawerWidth,
          boxSizing: 'border-box',
        },
      }}
    >
      {drawer}
    </Drawer>
  )
}

export default Sidebar