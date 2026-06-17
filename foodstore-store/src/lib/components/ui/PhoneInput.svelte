<script lang="ts">
	interface Props {
		id?: string;
		value?: string;
		label?: string;
		placeholder?: string;
		disabled?: boolean;
		class?: string;
	}

	let {
		id = 'phone-' + crypto.randomUUID(),
		value = $bindable(''),
		label = '',
		placeholder = '123-456-7890',
		disabled = false,
		class: className = ''
	}: Props = $props();
</script>

<div class={className}>
	{#if label}
		<label for={id} class="block mb-2.5 text-sm font-medium text-heading">{label}</label>
	{/if}
	<div class="flex">
		<span
			class="shrink-0 inline-flex items-center px-3 py-2.5 text-sm text-body bg-neutral-tertiary border border-default-medium border-e-0 rounded-s-base"
			>+84</span
		>
		<input
			{id}
			type="tel"
			bind:value
			{disabled}
			{placeholder}
			class="bg-neutral-secondary-medium border border-default-medium text-heading text-sm rounded-e-base focus:ring-brand focus:border-brand block w-full px-3 py-2.5 shadow-xs placeholder:text-body"
		/>
	</div>
</div>
