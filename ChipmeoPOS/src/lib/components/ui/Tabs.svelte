<script lang="ts">
	import { onMount } from 'svelte';
	import { initFlowbite } from 'flowbite';

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
		id = crypto.randomUUID(),
		tabs = [],
		contentId = crypto.randomUUID(),
		activeClasses = 'text-fg-brand border-brand',
		inactiveClasses = 'border-transparent text-body hover:text-fg-brand hover:border-brand',
		defaultTab = '',
		class: className = '',
		children
	}: Props = $props();

	onMount(() => {
		initFlowbite();
	});
</script>

<div class="mb-4 border-b border-default {className}">
	<ul
		class="flex flex-wrap -mb-px text-sm font-medium text-center"
		{id}
		data-tabs-toggle={'#' + contentId}
		data-tabs-active-classes={activeClasses}
		data-tabs-inactive-classes={inactiveClasses}
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
					aria-selected={tab.id === defaultTab ? 'true' : 'false'}
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
