import { Button } from '@/components/ui/button'
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog'
import { Field, FieldError, FieldLabel } from '@/components/ui/field'
import { Input } from '@/components/ui/input'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { toast } from '@/components/ui/toast'
import { zodResolver } from '@hookform/resolvers/zod'
import { CircleNotchIcon, PlusIcon } from '@phosphor-icons/react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { Controller, useForm } from 'react-hook-form'
import type { GroupSummaryDto } from '../../groups/groups.type'
import { studentsApi } from '../students.api'
import { createStudentSchema, type CreateStudentSchemaType } from '../students.schema'

interface IProps {
  slug: string
  groups: GroupSummaryDto[]
}

export default function AddStudentDialog({ slug, groups }: IProps) {
  const queryClient = useQueryClient()
  const [open, setOpen] = useState(false)

  const form = useForm({
    resolver: zodResolver(createStudentSchema),
    defaultValues: {
      name: '',
      groupId: '',
      customPrice: undefined,
      parentPhone: '',
      phone: undefined,
    },
  })
  const createStudentMutation = useMutation({
    mutationFn: (payload: CreateStudentSchemaType) => studentsApi.createStudent(slug, payload),
    onSuccess: (res) => {
      queryClient.invalidateQueries({
        queryKey: ['students', slug],
      })

      toast.add({
        type: 'success',
        title: res.message ?? 'تمت إضافة الطالب بنجاح',
      })
      form.reset({
        groupId: form.getValues('groupId'),
      })
      setTimeout(() => {
        form.setFocus('name')
      }, 0)
    },
    onError: (res) => {
      toast.add({
        type: 'error',
        title: res.message ?? 'حدث خطأ اثناء إضافة الطالب',
      })
    },
  })

  const onSubmit = (data: CreateStudentSchemaType) => {
    createStudentMutation.mutate(data)
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger>
        <Button size={'lg'}>
          <PlusIcon />
          أضف طالب
        </Button>
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>أضف طالب جديد</DialogTitle>
        </DialogHeader>
        <form onSubmit={form.handleSubmit(onSubmit)} id='create-student-form' className='space-y-4'>
          <Controller
            name='name'
            control={form.control}
            render={({ field, fieldState }) => (
              <Field data-invalid={fieldState.invalid}>
                <FieldLabel htmlFor={field.name}>اسم الطالب</FieldLabel>
                <Input
                  {...field}
                  value={field.value ?? ''}
                  id={field.name}
                  aria-invalid={fieldState.invalid}
                  placeholder='اكتب اسم الطالب'
                  autoComplete='off'
                />
                {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
              </Field>
            )}
          />
          <div className='grid grid-cols-2 gap-3'>
            <Controller
              name='parentPhone'
              control={form.control}
              render={({ field, fieldState }) => (
                <Field data-invalid={fieldState.invalid}>
                  <FieldLabel htmlFor={field.name}>رقم ولي الأمر</FieldLabel>
                  <Input
                    {...field}
                    value={field.value ?? ''}
                    id={field.name}
                    aria-invalid={fieldState.invalid}
                    placeholder='اكتب رقم ولي الأمر'
                    autoComplete='off'
                  />
                  {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
                </Field>
              )}
            />
            <Controller
              name='phone'
              control={form.control}
              render={({ field, fieldState }) => (
                <Field data-invalid={fieldState.invalid}>
                  <FieldLabel htmlFor={field.name}>رقم الطالب</FieldLabel>
                  <Input
                    {...field}
                    value={field.value ?? ''}
                    id={field.name}
                    aria-invalid={fieldState.invalid}
                    placeholder='اختياري'
                    autoComplete='off'
                    onChange={(e) => {
                      const val = e.target.value
                      field.onChange(val === '' ? undefined : val)
                    }}
                  />
                  {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
                </Field>
              )}
            />
          </div>
          <Controller
            name='customPrice'
            control={form.control}
            render={({ field, fieldState }) => (
              <Field data-invalid={fieldState.invalid}>
                <FieldLabel htmlFor={field.name}>سعر خاص للطالب</FieldLabel>
                <div className='relative'>
                  <Input
                    {...field}
                    id={field.name}
                    aria-invalid={fieldState.invalid}
                    placeholder='اتركه فارغا إن لم يكن له سعر خاص'
                    autoComplete='off'
                    type='number'
                    value={typeof field.value === 'number' ? field.value : ''}
                    onChange={(e) => {
                      const val = e.target.value
                      field.onChange(val === '' ? undefined : Number(val))
                    }}
                    className='[appearance:textfield] [&::-webkit-outer-spin-button]:appearance-none [&::-webkit-inner-spin-button]:appearance-none pl-10'
                  />
                  <span className='text-muted-foreground absolute bottom-2 left-3 text-sm'>
                    ج.م
                  </span>
                </div>
                {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
              </Field>
            )}
          />
          <Controller
            name='groupId'
            control={form.control}
            render={({ field, fieldState }) => {
              const selectedGroup = groups.find((g) => g.id === field.value)
              return (
                <Field data-invalid={fieldState.invalid}>
                  <FieldLabel htmlFor={field.name}>المجموعة</FieldLabel>
                  <Select
                    {...field}
                    id={field.name}
                    aria-invalid={fieldState.invalid}
                    value={field.value}
                    onValueChange={field.onChange}
                  >
                    <SelectTrigger>
                      <SelectValue placeholder='اختر مجموعة'>
                        {selectedGroup && (
                          <>
                            {selectedGroup?.grade} - {selectedGroup?.name}
                          </>
                        )}
                      </SelectValue>
                    </SelectTrigger>
                    <SelectContent>
                      {groups.map((group, idx) => (
                        <SelectItem key={idx} value={group.id}>
                          {group.grade} - {group.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
                </Field>
              )
            }}
          />
        </form>
        <DialogFooter>
          <Button variant='secondary' type='button' onClick={() => setOpen(false)}>
            إلغاء
          </Button>
          <Button
            type='submit'
            form='create-student-form'
            disabled={createStudentMutation.isPending}
            className='flex items-center gap-2'
          >
            {createStudentMutation.isPending ? (
              <>
                <CircleNotchIcon className='h-4 w-4 animate-spin' />
                <span>جاري الإضافة...</span>
              </>
            ) : (
              <span>أضف طالب</span>
            )}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
