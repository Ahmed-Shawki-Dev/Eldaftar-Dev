import { Button } from '@/components/ui/button'
import { Calendar } from '@/components/ui/calendar'
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
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { toast } from '@/components/ui/toast'
import { GRADES } from '@/lib/utils'
import { zodResolver } from '@hookform/resolvers/zod'
import { CaretDownIcon, CircleNotchIcon, PlusIcon } from '@phosphor-icons/react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { format } from 'date-fns'
import { arEG } from 'date-fns/locale'
import { useState } from 'react'
import { Controller, useForm } from 'react-hook-form'
import type { GroupSummaryDto } from '../../groups/groups.type'
import { createExamSchema, type CreateExamSchemaType } from '../exam.schema'
import { examsApi } from '../exams.api'
import type { CreateExamDto } from '../exams.type'

interface IProps {
  slug: string
  groups: GroupSummaryDto[]
}

export default function CreateExamDialog({ slug, groups }: IProps) {
  const queryClient = useQueryClient()
  const [open, setOpen] = useState(false)
  const [calendarOpen, setCalendarOpen] = useState(false)

  const form = useForm({
    resolver: zodResolver(createExamSchema),
    defaultValues: {
      title: '',
      grade: '',
      date: undefined,
      maxScore: undefined,
      groupId: 'all',
    },
  })

  // eslint-disable-next-line react-hooks/incompatible-library
  const selectedGrade = form.watch('grade')
  const availableGroups = selectedGrade ? groups.filter((g) => g.grade === selectedGrade) : []

  const createExamMutation = useMutation({
    mutationFn: (payload: CreateExamSchemaType) => examsApi.createExam(slug, payload),
    onSuccess: (res) => {
      queryClient.invalidateQueries({
        queryKey: ['exams'],
      })

      toast.add({
        type: 'success',
        title: res.message ?? 'تمت إضافة الامتحان بنجاح',
      })
      form.reset()
      setOpen(false)
    },
    onError: () => {
      toast.add({
        type: 'error',
        title: 'حدث خطأ أثناء إضافة الامتحان',
      })
    },
  })

  const onSubmit = (data: CreateExamSchemaType) => {
    const payload: CreateExamDto = {
      ...data,
      groupId: data.groupId === 'all' ? undefined : data.groupId,
      date: data.date ? data.date.toISOString() : undefined,
    }
    createExamMutation.mutate(payload as unknown as CreateExamSchemaType)
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger
        render={
          <Button size={'lg'}>
            <PlusIcon />
            أضف امتحان
          </Button>
        }
      />
      <DialogContent>
        <DialogHeader>
          <DialogTitle>اضف امتحان</DialogTitle>
        </DialogHeader>

        <form onSubmit={form.handleSubmit(onSubmit)} id='create-exam-form' className='space-y-3'>
          <Controller
            name='title'
            control={form.control}
            render={({ field, fieldState }) => (
              <Field data-invalid={fieldState.invalid}>
                <FieldLabel htmlFor={field.name}>اسم الامتحان</FieldLabel>
                <Input
                  {...field}
                  id={field.name}
                  type='text'
                  aria-invalid={fieldState.invalid}
                  placeholder='اكتب اسم الامتحان'
                />
                {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
              </Field>
            )}
          />

          <div className='grid grid-cols-1 gap-2 sm:grid-cols-2'>
            <Controller
              name='grade'
              control={form.control}
              render={({ field, fieldState }) => (
                <Field data-invalid={fieldState.invalid}>
                  <FieldLabel htmlFor={field.name}>المرحلة الدراسية</FieldLabel>
                  <Select
                    {...field}
                    id={field.name}
                    aria-invalid={fieldState.invalid}
                    value={field.value}
                    onValueChange={(val) => {
                      field.onChange(val)
                      form.setValue('groupId', 'all')
                    }}
                  >
                    <SelectTrigger>
                      <SelectValue placeholder='اختر المرحلة الدراسية' />
                    </SelectTrigger>
                    <SelectContent>
                      {GRADES.map((grade, idx) => (
                        <SelectItem key={idx} value={grade.value}>
                          {grade.label}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
                </Field>
              )}
            />

            <Controller
              name='groupId'
              control={form.control}
              render={({ field, fieldState }) => {
                const selectedGroup = availableGroups.find((group) => group.id === field.value)
                return (
                  <Field data-invalid={fieldState.invalid}>
                    <FieldLabel htmlFor={field.name}>المجموعة</FieldLabel>
                    <Select
                      {...field}
                      id={field.name}
                      aria-invalid={fieldState.invalid}
                      value={field.value ?? 'all'}
                      onValueChange={field.onChange}
                      disabled={!selectedGrade}
                    >
                      <SelectTrigger>
                        <SelectValue placeholder='اختر المجموعة'>
                          {selectedGroup
                            ? `${selectedGroup.grade} - ${selectedGroup.name}`
                            : field.value === 'all'
                              ? 'امتحان لكل المجاميع'
                              : 'اختر المجموعة'}
                        </SelectValue>
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value='all'>امتحان لكل المجاميع للمرحلة</SelectItem>
                        {availableGroups.map((group, idx) => (
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
          </div>

          <div className='grid grid-cols-1 gap-2 sm:grid-cols-2'>
            <Controller
              name='maxScore'
              control={form.control}
              render={({ field, fieldState }) => (
                <Field data-invalid={fieldState.invalid}>
                  <FieldLabel htmlFor={field.name}>درجة الامتحان</FieldLabel>
                  <Input
                    type='number'
                    {...field}
                    value={typeof field.value === 'number' ? field.value : ''}
                    onChange={(e) =>
                      field.onChange(e.target.value === '' ? undefined : Number(e.target.value))
                    }
                    className='[appearance:textfield] [&::-webkit-outer-spin-button]:appearance-none [&::-webkit-inner-spin-button]:appearance-none pl-10'
                    placeholder='اكتب درجة الامتحان'
                  />
                  {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
                </Field>
              )}
            />

            <Controller
              name='date'
              control={form.control}
              render={({ field, fieldState }) => {
                const selectedDate: Date | undefined = field.value as Date | undefined

                return (
                  <Field data-invalid={fieldState.invalid}>
                    <FieldLabel htmlFor={field.name}>ميعاد الامتحان</FieldLabel>

                    <Popover open={calendarOpen} onOpenChange={setCalendarOpen}>
                      <PopoverTrigger
                        render={
                          <Button
                            id={field.name}
                            variant={'outline'}
                            data-empty={!field.value}
                            className='w-full justify-between text-left font-normal data-[empty=true]:text-muted-foreground'
                          >
                            {selectedDate ? (
                              format(selectedDate, 'PPP', { locale: arEG })
                            ) : (
                              <span>اختر تاريخ الامتحان</span>
                            )}
                            <CaretDownIcon data-icon='inline-end' />
                          </Button>
                        }
                      />
                      <PopoverContent className='w-auto p-0' align='start'>
                        <Calendar
                          mode='single'
                          selected={selectedDate}
                          onSelect={(date) => {
                            field.onChange(date)
                            setCalendarOpen(false)
                          }}
                          defaultMonth={selectedDate}
                        />
                      </PopoverContent>
                    </Popover>

                    {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
                  </Field>
                )
              }}
            />
          </div>
        </form>

        <DialogFooter>
          <Button variant='secondary' type='button' onClick={() => setOpen(false)}>
            إلغاء
          </Button>
          <Button
            type='submit'
            form='create-exam-form'
            disabled={createExamMutation.isPending}
            className='flex items-center gap-2'
          >
            {createExamMutation.isPending ? (
              <>
                <CircleNotchIcon className='h-4 w-4 animate-spin' />
                <span>جاري الإضافة...</span>
              </>
            ) : (
              <span>أضف امتحان</span>
            )}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
