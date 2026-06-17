<script lang="ts">
	interface ButtonGroupItem {
		label: string;
		href?: string;
		icon?: string;
		active?: boolean;
		disabled?: boolean;
	}

	interface Props {
		items: ButtonGroupItem[];
		vertical?: boolean;
		class?: string;
	}

	let { items = [], vertical = false, class: className = '' }: Props = $props();
</script>

<div
	class="inline-flex {vertical
		? 'flex-col rounded-base shadow-xs -space-y-px'
		: 'rounded-base shadow-xs -space-x-px'} {className}"
	role="group"
>
	{#each items as item, i}
		{#if item.href}
			<a
				href={item.disabled ? undefined : item.href}
				class="text-body bg-neutral-primary-soft border border-default hover:bg-neutral-secondary-medium hover:text-heading focus:ring-3 focus:ring-neutral-tertiary-soft font-medium leading-5 text-sm px-3 py-2 focus:outline-none {item.active
					? 'text-fg-brand bg-neutral-secondary-medium'
					: ''} {item.disabled ? 'text-fg-disabled pointer-events-none' : ''} {i === 0
					? vertical
						? 'rounded-t-base'
						: 'rounded-s-base'
					: ''} {i === items.length - 1
					? vertical
						? 'rounded-b-base'
						: 'rounded-e-base'
					: ''} {item.icon ? 'inline-flex items-center' : ''}"
			>
				{#if item.icon}{@html item.icon}{/if}
				{item.label}
			</a>
		{:else}
			<button
				type="button"
				disabled={item.disabled}
				class="text-body bg-neutral-primary-soft border border-default hover:bg-neutral-secondary-medium hover:text-heading focus:ring-3 focus:ring-neutral-tertiary-soft font-medium leading-5 text-sm px-3 py-2 focus:outline-none {item.active
					? 'text-fg-brand bg-neutral-secondary-medium'
					: ''} {item.disabled ? 'text-fg-disabled cursor-not-allowed' : ''} {i === 0
					? vertical
						? 'rounded-t-base'
						: 'rounded-s-base'
					: ''} {i === items.length - 1
					? vertical
						? 'rounded-b-base'
						: 'rounded-e-base'
					: ''} {item.icon ? 'inline-flex items-center' : ''}"
			>
				{#if item.icon}{@html item.icon}{/if}
				{item.label}
			</button>
		{/if}
	{/each}
</div>
