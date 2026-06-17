<script lang="ts">
	import { onMount } from 'svelte';
	import { Drawer } from 'flowbite';
	import type { DrawerOptions, DrawerInterface } from 'flowbite';
	import Icon from './Icon.svelte';

	interface Props {
		id: string;
		open?: boolean;
		title?: string;
		position?: 'left' | 'right' | 'top' | 'bottom';
		closable?: boolean;
		children: any;
		footer?: any;
	}

	let {
		id,
		open = $bindable(false),
		title = '',
		position = 'left',
		closable = true,
		children,
		footer
	}: Props = $props();

	const positionClasses: Record<string, string> = {
		left: 'top-0 left-0 w-96 -translate-x-full border-e',
		right: 'top-0 right-0 w-96 translate-x-full border-s',
		top: 'top-0 left-0 right-0 h-96 -translate-y-full border-b',
		bottom: 'bottom-0 left-0 right-0 h-96 translate-y-full border-t'
	};

	let drawerEl: DrawerInterface;

	onMount(() => {
		const el = document.getElementById(id);
		if (!el) return;

		const options: DrawerOptions = {
			onHide: () => {
				open = false;
			}
		};

		drawerEl = new Drawer(el, options);
	});

	$effect(() => {
		if (!drawerEl) return;
		if (open) drawerEl.show();
		else drawerEl.hide();
	});
</script>

<div
	{id}
	tabindex="-1"
	aria-labelledby="{id}-label"
	data-drawer-placement={position}
	class="fixed z-40 h-screen p-4 overflow-y-auto transition-transform bg-neutral-primary-soft {positionClasses[
		position
	]} border-default"
>
	{#if title || closable}
		<div class="flex items-center justify-between mb-4">
			{#if title}
				<h5 id="{id}-label" class="text-lg font-medium text-heading">{title}</h5>
			{/if}
			{#if closable}
				<button
					type="button"
					data-drawer-hide={id}
					aria-controls={id}
					class="absolute top-2.5 end-2.5 text-body bg-transparent hover:text-heading hover:bg-neutral-tertiary rounded-base w-9 h-9 flex items-center justify-center"
				>
					<Icon name="tabler:x" class="w-5 h-5" />
					<span class="sr-only">Close drawer</span>
				</button>
			{/if}
		</div>
	{/if}
	<div class="py-4">
		{@render children()}
	</div>
	{#if footer}
		<div class="border-t border-default pt-4">
			{@render footer()}
		</div>
	{/if}
</div>
