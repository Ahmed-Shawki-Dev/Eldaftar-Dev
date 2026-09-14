'use client'

import { Button } from '@/components/ui/button'
import { IconMoon, IconSun } from '@tabler/icons-react'
import { useTheme } from 'next-themes'

export function ModeToggle() {
  const { theme, setTheme } = useTheme()

  return (
    <Button
      variant='secondary'
      size='icon'
      className='relative rounded-full size-9'
      onClick={() => setTheme(theme === 'dark' ? 'light' : 'dark')}
      aria-label='تبديل المظهر'
    >
      <IconMoon className='size-[1.2rem] scale-100 rotate-0 transition-all dark:scale-0 dark:-rotate-90' />
      <IconSun className='absolute size-[1.2rem] scale-0 rotate-90 transition-all dark:scale-100 dark:rotate-0' />
      <span className='sr-only'>تبديل المظهر</span>
    </Button>
  )
}
