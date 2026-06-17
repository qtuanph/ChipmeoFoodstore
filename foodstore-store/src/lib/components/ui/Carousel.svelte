<script lang="ts">
	import { onMount, onDestroy, untrack } from 'svelte';
	import { Carousel } from 'flowbite';
	import type { CarouselInterface } from 'flowbite';
	import type { CarouselOptions } from 'flowbite';
	import Icon from './Icon.svelte';

	interface Slide {
		content: any;
		active?: boolean;
		alt?: string;
	}

	interface Props {
		id?: string;
		slides?: Slide[];
		slide?: boolean;
		interval?: number;
		showIndicators?: boolean;
		showControls?: boolean;
		class?: string;
	}

	let {
		id: propId,
		slides = [],
		slide: autoSlide = true,
		interval = 3000,
		showIndicators = true,
		showControls = true,
		class: className = ''
	}: Props = $props();

	// Stable ID — same pattern as Modal
	const id: string = untrack(() => propId ?? 'carousel-' + crypto.randomUUID());

	let carouselEl = $state<CarouselInterface | null>(null);

	onMount(async () => {
		const el = document.getElementById(id);
		if (!el) return;

		const items = slides
			.map((s, i) => ({
				position: i,
				el: document.getElementById(id + '-item-' + i)
			}))
			.filter((item): item is { position: number; el: HTMLElement } => item.el !== null);

		const indicatorItems = slides
			.map((s, i) => ({
				position: i,
				el: document.getElementById(id + '-indicator-' + i)
			}))
			.filter((item): item is { position: number; el: HTMLElement } => item.el !== null);

		const options: CarouselOptions = {
			defaultPosition: slides.findIndex((s) => s.active) || 0,
			interval,
			indicators:
				indicatorItems.length > 0
					? {
							activeClasses: 'bg-white',
							inactiveClasses: 'bg-white/50 hover:bg-white',
							items: indicatorItems
						}
					: undefined
		};

		carouselEl = new Carousel(el, items, options);

		// Start auto-sliding if enabled
		if (autoSlide) {
			carouselEl.cycle();
		}
	});

	onDestroy(() => {
		carouselEl?.destroyAndRemoveInstance();
	});
</script>

<div {id} class="relative w-full {className}">
	<div class="relative h-56 overflow-hidden rounded-base md:h-96">
		{#each slides as slide, i}
			<div
				id={id + '-item-' + i}
				class="hidden duration-700 ease-in-out"
				data-carousel-item={slide.active ? 'active' : ''}
			>
				{@render slide.content()}
			</div>
		{/each}
	</div>
	{#if showIndicators}
		<div class="absolute z-30 flex -translate-x-1/2 bottom-5 left-1/2 space-x-3 rtl:space-x-reverse">
			{#each slides as _, i}
				<button
					id={id + '-indicator-' + i}
					type="button"
					class="w-3 h-3 rounded-base"
					aria-current={i === (slides.findIndex((s) => s.active) || 0) ? 'true' : 'false'}
					aria-label={'Slide ' + (i + 1)}
					onclick={() => carouselEl?.slideTo(i)}
				></button>
			{/each}
		</div>
	{/if}
	{#if showControls}
		<button
			type="button"
			class="absolute top-0 start-0 z-30 flex items-center justify-center h-full px-4 cursor-pointer group focus:outline-none"
			onclick={() => carouselEl?.prev()}
		>
			<span
				class="inline-flex items-center justify-center w-10 h-10 rounded-base bg-white/30 dark:bg-gray-800/30 group-hover:bg-white/50 group-focus:ring-4 group-focus:ring-white group-focus:outline-none"
			>
				<Icon name="tabler:chevron-left" class="w-5 h-5 text-white" />
				<span class="sr-only">Previous</span>
			</span>
		</button>
		<button
			type="button"
			class="absolute top-0 end-0 z-30 flex items-center justify-center h-full px-4 cursor-pointer group focus:outline-none"
			onclick={() => carouselEl?.next()}
		>
			<span
				class="inline-flex items-center justify-center w-10 h-10 rounded-base bg-white/30 dark:bg-gray-800/30 group-hover:bg-white/50 group-focus:ring-4 group-focus:ring-white group-focus:outline-none"
			>
				<Icon name="tabler:chevron-right" class="w-5 h-5 text-white" />
				<span class="sr-only">Next</span>
			</span>
		</button>
	{/if}
</div>
