<script lang="ts">
	import { Modal } from 'flowbite';
	import type { ModalOptions, ModalInterface } from 'flowbite';
	import { onMount, onDestroy, tick, untrack } from 'svelte';
	import Icon from './Icon.svelte';

	interface Props {
		id?: string;
		open?: boolean;
		title?: string;
		size?: 'sm' | 'md' | 'lg' | 'xl' | 'full';
		closable?: boolean;
		static?: boolean;
		closeOnOutsideClick?: boolean;
		onClose?: () => void;
		children: any;
		footer?: any;
	}

	let {
		id: propId,
		open = $bindable(false),
		title = '',
		size = 'md',
		closable = true,
		static: isStatic = false,
		closeOnOutsideClick = true,
		onClose,
		children,
		footer
	}: Props = $props();

	const sizeClasses = {
		sm: 'max-w-md',
		md: 'max-w-lg',
		lg: 'max-w-2xl',
		xl: 'max-w-4xl',
		full: 'max-w-full mx-4'
	};

	// Capture propId once via untrack so the ID stays stable and doesn't re-derive
	const id: string = untrack(() => propId ?? crypto.randomUUID());

	let modalEl = $state<ModalInterface | null>(null);

	onMount(async () => {
		// tick() ensures the div with `id` is in the DOM after Svelte hydration
		await tick();

		const el = document.getElementById(id);
		if (!el) return;

		// 'dynamic' = click outside closes; 'static' = click outside does NOT close
		const backdropMode: 'dynamic' | 'static' = isStatic || !closeOnOutsideClick ? 'static' : 'dynamic';
		const options: ModalOptions = {
			backdrop: backdropMode,
			closable,
			onHide: () => {
				open = false;
				onClose?.();
			},
			onShow: () => {
				open = true;
			}
		};

		modalEl = new Modal(el, options);

		// Sync immediately after init in case open=true was set before onMount ran
		if (open) {
			modalEl.show();
		}
	});

	onDestroy(() => {
		// Clean up Flowbite instance to prevent memory leaks on SPA navigation
		modalEl?.destroyAndRemoveInstance();
	});

	$effect(() => {
		if (!modalEl) return;
		if (open) modalEl.show();
		else modalEl.hide();
	});

	function handleClose() {
		modalEl?.hide();
	}
</script>

<div
	{id}
	tabindex="-1"
	aria-hidden="true"
	class="hidden overflow-y-auto overflow-x-hidden fixed top-0 right-0 left-0 z-50 justify-center items-center w-full md:inset-0 h-[calc(100%-1rem)] max-h-full"
>
	<div class="relative p-4 w-full {sizeClasses[size]} max-h-full">
		<div
			class="relative bg-neutral-primary-soft border border-default rounded-base shadow-sm p-4 md:p-6"
		>
			{#if title || closable}
				<div class="flex items-center justify-between border-b border-default pb-4 md:pb-5">
					{#if title}
						<h3 class="text-lg font-medium text-heading">{title}</h3>
					{/if}
					{#if closable}
						<button
							type="button"
							class="text-body bg-transparent hover:bg-neutral-tertiary hover:text-heading rounded-base text-sm w-9 h-9 ms-auto inline-flex justify-center items-center"
							onclick={handleClose}
						>
							<Icon name="tabler:x" class="w-5 h-5" />
							<span class="sr-only">Close modal</span>
						</button>
					{/if}
				</div>
			{/if}
			<div class="space-y-4 md:space-y-6 py-4 md:py-6">
				{@render children()}
			</div>
			{#if footer}
				<div class="flex items-center border-t border-default space-x-4 pt-4 md:pt-5">
					{@render footer()}
				</div>
			{/if}
		</div>
	</div>
</div>
