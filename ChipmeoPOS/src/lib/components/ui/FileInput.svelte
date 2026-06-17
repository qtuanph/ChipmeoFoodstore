<script lang="ts">
	interface Props {
		id?: string;
		label?: string;
		accept?: string;
		multiple?: boolean;
		disabled?: boolean;
		required?: boolean;
		helperText?: string;
		onchange?: (e: Event) => void;
	}

	let {
		id = '',
		label = '',
		accept = '',
		multiple = false,
		disabled = false,
		required = false,
		helperText = '',
		onchange
	}: Props = $props();
</script>

{#if label}
	<label for={id || undefined} class="block mb-2.5 text-sm font-medium text-heading">{label}</label>
{/if}
<input
	{id}
	{accept}
	{multiple}
	{disabled}
	{required}
	{onchange}
	type="file"
	class="block w-full text-sm rounded-base cursor-pointer bg-neutral-secondary-medium border border-default-medium text-heading focus:outline-none placeholder:text-body file:me-4 file:py-2.5 file:px-4 file:rounded-none file:border-0 file:text-sm file:font-medium file:text-heading file:bg-neutral-tertiary-medium hover:file:bg-neutral-quaternary"
/>
{#if helperText}
	<p class="mt-1 text-sm text-body">{helperText}</p>
{/if}
