export interface PaginationMetadata {
  pageNumber: number
  pageSize: number
  totalPages: number
  totalRecords: number
}

export interface ApiResponse<T> {
  success: boolean
  message: string
  data: T | null
  errors: string[] | null
  pagination: PaginationMetadata | null
}

export interface CurrentUserDto {
  userId: string
  role: string
  tenantSlug: string
  tenantId: string
  teacherId?: string | null
}
