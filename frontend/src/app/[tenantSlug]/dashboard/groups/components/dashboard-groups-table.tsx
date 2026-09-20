import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { DayOfWeekArabic, formatCurrency, formatTo12Hour } from '@/lib/utils'
import { BasketIcon, DotsThreeIcon, NutIcon } from '@phosphor-icons/react'
import { useState } from 'react'
import { PaymentType, type GroupSummaryDto } from '../groups.type'
import RemoveGroupDialog from './remove-group-dialog'
import UpdateGroupDialog from './update-group-dialog'

interface IProps {
  groups: GroupSummaryDto[]
  slug: string
}

export default function DashboardGroupsTable({ groups, slug }: IProps) {
  const [groupToUpdate, setGroupToUpdate] = useState<GroupSummaryDto | null>(null)
  const [groupToRemove, setGroupToRemove] = useState<GroupSummaryDto | null>(null)
  return (
    <>
      <div className='w-full rounded-xl border border-border bg-card overflow-hidden'>
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>المجموعة</TableHead>
              <TableHead>المواعيد</TableHead>
              <TableHead>السعر</TableHead>
              <TableHead className='w-24 text-center '>الإجراءات</TableHead>
            </TableRow>
          </TableHeader>

          <TableBody>
            {groups.length === 0 ? (
              <TableRow>
                <TableCell colSpan={4} className='h-32 text-center text-muted-foreground'>
                  لا توجد مجموعات مضافة حالياً
                </TableCell>
              </TableRow>
            ) : (
              groups.map((group) => (
                <TableRow key={group.id} className='transition-colors hover:bg-muted/40'>
                  <TableCell>
                    <div className='font-semibold'>
                      {group.grade} - {group.name}
                    </div>
                  </TableCell>

                  <TableCell>
                    <div className='flex flex-wrap gap-1.5'>
                      {group.schedules.map((schedule, idx) => (
                        <Badge key={idx} variant='outline' className='text-xs font-normal'>
                          {DayOfWeekArabic[schedule.dayOfWeek]} {formatTo12Hour(schedule.startTime)}{' '}
                          - {formatTo12Hour(schedule.endTime)}
                        </Badge>
                      ))}
                    </div>
                  </TableCell>

                  <TableCell>
                    <div className='flex items-center gap-2'>
                      <span className='font-bold text-foreground'>
                        {formatCurrency(group.price)}
                      </span>
                      <span className='text-xs text-muted-foreground'>
                        {group.paymentType === PaymentType.Monthly ? '/ شهرياً' : '/ بالحصة'}
                      </span>
                    </div>
                  </TableCell>

                  <TableCell className='text-center'>
                    <DropdownMenu>
                      <DropdownMenuTrigger render={<Button variant='ghost' />}>
                        <DotsThreeIcon weight='bold' />
                      </DropdownMenuTrigger>
                      <DropdownMenuContent>
                        <DropdownMenuGroup>
                          <DropdownMenuLabel>الإجراءات</DropdownMenuLabel>
                          <DropdownMenuItem onClick={() => setGroupToUpdate(group)}>
                            <NutIcon />
                            تعديل
                          </DropdownMenuItem>
                          <DropdownMenuItem
                            variant='destructive'
                            onClick={() => setGroupToRemove(group)}
                          >
                            <BasketIcon />
                            حذف
                          </DropdownMenuItem>
                        </DropdownMenuGroup>
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </div>
      {groupToUpdate && (
        <UpdateGroupDialog
          open={!!groupToUpdate}
          onOpenChange={(isOpen) => {
            if (isOpen === false) {
              setGroupToUpdate(null)
            }
          }}
          group={groupToUpdate}
          slug={slug}
        />
      )}
      {groupToRemove && (
        <RemoveGroupDialog
          open={!!groupToRemove}
          onOpenChange={(isOpen) => {
            if (isOpen === false) {
              setGroupToRemove(null)
            }
          }}
          group={groupToRemove}
          slug={slug}
        />
      )}
    </>
  )
}
