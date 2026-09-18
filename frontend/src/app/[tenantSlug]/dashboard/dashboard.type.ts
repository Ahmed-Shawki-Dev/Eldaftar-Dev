export interface DashboardStatsCardDto {
  todayIncome: number
  monthIncome: number
  overdueAmount: number
  overdueStudentsCount: number
  activeStudentsCount: number
}

export interface MonthlyRevenueDto {
  monthKey: string
  monthName: string
  totalAmount: number
}

export interface DashboardTodaySessionDto {
  sessionId: string | null
  groupId: string
  groupDisplayName: string
  startTime: string
  endTime: string
  status: string
  totalStudentsCount: number
}

export interface DashboardTeacherDataDto {
  stats: DashboardStatsCardDto
  monthlyRevenue: MonthlyRevenueDto[]
  todaySessions: DashboardTodaySessionDto[]
}
