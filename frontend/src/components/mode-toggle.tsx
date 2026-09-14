import { useTheme } from '@/components/theme-provider'
import { Button } from '@/components/ui/button'
import { MoonIcon, SunIcon } from '@phosphor-icons/react'

export function ModeToggle() {
  const { theme, setTheme } = useTheme()

  function handleToggle() {
    const nextTheme = theme === 'dark' ? 'light' : 'dark'
    setTheme(nextTheme)
  }

  return (
    <Button variant='outline' size='icon' onClick={handleToggle} aria-label='تبديل الثيم'>
      {theme === 'dark' ? (
        <SunIcon className='h-4 w-4 text-yellow-500 transition-all' />
      ) : (
        <MoonIcon className='h-4 w-4 text-slate-700 transition-all' />
      )}
    </Button>
  )
}
