import { queryClient } from '@/App'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Field, FieldError, FieldGroup, FieldLabel } from '@/components/ui/field'
import { Input } from '@/components/ui/input'
import { toast } from '@/components/ui/toast'
import { authApi } from '@/features/auth/auth.api'
import { useAuthStore } from '@/features/auth/auth.store'
import type { ApiResponse } from '@/types/api'
import { zodResolver } from '@hookform/resolvers/zod'
import { Controller, useForm } from 'react-hook-form'
import { useNavigate } from 'react-router'
import type { LoginDto } from '../../../../features/auth/auth.type'
import { loginSchema, type LoginSchemaType } from '../login.schema'

interface IProps {
  tenantSlug: string
}

export function LoginForm({ tenantSlug }: IProps) {
  const navigate = useNavigate()
  const { handleSubmit, control } = useForm<LoginSchemaType>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      phoneNumber: '',
      password: '',
    },
  })

  const { setAuthUser } = useAuthStore()

  const onSubmit = async (data: LoginDto) => {
    try {
      const loginResponse = await authApi.tenantLogin(tenantSlug, data)
      if (loginResponse.success) {
        toast.add({ title: loginResponse.message, type: 'success' })
        await queryClient.invalidateQueries({ queryKey: ['auth', 'me', tenantSlug] })
        setAuthUser(loginResponse.data!)
        navigate(`/${tenantSlug}/dashboard`, { replace: true })
      }
    } catch (error: unknown) {
      const errorMsg = (error as ApiResponse<unknown>).message
      toast.add({ title: errorMsg, type: 'error' })
    }
  }

  return (
    <Card className='w-100'>
      <CardHeader>
        <CardTitle className='text-center text-xl font-bold'>سجل الدخول</CardTitle>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit(onSubmit)}>
          <FieldGroup>
            <Controller
              name='phoneNumber'
              control={control}
              render={({ field, fieldState }) => (
                <Field data-invalid={fieldState.invalid}>
                  <FieldLabel htmlFor='phoneNumber'>رقم الهاتف</FieldLabel>
                  <Input
                    {...field}
                    aria-invalid={fieldState.invalid}
                    id='phoneNumber'
                    autoComplete='off'
                    placeholder='ادخل رقم الهاتف'
                  />
                  {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
                </Field>
              )}
            />

            <Controller
              name='password'
              control={control}
              render={({ field, fieldState }) => (
                <Field data-invalid={fieldState.invalid}>
                  <FieldLabel htmlFor='password'>كلمة السر</FieldLabel>
                  <Input
                    {...field}
                    aria-invalid={fieldState.invalid}
                    id='password'
                    type='password'
                    autoComplete='off'
                    placeholder='ادخل كلمة السر'
                  />
                  {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
                </Field>
              )}
            />

            <Field>
              <Button type='submit'>سجل الدخول</Button>
            </Field>
          </FieldGroup>
        </form>
      </CardContent>
    </Card>
  )
}
