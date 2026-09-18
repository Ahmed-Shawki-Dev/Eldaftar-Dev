import { formatCurrency } from '@/lib/utils'
import { CardsIcon, CoinIcon, UsersIcon, WalletIcon } from '@phosphor-icons/react'
import type { DashboardStatsCardDto } from '../dashboard.type'
import DashboardCard from './dashboard-card'

interface IProps {
  stats?: DashboardStatsCardDto
}

export default function DashboardStatsSection({ stats }: IProps) {
  return (
    <div className='flex flex-col md:flex-row w-full gap-4'>
      <DashboardCard
        title='أرباح اليوم'
        value={formatCurrency(stats?.todayIncome)}
        footer='إيرادات اليوم'
        icon={<CoinIcon size={20} />}
      />
      <DashboardCard
        title='المحصل هذا الشهر'
        value={formatCurrency(stats?.monthIncome)}
        footer='إيرادات الشهر'
        icon={<WalletIcon size={20} />}
      />
      <DashboardCard
        title='المستحقات المتأخرة'
        value={formatCurrency(stats?.overdueAmount)}
        footer={`${stats?.overdueStudentsCount ?? 0} طالب متأخر`}
        icon={<CardsIcon size={20} />}
      />
      <DashboardCard
        title='الطلاب'
        value={(stats?.activeStudentsCount ?? 0).toLocaleString('en-EG')}
        footer='طالب مسجل'
        icon={<UsersIcon size={20} />}
      />
    </div>
  )
}
