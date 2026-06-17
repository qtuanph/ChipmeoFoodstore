<script lang="ts">
	interface Props {
		id?: string;
		label?: string;
		value?: string;
		placeholder?: string;
		required?: boolean;
		disabled?: boolean;
		error?: string;
		rows?: number;
		oninput?: (e: Event) => void;
	}

	let {
		id = '',
		label = '',
		value = $bindable(''),
		placeholder = '',
		required = false,
		disabled = false,
		error = '',
		rows = 3,
		oninput
	}: Props = $props();
</script>

{#if label}
	<label for={id || undefined} class="mb-2.5 block text-sm font-medium text-heading">{label}</label>
{/if}
<textarea
	{id}
	{placeholder}
	{required}
	{disabled}
	{rows}
	bind:value
	{oninput}
	class="block w-full rounded-base border bg-neutral-secondary-medium px-3 py-2.5 text-sm shadow-xs placeholder:text-body focus:border-brand focus:ring-brand focus:outline-none disabled:text-fg-disabled {error
		? 'bg-danger-soft border-danger-subtle text-fg-danger-strong placeholder:text-fg-danger-strong focus:border-danger focus:ring-danger'
		: 'text-heading border-default-medium'}"></textarea>
{#if error}
	<p class="mt-2.5 text-sm text-fg-danger-strong">{error}</p>
{/if}
