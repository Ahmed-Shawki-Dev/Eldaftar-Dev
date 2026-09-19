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

export class GroupParamsDto {
  private static readonly MaxPageSize = 50

  private _pageNumber: number = 1
  public get pageNumber(): number {
    return this._pageNumber
  }
  public set pageNumber(value: number) {
    this._pageNumber = value < 1 ? 1 : value
  }

  private _pageSize: number = 10
  public get pageSize(): number {
    return this._pageSize
  }
  public set pageSize(value: number) {
    this._pageSize =
      value > GroupParamsDto.MaxPageSize ? GroupParamsDto.MaxPageSize : value < 1 ? 10 : value
  }

  public searchTerm?: string
  public grade?: string

  constructor(init?: Partial<GroupParamsDto>) {
    if (init) {
      if (init.pageNumber !== undefined) this.pageNumber = init.pageNumber
      if (init.pageSize !== undefined) this.pageSize = init.pageSize
      this.searchTerm = init.searchTerm
      this.grade = init.grade
    }
  }
}
