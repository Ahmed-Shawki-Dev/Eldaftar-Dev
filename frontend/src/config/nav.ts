import {
  CalendarBlankIcon,
  ClockIcon,
  ExamIcon,
  FileIcon,
  GearIcon,
  HouseSimpleIcon,
  MoneyIcon,
  StudentIcon,
  UserIcon,
} from '@phosphor-icons/react'

export const navItems = [
  { title: 'الصفحة الرئيسية', url: '', icon: HouseSimpleIcon },
  { title: 'الغياب', url: 'attendance', icon: CalendarBlankIcon },
  { title: 'المجاميع', url: 'groups', icon: ClockIcon },
  { title: 'الطلاب', url: 'students', icon: StudentIcon },
  { title: 'المدفوعات', url: 'payments', icon: MoneyIcon },
  { title: 'الامتحانات', url: 'exams', icon: ExamIcon },
  { title: 'السكرتيرات', url: 'staff', icon: UserIcon },
  { title: 'التقارير', url: 'reports', icon: FileIcon },
  { title: 'الإعدادات', url: 'settings', icon: GearIcon },
]
