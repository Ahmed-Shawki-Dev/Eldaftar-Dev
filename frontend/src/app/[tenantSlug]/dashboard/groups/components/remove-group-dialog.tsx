import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog'
import { toast } from '@/components/ui/toast'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { groupsApi } from '../groups.api'
import type { GroupSummaryDto } from '../groups.type'

interface IProps {
  slug: string
  group: GroupSummaryDto
  open: boolean
  onOpenChange: React.Dispatch<React.SetStateAction<boolean>>
}

export default function RemoveGroupDialog({ group, onOpenChange, open, slug }: IProps) {
  const queryClient = useQueryClient()
  const removeGroupMutation = useMutation({
    mutationFn: () => groupsApi.removeGroup(slug, group.id),
    onSuccess: (res) => {
      queryClient.invalidateQueries({
        queryKey: ['groups'],
        refetchType: 'active',
      })

      toast.add({
        type: 'success',
        title: res.message ?? 'تم حذف المجموعة بنجاح',
      })
      onOpenChange(false)
    },
    onError: () => {
      toast.add({
        type: 'error',
        title: 'حدث خطأ أثناء حذف المجموعة',
      })
    },
  })

  return (
    <AlertDialog open={open} onOpenChange={onOpenChange}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>هل أنت متأكد من حذف المجموعة؟</AlertDialogTitle>
          <AlertDialogDescription>
            سيتم حذف المجموعة نهائياً من النظام. هذا الإجراء لا يمكن التراجع عنه.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel>إلغاء</AlertDialogCancel>
          <AlertDialogAction
            variant='destructive'
            disabled={removeGroupMutation.isPending}
            onClick={(e) => {
              e.preventDefault()
              removeGroupMutation.mutate()
            }}
          >
            {removeGroupMutation.isPending ? 'جاري الحذف...' : 'حذف المجموعة'}
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  )
}
