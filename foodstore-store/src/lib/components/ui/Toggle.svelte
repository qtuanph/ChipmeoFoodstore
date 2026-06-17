<script lang="ts">
	interface Props {
		id?: string;
		label?: string;
		checked?: boolean;
		disabled?: boolean;
		onchange?: (e: Event) => void;
	}

	let {
		id = '',
		label = '',
		checked = $bindable(false),
		disabled = false,
		onchange
	}: Props = $props();
</script>

<label class="inline-flex items-center cursor-pointer">
	<input {id} {disabled} bind:checked {onchange} type="checkbox" class="sr-only peer" />
	<div
		class="relative w-9 h-5 rounded-full peer peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-brand-soft peer-checked:after:translate-x-full rtl:peer-checked:after:-translate-x-full peer-checked:after:border-buffer after:content-[''] after:absolute after:top-[2px] after:start-[2px] after:bg-white after:rounded-full after:h-4 after:w-4 after:transition-all peer-checked:bg-brand {disabled
			? 'bg-neutral-tertiary'
			: 'bg-neutral-quaternary'}"
	></div>
	{#if label}
		<span
			class="select-none ms-3 text-sm font-medium {disabled ? 'text-fg-disabled' : 'text-heading'}"
			>{label}</span
		>
	{/if}
</label>
