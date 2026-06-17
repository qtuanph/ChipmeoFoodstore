<script lang="ts">
	import { onMount } from 'svelte';
	import { initFlowbite } from 'flowbite';

	interface Props {
		id: string;
		trigger?: string;
		placement?: 'top' | 'right' | 'bottom' | 'left';
		title?: string;
		content?: string;
		children?: any;
	}

	let {
		id,
		trigger = '',
		placement = 'bottom',
		title = '',
		content = '',
		children
	}: Props = $props();

	onMount(() => {
		initFlowbite();
	});
</script>

<div
	{id}
	role="tooltip"
	data-popover-placement={placement}
	class="absolute z-10 invisible inline-block w-64 text-sm text-body transition-opacity duration-300 bg-neutral-primary-soft border border-default rounded-base shadow-xs opacity-0 popover"
>
	<div class="px-3 py-2 bg-neutral-tertiary border-b border-default rounded-t-base">
		<h3 class="font-medium text-heading">{title}</h3>
	</div>
	<div class="px-3 py-2">
		{#if children}
			{@render children()}
		{:else if content}
			<p>{content}</p>
		{/if}
	</div>
	<div data-popper-arrow></div>
</div>
