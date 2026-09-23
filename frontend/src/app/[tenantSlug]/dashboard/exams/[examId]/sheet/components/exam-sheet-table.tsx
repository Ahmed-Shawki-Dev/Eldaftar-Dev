import type { ExamSheetDto } from '../../../exams.type'

interface IProps {
  sheet: ExamSheetDto[]
}

export default function ExamSheetTable({ sheet }: IProps) {
  return <div>{JSON.stringify(sheet)}</div>
}
