<script lang="ts">
	interface Props {
		type?: 'solid' | 'outline' | 'dot' | 'ping';
		color?: 'brand' | 'success' | 'warning' | 'danger' | 'neutral';
		size?: 'sm' | 'md' | 'lg';
		children?: any;
		class?: string;
	}

	let {
		type = 'dot',
		color = 'brand',
		size = 'md',
		children,
		class: className = ''
	}: Props = $props();

	const colorMap: Record<string, string> = {
		brand: 'bg-brand',
		success: 'bg-success',
		warning: 'bg-warning',
		danger: 'bg-danger',
		neutral: 'bg-neutral-tertiary'
	};

	const outlineColorMap: Record<string, string> = {
		brand: 'border-brand',
		success: 'border-success',
		warning: 'border-warning',
		danger: 'border-danger',
		neutral: 'border-neutral-tertiary'
	};

	const sizeMap: Record<string, string> = {
		sm: 'w-2 h-2',
		md: 'w-3 h-3',
		lg: 'w-4 h-4'
	};

	const solidSizeMap: Record<string, string> = {
		sm: 'w-5 h-5 text-[10px]',
		md: 'w-6 h-6 text-xs',
		lg: 'w-7 h-7 text-sm'
	};

	const bg = $derived(colorMap[color]);
	const outlineBorder = $derived(outlineColorMap[color]);
	const dotSize = $derived(sizeMap[size]);
	const solidSize = $derived(solidSizeMap[size]);
</script>

{#if children}
	<span class="relative inline-flex {className}">
		{@render children()}
		{#if type === 'dot'}
			<span class="absolute -top-1 -right-1 flex {dotSize} {bg} rounded-full border-2 border-white"
			></span>
		{:else if type === 'ping'}
			<span class="absolute -top-1 -right-1 flex {dotSize}">
				<span class="animate-ping absolute inline-flex h-full w-full rounded-full {bg} opacity-75"
				></span>
				<span class="relative inline-flex rounded-full {dotSize} {bg}"></span>
			</span>
		{:else if type === 'outline'}
			<span
				class="absolute -top-1 -right-1 flex {dotSize} bg-transparent border-2 {outlineBorder} rounded-full"
			></span>
		{:else if type === 'solid'}
			<span
				class="absolute -top-2 -right-2 inline-flex items-center justify-center {solidSize} text-xs font-medium text-white {bg} rounded-full"
			></span>
		{/if}
	</span>
{:else}
	{#if type === 'dot'}
		<span class="flex {dotSize} {bg} rounded-full {className}"></span>
	{:else if type === 'ping'}
		<span class="relative flex {dotSize} {className}">
			<span class="animate-ping absolute inline-flex h-full w-full rounded-full {bg} opacity-75"
			></span>
			<span class="relative inline-flex rounded-full {dotSize} {bg}"></span>
		</span>
	{:else if type === 'outline'}
		<span class="flex {dotSize} bg-transparent border-2 {outlineBorder} rounded-full {className}"
		></span>
	{:else if type === 'solid'}
		<span
			class="inline-flex items-center justify-center {solidSize} text-xs font-medium text-white {bg} rounded-full {className}"
		></span>
	{/if}
{/if}
