import {
  Sidebar,
  SidebarContent,
  SidebarGroup,
  SidebarGroupContent,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from '@/components/ui/sidebar'
import { navItems } from '@/config/nav'
import { NavLink } from 'react-router'

export function AppSidebar() {
  return (
    <Sidebar side='right' collapsible='icon' className='border-l border-border'>
      <SidebarHeader className='flex h-14 items-center justify-center border-b border-l px-4 text-lg font-bold'>
        Test
      </SidebarHeader>

      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupContent>
            <SidebarMenu>
              {navItems.map((item) => (
                <SidebarMenuItem key={item.url} className='mb-1'>
                  <NavLink to={item.url} end={item.url === ''} className='w-full no-underline'>
                    {({ isActive }) => (
                      <SidebarMenuButton
                        isActive={isActive}
                        className={
                          isActive
                            ? 'bg-sidebar-accent text-sidebar-accent-foreground font-semibold'
                            : ''
                        }
                      >
                        <item.icon />
                        <span>{item.title}</span>
                      </SidebarMenuButton>
                    )}
                  </NavLink>
                </SidebarMenuItem>
              ))}
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarContent>
    </Sidebar>
  )
}
