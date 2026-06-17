<script lang="ts">
	import { onMount, onDestroy } from 'svelte';
	import { Datepicker } from 'flowbite';
	import type { DatepickerInterface } from 'flowbite';
	import Icon from './Icon.svelte';

	interface Props {
		value?: string;
		label?: string;
		placeholder?: string;
		disabled?: boolean;
		format?: string;
		autohide?: boolean;
		class?: string;
		onchange?: (value: string) => void;
	}

	let {
		value = $bindable(''),
		label = '',
		placeholder = 'Select date',
		disabled = false,
		format = 'mm/dd/yyyy',
		autohide = true,
		class: className = '',
		onchange
	}: Props = $props();

	const id = 'datepicker-' + crypto.randomUUID();
	let datepickerEl = $state<DatepickerInterface | null>(null);

	onMount(() => {
		const inputEl = document.getElementById(id) as HTMLInputElement;
		if (!inputEl) return;

		datepickerEl = new Datepicker(inputEl, {
			autohide,
			format,
			orientation: 'bottom',
			buttons: false,
			onShow: () => {},
			onHide: () => {
				// Sync value back to Svelte binding
				value = inputEl.value;
				onchange?.(inputEl.value);
			}
		});
	});

	onDestroy(() => {
		datepickerEl?.destroyAndRemoveInstance();
	});
</script>

<div class={className}>
	{#if label}
		<label for={id} class="block mb-2.5 text-sm font-medium text-heading">{label}</label>
	{/if}
	<div class="relative max-w-sm">
		<div class="absolute inset-y-0 start-0 flex items-center ps-3 pointer-events-none">
			<Icon name="tabler:calendar" class="w-4 h-4 text-body" />
		</div>
		<input
			{id}
			type="text"
			bind:value
			{disabled}
			{placeholder}
			class="bg-neutral-secondary-medium border border-default-medium text-heading text-sm rounded-base focus:ring-brand focus:border-brand block w-full ps-9 px-3 py-2.5 shadow-xs placeholder:text-body"
			autocomplete="off"
		/>
	</div>
</div>
