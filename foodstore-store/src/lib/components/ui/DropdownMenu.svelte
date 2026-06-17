<script lang="ts">
	import { onMount, onDestroy, tick } from 'svelte';
	import { Dropdown } from 'flowbite';
	import type { DropdownInterface } from 'flowbite';

	interface Props {
		id: string;
		triggerId: string;
		trigger?: 'click' | 'hover';
		placement?: 'top' | 'bottom' | 'left' | 'right' | 'bottom-start' | 'bottom-end' | 'top-start' | 'top-end';
		width?: string;
		class?: string;
		children: any;
	}

	let {
		id,
		triggerId,
		trigger = 'click',
		placement = 'bottom',
		width = 'w-44',
		class: className = '',
		children
	}: Props = $props();

	let dropdownEl = $state<DropdownInterface | null>(null);

	onMount(async () => {
		await tick();
		const targetEl = document.getElementById(id);
		const triggerEl = document.getElementById(triggerId);
		if (!targetEl || !triggerEl) return;

		dropdownEl = new Dropdown(targetEl, triggerEl, {
			placement,
			triggerType: trigger,
			offsetDistance: 8
		});
	});

	onDestroy(() => {
		dropdownEl?.destroyAndRemoveInstance();
	});
</script>

<div
	{id}
	class="z-10 hidden bg-neutral-primary-medium border border-default-medium rounded-base shadow-lg {width} {className}"
>
	{@render children()}
</div>
