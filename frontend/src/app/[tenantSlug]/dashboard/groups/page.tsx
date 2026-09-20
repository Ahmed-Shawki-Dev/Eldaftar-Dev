import DashboardPagination from '@/components/dashboard-pagination'
import DashboardSearchWithFilter from '@/components/dashboard-search-with-filter'
import PageContainer from '@/components/page-container'
import { GRADES } from '@/lib/utils'
import { ClockIcon } from '@phosphor-icons/react'
import { useQuery } from '@tanstack/react-query'
import { useParams, useSearchParams } from 'react-router'
import AddGroupDialog from './components/add-group-dialog'
import DashboardGroupsTable from './components/dashboard-groups-table'
import { groupsApi } from './groups.api'

export default function GroupsPage() {
  const { tenantSlug } = useParams()
  const [searchParams] = useSearchParams()
  const searchTerm = searchParams.get('searchTerm') ?? undefined
  const pageSize = Number(searchParams.get('pageSize')) || 10
  const pageNumber = Number(searchParams.get('pageNumber')) || 1
  const grade = searchParams.get('grade') ?? undefined
  const query = useQuery({
    queryKey: ['groups', tenantSlug, searchTerm, pageNumber, grade],
    enabled: !!tenantSlug,
    queryFn: () =>
      groupsApi.getAllGroups(tenantSlug as string, { searchTerm, pageSize, pageNumber, grade }),
  })

  return (
    <PageContainer
      title='المجاميع'
      description='نظرة عامة على المجموعات الدراسية'
      icon={<ClockIcon size={32} />}
      action={<AddGroupDialog slug={tenantSlug ?? ''} />}
    >
      <DashboardSearchWithFilter
        searchKey='searchTerm'
        searchPlaceholder='ابحث باسم المجموعة...'
        filterKey='grade'
        filterPlaceholder='اختار الصف الدراسي...'
        filterOptions={GRADES}
      />
      <DashboardGroupsTable groups={query.data?.data ?? []} slug={tenantSlug ?? ''} />
      <div className='self-start'>
        <DashboardPagination paginationMetadata={query.data?.pagination} />
      </div>
    </PageContainer>
  )
}
