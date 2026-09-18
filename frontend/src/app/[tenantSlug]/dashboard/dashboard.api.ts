import { apiClient } from '@/lib/api-client'
import type { DashboardTeacherDataDto } from './dashboard.type'

export const dashboardApi = {
  getDashboardData: (slug: string) => {
    return apiClient.get<DashboardTeacherDataDto>(`/${slug}/dashboard`)
  },
}
