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

<div class="flex items-center">
	<input
		{id}
		{disabled}
		bind:checked
		{onchange}
		type="checkbox"
		class="w-4 h-4 rounded-xs border bg-neutral-secondary-medium focus:ring-2 focus:ring-brand-soft {disabled
			? 'border-light'
			: 'border-default-medium'}"
	/>
	{#if label}
		<label
			for={id || undefined}
			class="select-none ms-2 text-sm font-medium {disabled ? 'text-fg-disabled' : 'text-heading'}"
			>{label}</label
		>
	{/if}
</div>
