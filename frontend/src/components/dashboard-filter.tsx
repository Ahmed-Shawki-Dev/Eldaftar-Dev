import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { useSearchParams } from 'react-router'

export interface FilterOption {
  value: string
  label: string
}

interface GenericFilterProps {
  filterKey: string
  placeholder: string
  options: readonly FilterOption[]
}

export default function DashboardFilter({ filterKey, options, placeholder }: GenericFilterProps) {
  const [searchParams, setSearchParams] = useSearchParams()
  const currentValue = searchParams.get(filterKey) ?? ''

  const handleSelect = (selectedValue: string | null) => {
    setSearchParams((prev) => {
      if (!selectedValue || selectedValue === 'all') {
        prev.delete(filterKey)
      } else {
        prev.set(filterKey, selectedValue)
      }

      prev.delete('pageNumber')
      return prev
    })
  }

  return (
    <Select value={currentValue} onValueChange={handleSelect}>
      <SelectTrigger className='h-10 w-48 bg-card border-border'>
        <SelectValue placeholder={placeholder} />
      </SelectTrigger>
      <SelectContent>
        <SelectItem value='all'>الكل</SelectItem>

        {options.map((option) => (
          <SelectItem key={option.value} value={option.value}>
            {option.label}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  )
}
