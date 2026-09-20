import { apiClient } from '@/lib/api-client'
import type { CreateGroupSchemaType, UpdateGroupSchemaType } from './groups.schema'
import type { GroupParamsDto, GroupSummaryDto } from './groups.type'

export const groupsApi = {
  getAllGroups: (slug: string, filters?: GroupParamsDto) =>
    apiClient.get<GroupSummaryDto[]>(`/${slug}/groups`, {
      params: filters,
    }),
  createGroup: (slug: string, data: CreateGroupSchemaType) =>
    apiClient.post<GroupSummaryDto>(`/${slug}/groups`, data),
  updateGroup: (slug: string, groupId: string, data: UpdateGroupSchemaType) =>
    apiClient.put<GroupSummaryDto>(`/${slug}/groups/${groupId}`, data),
  removeGroup: (slug: string, groupId: string) =>
    apiClient.delete<null>(`/${slug}/groups/${groupId}`),
}
