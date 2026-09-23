import { apiClient } from '@/lib/api-client'
import type { CreateExamSchemaType, UpdateExamSchemaType } from './exam.schema'
import type { ExamDto, ExamParamsDto, ExamSheetDto, StudentScoreDto } from './exams.type'

export const examsApi = {
  getAllExams: (slug: string, filters: ExamParamsDto) =>
    apiClient.get<ExamDto[]>(`/${slug}/exams`, {
      params: { ...filters },
    }),

  getExamSheet: (slug: string, examId: string) =>
    apiClient.get<ExamSheetDto[]>(`/${slug}/exams/${examId}/sheet`),

  createExam: (slug: string, exam: CreateExamSchemaType) =>
    apiClient.post<ExamDto>(`/${slug}/exams`, exam),

  markExam: (slug: string, examId: string, scores: StudentScoreDto[]) =>
    apiClient.post<null>(`/${slug}/exams/${examId}/sheet`, scores),

  updateExam: (slug: string, examId: string, exam: UpdateExamSchemaType) =>
    apiClient.put<ExamDto>(`/${slug}/exams/${examId}`, exam),

  removeExam: (slug: string, examId: string) => apiClient.delete<null>(`/${slug}/exams/${examId}`),
}
