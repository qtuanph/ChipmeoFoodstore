<script lang="ts">
	import Icon from './Icon.svelte';
	interface ListGroupItem {
		label: string;
		href?: string;
		icon?: string; // icon name, e.g. 'tabler:home'
		active?: boolean;
		disabled?: boolean;
	}

	interface Props {
		items: ListGroupItem[];
		class?: string;
	}

	let { items = [], class: className = '' }: Props = $props();

	let hasLinks = $derived(items.some((item) => item.href));
</script>

<div
	class="w-48 text-sm font-medium text-heading bg-neutral-primary-soft border border-default rounded-base {className}"
>
	{#each items as item, i}
		{@const isFirst = i === 0}
		{@const isLast = i === items.length - 1}
		{#if hasLinks}
			<a
				href={item.disabled ? undefined : item.href}
				class="block w-full px-4 py-2 border-b border-default cursor-pointer hover:bg-neutral-secondary-medium hover:text-fg-brand focus:outline-none focus:ring-2 focus:ring-brand {item.active
					? 'text-fg-brand bg-neutral-secondary-medium'
					: ''} {item.disabled
					? 'text-fg-disabled cursor-not-allowed pointer-events-none'
					: ''} {isFirst ? 'rounded-t-base' : ''} {isLast ? 'rounded-b-base' : ''} {item.icon
					? 'flex items-center'
					: ''}"
			>
				{#if item.icon}<Icon name={item.icon} class="w-5 h-5 me-2" />{/if}
				{item.label}
			</a>
		{:else}
			<button
				type="button"
				disabled={item.disabled}
				class="w-full px-4 py-2 border-b border-default cursor-pointer hover:bg-neutral-secondary-medium hover:text-fg-brand focus:outline-none focus:ring-2 focus:ring-brand {item.active
					? 'text-fg-brand bg-neutral-secondary-medium'
					: ''} {item.disabled ? 'text-fg-disabled cursor-not-allowed' : ''} {isFirst
					? 'rounded-t-base'
					: ''} {isLast ? 'rounded-b-base' : ''} {item.icon ? 'flex items-center' : ''}"
			>
				{#if item.icon}<Icon name={item.icon} class="w-5 h-5 me-2" />{/if}
				{item.label}
			</button>
		{/if}
	{/each}
</div>
