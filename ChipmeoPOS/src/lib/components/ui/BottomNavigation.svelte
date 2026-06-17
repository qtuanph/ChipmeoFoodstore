<script lang="ts">
	interface NavItem {
		label: string;
		icon: string;
		active?: boolean;
		href?: string;
	}

	interface Props {
		items?: NavItem[];
	}

	let { items = [] }: Props = $props();
</script>

<div class="fixed bottom-0 left-0 z-50 w-full h-16 bg-neutral-primary-soft border-t border-default">
	<div class="grid h-full max-w-lg grid-cols-{Math.max(items.length, 1)} mx-auto font-medium">
		{#each items as item}
			{#if item.href}
				<a
					href={item.href}
					class="inline-flex flex-col items-center justify-center px-5 hover:bg-neutral-secondary-medium group {item.active
						? 'bg-neutral-secondary-medium'
						: ''}"
				>
					{@html item.icon}
					<span class="text-sm text-body group-hover:text-fg-brand">{item.label}</span>
				</a>
			{:else}
				<button
					type="button"
					class="inline-flex flex-col items-center justify-center px-5 hover:bg-neutral-secondary-medium group {item.active
						? 'bg-neutral-secondary-medium'
						: ''}"
				>
					{@html item.icon}
					<span class="text-sm text-body group-hover:text-fg-brand">{item.label}</span>
				</button>
			{/if}
		{/each}
	</div>
</div>
