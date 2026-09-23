export interface CreateExamDto {
  title: string
  grade: string
  maxScore: number
  date?: string | null
  groupId?: string | null
}

export interface UpdateExamDto {
  title: string
  grade: string
  maxScore: number
  date: string
  groupId?: string | null
}

export interface ExamDto {
  id: string
  title: string
  grade: string
  maxScore: number
  date: string
  groupId?: string | null
  groupName?: string | null
  createdAt: string
}

export interface ExamSheetDto {
  studentId: string
  studentName: string
  score?: number | null
  percentage?: number | null
}

export interface StudentScoreDto {
  studentId: string
  score?: number | null
}

export interface UpdateExamSheetDto {
  studentsScores: StudentScoreDto[]
}

export interface ExamParamsDto {
  pageNumber?: number
  pageSize?: number
  searchTerm?: string
  grade?: string
  groupId?: string
}
