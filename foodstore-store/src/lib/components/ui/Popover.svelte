<script lang="ts">
	import { onMount, onDestroy } from 'svelte';
	import { Popover } from 'flowbite';
	import type { PopoverInterface } from 'flowbite';

	interface Props {
		id: string;
		triggerId: string;
		placement?: 'top' | 'right' | 'bottom' | 'left';
		triggerType?: 'hover' | 'click' | 'none';
		title?: string;
		content?: string;
		children?: any;
	}

	let {
		id,
		triggerId,
		placement = 'bottom',
		triggerType = 'hover',
		title = '',
		content = '',
		children
	}: Props = $props();

	let popoverEl = $state<PopoverInterface | null>(null);

	onMount(() => {
		const targetEl = document.getElementById(id);
		const triggerEl = document.getElementById(triggerId);
		if (!targetEl || !triggerEl) return;

		popoverEl = new Popover(targetEl, triggerEl, {
			placement,
			triggerType,
			offset: 10
		});
	});

	onDestroy(() => {
		popoverEl?.destroyAndRemoveInstance();
	});
</script>

<div
	{id}
	role="tooltip"
	data-popover-placement={placement}
	class="absolute z-10 invisible inline-block w-64 text-sm text-body transition-opacity duration-300 bg-neutral-primary-soft border border-default rounded-base shadow-xs opacity-0 popover"
>
	{#if title}
		<div class="px-3 py-2 bg-neutral-tertiary border-b border-default rounded-t-base">
			<h3 class="font-medium text-heading">{title}</h3>
		</div>
	{/if}
	<div class="px-3 py-2">
		{#if children}
			{@render children()}
		{:else if content}
			<p>{content}</p>
		{/if}
	</div>
	<div data-popper-arrow></div>
</div>
