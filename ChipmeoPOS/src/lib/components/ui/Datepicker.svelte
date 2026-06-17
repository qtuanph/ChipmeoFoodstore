<script lang="ts">
	import { onMount } from 'svelte';
	import { initFlowbite } from 'flowbite';
	import Icon from './Icon.svelte';

	interface Props {
		value?: string;
		label?: string;
		placeholder?: string;
		disabled?: boolean;
		class?: string;
	}

	let {
		value = $bindable(''),
		label = '',
		placeholder = 'Select date',
		disabled = false,
		class: className = ''
	}: Props = $props();

	const id = 'datepicker-' + crypto.randomUUID();

	onMount(() => {
		initFlowbite();
	});
</script>

<div class={className}>
	{#if label}
		<label for={id} class="block mb-2.5 text-sm font-medium text-heading">{label}</label>
	{/if}
	<div class="relative max-w-sm">
		<div class="absolute inset-y-0 start-0 flex items-center ps-3 pointer-events-none">
			<Icon name="tabler:calendar" class="w-4 h-4 text-body" />
		</div>
		<input
			{id}
			type="text"
			bind:value
			{disabled}
			{placeholder}
			class="bg-neutral-secondary-medium border border-default-medium text-heading text-sm rounded-base focus:ring-brand focus:border-brand block w-full ps-9 px-3 py-2.5 shadow-xs placeholder:text-body"
		/>
	</div>
</div>
