<script lang="ts">
	import { onMount, onDestroy } from 'svelte';
	import { Tabs } from 'flowbite';
	import type { TabsInterface } from 'flowbite';

	interface Tab {
		id: string;
		label: string;
	}

	interface Props {
		id?: string;
		tabs: Tab[];
		contentId?: string;
		activeClasses?: string;
		inactiveClasses?: string;
		defaultTab?: string;
		class?: string;
		children: any;
	}

	let {
		id = 'tabs-' + crypto.randomUUID(),
		tabs = [],
		contentId = 'tabs-content-' + crypto.randomUUID(),
		activeClasses = 'text-fg-brand border-brand',
		inactiveClasses = 'border-transparent text-body hover:text-fg-brand hover:border-brand',
		defaultTab = '',
		class: className = '',
		children
	}: Props = $props();

	let tabsEl = $state<TabsInterface | null>(null);

	onMount(() => {
		const tabElements = tabs.map((tab) => ({
			id: tab.id,
			triggerEl: document.querySelector<HTMLElement>(`#${tab.id}-tab`),
			targetEl: document.querySelector<HTMLElement>(`#${tab.id}`)
		}));

		const options = {
			defaultTabId: defaultTab || (tabs[0]?.id ?? ''),
			activeClasses,
			inactiveClasses
		};

		tabsEl = new Tabs(tabElements as any, options as any);
	});

	onDestroy(() => {
		tabsEl?.destroyAndRemoveInstance();
	});
</script>

<div class="mb-4 border-b border-default {className}">
	<ul
		class="flex flex-wrap -mb-px text-sm font-medium text-center"
		{id}
		role="tablist"
	>
		{#each tabs as tab, i (tab.id)}
			<li class="me-2" role="presentation">
				<button
					class="inline-block p-4 border-b-2 rounded-t-base"
					id={tab.id + '-tab'}
					data-tabs-target={'#' + tab.id}
					type="button"
					role="tab"
					aria-controls={tab.id}
					aria-selected={tab.id === defaultTab || (i === 0 && !defaultTab) ? 'true' : 'false'}
				>
					{tab.label}
				</button>
			</li>
		{/each}
	</ul>
</div>
<div id={contentId}>
	{@render children()}
</div>
