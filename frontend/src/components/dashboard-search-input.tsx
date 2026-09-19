import { Input } from '@/components/ui/input'
import { useEffect, useState, type ChangeEvent } from 'react'
import { useSearchParams } from 'react-router'

interface IProps {
  searchKey: string
  inputPlaceholder: string
}

export default function DashboardSearchInput({ searchKey, inputPlaceholder }: IProps) {
  const [searchParams, setSearchParams] = useSearchParams()
  const [text, setText] = useState(searchParams.get(searchKey) ?? '')

  useEffect(() => {
    const timer = setTimeout(() => {
      setSearchParams((prev) => {
        const currentValue = prev.get(searchKey) ?? ''

        if (currentValue === text) {
          return prev
        }

        if (text) {
          prev.set(searchKey, text)
        } else {
          prev.delete(searchKey)
        }

        prev.delete('pageNumber')

        return prev
      })
    }, 300)

    return () => clearTimeout(timer)
  }, [text, searchKey, setSearchParams])
  return (
    <Input
      type='text'
      value={text}
      className='h-10 bg-card border-border'
      placeholder={inputPlaceholder}
      onChange={(e: ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value
        setText(value)
      }}
    />
  )
}
