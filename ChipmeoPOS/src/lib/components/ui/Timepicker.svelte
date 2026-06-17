<script lang="ts">
	interface Props {
		id?: string;
		value?: string;
		label?: string;
		disabled?: boolean;
		class?: string;
	}

	let {
		id = 'timepicker-' + crypto.randomUUID(),
		value = $bindable(''),
		label = '',
		disabled = false,
		class: className = ''
	}: Props = $props();
</script>

<div class={className}>
	{#if label}
		<label for={id} class="block mb-2.5 text-sm font-medium text-heading">{label}</label>
	{/if}
	<input
		{id}
		type="time"
		bind:value
		{disabled}
		class="bg-neutral-secondary-medium border border-default-medium text-heading text-sm rounded-base focus:ring-brand focus:border-brand block w-full px-3 py-2.5 shadow-xs placeholder:text-body"
	/>
</div>
