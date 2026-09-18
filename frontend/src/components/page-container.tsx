import type React from 'react'
import { Card } from './ui/card'

interface IProps {
  title: string
  description?: string
  icon?: React.ReactNode
  action?: React.ReactNode
  children: React.ReactNode
}

export default function PageContainer({ title, description, icon, action, children }: IProps) {
  return (
    <div className='flex flex-col gap-6 p-6'>
      <Card className='flex flex-col md:flex-row md:items-center md:justify-between gap-4 p-4'>
        <div className='flex items-center gap-3'>
          {icon && (
            <div className='flex items-center justify-center bg-muted text-foreground p-2.5 rounded-xl'>
              {icon}
            </div>
          )}

          <div>
            <h1 className='text-xl font-bold tracking-tight text-foreground'>{title}</h1>
            {description && <p className='text-sm text-muted-foreground mt-0.5'>{description}</p>}
          </div>
        </div>

        {action && (
          <div className='flex items-center gap-2 self-start md:self-center text-muted-foreground'>
            {action}
          </div>
        )}
      </Card>

      <div className='flex flex-col gap-10 h-full '>{children}</div>
    </div>
  )
}
