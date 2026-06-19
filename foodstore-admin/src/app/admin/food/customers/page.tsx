"use client"

import * as React from "react"
import { Mail, Phone } from "lucide-react"
import type { ColumnDef } from "@tanstack/react-table"
import { toast } from "sonner"

import { Breadcrumb, BreadcrumbItem, BreadcrumbList, BreadcrumbPage } from "@/components/ui/breadcrumb"
import { Separator } from "@/components/ui/separator"
import { SidebarTrigger } from "@/components/ui/sidebar"
import { DataTable } from "@/components/data-table"
import { StatusBadge } from "@/components/status-badge"
import { customerService } from "@/lib/services/customer-service"
import { formatDateTime } from "@/lib/utils"
import type { Customer } from "@/lib/types"

export default function CustomersPage() {
  const [data, setData] = React.useState<Customer[]>([])
  const [loading, setLoading] = React.useState(true)

  const loadData = React.useCallback(async () => {
    setLoading(true)
    try { const res = await customerService.getAll(); setData(res) }
    catch { toast.error("Không thể tải khách hàng") }
    finally { setLoading(false) }
  }, [])

  React.useEffect(() => { loadData() }, [loadData])

  const columns: ColumnDef<Customer>[] = [
    { id: "name", accessorKey: "name", header: "Họ tên", cell: ({ row }) => (
      <div className="flex items-center gap-2">
        {row.original.avatarUrl ? <img src={row.original.avatarUrl} alt="" className="size-8 rounded-full object-cover" /> : <div className="flex size-8 items-center justify-center rounded-full bg-muted text-xs font-medium">{row.original.name.charAt(0)}</div>}
        <span>{row.original.name}</span>
      </div>
    )},
    {
      id: "email",
      accessorKey: "email",
      header: "Email",
      cell: ({ row }) => (
        <div className="flex items-center gap-1.5">
          <Mail className="size-3.5 text-muted-foreground" />
          <span>{row.original.email}</span>
        </div>
      ),
    },
    {
      id: "phone",
      header: "SĐT",
      cell: ({ row }) => (
        <div className="flex items-center gap-1.5">
          <Phone className="size-3.5 text-muted-foreground" />
          <span>{row.original.phone ?? "—"}</span>
        </div>
      ),
    },
    { id: "loyaltyPoints", accessorKey: "loyaltyPoints", header: "Điểm" },
    { id: "customerCode", accessorKey: "customerCode", header: "Mã KH" },
    { id: "createdAt", header: "Ngày tạo", cell: ({ row }) => formatDateTime(row.original.createdAt) },
  ]

  return (
    <>
      <header className="flex h-16 shrink-0 items-center gap-2 transition-[width,height] ease-linear group-has-data-[collapsible=icon]/sidebar-wrapper:h-12">
        <div className="flex items-center gap-2 px-4">
          <SidebarTrigger className="-ml-1" />
          <Separator orientation="vertical" className="mr-2 h-4" />
          <Breadcrumb><BreadcrumbList><BreadcrumbItem><BreadcrumbPage>Khách hàng</BreadcrumbPage></BreadcrumbItem></BreadcrumbList></Breadcrumb>
        </div>
      </header>
      <div className="flex flex-1 flex-col gap-4 p-4 pt-0">
        <DataTable columns={columns} data={data} searchKey="name" searchPlaceholder="Tìm khách hàng..." loading={loading} />
      </div>
    </>
  )
}
