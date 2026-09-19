import PageContainer from '@/components/page-container'
import { ClockIcon } from '@phosphor-icons/react'
import { useQuery } from '@tanstack/react-query'
import { useParams } from 'react-router'
import DashboardGroupsTable from './components/dashboard-groups-table'
import { groupsApi } from './groups.api'

export default function GroupsPage() {
  const { tenantSlug } = useParams()

  const query = useQuery({
    queryKey: ['groups', tenantSlug],
    enabled: !!tenantSlug,
    queryFn: () => groupsApi.getAllGroups(tenantSlug as string),
  })

  return (
    <PageContainer
      title='المجاميع'
      description='نظرة عامة على المجموعات الدراسية'
      icon={<ClockIcon size={32} />}
    >
      {/* <div className='flex'></div> */}
      <DashboardGroupsTable groups={query.data?.data ?? []} />
    </PageContainer>
  )
}
