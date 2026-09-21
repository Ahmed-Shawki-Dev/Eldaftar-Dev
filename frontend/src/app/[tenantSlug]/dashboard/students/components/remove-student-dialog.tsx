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
import { studentsApi } from '../students.api'
import type { StudentDto } from '../students.type'

interface IProps {
  slug: string
  student: StudentDto
  open: boolean
  onOpenChange: React.Dispatch<React.SetStateAction<boolean>>
}

export default function RemoveStudentDialog({ student, onOpenChange, open, slug }: IProps) {
  const queryClient = useQueryClient()
  const removeStudentMutation = useMutation({
    mutationFn: () => studentsApi.removeStudent(slug, student.id),
    onSuccess: (res) => {
      queryClient.invalidateQueries({
        queryKey: ['students', slug],
        refetchType: 'active',
      })

      toast.add({
        type: 'success',
        title: res.message ?? 'تم حذف الطالب بنجاح',
      })
      onOpenChange(false)
    },
    onError: () => {
      toast.add({
        type: 'error',
        title: 'حدث خطأ أثناء حذف الطالب',
      })
    },
  })

  return (
    <AlertDialog open={open} onOpenChange={onOpenChange}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>هل أنت متأكد من حذف الطالب؟</AlertDialogTitle>
          <AlertDialogDescription>
            سيتم حذف الطالب نهائياً من النظام. هذا الإجراء لا يمكن التراجع عنه.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel>إلغاء</AlertDialogCancel>
          <AlertDialogAction
            variant='destructive'
            disabled={removeStudentMutation.isPending}
            onClick={(e) => {
              e.preventDefault()
              removeStudentMutation.mutate()
            }}
          >
            {removeStudentMutation.isPending ? 'جاري الحذف...' : 'حذف الطالب'}
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  )
}
