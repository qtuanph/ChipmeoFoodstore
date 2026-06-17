<script lang="ts">
	import { onMount } from 'svelte';
	import { initFlowbite } from 'flowbite';
	import Icon from './Icon.svelte';

	interface AccordionItem {
		title: string;
		content?: string;
		open?: boolean;
		children?: any;
	}

	interface Props {
		id?: string;
		items: AccordionItem[];
		class?: string;
	}

	let {
		id = 'accordion-' + crypto.randomUUID(),
		items = [],
		class: className = ''
	}: Props = $props();

	onMount(() => {
		initFlowbite();
	});
</script>

<div
	{id}
	data-accordion="collapse"
	class="rounded-base border border-default overflow-hidden shadow-xs {className}"
>
	{#each items as item, i (item.title)}
		<h2 id="{id}-heading-{i}">
			<button
				type="button"
				class="flex items-center justify-between w-full p-5 font-medium rtl:text-right text-body hover:text-heading hover:bg-neutral-secondary-medium gap-3
					{i === 0 ? 'rounded-t-base border border-t-0 border-x-0 border-b-default' : ''}
					{i > 0 && i < items.length - 1 ? 'border border-x-0 border-b-default border-t-0' : ''}"
				data-accordion-target="#{id}-body-{i}"
				aria-expanded={item.open || false}
				aria-controls="{id}-body-{i}"
			>
				<span>{item.title}</span>
				<Icon
					name="tabler:chevron-down"
					class="w-5 h-5 shrink-0 {item.open ? 'rotate-180' : ''}"
					data-accordion-icon
				/>
			</button>
		</h2>
		<div
			id="{id}-body-{i}"
			class="hidden {i < items.length - 1
				? 'border border-s-0 border-e-0 border-t-0 border-b-default'
				: ''}"
			aria-labelledby="{id}-heading-{i}"
		>
			<div
				class="p-4 md:p-5 {i === items.length - 1
					? 'border border-t-default border-b-0 border-x-0'
					: ''}"
			>
				{#if item.content}
					<p class="mb-2 text-body">{item.content}</p>
				{/if}
				{#if item.children}
					{@render item.children()}
				{/if}
			</div>
		</div>
	{/each}
</div>
