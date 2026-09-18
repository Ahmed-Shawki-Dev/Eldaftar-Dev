import type { CurrentUserDto } from '@/types/api'
import { create } from 'zustand'

interface IAuthStore {
  isAuthenticated: boolean
  isLoading: boolean
  currentUserData: CurrentUserDto | null
  setAuthUser: (data: CurrentUserDto) => void
  clearAuth: () => void
}

export const useAuthStore = create<IAuthStore>()((set) => ({
  isAuthenticated: false,
  isLoading: true,
  currentUserData: null,
  setAuthUser: (data) => {
    set(() => ({ currentUserData: data, isAuthenticated: true, isLoading: false }))
  },
  clearAuth: () => set(() => ({ currentUserData: null, isAuthenticated: false, isLoading: false })),
}))
