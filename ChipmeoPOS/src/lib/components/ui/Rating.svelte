<script lang="ts">
	import Icon from './Icon.svelte';

	interface Props {
		value?: number;
		max?: number;
		size?: 'sm' | 'md' | 'lg';
		showText?: boolean;
		text?: string;
		showCount?: boolean;
		count?: number;
		countHref?: string;
		class?: string;
	}

	let {
		value = 0,
		max = 5,
		size = 'md',
		showText = false,
		text = '',
		showCount = false,
		count = 0,
		countHref = '',
		class: className = ''
	}: Props = $props();

	const sizeClasses: Record<string, string> = {
		sm: 'w-4 h-4',
		md: 'w-5 h-5',
		lg: 'w-6 h-6'
	};

	const starSize = $derived(sizeClasses[size]);

	let stars = $derived(Array.from({ length: max }, (_, i) => i < Math.round(value)));
</script>

<div class="flex items-center space-x-1 {className}">
	{#each stars as filled, i}
		<Icon
			name="tabler:star-filled"
			class="{starSize} {filled ? 'text-fg-yellow' : 'text-fg-disabled'}"
		/>
	{/each}
	{#if showText}
		<p class="ms-2 text-sm font-medium text-body">{text}</p>
	{/if}
	{#if showCount}
		<span class="w-1 h-1 mx-1.5 bg-neutral-quaternary rounded-full"></span>
		{#if countHref}
			<a href={countHref} class="text-sm font-medium text-heading underline hover:no-underline"
				>{count}</a
			>
		{:else}
			<span class="text-sm font-medium text-heading">{count}</span>
		{/if}
	{/if}
</div>
