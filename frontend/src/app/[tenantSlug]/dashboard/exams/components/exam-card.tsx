import { Button } from '@/components/ui/button'
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'
import { format, parseISO } from 'date-fns'
import { arEG } from 'date-fns/locale'
import { Link, useParams } from 'react-router'
import type { GroupSummaryDto } from '../../groups/groups.type'
import type { ExamDto } from '../exams.type'
import RemoveExamDialog from './remove-exam-dialog'
import UpdateExamDialog from './update-exam-dialog'

interface IProps {
  exam: ExamDto
  groups: GroupSummaryDto[]
  slug: string
}

export default function ExamCard({ exam, groups, slug }: IProps) {
  const { tenantSlug } = useParams()

  const formattedDate = exam.date
    ? format(parseISO(exam.date), 'd MMMM yyyy', { locale: arEG })
    : 'بدون تاريخ'

  return (
    <Card>
      <CardHeader>
        <div className='flex items-start justify-between gap-2'>
          <CardTitle className='line-clamp-1'>{exam.title}</CardTitle>
          <div className='flex items-center gap-1 shrink-0'>
            <UpdateExamDialog exam={exam} groups={groups} slug={slug} />
            <RemoveExamDialog exam={exam} slug={slug} />
          </div>
        </div>
        <CardDescription>
          {exam.grade} - {exam.groupName ?? 'كل المجموعات'}
        </CardDescription>
      </CardHeader>

      <CardContent className='flex justify-between text-sm text-muted-foreground'>
        <span>{formattedDate}</span>
        <span>{exam.maxScore} درجة</span>
      </CardContent>

      <CardFooter>
        <Button
          variant='outline'
          size='sm'
          className='w-full'
          render={<Link to={`/${tenantSlug}/exams/${exam.id}/sheet`} />}
        >
          رصد الدرجات
        </Button>
      </CardFooter>
    </Card>
  )
}
