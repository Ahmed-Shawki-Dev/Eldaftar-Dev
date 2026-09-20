import * as z from 'zod'

export const createGroupSchema = z.object({
  grade: z.string().min(1, 'اختار الصف الدراسي'),
  name: z.string().nullish(),
  price: z.coerce.number().min(0, 'السعر مطلوب'),
  paymentType: z
    .string()
    .min(1, 'لازم تختار نظام الدفع')
    .refine((val) => val === 'PerSession' || val === 'Monthly', 'قيمة نظام الدفع غير صحيحة'),
  schedules: z
    .array(
      z.object({
        dayOfWeek: z.string().min(1, 'اختار اليوم'),
        startTime: z.string().min(1, 'اختار وقت البداية'),
        endTime: z.string().min(1, 'اختار وقت النهاية'),
      }),
    )
    .min(1, 'يجب إضافة موعد واحد على الأقل للمجموعة'),
})

export type CreateGroupSchemaType = z.infer<typeof createGroupSchema>

export const updateGroupSchema = z.object({
  grade: z.string().min(1, 'اختار الصف الدراسي'),
  name: z.string().nullish(),
  price: z.coerce.number().min(0, 'السعر مطلوب'),
  paymentType: z
    .string()
    .min(1, 'لازم تختار نظام الدفع')
    .refine((val) => val === 'PerSession' || val === 'Monthly', 'قيمة نظام الدفع غير صحيحة'),
  schedules: z
    .array(
      z.object({
        dayOfWeek: z.string().min(1, 'اختار اليوم'),
        startTime: z.string().min(1, 'اختار وقت البداية'),
        endTime: z.string().min(1, 'اختار وقت النهاية'),
      }),
    )
    .min(1, 'يجب إضافة موعد واحد على الأقل للمجموعة'),
})

export type UpdateGroupSchemaType = z.infer<typeof updateGroupSchema>
