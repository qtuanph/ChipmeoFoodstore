<script lang="ts">
	import { onMount, onDestroy } from 'svelte';
	import { Dropdown } from 'flowbite';
	import type { DropdownInterface } from 'flowbite';
	import Icon from './Icon.svelte';

	interface MegaMenuColumn {
		title?: string;
		links?: { label: string; href: string; description?: string }[];
	}

	interface Props {
		id?: string;
		triggerLabel?: string;
		columns?: MegaMenuColumn[];
		class?: string;
	}

	let {
		id = 'megamenu-' + crypto.randomUUID(),
		triggerLabel = 'Categories',
		columns = [],
		class: className = ''
	}: Props = $props();

	let dropdownEl = $state<DropdownInterface | null>(null);

	onMount(() => {
		const targetEl = document.getElementById(id);
		const triggerEl = document.getElementById(id + '-button');
		if (!targetEl || !triggerEl) return;

		dropdownEl = new Dropdown(targetEl, triggerEl, {
			placement: 'bottom',
			triggerType: 'click',
			offsetDistance: 8
		});
	});

	onDestroy(() => {
		dropdownEl?.destroyAndRemoveInstance();
	});
</script>

<div class="relative {className}">
	<button
		id={id + '-button'}
		class="flex items-center justify-between w-full py-2 px-3 font-medium text-heading md:w-auto hover:bg-neutral-secondary-soft md:hover:bg-transparent md:border-0 md:hover:text-fg-brand md:p-0"
	>
		{triggerLabel}
		<Icon name="tabler:chevron-down" class="w-4 h-4 ms-1.5" />
	</button>
	<div
		{id}
		class="absolute z-10 grid hidden w-auto grid-cols-2 text-sm bg-neutral-primary-soft border border-default rounded-base shadow md:grid-cols-3"
	>
		{#each columns as col}
			<div class="p-4 pb-0 text-heading md:pb-4">
				{#if col.title}
					<h3 class="mb-2 text-sm font-semibold text-heading">{col.title}</h3>
				{/if}
				<ul class="space-y-3" aria-labelledby={id + '-button'}>
					{#each col.links || [] as link}
						<li>
							<a
								href={link.href}
								class="block text-sm text-body hover:text-fg-brand rounded px-2 py-1 hover:bg-neutral-tertiary"
								onclick={() => dropdownEl?.hide()}
								>{link.label}</a
							>
						</li>
					{/each}
				</ul>
			</div>
		{/each}
	</div>
</div>
