<script lang="ts">
	import Icon from './Icon.svelte';

	interface NavItem {
		label: string;
		icon: string; // icon name for Icon component, e.g. 'tabler:home'
		active?: boolean;
		href?: string;
		onclick?: () => void;
	}

	interface Props {
		items?: NavItem[];
	}

	let { items = [] }: Props = $props();
</script>

<div class="fixed bottom-0 left-0 z-50 w-full h-16 bg-neutral-primary-soft border-t border-default">
	<div
		class="grid h-full max-w-lg mx-auto font-medium"
		style="grid-template-columns: repeat({Math.max(items.length, 1)}, minmax(0, 1fr))"
	>
		{#each items as item}
			{#if item.href}
				<a
					href={item.href}
					class="inline-flex flex-col items-center justify-center px-5 hover:bg-neutral-secondary-medium group {item.active
						? 'bg-neutral-secondary-medium'
						: ''}"
				>
					<Icon
						name={item.icon}
						class="w-5 h-5 mb-1 {item.active
							? 'text-fg-brand'
							: 'text-body group-hover:text-fg-brand'}"
					/>
					<span
						class="text-xs {item.active
							? 'text-fg-brand'
							: 'text-body group-hover:text-fg-brand'}">{item.label}</span
					>
				</a>
			{:else}
				<button
					type="button"
					onclick={item.onclick}
					class="inline-flex flex-col items-center justify-center px-5 hover:bg-neutral-secondary-medium group {item.active
						? 'bg-neutral-secondary-medium'
						: ''}"
				>
					<Icon
						name={item.icon}
						class="w-5 h-5 mb-1 {item.active
							? 'text-fg-brand'
							: 'text-body group-hover:text-fg-brand'}"
					/>
					<span
						class="text-xs {item.active
							? 'text-fg-brand'
							: 'text-body group-hover:text-fg-brand'}">{item.label}</span
					>
				</button>
			{/if}
		{/each}
	</div>
</div>
