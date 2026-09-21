import DashboardPagination from '@/components/dashboard-pagination'
import DashboardSearchWithFilter from '@/components/dashboard-search-with-filter'
import PageContainer from '@/components/page-container'
import { StudentIcon } from '@phosphor-icons/react'
import { useQuery } from '@tanstack/react-query'
import { useMemo } from 'react'
import { useParams, useSearchParams } from 'react-router'
import { groupsApi } from '../groups/groups.api'
import AddStudentDialog from './components/add-student-dialog'
import DashboardStudentsTable from './components/dashboard-students-table'
import { studentsApi } from './students.api'

export default function StudentsPage() {
  const { tenantSlug } = useParams()
  const [searchParams] = useSearchParams()
  const searchTerm = searchParams.get('searchTerm') ?? undefined
  const pageSize = Number(searchParams.get('pageSize')) || 10
  const pageNumber = Number(searchParams.get('pageNumber')) || 1
  const groupId = searchParams.get('groupId') ?? undefined
  const studentsQuery = useQuery({
    queryKey: ['students', tenantSlug, searchTerm, pageNumber, groupId],
    enabled: !!tenantSlug,
    queryFn: () =>
      studentsApi.getAllStudents(tenantSlug as string, {
        searchTerm,
        pageSize,
        pageNumber,
        groupId,
      }),
  })
  const groupQuery = useQuery({
    queryKey: ['groups'],
    enabled: !!tenantSlug,
    queryFn: () => groupsApi.getAllGroups(tenantSlug as string),
  })

  const groupOptions = useMemo(() => {
    const rawGroups = groupQuery.data?.data ?? []
    return rawGroups.map((g) => ({
      value: g.id,
      label: `${g.grade} - ${g.name}`,
    }))
  }, [groupQuery.data?.data])

  return (
    <PageContainer
      title='الطلاب'
      description='نظرة عامة على الطلاب المسجلين'
      icon={<StudentIcon size={32} />}
      action={<AddStudentDialog slug={tenantSlug ?? ''} groups={groupQuery?.data?.data ?? []} />}
    >
      <DashboardSearchWithFilter
        searchKey='searchTerm'
        searchPlaceholder='ابحث باسم الطالب أو الكود...'
        filterKey='groupId'
        filterPlaceholder='اختار المجموعة...'
        filterOptions={groupOptions}
      />
      <DashboardStudentsTable
        students={studentsQuery.data?.data ?? []}
        slug={tenantSlug ?? ''}
        groups={groupQuery?.data?.data ?? []}
      />
      <div className='self-start'>
        <DashboardPagination paginationMetadata={studentsQuery.data?.pagination} />
      </div>
    </PageContainer>
  )
}
