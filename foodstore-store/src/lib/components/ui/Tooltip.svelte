<script lang="ts">
	import { onMount, onDestroy } from 'svelte';
	import { Tooltip } from 'flowbite';
	import type { TooltipInterface } from 'flowbite';

	interface Props {
		id: string;
		triggerId: string;
		content: string;
		placement?: 'top' | 'right' | 'bottom' | 'left';
		triggerType?: 'hover' | 'click' | 'none';
		style?: 'dark' | 'light';
	}

	let {
		id,
		triggerId,
		content,
		placement = 'top',
		triggerType = 'hover',
		style = 'dark'
	}: Props = $props();

	let tooltipEl = $state<TooltipInterface | null>(null);

	onMount(() => {
		const targetEl = document.getElementById(id);
		const triggerEl = document.getElementById(triggerId);
		if (!targetEl || !triggerEl) return;

		tooltipEl = new Tooltip(targetEl, triggerEl, {
			placement,
			triggerType
		});
	});

	onDestroy(() => {
		tooltipEl?.destroyAndRemoveInstance();
	});
</script>

<div
	{id}
	role="tooltip"
	class="absolute z-10 invisible inline-block px-3 py-2 text-sm font-medium transition-opacity duration-300 opacity-0 tooltip {style ===
	'light'
		? 'text-heading bg-neutral-primary-medium border border-default rounded-base shadow-xs'
		: 'text-white bg-dark rounded-base shadow-xs'}"
>
	{content}
	<div class="tooltip-arrow" data-popper-arrow></div>
</div>
