<script lang="ts">
	interface Option {
		value: string | number;
		label: string;
	}

	interface Props {
		id?: string;
		label?: string;
		value?: string | number;
		options: Option[];
		placeholder?: string;
		required?: boolean;
		disabled?: boolean;
		error?: string;
		onchange?: (e: Event) => void;
	}

	let {
		id = '',
		label = '',
		value = $bindable(''),
		options = [],
		placeholder = '',
		required = false,
		disabled = false,
		error = '',
		onchange
	}: Props = $props();
</script>

{#if label}
	<label for={id || undefined} class="mb-2.5 block text-sm font-medium text-heading">{label}</label>
{/if}
<select
	{id}
	{required}
	{disabled}
	bind:value
	{onchange}
	class="block w-full rounded-base border bg-neutral-secondary-medium px-3 py-2.5 text-sm shadow-xs focus:border-brand focus:ring-brand focus:outline-none disabled:text-fg-disabled {error
		? 'bg-danger-soft border-danger-subtle text-fg-danger-strong focus:border-danger focus:ring-danger'
		: 'text-heading border-default-medium'}"
>
	{#if placeholder}
		<option value="" disabled>{placeholder}</option>
	{/if}
	{#each options as opt}
		<option value={opt.value}>{opt.label}</option>
	{/each}
</select>
{#if error}
	<p class="mt-2.5 text-sm text-fg-danger-strong">{error}</p>
{/if}
