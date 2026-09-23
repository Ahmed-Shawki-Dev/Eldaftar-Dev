import { useMutation, useQueryClient } from '@tanstack/react-query'
import type { ExamDto } from '../exams.type'

import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from '@/components/ui/alert-dialog'
import { Button } from '@/components/ui/button'
import { toast } from '@/components/ui/toast'
import { TrashIcon } from '@phosphor-icons/react'
import { useState } from 'react'
import { examsApi } from '../exams.api'

interface IProps {
  exam: ExamDto
  slug: string
}

export default function RemoveExamDialog({ exam, slug }: IProps) {
  const [open, setOpen] = useState(false)
  const queryClient = useQueryClient()
  const removeExamMutation = useMutation({
    mutationFn: () => examsApi.removeExam(slug, exam.id),
    onSuccess: (res) => {
      queryClient.invalidateQueries({
        queryKey: ['exams'],
        refetchType: 'active',
      })

      toast.add({
        type: 'success',
        title: res.message ?? 'تم حذف الامتحان بنجاح',
      })
      setOpen(false)
    },
    onError: () => {
      toast.add({
        type: 'error',
        title: 'حدث خطأ أثناء حذف المجموعة',
      })
    },
  })

  return (
    <AlertDialog open={open} onOpenChange={setOpen}>
      <AlertDialogTrigger
        render={
          <Button size={'icon-sm'} variant={'destructive'}>
            <TrashIcon />
          </Button>
        }
      />
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>هل أنت متأكد من حذف الامتحان</AlertDialogTitle>
          <AlertDialogDescription>
            سيتم حذف الامتحان نهائياً من النظام. هذا الإجراء لا يمكن التراجع عنه.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel>الغاء</AlertDialogCancel>
          <AlertDialogAction
            variant='destructive'
            disabled={removeExamMutation.isPending}
            onClick={(e) => {
              e.preventDefault()
              removeExamMutation.mutate()
            }}
          >
            {removeExamMutation.isPending ? 'جاري الحذف...' : 'حذف الامتحان'}
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  )
}
