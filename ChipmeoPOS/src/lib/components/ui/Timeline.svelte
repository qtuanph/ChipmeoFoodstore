<script lang="ts">
	import Icon from './Icon.svelte';

	interface TimelineItem {
		time?: string;
		title?: string;
		description?: string;
		icon?: any;
		active?: boolean;
		completed?: boolean;
	}

	interface Props {
		items: TimelineItem[];
		action?: (item: TimelineItem) => void;
		actionLabel?: string;
		class?: string;
	}

	let { items, action, actionLabel = 'Learn more', class: className = '' }: Props = $props();
</script>

<ol class="relative border-s border-default {className}">
	{#each items as item, i}
		<li class="ms-6 {i === items.length - 1 ? 'mb-0' : 'mb-10'}">
			<span
				class="absolute flex items-center justify-center w-6 h-6 rounded-full -start-3 ring-8 ring-buffer-medium
					{item.completed
					? 'bg-brand text-white'
					: item.active
						? 'bg-brand-softer text-fg-brand'
						: 'bg-neutral-quaternary text-body'}"
			>
				{#if item.icon}
					{@render item.icon()}
				{:else}
					<Icon name="tabler:circle-filled" class="w-3 h-3" />
				{/if}
			</span>
			{#if item.time}
				<time class="mb-1 text-sm font-normal text-body leading-none">{item.time}</time>
			{/if}
			{#if item.title}
				<h3 class="text-lg font-semibold text-heading">{item.title}</h3>
			{/if}
			{#if item.description}
				<p class="mb-4 text-base font-normal text-body">{item.description}</p>
			{/if}
			{#if action}
				<button
					type="button"
					onclick={() => action(item)}
					class="text-body bg-neutral-secondary-medium box-border border border-default-medium hover:bg-neutral-tertiary-medium hover:text-heading focus:ring-4 focus:ring-neutral-tertiary shadow-xs font-medium leading-5 rounded-base text-sm px-3 py-2 focus:outline-none"
				>
					{actionLabel}
				</button>
			{/if}
		</li>
	{/each}
</ol>
