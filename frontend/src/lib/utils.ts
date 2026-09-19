import { DayOfWeekEnum } from '@/app/[tenantSlug]/dashboard/groups/groups.type'

export const formatCurrency = (amount?: number): string => {
  if (amount == null) return '---'

  return new Intl.NumberFormat('ar-EG-u-nu-latn', {
    style: 'currency',
    currency: 'EGP',
    maximumFractionDigits: 0,
  }).format(amount)
}

export function formatTo12Hour(timeStr: string): string {
  if (!timeStr) return ''

  const [hoursPart, minutesPart] = timeStr.split(':')
  const hours = parseInt(hoursPart, 10)
  const minutes = minutesPart ? minutesPart.slice(0, 2) : '00'

  if (isNaN(hours)) return timeStr

  const period = hours >= 12 ? 'م' : 'ص'
  const adjustedHours = hours % 12 || 12

  return `${adjustedHours}:${minutes} ${period}`
}

export const DayOfWeekArabic: Record<DayOfWeekEnum, string> = {
  [DayOfWeekEnum.Saturday]: 'السبت',
  [DayOfWeekEnum.Sunday]: 'الأحد',
  [DayOfWeekEnum.Monday]: 'الإثنين',
  [DayOfWeekEnum.Tuesday]: 'الثلاثاء',
  [DayOfWeekEnum.Wednesday]: 'الأربعاء',
  [DayOfWeekEnum.Thursday]: 'الخميس',
  [DayOfWeekEnum.Friday]: 'الجمعة',
}
