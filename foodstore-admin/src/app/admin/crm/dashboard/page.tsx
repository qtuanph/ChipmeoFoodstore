"use client"

import * as React from "react"
import { Users, ShoppingBag, DollarSign, TrendingUp } from "lucide-react"
import { toast } from "sonner"

import { Breadcrumb, BreadcrumbItem, BreadcrumbList, BreadcrumbPage } from "@/components/ui/breadcrumb"
import { Separator } from "@/components/ui/separator"
import { SidebarTrigger } from "@/components/ui/sidebar"
import { dashboardService } from "@/lib/services/dashboard-service"
import { formatCurrency } from "@/lib/utils"

export default function CrmDashboard() {
  const [totalCustomers, setTotalCustomers] = React.useState<number | null>(null)
  const [todayOrders, setTodayOrders] = React.useState<number | null>(null)
  const [todayRevenue, setTodayRevenue] = React.useState<number | null>(null)
  const [totalRevenue, setTotalRevenue] = React.useState<number | null>(null)

  React.useEffect(() => {
    (async () => {
      try {
        const stats = await dashboardService.getStats()
        setTotalCustomers(stats.totalCustomers)
        setTodayOrders(stats.today.orders)
        setTodayRevenue(stats.today.revenue)
        setTotalRevenue(stats.total.revenue)
      } catch {
        toast.error("Không thể tải thống kê")
      }
    })()
  }, [])

  const cards = [
    { label: "Tổng khách hàng", value: totalCustomers !== null ? totalCustomers.toLocaleString() : "…", icon: Users },
    { label: "Đơn hàng hôm nay", value: todayOrders !== null ? todayOrders.toLocaleString() : "…", icon: ShoppingBag },
    { label: "Doanh thu hôm nay", value: todayRevenue !== null ? formatCurrency(todayRevenue) : "…", icon: DollarSign },
    { label: "Tổng doanh thu", value: totalRevenue !== null ? formatCurrency(totalRevenue) : "…", icon: TrendingUp },
  ]

  return (
    <>
      <header className="flex h-16 shrink-0 items-center gap-2 transition-[width,height] ease-linear group-has-data-[collapsible=icon]/sidebar-wrapper:h-12">
        <div className="flex items-center gap-2 px-4">
          <SidebarTrigger className="-ml-1" />
          <Separator orientation="vertical" className="mr-2 h-4" />
          <Breadcrumb><BreadcrumbList><BreadcrumbItem><BreadcrumbPage>Tổng quan khách hàng</BreadcrumbPage></BreadcrumbItem></BreadcrumbList></Breadcrumb>
        </div>
      </header>
      <div className="flex flex-1 flex-col gap-4 p-4 pt-0">
        <div className="grid auto-rows-min gap-4 md:grid-cols-4">
          {cards.map((card) => (
            <div key={card.label} className="flex items-center gap-4 rounded-xl border p-4">
              <div className="flex size-12 items-center justify-center rounded-lg bg-primary/10">
                <card.icon className="size-6 text-primary" />
              </div>
              <div>
                <p className="text-2xl font-bold">{card.value}</p>
                <p className="text-sm text-muted-foreground">{card.label}</p>
              </div>
            </div>
          ))}
        </div>
      </div>
    </>
  )
}
