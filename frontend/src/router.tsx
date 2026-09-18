import DashboardLayout from '@/app/[tenantSlug]/dashboard/layout'
import EldaftarPage from '@/app/page'
import { createBrowserRouter } from 'react-router'
import AttendancePage from './app/[tenantSlug]/dashboard/attendance/page'
import ExamsPage from './app/[tenantSlug]/dashboard/exams/page'
import GroupsPage from './app/[tenantSlug]/dashboard/groups/page'
import DashboardPage from './app/[tenantSlug]/dashboard/page'
import PaymentsPage from './app/[tenantSlug]/dashboard/payments/page'
import ReportsPage from './app/[tenantSlug]/dashboard/reports/page'
import SettingsPage from './app/[tenantSlug]/dashboard/settings/page'
import StaffPage from './app/[tenantSlug]/dashboard/staff/page'
import StudentsPage from './app/[tenantSlug]/dashboard/students/page'
import LoginPage from './app/[tenantSlug]/login/page'
import TenantLandingPage from './app/[tenantSlug]/page'
import AuthGuard from './features/auth/auth-guard'
import AuthInitializer from './features/auth/auth-initializer'
import PublicGuard from './features/auth/public-guard'

export const router = createBrowserRouter([
  {
    path: '/',
    children: [
      { index: true, Component: EldaftarPage },
      {
        path: ':tenantSlug',
        Component: AuthInitializer,
        children: [
          {
            Component: AuthGuard,
            children: [
              {
                path: 'dashboard',
                Component: DashboardLayout,
                children: [
                  { index: true, Component: DashboardPage },
                  { path: 'attendance', Component: AttendancePage },
                  { path: 'groups', Component: GroupsPage },
                  { path: 'students', Component: StudentsPage },
                  { path: 'payments', Component: PaymentsPage },
                  { path: 'exams', Component: ExamsPage },
                  { path: 'staff', Component: StaffPage },
                  { path: 'reports', Component: ReportsPage },
                  { path: 'settings', Component: SettingsPage },
                ],
              },
            ],
          },
          {
            Component: PublicGuard,
            children: [
              { index: true, Component: TenantLandingPage },
              { path: 'login', Component: LoginPage },
            ],
          },
        ],
      },
    ],
  },
])
