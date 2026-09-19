import { apiClient } from '@/lib/api-client'
import type { GroupSummaryDto } from './groups.type'

export const groupsApi = {
  getAllGroups: (slug: string) => apiClient.get<GroupSummaryDto[]>(`${slug}/groups`),
}
