import { useAuthStore } from '@/features/auth/auth.store'
import { Navigate, Outlet, useParams } from 'react-router'

export default function AuthGuard() {
  const { tenantSlug } = useParams()
  const { isAuthenticated, isLoading, currentUserData } = useAuthStore()

  if (isLoading) {
    return <div>Loading...</div>
  }

  if (!isAuthenticated || currentUserData?.tenantSlug !== tenantSlug) {
    return <Navigate replace to={`/${tenantSlug}/login`} />
  }

  return <Outlet />
}
