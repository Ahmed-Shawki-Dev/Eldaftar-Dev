import AppHeader from '@/components/app-header'
import { AppSidebar } from '@/components/app-sidebar'
import { SidebarProvider } from '@/components/ui/sidebar'
import { Outlet } from 'react-router'

export default function DashboardLayout() {
  return (
    <SidebarProvider>
      <AppSidebar />
      <div className='flex flex-1 flex-col min-w-0 min-h-screen'>
        <AppHeader />
        <main className='flex-1 p-6 overflow-y-auto'>
          <Outlet />
        </main>
      </div>
    </SidebarProvider>
  )
}
