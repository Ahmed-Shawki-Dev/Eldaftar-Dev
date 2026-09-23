import { z } from 'zod'

export const createExamSchema = z.object({
  title: z
    .string({ message: 'اسم الامتحان مطلوب' })
    .trim()
    .min(2, 'اسم الامتحان لازم يكون حرفين على الأقل'),

  grade: z.string({ message: 'المرحلة الدراسية مطلوبة' }).min(1, 'يجب اختيار المرحلة الدراسية'),

  date: z.coerce.date({ message: 'تاريخ الامتحان مطلوب' }),

  maxScore: z.coerce
    .number({ message: 'الدرجة العظمى مطلوبة ويجب أن تكون رقماً' })
    .positive('الدرجة العظمى لازم تكون أكبر من صفر'),

  groupId: z.string().optional(),
})

export type CreateExamSchemaType = z.infer<typeof createExamSchema>

export const updateExamSchema = createExamSchema

export type UpdateExamSchemaType = z.infer<typeof updateExamSchema>
