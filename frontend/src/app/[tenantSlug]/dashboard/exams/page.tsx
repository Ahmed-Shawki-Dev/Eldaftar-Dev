import DashboardPagination from '@/components/dashboard-pagination'
import DashboardSearchWithFilter from '@/components/dashboard-search-with-filter'
import PageContainer from '@/components/page-container'
import { GRADES } from '@/lib/utils'
import { ExamIcon } from '@phosphor-icons/react'
import { useQuery } from '@tanstack/react-query'
import { useParams, useSearchParams } from 'react-router'
import { groupsApi } from '../groups/groups.api'
import CreateExamDialog from './components/create-exam-dialog'
import ExamList from './components/exam-list'
import { examsApi } from './exams.api'

export default function ExamsPage() {
  const { tenantSlug } = useParams()
  const [searchParams] = useSearchParams()
  const searchTerm = searchParams.get('searchTerm') ?? undefined
  const pageSize = Number(searchParams.get('pageSize')) || 12
  const pageNumber = Number(searchParams.get('pageNumber')) || 1
  const grade = searchParams.get('grade') ?? undefined
  const examQuery = useQuery({
    queryKey: ['exams', tenantSlug, searchTerm, pageNumber, grade],
    enabled: !!tenantSlug,
    queryFn: () =>
      examsApi.getAllExams(tenantSlug as string, { searchTerm, pageSize, pageNumber, grade }),
  })
  const groupQuery = useQuery({
    queryKey: ['groups', tenantSlug],
    enabled: !!tenantSlug,
    queryFn: () => groupsApi.getAllGroups(tenantSlug as string),
  })
  return (
    <PageContainer
      title='قائمة الامتحانات'
      description='نظرة عامة على الامتحانات ودرجاتها'
      icon={<ExamIcon size={32} />}
      action={<CreateExamDialog slug={tenantSlug ?? ''} groups={groupQuery.data?.data ?? []} />}
    >
      <DashboardSearchWithFilter
        searchKey='searchTerm'
        searchPlaceholder='ابحث باسم المجموعة...'
        filterKey='grade'
        filterPlaceholder='اختار الصف الدراسي...'
        filterOptions={GRADES}
      />
      <ExamList
        exams={examQuery.data?.data ?? []}
        slug={tenantSlug ?? ''}
        groups={groupQuery.data?.data ?? []}
      />
      <div className='self-start'>
        <DashboardPagination paginationMetadata={examQuery.data?.pagination} />
      </div>
    </PageContainer>
  )
}
