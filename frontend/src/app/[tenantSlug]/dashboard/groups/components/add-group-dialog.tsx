import { queryClient } from '@/App'
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
import { Label } from '@/components/ui/label'
import { RadioGroup, RadioGroupItem } from '@/components/ui/radio-group'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { toast } from '@/components/ui/toast'
import { DayOfWeekArabic, DAYS_OF_WEEK, GRADES } from '@/lib/utils'
import { zodResolver } from '@hookform/resolvers/zod'
import { CircleNotchIcon, PlusIcon } from '@phosphor-icons/react'
import { useMutation } from '@tanstack/react-query'
import { useState } from 'react'
import { Controller, useFieldArray, useForm } from 'react-hook-form'
import { groupsApi } from '../groups.api'
import { createGroupSchema, type CreateGroupSchemaType } from '../groups.schema'
import type { DayOfWeekEnum } from '../groups.type'

interface IProps {
  slug: string
}

export default function AddGroupDialog({ slug }: IProps) {
  const [open, setOpen] = useState(false)
  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm({
    resolver: zodResolver(createGroupSchema),
    defaultValues: {
      name: '',
      grade: '',
      price: 0,
      paymentType: 'PerSession',
      schedules: [],
    },
  })

  const { fields, append, remove } = useFieldArray({
    control: control,
    name: 'schedules',
  })

  const createGroupMutation = useMutation({
    mutationFn: (payload: CreateGroupSchemaType) => groupsApi.createGroup(slug, payload),
    onSuccess: (res) => {
      queryClient.invalidateQueries({
        queryKey: ['groups'],
      })

      toast.add({
        type: 'success',
        title: res.message ?? 'تمت إضافة مجموعة بنجاح',
      })
      reset()
      setOpen(false)
    },
    onError: () => {
      toast.add({
        type: 'error',
        title: 'حدث خطأ أثناء إضافة المجموعة',
      })
    },
  })

  const onSubmit = (data: CreateGroupSchemaType) => {
    createGroupMutation.mutate(data)
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger>
        <Button size={'lg'}>
          <PlusIcon />
          أضف مجموعة
        </Button>
      </DialogTrigger>
      <DialogContent className='sm:max-w-xl md:max-w-xl'>
        <DialogHeader>
          <DialogTitle>أضف مجموعة جديدة</DialogTitle>
        </DialogHeader>
        <form id='create-group-form' onSubmit={handleSubmit(onSubmit)} className='space-y-4'>
          <Controller
            name='grade'
            control={control}
            render={({ field, fieldState }) => (
              <Field data-invalid={fieldState.invalid}>
                <FieldLabel htmlFor={field.name}>الصف الدراسي</FieldLabel>
                <Select
                  {...field}
                  id={field.name}
                  aria-invalid={fieldState.invalid}
                  value={field.value}
                  onValueChange={field.onChange}
                >
                  <SelectTrigger>
                    <SelectValue placeholder='اختر مجموعة' />
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
            name='name'
            control={control}
            render={({ field, fieldState }) => (
              <Field data-invalid={fieldState.invalid}>
                <FieldLabel htmlFor={field.name}>اسم المجموعة</FieldLabel>
                <Input
                  {...field}
                  value={field.value ?? ''}
                  id={field.name}
                  aria-invalid={fieldState.invalid}
                  placeholder='اكتب اسم المجموعة'
                  autoComplete='off'
                />
                {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
              </Field>
            )}
          />
          <div className='grid grid-cols-1 sm:grid-cols-2 gap-4'>
            <Controller
              name='paymentType'
              control={control}
              render={({ field, fieldState }) => (
                <Field data-invalid={fieldState.invalid}>
                  <FieldLabel htmlFor={field.name}>نظام الدفع</FieldLabel>
                  <RadioGroup
                    defaultValue='PerSession'
                    value={field.value}
                    onValueChange={field.onChange}
                  >
                    <div className='flex items-center gap-3'>
                      <RadioGroupItem value='PerSession' id='PerSession' />
                      <Label htmlFor='PerSession'>بالحصة</Label>
                    </div>
                    <div className='flex items-center gap-3'>
                      <RadioGroupItem value='Monthly' id='Monthly' />
                      <Label htmlFor='Monthly'>شهري</Label>
                    </div>
                  </RadioGroup>
                  {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
                </Field>
              )}
            />
            <Controller
              name='price'
              control={control}
              render={({ field, fieldState }) => (
                <Field data-invalid={fieldState.invalid}>
                  <FieldLabel htmlFor={field.name}>تسعير المجموعة</FieldLabel>
                  <div className='relative'>
                    <Input
                      {...field}
                      id={field.name}
                      aria-invalid={fieldState.invalid}
                      placeholder='اكتب تسعير المجموعة'
                      autoComplete='off'
                      type='number'
                      value={typeof field.value === 'number' ? field.value : ''}
                      onChange={(e) => field.onChange(Number(e.target.value))}
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
          </div>

          <div className='space-y-3 pt-2'>
            <div className='flex items-center justify-between'>
              <Label className='font-bold text-sm'>مواعيد الحصص الأسبوعية</Label>
              <Button
                type='button'
                variant='outline'
                size='sm'
                onClick={() =>
                  append({ dayOfWeek: 'Saturday', startTime: '16:00', endTime: '18:00' })
                }
              >
                <PlusIcon className='mr-1 h-4 w-4' />
                إضافة موعد
              </Button>
            </div>

            {fields.length === 0 && (
              <p className='text-xs text-muted-foreground border border-dashed rounded-md p-3 text-center'>
                لم يتم تحديد مواعيد بعد. يمكنك إضافة مواعيد الحصص الآن.
              </p>
            )}

            {errors.schedules?.root?.message || errors.schedules?.message ? (
              <p className='text-xs font-medium text-destructive'>
                {errors.schedules?.root?.message ?? errors.schedules?.message}
              </p>
            ) : null}

            <div className='space-y-2 max-h-56 overflow-y-auto pl-1'>
              {fields.map((fieldItem, index) => (
                <div
                  key={fieldItem.id}
                  className='flex flex-col gap-2 rounded-lg border border-border bg-muted/20 p-3 sm:flex-row sm:items-center'
                >
                  <div className='w-full sm:flex-1'>
                    <Controller
                      name={`schedules.${index}.dayOfWeek`}
                      control={control}
                      render={({ field }) => (
                        <Select value={field.value} onValueChange={field.onChange}>
                          <SelectTrigger className='h-9 text-xs'>
                            <SelectValue placeholder='اختر اليوم'>
                              {field.value
                                ? DayOfWeekArabic[field.value as DayOfWeekEnum]
                                : undefined}
                            </SelectValue>
                          </SelectTrigger>
                          <SelectContent>
                            {DAYS_OF_WEEK.map((day) => (
                              <SelectItem key={day.value} value={day.value}>
                                {day.label}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                      )}
                    />
                  </div>

                  <div className='flex items-center gap-2'>
                    <div className='flex-1 sm:w-28'>
                      <Controller
                        name={`schedules.${index}.startTime`}
                        control={control}
                        render={({ field }) => (
                          <Input type='time' className='h-9 text-xs dark:scheme-dark' {...field} />
                        )}
                      />
                    </div>

                    <span className='text-xs text-muted-foreground'>إلى</span>

                    <div className='flex-1 sm:w-28'>
                      <Controller
                        name={`schedules.${index}.endTime`}
                        control={control}
                        render={({ field }) => (
                          <Input type='time' className='h-9 text-xs dark:scheme-dark' {...field} />
                        )}
                      />
                    </div>

                    <Button
                      type='button'
                      variant='ghost'
                      size='icon'
                      className='h-9 w-9 shrink-0 text-destructive hover:bg-destructive/10'
                      onClick={() => remove(index)}
                    >
                      ✕
                    </Button>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </form>
        <DialogFooter>
          <Button variant='secondary' type='button' onClick={() => setOpen(false)}>
            إلغاء
          </Button>
          <Button
            type='submit'
            form='create-group-form'
            disabled={createGroupMutation.isPending}
            className='flex items-center gap-2'
          >
            {createGroupMutation.isPending ? (
              <>
                <CircleNotchIcon className='h-4 w-4 animate-spin' />
                <span>جاري الإضافة...</span>
              </>
            ) : (
              <span>أضف مجموعة</span>
            )}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
