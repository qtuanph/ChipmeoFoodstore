<script lang="ts">
	import { onMount } from 'svelte';
	import { initFlowbite } from 'flowbite';
	import Icon from './Icon.svelte';

	interface Action {
		label: string;
		icon: string;
		onclick?: () => void;
	}

	interface Props {
		id?: string;
		actions?: Action[];
		position?: 'tl' | 'tr' | 'bl' | 'br';
		class?: string;
	}

	let {
		id = 'speeddial-' + crypto.randomUUID(),
		actions = [],
		position = 'br',
		class: className = ''
	}: Props = $props();

	const posClasses: Record<string, string> = {
		tl: 'top-5 left-5',
		tr: 'top-5 right-5',
		bl: 'bottom-5 left-5',
		br: 'bottom-5 right-5'
	};

	onMount(() => {
		initFlowbite();
	});
</script>

<div data-dial-init class="fixed {posClasses[position]} z-50 group {className}">
	<div id={id + '-menu'} class="flex flex-col items-center hidden mb-4 space-y-2">
		{#each actions as action}
			<button
				type="button"
				onclick={action.onclick}
				class="w-[52px] h-[52px] text-body hover:text-heading bg-neutral-primary-soft rounded-full border border-default shadow-xs hover:bg-neutral-secondary-medium hover:border-default-medium focus:ring-4 focus:ring-neutral-secondary-soft focus:outline-none flex items-center justify-center"
			>
				{@html action.icon}
				<span class="sr-only">{action.label}</span>
			</button>
		{/each}
	</div>
	<button
		id={id + '-button'}
		data-dial-toggle={id + '-menu'}
		type="button"
		aria-controls={id + '-menu'}
		aria-expanded="false"
		class="flex items-center justify-center text-white bg-brand rounded-full w-14 h-14 hover:bg-brand-strong focus:ring-4 focus:ring-brand-medium focus:outline-none"
	>
		<Icon name="tabler:plus" class="w-5 h-5 transition-transform group-hover:rotate-45" />
		<span class="sr-only">Open actions menu</span>
	</button>
</div>
