import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { formatTo12Hour } from '@/lib/utils'
import { CalendarXIcon, ClockIcon } from '@phosphor-icons/react'
import type { DashboardTodaySessionDto } from '../dashboard.type'

interface IProps {
  sessions?: DashboardTodaySessionDto[]
}

export default function UpcomingSessionsCard({ sessions }: IProps) {
  const hasSessions = sessions && sessions.length > 0

  return (
    <Card className='w-full'>
      <CardHeader className='flex flex-row items-center justify-between pb-3'>
        <CardTitle className='text-base font-bold text-foreground'>جدول حصص اليوم</CardTitle>
        {hasSessions && (
          <span className='text-xs font-medium text-muted-foreground'>
            {sessions.length} حصص مسجلة
          </span>
        )}
      </CardHeader>

      <CardContent>
        {!hasSessions ? (
          <div className='flex h-64 flex-col items-center justify-center gap-2 rounded-lg p-6 text-center'>
            <CalendarXIcon className='h-10 w-10 text-muted-foreground/60' />
            <p className='text-sm font-semibold text-foreground'>لا توجد حصص مجدولة اليوم</p>
          </div>
        ) : (
          <div className='flex max-h-90 flex-col gap-2 overflow-y-auto pl-1'>
            {sessions.map((session) => (
              <div
                key={session.groupId + session.startTime}
                className='flex items-center justify-between rounded-lg border border-border/60 bg-muted/20 px-3.5 py-5 transition-colors hover:bg-muted/50'
              >
                <span className='text-sm font-semibold text-foreground'>
                  {session.groupDisplayName}
                </span>

                <div className='flex items-center gap-1.5 text-xs text-muted-foreground'>
                  <ClockIcon className='h-3.5 w-3.5' />
                  <span>
                    {formatTo12Hour(session.startTime)} - {formatTo12Hour(session.endTime)}
                  </span>
                </div>
              </div>
            ))}
          </div>
        )}
      </CardContent>
    </Card>
  )
}
