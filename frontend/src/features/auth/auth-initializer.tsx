import type { ApiResponse, CurrentUserDto } from '@/types/api'
import { useQuery } from '@tanstack/react-query'
import { useEffect } from 'react'
import { Outlet, useParams } from 'react-router'
import { authApi } from './auth.api'
import { useAuthStore } from './auth.store'
export default function AuthInitializer() {
  const { tenantSlug } = useParams()
  const { isSuccess, isError, data } = useQuery<ApiResponse<CurrentUserDto>>({
    queryKey: ['auth', 'me', tenantSlug],
    enabled: !!tenantSlug,
    queryFn: () => authApi.getMe(tenantSlug!),
    retry: false,
  })

  const { setAuthUser, clearAuth } = useAuthStore()

  useEffect(() => {
    if (!tenantSlug) {
      clearAuth()
      return
    }

    if (isSuccess && data?.data) {
      setAuthUser(data.data)
    }

    if (isError) {
      clearAuth()
    }
  }, [tenantSlug, isSuccess, isError, data, setAuthUser, clearAuth])

  return <Outlet />
}
