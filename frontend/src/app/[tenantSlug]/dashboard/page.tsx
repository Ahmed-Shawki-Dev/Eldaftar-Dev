import PageContainer from '@/components/page-container'
import { LayoutIcon } from '@phosphor-icons/react'
import { useQuery } from '@tanstack/react-query'
import { useParams } from 'react-router'
import DashboardStatsSection from './components/dashboard-stats-section'
import MonthlyRevenueChart from './components/monthly-revenue-chart'
import UpcomingSessionsCard from './components/upcoming-sessions-card'
import { dashboardApi } from './dashboard.api'

export default function DashboardPage() {
  const { tenantSlug } = useParams()
  const { data } = useQuery({
    queryKey: ['dashboard', tenantSlug],
    queryFn: () => dashboardApi.getDashboardData(tenantSlug!),
    enabled: !!tenantSlug,
  })

  console.log(data)

  return (
    <PageContainer
      title='الصفحة الرئيسية'
      description='نظرة عامة على نشاطك وأرقامك الفعلية'
      icon={<LayoutIcon size={32} />}
    >
      <DashboardStatsSection stats={data?.data?.stats} />
      <div className='flex flex-col md:flex-row w-full gap-4 flex-1'>
        <UpcomingSessionsCard sessions={data?.data?.todaySessions} />
        <MonthlyRevenueChart data={data?.data?.monthlyRevenue} />
      </div>
    </PageContainer>
  )
}
