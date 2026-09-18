import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import type React from 'react'

interface IProps {
  title: string
  value?: string
  icon: React.ReactNode
  footer: string
}

export default function DashboardCard({ title, value, icon, footer }: IProps) {
  return (
    <Card className='flex-1'>
      <CardHeader className='flex flex-row items-center justify-between pb-2'>
        <CardTitle className='text-sm font-medium text-muted-foreground'>{title}</CardTitle>
        <div className='text-muted-foreground'>{icon}</div>
      </CardHeader>
      <CardContent>
        <div className='text-2xl font-bold'>{value ?? '---'}</div>
        <p className='text-xs text-muted-foreground mt-1'>{footer}</p>
      </CardContent>
    </Card>
  )
}
