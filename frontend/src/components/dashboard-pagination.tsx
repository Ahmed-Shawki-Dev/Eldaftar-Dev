import { Button } from '@/components/ui/button'
import type { PaginationMetadata } from '@/types/api'
import { CaretLeftIcon, CaretRightIcon } from '@phosphor-icons/react'
import { useSearchParams } from 'react-router'

interface IProps {
  paginationMetadata: PaginationMetadata | null | undefined
}

export default function DashboardPagination({ paginationMetadata }: IProps) {
  const [searchParams, setSearchParams] = useSearchParams()

  const totalPages = paginationMetadata?.totalPages ?? 1
  const currentPage = Number(searchParams.get('pageNumber')) || 1

  if (!paginationMetadata || totalPages <= 1) {
    return null
  }

  const hasPrevious = currentPage > 1
  const hasNext = currentPage < totalPages

  const handlePageChange = (newPage: number) => {
    setSearchParams((prev) => {
      if (newPage <= 1) {
        prev.delete('pageNumber')
      } else {
        prev.set('pageNumber', String(newPage))
      }
      return prev
    })
  }

  return (
    <div className='flex items-center gap-2'>
      <Button
        variant='ghost'
        size='sm'
        disabled={!hasPrevious}
        onClick={() => handlePageChange(currentPage - 1)}
        className='gap-1.5'
      >
        <CaretRightIcon size={16} />
        <span>السابق</span>
      </Button>

      <span className='text-xs text-muted-foreground px-2'>
        {currentPage} من {totalPages}
      </span>

      <Button
        variant='ghost'
        size='sm'
        disabled={!hasNext}
        onClick={() => handlePageChange(currentPage + 1)}
        className='gap-1.5'
      >
        <span>التالي</span>
        <CaretLeftIcon size={16} />
      </Button>
    </div>
  )
}
