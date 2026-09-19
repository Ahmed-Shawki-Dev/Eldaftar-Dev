import DashboardFilter from '@/components/dashboard-filter'
import GroupsSearch from '@/components/dashboard-search-input'

interface FilterOption {
  readonly label: string
  readonly value: string
}

interface DashboardSearchWithFilterProps {
  searchKey: string
  searchPlaceholder?: string

  filterKey: string
  filterPlaceholder: string
  filterOptions: readonly FilterOption[]
}

export default function DashboardSearchWithFilter({
  searchKey,
  searchPlaceholder = 'بحث...',
  filterKey,
  filterPlaceholder,
  filterOptions,
}: DashboardSearchWithFilterProps) {
  return (
    <div className='flex flex-col gap-3 sm:flex-row sm:items-center justify-between w-full '>
      <div className='w-full sm:w-72 md:w-80'>
        <GroupsSearch searchKey={searchKey} inputPlaceholder={searchPlaceholder} />
      </div>

      <div className='w-full sm:w-56'>
        <DashboardFilter
          filterKey={filterKey}
          options={filterOptions}
          placeholder={filterPlaceholder}
        />
      </div>
    </div>
  )
}
