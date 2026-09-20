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

export const DAYS_OF_WEEK = [
  { value: 'Saturday', label: 'السبت' },
  { value: 'Sunday', label: 'الأحد' },
  { value: 'Monday', label: 'الإثنين' },
  { value: 'Tuesday', label: 'الثلاثاء' },
  { value: 'Wednesday', label: 'الأربعاء' },
  { value: 'Thursday', label: 'الخميس' },
  { value: 'Friday', label: 'الجمعة' },
] as const

export const GRADES = [
  { value: 'الصف الثالث الثانوي', label: 'الصف الثالث الثانوي' },
  { value: 'الصف الثاني الثانوي', label: 'الصف الثاني الثانوي' },
  { value: 'الصف الأول الثانوي', label: 'الصف الأول الثانوي' },

  { value: 'الصف الثالث الإعدادي', label: 'الصف الثالث الإعدادي' },
  { value: 'الصف الثاني الإعدادي', label: 'الصف الثاني الإعدادي' },
  { value: 'الصف الأول الإعدادي', label: 'الصف الأول الإعدادي' },

  { value: 'الصف السادس الابتدائي', label: 'الصف السادس الابتدائي' },
  { value: 'الصف الخامس الابتدائي', label: 'الصف الخامس الابتدائي' },
  { value: 'الصف الرابع الابتدائي', label: 'الصف الرابع الابتدائي' },
  { value: 'الصف الثالث الابتدائي', label: 'الصف الثالث الابتدائي' },
  { value: 'الصف الثاني الابتدائي', label: 'الصف الثاني الابتدائي' },
  { value: 'الصف الأول الابتدائي', label: 'الصف الأول الابتدائي' },
] as const

export type GradeType = (typeof GRADES)[number]['value']
