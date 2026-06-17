<script lang="ts">
	import Icon from './Icon.svelte';

	interface Props {
		src?: string;
		alt?: string;
		fallback?: string;
		size?: 'sm' | 'md' | 'lg' | 'xl';
		rounded?: 'full' | 'lg' | 'md';
		class?: string;
	}

	let {
		src = '',
		alt = 'Avatar',
		fallback = '',
		size = 'md',
		rounded = 'full',
		class: className = ''
	}: Props = $props();

	const sizeClasses = {
		sm: 'w-8 h-8',
		md: 'w-10 h-10',
		lg: 'w-14 h-14',
		xl: 'w-20 h-20'
	};

	const roundedClasses = {
		full: 'rounded-full',
		lg: 'rounded-lg',
		md: 'rounded-md'
	};
</script>

<div
	class="relative overflow-hidden bg-neutral-secondary-medium {sizeClasses[size]} {roundedClasses[
		rounded
	]} {className}"
>
	{#if src}
		<img {src} {alt} class="w-full h-full object-cover" />
	{:else if fallback}
		<div
			class="flex w-full h-full items-center justify-center bg-brand-softer text-fg-brand font-medium text-sm"
		>
			{fallback}
		</div>
	{:else}
		<div class="flex w-full h-full items-center justify-center text-body">
			<Icon name="tabler:user" class="w-1/2 h-1/2" />
		</div>
	{/if}
</div>
