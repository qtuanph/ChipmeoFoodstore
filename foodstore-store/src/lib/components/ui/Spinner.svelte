<script lang="ts">
	interface Props {
		size?: 'sm' | 'md' | 'lg' | 'xl';
		color?: 'brand' | 'neutral' | 'white';
		class?: string;
	}

	let { size = 'md', color = 'brand', class: className = '' }: Props = $props();

	const sizeClasses = {
		sm: 'w-5 h-5 border-2',
		md: 'w-8 h-8 border-[3px]',
		lg: 'w-10 h-10 border-4',
		xl: 'w-12 h-12 border-4'
	};

	const colorClasses = {
		brand: 'border-neutral-quaternary border-t-brand',
		neutral: 'border-neutral-quaternary border-t-neutral-tertiary-medium',
		white: 'border-neutral-tertiary-medium border-t-white'
	};
</script>

<div
	class="inline-block {sizeClasses[size]} {colorClasses[
		color
	]} animate-spin rounded-full {className}"
></div>
