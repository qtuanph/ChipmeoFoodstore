"use client"

import * as React from "react"
import { Edit, Plus, QrCode, Trash2 } from "lucide-react"
import { QRCodeSVG } from "qrcode.react"
import { toast } from "sonner"
import type { ColumnDef } from "@tanstack/react-table"

import { Breadcrumb, BreadcrumbItem, BreadcrumbList, BreadcrumbPage } from "@/components/ui/breadcrumb"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { Input } from "@/components/ui/input"
import { Separator } from "@/components/ui/separator"
import { SidebarTrigger } from "@/components/ui/sidebar"
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog"
import { DataTable } from "@/components/data-table"
import { CrudSheet } from "@/components/crud-sheet"
import { DeleteConfirmDialog } from "@/components/confirm-dialog"
import { StatusBadge } from "@/components/status-badge"
import { customerService } from "@/lib/services/customer-service"
import { formatDateTime } from "@/lib/utils"
import type { Customer, CreateCustomerDto, UpdateCustomerAdminDto } from "@/lib/types"

export default function CustomersPage() {
  const [data, setData] = React.useState<Customer[]>([])
  const [loading, setLoading] = React.useState(true)
  const [sheetOpen, setSheetOpen] = React.useState(false)
  const [editing, setEditing] = React.useState<Customer | null>(null)
  const [submitting, setSubmitting] = React.useState(false)
  const [deleteOpen, setDeleteOpen] = React.useState(false)
  const [deleting, setDeleting] = React.useState(false)
  const [deleteTarget, setDeleteTarget] = React.useState<Customer | null>(null)
  const [qrTarget, setQrTarget] = React.useState<Customer | null>(null)

  const [formName, setFormName] = React.useState("")
  const [formPhone, setFormPhone] = React.useState("")
  const [formEmail, setFormEmail] = React.useState("")
  const [formUsername, setFormUsername] = React.useState("")
  const [formPassword, setFormPassword] = React.useState("")

  const loadData = React.useCallback(async () => {
    setLoading(true)
    try { const res = await customerService.getAll(); setData(res) }
    catch { toast.error("Không thể tải khách hàng") }
    finally { setLoading(false) }
  }, [])

  React.useEffect(() => { loadData() }, [loadData])

  const resetForm = () => { setFormName(""); setFormPhone(""); setFormEmail(""); setFormUsername(""); setFormPassword(""); setEditing(null) }
  const openCreate = () => { resetForm(); setSheetOpen(true) }
  const openEdit = (item: Customer) => {
    setEditing(item); setFormName(item.name); setFormPhone(item.phone ?? ""); setFormEmail(item.email); setFormUsername(item.username); setFormPassword(""); setSheetOpen(true)
  }

  const handleSubmit = async () => {
    if (!formName.trim()) { toast.error("Vui lòng nhập họ tên"); return }
    setSubmitting(true)
    try {
      if (editing) {
        await customerService.update(editing.id, { name: formName.trim(), phone: formPhone || undefined } as UpdateCustomerAdminDto)
        toast.success("Cập nhật khách hàng thành công")
      } else {
        await customerService.create({ name: formName.trim(), phone: formPhone || undefined, username: formUsername || undefined, email: formEmail || undefined, password: formPassword || undefined } as CreateCustomerDto)
        toast.success("Thêm khách hàng thành công")
      }
      setSheetOpen(false); loadData()
    } catch (e) { toast.error((e as Error).message) }
    finally { setSubmitting(false) }
  }

  const confirmDelete = (item: Customer) => { setDeleteTarget(item); setDeleteOpen(true) }
  const handleDelete = async () => {
    if (!deleteTarget) return
    setDeleting(true)
    try { await customerService.delete(deleteTarget.id); toast.success("Xóa khách hàng thành công"); setDeleteOpen(false); loadData() }
    catch { toast.error("Không thể xóa khách hàng") }
    finally { setDeleting(false) }
  }

  const columns: ColumnDef<Customer>[] = [
    {
      id: "qrCode",
      header: "QR",
      size: 60,
      cell: ({ row }) => (
        <Button variant="ghost" size="icon-sm" className="size-8" onClick={(e) => { e.stopPropagation(); setQrTarget(row.original) }}>
          <QrCode className="size-4" />
        </Button>
      ),
    },
    {
      id: "name",
      accessorKey: "name",
      header: "Họ tên",
      cell: ({ row }) => (
        <div className="flex items-center gap-2">
          {row.original.avatarUrl ? <img src={row.original.avatarUrl} alt="" className="size-8 rounded-full object-cover" /> : <div className="flex size-8 items-center justify-center rounded-full bg-muted text-xs font-medium">{row.original.name.charAt(0)}</div>}
          <span>{row.original.name}</span>
        </div>
      ),
    },
    { id: "email", accessorKey: "email", header: "Email" },
    { id: "phone", accessorKey: "phone", header: "SĐT", cell: ({ row }) => row.original.phone || "—" },
    { id: "customerCode", accessorKey: "customerCode", header: "Mã KH" },
    { id: "loyaltyPoints", accessorKey: "loyaltyPoints", header: "Điểm" },
    { id: "membershipLevel", header: "Hạng", cell: ({ row }) => row.original.membershipLevel ? <StatusBadge status={row.original.membershipLevel} /> : "—" },
    { id: "createdAt", header: "Ngày tạo", cell: ({ row }) => formatDateTime(row.original.createdAt) },
    {
      id: "actions",
      header: "",
      cell: ({ row }) => (
        <div className="flex justify-end gap-1">
          <Button variant="ghost" size="icon-sm" onClick={() => openEdit(row.original)}><Edit className="size-4" /></Button>
          <Button variant="ghost" size="icon-sm" onClick={() => confirmDelete(row.original)}><Trash2 className="size-4 text-destructive" /></Button>
        </div>
      ),
    },
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
        <DataTable columns={columns} data={data} searchKey="name" searchPlaceholder="Tìm khách hàng..." loading={loading}
          toolbarActions={<Button className="gap-1.5" onClick={openCreate}><Plus className="size-4" />Thêm khách hàng</Button>} />
      </div>
      <CrudSheet open={sheetOpen} onOpenChange={setSheetOpen} title={editing ? "Sửa khách hàng" : "Thêm khách hàng"} onSubmit={handleSubmit} submitting={submitting}>
        <div className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="name">Họ tên</Label>
            <Input id="name" value={formName} onChange={(e) => setFormName(e.target.value)} placeholder="Nguyễn Văn A" />
          </div>
          <div className="space-y-2">
            <Label htmlFor="email">Email</Label>
            <Input id="email" type="email" value={formEmail} onChange={(e) => setFormEmail(e.target.value)} placeholder="a@example.com" />
          </div>
          <div className="space-y-2">
            <Label htmlFor="phone">Số điện thoại</Label>
            <Input id="phone" value={formPhone} onChange={(e) => setFormPhone(e.target.value)} placeholder="0123456789" />
          </div>
          {!editing && (
            <>
              <div className="space-y-2">
                <Label htmlFor="username">Tên đăng nhập</Label>
                <Input id="username" value={formUsername} onChange={(e) => setFormUsername(e.target.value)} placeholder="Để trống = tự động" />
              </div>
              <div className="space-y-2">
                <Label htmlFor="password">Mật khẩu</Label>
                <Input id="password" type="password" value={formPassword} onChange={(e) => setFormPassword(e.target.value)} placeholder="Để trống = 123456" />
              </div>
            </>
          )}
        </div>
      </CrudSheet>
      <DeleteConfirmDialog open={deleteOpen} onOpenChange={setDeleteOpen} onConfirm={handleDelete} loading={deleting} itemName={deleteTarget?.name} />
      <Dialog open={!!qrTarget} onOpenChange={(open) => { if (!open) setQrTarget(null) }}>
        <DialogContent className="w-fit sm:max-w-sm" showCloseButton={true}>
          <DialogHeader>
            <DialogTitle>Mã QR - {qrTarget?.name}</DialogTitle>
          </DialogHeader>
          <div className="flex flex-col items-center gap-3 py-4">
            {qrTarget && <QRCodeSVG value={qrTarget.customerCode} size={200} />}
            <p className="text-sm text-muted-foreground">Mã KH: {qrTarget?.customerCode}</p>
          </div>
        </DialogContent>
      </Dialog>
    </>
  )
}
