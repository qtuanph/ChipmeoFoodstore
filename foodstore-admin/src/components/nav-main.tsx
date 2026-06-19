"use client"

import * as React from "react"
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/components/ui/collapsible"
import {
  SidebarGroup,
  SidebarGroupLabel,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarMenuSub,
  SidebarMenuSubButton,
  SidebarMenuSubItem,
} from "@/components/ui/sidebar"
import { ChevronRightIcon } from "lucide-react"
import { usePathname } from "next/navigation"

function isActiveParent(pathname: string, subItems?: { url: string }[]): boolean {
  return subItems?.some((sub) => pathname === sub.url || pathname.startsWith(sub.url + "/")) ?? false
}

export function NavMain({
  items,
}: {
  items: {
    title: string
    url?: string
    icon?: React.ReactNode
    isActive?: boolean
    items?: {
      title: string
      url: string
    }[]
  }[]
}) {
  const pathname = usePathname()
  const [openKeys, setOpenKeys] = React.useState<string[]>(() =>
    items.filter((item) => item.items && isActiveParent(pathname, item.items)).map((item) => item.title)
  )

  React.useEffect(() => {
    for (const item of items) {
      if (item.items && isActiveParent(pathname, item.items)) {
        setOpenKeys((prev) => prev.includes(item.title) ? prev : [...prev, item.title])
      }
    }
  }, [pathname, items])

  const toggle = (title: string) => {
    setOpenKeys((prev) => prev.includes(title) ? prev.filter((k) => k !== title) : [...prev, title])
  }

  return (
    <SidebarGroup>
      <SidebarGroupLabel>Module</SidebarGroupLabel>
      <SidebarMenu>
        {items.map((item) =>
          item.items && item.items.length > 0 ? (
            <Collapsible
              key={item.title}
              open={openKeys.includes(item.title)}
              onOpenChange={() => toggle(item.title)}
              className="group/collapsible"
              render={<SidebarMenuItem />}
            >
              <CollapsibleTrigger
                render={<SidebarMenuButton tooltip={item.title} isActive={isActiveParent(pathname, item.items)} />}
              >
                {item.icon}
                <span>{item.title}</span>
                <ChevronRightIcon className="ml-auto transition-transform duration-200 group-data-open/collapsible:rotate-90" />
              </CollapsibleTrigger>
              <CollapsibleContent>
                <SidebarMenuSub>
                  {item.items?.map((subItem) => (
                    <SidebarMenuSubItem key={subItem.title}>
                      <SidebarMenuSubButton render={<a href={subItem.url} />} isActive={pathname === subItem.url}>
                        <span>{subItem.title}</span>
                      </SidebarMenuSubButton>
                    </SidebarMenuSubItem>
                  ))}
                </SidebarMenuSub>
              </CollapsibleContent>
            </Collapsible>
          ) : (
            <SidebarMenuItem key={item.title}>
              <SidebarMenuButton tooltip={item.title} render={<a href={item.url!} />}>
                {item.icon}
                <span>{item.title}</span>
              </SidebarMenuButton>
            </SidebarMenuItem>
          )
        )}
      </SidebarMenu>
    </SidebarGroup>
  )
}