<script lang="ts">
	import Icon from './Icon.svelte';

	interface SidebarItem {
		label: string;
		href?: string;
		icon?: string;
		active?: boolean;
		badge?: string;
		badgeVariant?: 'pro' | 'default';
		children?: SidebarItem[];
	}

	interface Props {
		id?: string;
		items?: SidebarItem[];
	}

	let { id = 'sidebar', items = [] }: Props = $props();

	// Track open/closed state for sub-menus with Svelte state
	// More reliable than Flowbite Collapse in SPA context
	let openSubmenus = $state<Record<number, boolean>>({});

	function toggleSubmenu(i: number) {
		openSubmenus[i] = !openSubmenus[i];
	}
</script>

<!-- Mobile toggle button — should be placed in layout, kept here for convenience -->
<button
	onclick={() => {
		const sidebar = document.getElementById(id);
		sidebar?.classList.toggle('-translate-x-full');
	}}
	type="button"
	class="inline-flex items-center p-2 mt-2 ms-3 text-sm text-body rounded-base sm:hidden hover:bg-neutral-tertiary focus:outline-none focus:ring-2 focus:ring-neutral-tertiary-soft"
	aria-controls={id}
>
	<span class="sr-only">Open sidebar</span>
	<Icon name="tabler:menu" class="w-6 h-6" />
</button>

<aside
	{id}
	class="fixed top-0 left-0 z-40 w-64 h-full transition-transform -translate-x-full sm:translate-x-0"
	aria-label="Sidebar"
>
	<div class="h-full px-3 py-4 overflow-y-auto bg-neutral-primary-soft border-e border-default">
		<ul class="space-y-2 font-medium">
			{#each items as item, i}
				<li>
					{#if item.children && item.children.length > 0}
						<!-- Expandable sub-menu — Svelte state, no Flowbite Collapse needed -->
						<button
							type="button"
							onclick={() => toggleSubmenu(i)}
							aria-expanded={openSubmenus[i] ?? false}
							class="flex items-center w-full px-2 py-1.5 text-body rounded-base hover:bg-neutral-tertiary hover:text-fg-brand group"
						>
							{#if item.icon}
								<Icon name={item.icon} class="w-5 h-5 transition duration-75 group-hover:text-fg-brand" />
							{/if}
							<span class="flex-1 ms-3 text-left rtl:text-right whitespace-nowrap">{item.label}</span>
							{#if item.badge}
								{#if item.badgeVariant === 'pro'}
									<span class="bg-neutral-secondary-medium border border-default-medium text-heading text-xs font-medium px-1.5 py-0.5 rounded-sm">{item.badge}</span>
								{:else}
									<span class="inline-flex items-center justify-center w-4.5 h-4.5 ms-2 text-xs font-medium text-fg-danger-strong bg-danger-soft border border-danger-subtle rounded-full">{item.badge}</span>
								{/if}
							{/if}
							<Icon
								name="tabler:chevron-down"
								class="w-3 h-3 ms-auto transition-transform duration-200 {openSubmenus[i] ? 'rotate-180' : ''}"
							/>
						</button>
						{#if openSubmenus[i]}
							<ul class="py-2 space-y-2">
								{#each item.children as child}
									<li>
										<a
											href={child.href || '#'}
											class="flex items-center px-3 py-1.5 text-body rounded-base hover:bg-neutral-tertiary hover:text-fg-brand group {child.active
												? 'bg-neutral-tertiary text-fg-brand'
												: ''}"
										>
											{#if child.icon}
												<Icon name={child.icon} class="w-4 h-4 transition duration-75 group-hover:text-fg-brand" />
												<span class="ms-3 whitespace-nowrap">{child.label}</span>
											{:else}
												<span class="ms-6 whitespace-nowrap">{child.label}</span>
											{/if}
											{#if child.badge}
												<span class="inline-flex items-center justify-center w-4.5 h-4.5 ms-2 text-xs font-medium text-fg-danger-strong bg-danger-soft border border-danger-subtle rounded-full">{child.badge}</span>
											{/if}
										</a>
									</li>
								{/each}
							</ul>
						{/if}
					{:else}
						<a
							href={item.href || '#'}
							class="flex items-center px-2 py-1.5 text-body rounded-base hover:bg-neutral-tertiary hover:text-fg-brand group {item.active
								? 'bg-neutral-tertiary text-fg-brand'
								: ''}"
						>
							{#if item.icon}
								<Icon name={item.icon} class="w-5 h-5 transition duration-75 group-hover:text-fg-brand" />
							{/if}
							<span class="flex-1 ms-3 whitespace-nowrap">{item.label}</span>
							{#if item.badge}
								{#if item.badgeVariant === 'pro'}
									<span class="bg-neutral-secondary-medium border border-default-medium text-heading text-xs font-medium px-1.5 py-0.5 rounded-sm">{item.badge}</span>
								{:else}
									<span class="inline-flex items-center justify-center w-4.5 h-4.5 ms-2 text-xs font-medium text-fg-danger-strong bg-danger-soft border border-danger-subtle rounded-full">{item.badge}</span>
								{/if}
							{/if}
						</a>
					{/if}
				</li>
			{/each}
		</ul>
	</div>
</aside>
