<script lang="ts">
	import Icon from './Icon.svelte';

	interface Props {
		type?: 'text' | 'avatar' | 'card' | 'image' | 'list';
		lines?: number;
		class?: string;
		width?: string;
		height?: string;
	}

	let {
		type = 'text',
		lines = 3,
		class: className = '',
		width = '',
		height = ''
	}: Props = $props();

	const widths = ['w-full', 'w-3/4', 'w-1/2'];
	const barWidths = $derived(Array.from({ length: lines }, (_, i) => widths[i % widths.length]));
</script>

{#if type === 'text'}
	<div
		role="status"
		class="space-y-2.5 animate-pulse {width} {className}"
		style={height ? 'height: ' + height : ''}
	>
		{#each barWidths as w}
			<div class="h-2 bg-neutral-quaternary rounded-full {w}"></div>
		{/each}
		<span class="sr-only">Loading...</span>
	</div>
{:else if type === 'card'}
	<div
		role="status"
		class="animate-pulse {width} {className}"
		style={height ? 'height: ' + height : ''}
	>
		<div class="flex items-center justify-center h-48 bg-neutral-quaternary rounded-base mb-4">
			<Icon name="tabler:photo" class="w-11 h-11 text-fg-disabled" />
			<span class="sr-only">Loading...</span>
		</div>
		<div class="space-y-3">
			{#each { length: 3 } as _}
				<div class="h-2.5 bg-neutral-quaternary rounded-full"></div>
			{/each}
		</div>
	</div>
{:else if type === 'avatar'}
	<div role="status" class="animate-pulse {className}">
		<div class="flex items-center">
			<div class="w-10 h-10 bg-neutral-quaternary rounded-full"></div>
			<div class="space-y-2 ms-3 flex-1 {width}">
				<div class="h-2.5 bg-neutral-quaternary rounded-full w-32"></div>
				<div class="h-2 bg-neutral-quaternary rounded-full w-24"></div>
			</div>
		</div>
		<span class="sr-only">Loading...</span>
	</div>
{:else if type === 'image'}
	<div
		role="status"
		class="flex items-center justify-center h-56 bg-neutral-quaternary rounded-base animate-pulse {width} {className}"
		style={height ? 'height: ' + height : ''}
	>
		<Icon name="tabler:photo" class="w-11 h-11 text-fg-disabled" />
		<span class="sr-only">Loading...</span>
	</div>
{:else if type === 'list'}
	<div role="status" class="space-y-4 animate-pulse {width} {className}">
		{#each { length: lines } as _}
			<div class="flex items-center">
				<div class="w-8 h-8 bg-neutral-quaternary rounded-full shrink-0"></div>
				<div class="ms-3 flex-1">
					<div class="h-2 bg-neutral-quaternary rounded-full w-full mb-2"></div>
					<div class="h-2 bg-neutral-quaternary rounded-full w-3/4"></div>
				</div>
			</div>
		{/each}
		<span class="sr-only">Loading...</span>
	</div>
{/if}
