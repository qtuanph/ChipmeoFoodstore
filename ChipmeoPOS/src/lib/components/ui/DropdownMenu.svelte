<script lang="ts">
	import { onMount } from 'svelte';
	import { initFlowbite } from 'flowbite';

	interface Props {
		id: string;
		trigger?: 'click' | 'hover';
		width?: string;
		class?: string;
		children: any;
	}

	let { id, trigger = 'click', width = 'w-44', class: className = '', children }: Props = $props();

	onMount(() => {
		initFlowbite();
	});
</script>

<div
	{id}
	data-dropdown-trigger={trigger === 'hover' ? 'hover' : undefined}
	class="z-10 hidden bg-neutral-primary-medium border border-default-medium rounded-base shadow-lg {width} {className}"
>
	{@render children()}
</div>
