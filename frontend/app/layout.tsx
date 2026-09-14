import { ThemeProvider } from '@/components/theme-provider'
import { DirectionProvider } from '@/components/ui/direction'

import { Toaster } from '@/components/ui/toast'
import type { Metadata } from 'next'
import { Geist_Mono, IBM_Plex_Sans_Arabic } from 'next/font/google'
import './globals.css'

const ibmSans = IBM_Plex_Sans_Arabic({
  variable: '--font-ibm-sans',
  subsets: ['arabic'],
  weight: ['100', '200', '300', '400', '500', '600', '700'],
})

const geistMono = Geist_Mono({
  variable: '--font-geist-mono',
  subsets: ['latin'],
})

export const metadata: Metadata = {
  title: 'Eldaftar',
  description: 'Eldaftar Webapp',
}

export default function RootLayout({ children }: LayoutProps<'/'>) {
  return (
    <html
      lang='ar'
      dir='rtl'
      className={`${ibmSans.variable} ${geistMono.variable} h-full antialiased`}
      suppressHydrationWarning
    >
      <body className='min-h-full flex flex-col font-sans'>
        <ThemeProvider
          attribute='class'
          defaultTheme='system'
          enableSystem
          disableTransitionOnChange
        >
          <DirectionProvider direction='rtl'>
            <main>{children}</main>
            <Toaster />
          </DirectionProvider>
        </ThemeProvider>
      </body>
    </html>
  )
}
