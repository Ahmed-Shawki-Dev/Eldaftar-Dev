export enum DayOfWeekEnum {
  Sunday = 'Sunday',
  Monday = 'Monday',
  Tuesday = 'Tuesday',
  Wednesday = 'Wednesday',
  Thursday = 'Thursday',
  Friday = 'Friday',
  Saturday = 'Saturday',
}

export enum PaymentType {
  Monthly = 'Monthly',
  PerSession = 'PerSession',
  Term = 'Term',
}
export interface CreateScheduleDto {
  dayOfWeek: DayOfWeekEnum
  startTime: string
  endTime: string
}

export interface CreateGroupDto {
  grade: string
  name: string
  price: number
  paymentType: PaymentType
  academicTermId?: string | null
  schedules: CreateScheduleDto[]
}

export interface UpdateGroupDto {
  grade: string
  name: string
  price: number
  paymentType: PaymentType
  academicTermId?: string | null
  schedules: CreateScheduleDto[]
}

export interface GroupSummaryDto {
  id: string
  grade: string
  name: string
  price: number
  paymentType: PaymentType
  schedules: CreateScheduleDto[]
}

export interface GroupParamsDto {
  pageNumber?: number
  pageSize?: number
  searchTerm?: string
  grade?: string
}
