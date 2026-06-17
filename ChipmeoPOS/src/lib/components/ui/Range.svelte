<script lang="ts">
	interface Props {
		id?: string;
		value?: number;
		min?: number;
		max?: number;
		step?: number;
		disabled?: boolean;
		label?: string;
		class?: string;
	}

	let {
		id = 'range-' + crypto.randomUUID(),
		value = $bindable(0),
		min = 0,
		max = 100,
		step = 1,
		disabled = false,
		label = '',
		class: className = ''
	}: Props = $props();
</script>

{#if label}
	<label
		for={id}
		class="block mb-2.5 text-sm font-medium {disabled ? 'text-fg-disabled' : 'text-heading'}"
		>{label}</label
	>
{/if}
<input
	{id}
	type="range"
	bind:value
	{min}
	{max}
	{step}
	{disabled}
	class="w-full h-2 bg-neutral-quaternary rounded-full appearance-none cursor-pointer {disabled
		? 'opacity-50 cursor-not-allowed'
		: ''} {className}"
/>
