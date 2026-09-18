import { SidebarTrigger } from '@/components/ui/sidebar'
import { ModeToggle } from './mode-toggle'

export default function AppHeader() {
  return (
    <header className='sticky top-0 z-10 flex h-14 w-full items-center border-b bg-sidebar px-4'>
      <nav className='flex items-center gap-2 justify-between w-full'>
        <SidebarTrigger />
        <ModeToggle />
      </nav>
    </header>
  )
}
