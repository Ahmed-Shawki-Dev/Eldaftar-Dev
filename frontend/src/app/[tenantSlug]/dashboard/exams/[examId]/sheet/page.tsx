import PageContainer from '@/components/page-container'
import { CheckCircleIcon } from '@phosphor-icons/react'

export default function ExamSheetPage() {
  return (
    <PageContainer
      title='شييت امتحان'
      description='الشييت الخاص بتعليم الامتحان'
      icon={<CheckCircleIcon size={32} />}
    >
      <>Hello World</>
    </PageContainer>
  )
}
