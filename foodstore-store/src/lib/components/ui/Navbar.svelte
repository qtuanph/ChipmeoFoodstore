<script lang="ts">
	import { onMount, onDestroy } from 'svelte';
	import { Dropdown } from 'flowbite';
	import type { DropdownInterface } from 'flowbite';
	import Icon from './Icon.svelte';

	interface NavItem {
		label: string;
		href: string;
		active?: boolean;
		items?: NavItem[];
	}

	interface Brand {
		url?: string;
		img?: string;
		text?: string;
	}

	interface Props {
		brand?: Brand;
		items?: NavItem[];
		cta?: { label: string; href?: string; onclick?: () => void };
		sticky?: boolean;
	}

	let { brand = {}, items = [], cta, sticky = false }: Props = $props();

	// Svelte state for mobile menu — no Flowbite Collapse needed
	let mobileOpen = $state(false);

	// Dropdown instances for nav items with sub-menus
	let dropdownInstances: DropdownInterface[] = [];

	onMount(() => {
		// Init a Flowbite Dropdown for each nav item that has sub-items
		items.forEach((item, i) => {
			if (!item.items || item.items.length === 0) return;
			const targetEl = document.getElementById(`dropdownNavbar-${i}`);
			const triggerEl = document.getElementById(`dropdownNavbarButton-${i}`);
			if (!targetEl || !triggerEl) return;

			const dropdown = new Dropdown(targetEl, triggerEl, {
				placement: 'bottom',
				triggerType: 'click',
				offsetDistance: 4
			});
			dropdownInstances.push(dropdown);
		});
	});

	onDestroy(() => {
		dropdownInstances.forEach((d) => d.destroyAndRemoveInstance());
		dropdownInstances = [];
	});
</script>

<nav
	class="bg-neutral-primary border-b border-default {sticky
		? 'fixed w-full z-20 top-0 start-0'
		: ''}"
>
	<div class="max-w-screen-xl flex flex-wrap items-center justify-between mx-auto p-4">
		<a href={brand.url || '/'} class="flex items-center space-x-3 rtl:space-x-reverse">
			{#if brand.img}
				<img src={brand.img} class="h-8" alt={brand.text || 'Brand'} />
			{/if}
			{#if brand.text}
				<span class="self-center text-xl font-semibold whitespace-nowrap text-heading"
					>{brand.text}</span
				>
			{/if}
		</a>

		<div class="flex items-center md:order-2 space-x-3 rtl:space-x-reverse">
			{#if cta}
				{#if cta.href}
					<a
						href={cta.href}
						class="text-white bg-brand hover:bg-brand-strong box-border border border-transparent focus:ring-4 focus:ring-brand-medium shadow-xs font-medium leading-5 rounded-base text-sm px-3 py-2 focus:outline-none"
					>
						{cta.label}
					</a>
				{:else}
					<button
						onclick={cta.onclick}
						class="text-white bg-brand hover:bg-brand-strong box-border border border-transparent focus:ring-4 focus:ring-brand-medium shadow-xs font-medium leading-5 rounded-base text-sm px-3 py-2 focus:outline-none"
					>
						{cta.label}
					</button>
				{/if}
			{/if}

			<!-- Mobile menu toggle — pure Svelte state, no Flowbite needed -->
			<button
				type="button"
				onclick={() => (mobileOpen = !mobileOpen)}
				class="inline-flex items-center p-2 w-10 h-10 justify-center text-sm text-body rounded-base md:hidden hover:bg-neutral-secondary-soft hover:text-heading focus:outline-none focus:ring-2 focus:ring-neutral-tertiary"
				aria-controls="navbar-default"
				aria-expanded={mobileOpen}
			>
				<span class="sr-only">Open main menu</span>
				<Icon name={mobileOpen ? 'tabler:x' : 'tabler:menu'} class="w-6 h-6" />
			</button>
		</div>

		<div class="{mobileOpen ? 'block' : 'hidden'} w-full md:block md:w-auto" id="navbar-default">
			<ul
				class="font-medium flex flex-col p-4 md:p-0 mt-4 border border-default rounded-base bg-neutral-secondary-soft md:flex-row md:space-x-8 rtl:space-x-reverse md:mt-0 md:border-0 md:bg-neutral-primary"
			>
				{#each items as item, i}
					<li>
						{#if item.items && item.items.length > 0}
							<button
								id="dropdownNavbarButton-{i}"
								class="flex items-center justify-between w-full py-2 px-3 rounded font-medium text-heading md:w-auto hover:bg-neutral-tertiary md:hover:bg-transparent md:border-0 md:hover:text-fg-brand md:p-0"
							>
								{item.label}
								<Icon name="tabler:chevron-down" class="w-4 h-4 ms-1.5" />
							</button>
							<div
								id="dropdownNavbar-{i}"
								class="z-10 hidden bg-neutral-primary-medium border border-default-medium rounded-base shadow-lg w-44"
							>
								<ul
									class="p-2 text-sm text-body font-medium"
									aria-labelledby="dropdownNavbarButton-{i}"
								>
									{#each item.items as subItem}
										<li>
											<a
												href={subItem.href}
												class="inline-flex items-center w-full p-2 hover:bg-neutral-tertiary-medium hover:text-heading rounded {subItem.active
													? 'text-fg-brand'
													: 'text-body'}"
											>
												{subItem.label}
											</a>
										</li>
									{/each}
								</ul>
							</div>
						{:else}
							<a
								href={item.href}
								class="block py-2 px-3 text-heading rounded hover:bg-neutral-tertiary md:hover:bg-transparent md:border-0 md:hover:text-fg-brand md:p-0 {item.active
									? 'text-white bg-brand rounded md:bg-transparent md:text-fg-brand md:p-0'
									: ''}"
							>
								{item.label}
							</a>
						{/if}
					</li>
				{/each}
			</ul>
		</div>
	</div>
</nav>
