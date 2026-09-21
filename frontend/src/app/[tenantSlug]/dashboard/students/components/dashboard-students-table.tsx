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
import { BasketIcon, DotsThreeIcon, NutIcon } from '@phosphor-icons/react'
import { useState } from 'react'
import type { GroupSummaryDto } from '../../groups/groups.type'
import type { StudentDto } from '../students.type'
import RemoveStudentDialog from './remove-student-dialog'
import UpdateStudentDialog from './update-student-dialog'

interface IProps {
  students: StudentDto[]
  slug: string
  groups: GroupSummaryDto[]
}

export default function DashboardStudentsTable({ students, slug, groups }: IProps) {
  const [studentToUpdate, setStudentToUpdate] = useState<StudentDto | null>(null)
  const [studentToRemove, setStudentToRemove] = useState<StudentDto | null>(null)
  return (
    <>
      <div className='w-full rounded-xl border border-border bg-card overflow-hidden'>
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>كود الطالب</TableHead>
              <TableHead>الاسم</TableHead>
              <TableHead>رقم ولي الأمر</TableHead>
              <TableHead className='w-24 text-center '>الإجراءات</TableHead>
            </TableRow>
          </TableHeader>

          <TableBody>
            {students.length === 0 ? (
              <TableRow>
                <TableCell colSpan={4} className='h-32 text-center text-muted-foreground'>
                  لا يوجد طلاب مضافة حاليا
                </TableCell>
              </TableRow>
            ) : (
              students.map((student) => (
                <TableRow key={student.id} className='transition-colors hover:bg-muted/40'>
                  <TableCell>
                    <div className='font-semibold'>{student.studentCode}</div>
                  </TableCell>

                  <TableCell>
                    <div className='font-semibold'>{student.name}</div>
                  </TableCell>

                  <TableCell>
                    <div className='font-semibold'>{student.parentPhone}</div>
                  </TableCell>

                  <TableCell className='text-center'>
                    <DropdownMenu>
                      <DropdownMenuTrigger render={<Button variant='ghost' />}>
                        <DotsThreeIcon weight='bold' />
                      </DropdownMenuTrigger>
                      <DropdownMenuContent>
                        <DropdownMenuGroup>
                          <DropdownMenuLabel>الإجراءات</DropdownMenuLabel>
                          <DropdownMenuItem onClick={() => setStudentToUpdate(student)}>
                            <NutIcon />
                            تعديل
                          </DropdownMenuItem>
                          <DropdownMenuItem
                            variant='destructive'
                            onClick={() => setStudentToRemove(student)}
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
      {studentToUpdate && (
        <UpdateStudentDialog
          groups={groups}
          slug={slug}
          key={studentToUpdate.id}
          student={studentToUpdate}
          open={!!studentToUpdate}
          onOpenChange={(isOpen) => {
            if (!isOpen) {
              setStudentToUpdate(null)
            }
          }}
        />
      )}
      {studentToRemove && (
        <RemoveStudentDialog
          slug={slug}
          student={studentToRemove}
          open={!!studentToRemove}
          key={studentToRemove.id}
          onOpenChange={(isOpen) => {
            if (isOpen === false) {
              setStudentToRemove(null)
            }
          }}
        />
      )}
    </>
  )
}
