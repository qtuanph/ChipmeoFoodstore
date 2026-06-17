<script lang="ts">
	interface Props {
		value: number;
		color?: 'brand' | 'success' | 'warning' | 'danger';
		size?: 'sm' | 'md' | 'lg';
		showLabel?: boolean;
		class?: string;
	}

	let {
		value,
		color = 'brand',
		size = 'md',
		showLabel = false,
		class: className = ''
	}: Props = $props();

	const colorClasses: Record<string, string> = {
		brand: 'bg-brand',
		success: 'bg-success',
		warning: 'bg-warning',
		danger: 'bg-danger'
	};

	const sizeClasses: Record<string, string> = {
		sm: 'h-1.5',
		md: 'h-2.5',
		lg: 'h-4'
	};

	const clampedValue = $derived(Math.max(0, Math.min(100, value)));
</script>

<div class="w-full bg-neutral-quaternary rounded-full {sizeClasses[size]} {className}">
	<div
		class="{colorClasses[color]} {sizeClasses[
			size
		]} text-xs font-medium text-white text-center leading-none rounded-full {showLabel &&
		clampedValue > 0
			? 'p-0.5'
			: ''}"
		style="width: {clampedValue}%"
	>
		{#if showLabel && clampedValue > 0}
			{Math.round(clampedValue)}%
		{/if}
	</div>
</div>
