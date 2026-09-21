import { apiClient } from '@/lib/api-client'
import type { CreateStudentSchemaType, UpdateStudentSchemaType } from './students.schema'
import type { StudentDto, StudentParamsDto } from './students.type'

export const studentsApi = {
  getAllStudents: (slug: string, filters: StudentParamsDto) =>
    apiClient.get<StudentDto[]>(`/${slug}/students`, {
      params: filters,
    }),
  getStudent: (slug: string, studentId: string) =>
    apiClient.get<StudentDto>(`/${slug}/students/${studentId}`),
  createStudent: (slug: string, data: CreateStudentSchemaType) =>
    apiClient.post(`/${slug}/students`, data),
  updateStudent: (slug: string, studentId: string, data: UpdateStudentSchemaType) =>
    apiClient.put(`/${slug}/students/${studentId}`, data),
  removeStudent: (slug: string, studentId: string) =>
    apiClient.delete(`/${slug}/students/${studentId}`),
}
