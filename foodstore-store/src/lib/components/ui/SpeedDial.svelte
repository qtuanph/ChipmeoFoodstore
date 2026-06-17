<script lang="ts">
	import { onMount, onDestroy } from 'svelte';
	import { Dial } from 'flowbite';
	import type { DialInterface } from 'flowbite';
	import Icon from './Icon.svelte';

	interface Action {
		label: string;
		icon: string;
		onclick?: () => void;
	}

	interface Props {
		id?: string;
		actions?: Action[];
		trigger?: 'hover' | 'click';
		position?: 'tl' | 'tr' | 'bl' | 'br';
		class?: string;
	}

	let {
		id = 'speeddial-' + crypto.randomUUID(),
		actions = [],
		trigger = 'hover',
		position = 'br',
		class: className = ''
	}: Props = $props();

	const posClasses: Record<string, string> = {
		tl: 'top-5 left-5',
		tr: 'top-5 right-5',
		bl: 'bottom-5 left-5',
		br: 'bottom-5 right-5'
	};

	let dialEl = $state<DialInterface | null>(null);

	onMount(() => {
		const parentEl = document.getElementById(id + '-container');
		const triggerEl = document.getElementById(id + '-button');
		const targetEl = document.getElementById(id + '-menu');
		if (!parentEl || !triggerEl || !targetEl) return;

		dialEl = new Dial(parentEl, triggerEl, targetEl, {
			triggerType: trigger,
			onShow: () => {},
			onHide: () => {}
		});
	});

	onDestroy(() => {
		dialEl?.destroyAndRemoveInstance();
	});
</script>

<div
	id={id + '-container'}
	class="fixed {posClasses[position]} z-50 group {className}"
>
	<div id={id + '-menu'} class="flex flex-col items-center hidden mb-4 space-y-2">
		{#each actions as action}
			<button
				type="button"
				onclick={() => {
					action.onclick?.();
					dialEl?.hide();
				}}
				title={action.label}
				class="w-[52px] h-[52px] text-body hover:text-heading bg-neutral-primary-soft rounded-full border border-default shadow-xs hover:bg-neutral-secondary-medium hover:border-default-medium focus:ring-4 focus:ring-neutral-secondary-soft focus:outline-none flex items-center justify-center"
			>
				<Icon name={action.icon} class="w-5 h-5" />
				<span class="sr-only">{action.label}</span>
			</button>
		{/each}
	</div>
	<button
		id={id + '-button'}
		type="button"
		aria-controls={id + '-menu'}
		aria-expanded="false"
		class="flex items-center justify-center text-white bg-brand rounded-full w-14 h-14 hover:bg-brand-strong focus:ring-4 focus:ring-brand-medium focus:outline-none"
	>
		<Icon name="tabler:plus" class="w-5 h-5 transition-transform group-hover:rotate-45" />
		<span class="sr-only">Open actions menu</span>
	</button>
</div>
