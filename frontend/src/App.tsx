import { ThemeProvider } from '@/components/theme-provider'
import { DirectionProvider } from '@/components/ui/direction'

export function App() {
  return (
    <DirectionProvider direction='rtl'>
      <ThemeProvider defaultTheme='dark'>
        <main className='h-screen w-full flex justify-center items-center'>السلام عليكم</main>
      </ThemeProvider>
    </DirectionProvider>
  )
}

export default App
