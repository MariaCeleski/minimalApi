# Personal Financial Management Frontend

This is the React + TypeScript frontend for the Personal Financial Management application.

## Features

- **React 18** with TypeScript
- **Vite** as build tool
- **Material UI** for components
- **TailwindCSS** for styling
- **Framer Motion** for animations
- **Recharts** for data visualization
- **Axios** for API calls
- **React Router DOM** for routing
- **Context API** for state management
- **Dark/Light theme** support
- **Responsive design** for all devices

## Getting Started

1. Install dependencies:
```bash
npm install
```

2. Start development server:
```bash
npm run dev
```

3. Build for production:
```bash
npm run build
```

## Project Structure

```
src/
├── components/          # Reusable components
│   ├── layout/         # Layout components
│   ├── common/         # Common UI components
│   ├── dashboard/      # Dashboard specific components
│   ├── transactions/   # Transaction components
│   └── ...
├── pages/              # Page components
├── context/            # React Context providers
├── services/           # API services
├── hooks/              # Custom hooks
├── types/              # TypeScript type definitions
└── utils/              # Utility functions
```

## Environment Variables

Copy `.env.example` to `.env.local` and configure:

```bash
VITE_API_BASE_URL=http://localhost:5000/api
```

## Dependencies

### Core Dependencies
- react, react-dom
- typescript
- vite
- react-router-dom

### UI Libraries
- @mui/material, @mui/icons-material
- @emotion/react, @emotion/styled
- tailwindcss
- framer-motion

### Data & API
- axios
- recharts

### Development
- eslint
- @typescript-eslint/*
- @vitejs/plugin-react