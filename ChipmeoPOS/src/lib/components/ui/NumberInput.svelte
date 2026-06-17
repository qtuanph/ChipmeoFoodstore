<script lang="ts">
	import Icon from './Icon.svelte';

	interface Props {
		id?: string;
		value?: number;
		min?: number;
		max?: number;
		step?: number;
		label?: string;
		disabled?: boolean;
		placeholder?: string;
		class?: string;
	}

	let {
		id = 'numberinput-' + crypto.randomUUID(),
		value = $bindable(0),
		min = 0,
		max = 100,
		step = 1,
		label = '',
		disabled = false,
		placeholder = '',
		class: className = ''
	}: Props = $props();

	function decrement() {
		if (value > min) value = Math.max(min, value - step);
	}

	function increment() {
		if (value < max) value = Math.min(max, value + step);
	}
</script>

<div class={className}>
	{#if label}
		<label for={id} class="block mb-2.5 text-sm font-medium text-heading">{label}</label>
	{/if}
	<div class="relative flex items-center max-w-[8rem]">
		<button
			type="button"
			aria-label="Decrease"
			onclick={decrement}
			disabled={disabled || value <= min}
			class="shrink-0 bg-neutral-primary-soft border border-default hover:bg-neutral-secondary-medium hover:text-heading rounded-s-base p-2.5 h-10 focus:ring-2 focus:ring-neutral-tertiary focus:outline-none {disabled
				? 'opacity-50 cursor-not-allowed'
				: ''}"
		>
			<Icon name="tabler:minus" class="w-4 h-4" />
		</button>
		<input
			{id}
			type="number"
			bind:value
			{min}
			{max}
			{step}
			{disabled}
			{placeholder}
			class="bg-neutral-secondary-medium border border-x-0 border-default-medium text-heading text-sm rounded-none block w-full px-3 py-2.5 shadow-xs placeholder:text-body text-center focus:ring-0 focus:border-default-medium"
		/>
		<button
			type="button"
			aria-label="Increase"
			onclick={increment}
			disabled={disabled || value >= max}
			class="shrink-0 bg-neutral-primary-soft border border-default hover:bg-neutral-secondary-medium hover:text-heading rounded-e-base p-2.5 h-10 focus:ring-2 focus:ring-neutral-tertiary focus:outline-none {disabled
				? 'opacity-50 cursor-not-allowed'
				: ''}"
		>
			<Icon name="tabler:plus" class="w-4 h-4" />
		</button>
	</div>
</div>
