import { useParams } from 'react-router'
import { LoginForm } from './components/login-form'

export default function LoginPage() {
  const { tenantSlug } = useParams()
  console.log(tenantSlug)
  return (
    <div className='h-screen w-full flex justify-center items-center'>
      <LoginForm tenantSlug={tenantSlug!} />
    </div>
  )
}
