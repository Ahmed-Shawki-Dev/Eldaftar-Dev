import DashboardLayout from '@/app/[tenantSlug]/dashboard/layout'
import RootLayout from '@/app/layout'
import EldaftarPage from '@/app/page'
import { createBrowserRouter } from 'react-router'
import AttendancePage from './app/[tenantSlug]/dashboard/attendance/page'
import DashboardPage from './app/[tenantSlug]/dashboard/page'
import StudentsPage from './app/[tenantSlug]/dashboard/students/page'
import TenantLandingPage from './app/[tenantSlug]/page'

export const router = createBrowserRouter([
  {
    path: '/',
    Component: RootLayout,
    children: [
      { index: true, Component: EldaftarPage },
      {
        path: ':tenantSlug',
        children: [
          { index: true, Component: TenantLandingPage },
          {
            path: 'dashboard',
            Component: DashboardLayout,
            children: [
              { index: true, Component: DashboardPage },
              { path: 'students', Component: StudentsPage },
              { path: 'attendance', Component: AttendancePage },
            ],
          },
        ],
      },
    ],
  },
])
