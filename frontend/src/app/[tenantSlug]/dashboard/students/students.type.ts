export interface CreateStudentDto {
  name: string
  parentPhone: string
  phone?: string
  groupId: string
  customPrice?: number
}

export interface UpdateStudentDto {
  name: string
  parentPhone: string
  phone?: string
  groupId: string
  customPrice?: number | null
}

export interface StudentDto {
  id: string
  studentCode: string
  name: string
  parentPhone: string
  phone?: string
  groupId: string
  groupName: string
  customPrice: string
}

export class StudentParamsDto {
  pageNumber?: number
  pageSize?: number
  searchTerm?: string
  groupId?: string
}
