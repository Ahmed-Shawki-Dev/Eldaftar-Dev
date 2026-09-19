import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import {
  ChartContainer,
  ChartTooltip,
  ChartTooltipContent,
  type ChartConfig,
} from '@/components/ui/chart'
import { Bar, BarChart, CartesianGrid, XAxis } from 'recharts'
import type { MonthlyRevenueDto } from '../dashboard.type'

interface IProps {
  data?: MonthlyRevenueDto[]
}

const chartConfig = {
  totalAmount: {
    label: 'الإيراد',
    color: 'var(--primary)',
  },
} satisfies ChartConfig

export default function MonthlyRevenueChart({ data = [] }: IProps) {
  return (
    <Card className='w-full h-full'>
      <CardHeader className='pb-2'>
        <CardTitle className='text-base font-bold text-foreground'>إيرادات الشهور</CardTitle>
      </CardHeader>
      <CardContent>
        <ChartContainer config={chartConfig} className='min-h-65 w-full'>
          <BarChart accessibilityLayer data={data}>
            <CartesianGrid vertical={false} strokeDasharray='3 3' />

            <XAxis dataKey='monthName' tickLine={false} tickMargin={10} axisLine={false} />

            <ChartTooltip content={<ChartTooltipContent hideLabel indicator='dashed' />} />

            <Bar dataKey='totalAmount' fill='var(--color-totalAmount)' radius={[6, 6, 0, 0]} />
          </BarChart>
        </ChartContainer>
      </CardContent>
    </Card>
  )
}
