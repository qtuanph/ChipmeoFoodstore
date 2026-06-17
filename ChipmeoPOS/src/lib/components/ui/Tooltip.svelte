<script lang="ts">
	import { onMount } from 'svelte';
	import { initFlowbite } from 'flowbite';

	interface Props {
		target: string;
		content: string;
		placement?: 'top' | 'right' | 'bottom' | 'left';
		style?: 'dark' | 'light';
	}

	let { target, content, placement = 'top', style = 'dark' }: Props = $props();

	onMount(() => {
		initFlowbite();
	});
</script>

<div
	id={target}
	role="tooltip"
	data-tooltip-placement={placement}
	class="absolute z-10 invisible inline-block px-3 py-2 text-sm font-medium transition-opacity duration-300 opacity-0 tooltip {style ===
	'light'
		? 'text-heading bg-neutral-primary-medium border border-default rounded-base shadow-xs'
		: 'text-white bg-dark rounded-base shadow-xs'}"
>
	{content}
	<div class="tooltip-arrow" data-popper-arrow></div>
</div>
