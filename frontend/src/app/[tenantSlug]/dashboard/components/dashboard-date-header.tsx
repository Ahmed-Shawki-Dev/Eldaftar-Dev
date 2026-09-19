import { format } from 'date-fns'
import { ar } from 'date-fns/locale'

export default function DashboardDateHeader() {
  const formattedDate = format(new Date(), 'EEEE، d MMMM', { locale: ar })

  return (
    <div className='bg-accent/50 text-foreground text-xs font-medium py-1.5 px-3.5 rounded-2xl border border-border/40'>
      {formattedDate}
    </div>
  )
}
