import { apiClient } from '@/lib/api-client'
import type { GroupParamsDto, GroupSummaryDto } from './groups.type'

export const groupsApi = {
  getAllGroups: (slug: string, filters?: GroupParamsDto) =>
    apiClient.get<GroupSummaryDto[]>(`${slug}/groups`, {
      params: filters,
    }),
}
