import { ThemeProvider } from '@/components/theme-provider'
import { DirectionProvider } from '@/components/ui/direction'
import { Toaster } from '@/components/ui/toast'
import { router } from '@/router'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { RouterProvider } from 'react-router'

// eslint-disable-next-line react-refresh/only-export-components
export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      staleTime: 1000 * 60 * 5,
    },
  },
})

export function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <DirectionProvider direction='rtl'>
        <ThemeProvider defaultTheme='dark'>
          <RouterProvider router={router} />
          <Toaster />
        </ThemeProvider>
      </DirectionProvider>
    </QueryClientProvider>
  )
}

export default App
