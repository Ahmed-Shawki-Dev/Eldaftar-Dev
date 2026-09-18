import { apiClient } from '@/lib/api-client'
import type { CurrentUserDto } from '@/types/api'
import type { LoginDto } from './auth.type'

export const authApi = {
  getMe: (slug: string) => apiClient.get<CurrentUserDto>(`/${slug}/auth/me`),
  tenantLogin: (slug: string, data: LoginDto) =>
    apiClient.post<CurrentUserDto>(`/${slug}/auth/login`, data),
}
