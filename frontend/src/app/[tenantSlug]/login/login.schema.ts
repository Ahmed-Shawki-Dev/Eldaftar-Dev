import * as z from 'zod'

export const loginSchema = z.object({
  phoneNumber: z.string().regex(/^01[0125][0-9]{8}$/, { message: 'أدخل رقم هاتف صحيح' }),
  password: z.string().min(8, { message: 'كلمة السر يجب أن لا تقل عن 8 حروف' }),
})

export type LoginSchemaType = z.infer<typeof loginSchema>
