<script lang="ts">
	interface Props {
		id?: string;
		name?: string;
		label?: string;
		value?: string;
		group?: string;
		disabled?: boolean;
		onchange?: (e: Event) => void;
	}

	let {
		id = '',
		name = '',
		label = '',
		value = '',
		group = $bindable(''),
		disabled = false,
		onchange
	}: Props = $props();
</script>

<div class="flex items-center">
	<input
		{id}
		{name}
		{disabled}
		{onchange}
		type="radio"
		bind:group
		{value}
		class="w-4 h-4 text-neutral-primary border-default-strong bg-neutral-secondary-strong rounded-full checked:border-brand focus:ring-2 focus:outline-none focus:ring-brand-subtle border border-default appearance-none"
	/>
	{#if label}
		<label
			for={id || undefined}
			class="select-none ms-2 text-sm font-medium {disabled ? 'text-fg-disabled' : 'text-heading'}"
			>{label}</label
		>
	{/if}
</div>
