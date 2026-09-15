import { ThemeProvider } from '@/components/theme-provider'
import { DirectionProvider } from '@/components/ui/direction'
import { router } from '@/router'
import { RouterProvider } from 'react-router'

export function App() {
  return (
    <DirectionProvider direction='rtl'>
      <ThemeProvider defaultTheme='dark'>
        <RouterProvider router={router} />
      </ThemeProvider>
    </DirectionProvider>
  )
}

export default App
