import type { GroupSummaryDto } from '../../groups/groups.type'
import type { ExamDto } from '../exams.type'
import ExamCard from './exam-card'

interface IProps {
  exams: ExamDto[]
  groups: GroupSummaryDto[]
  slug: string
}

export default function ExamList({ exams, groups, slug }: IProps) {
  return (
    <div className='grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4'>
      {exams.map((exam) => (
        <ExamCard exam={exam} groups={groups} key={exam.id} slug={slug} />
      ))}
    </div>
  )
}
