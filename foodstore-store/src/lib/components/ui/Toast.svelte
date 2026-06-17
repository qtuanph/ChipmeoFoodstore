<script lang="ts">
	import Icon from './Icon.svelte';

	interface Props {
		message: string;
		type?: 'success' | 'error' | 'warning' | 'info';
		show: boolean;
		onClose?: () => void;
		duration?: number;
	}

	let {
		message,
		type = 'info',
		show = $bindable(false),
		onClose,
		duration = 4000
	}: Props = $props();

	const id = 'toast-' + crypto.randomUUID();

	const colorClasses: Record<string, string> = {
		success: 'text-fg-success bg-success-soft',
		error: 'text-fg-danger bg-danger-soft',
		warning: 'text-fg-warning bg-warning-soft',
		info: 'text-fg-brand bg-brand-soft'
	};

	const iconName: Record<string, string> = {
		success: 'tabler:check',
		error: 'tabler:x',
		warning: 'tabler:alert-circle',
		info: 'tabler:info-circle'
	};

	// Auto-dismiss timer
	$effect(() => {
		if (show) {
			const timer = setTimeout(() => {
				show = false;
				onClose?.();
			}, duration);
			return () => clearTimeout(timer);
		}
	});

	function handleClose() {
		show = false;
		onClose?.();
	}
</script>

{#if show}
	<div
		{id}
		class="fixed top-4 right-4 z-50 flex items-center w-full max-w-xs p-4 text-body bg-neutral-primary-soft rounded-base shadow-xs border border-default"
		role="alert"
	>
		<div class="inline-flex items-center justify-center shrink-0 w-7 h-7 {colorClasses[type]} rounded">
			<Icon name={iconName[type]} class="w-5 h-5" />
			<span class="sr-only">{type} icon</span>
		</div>
		<div class="ms-3 text-sm font-normal text-heading">{message}</div>
		<button
			type="button"
			aria-label="Close"
			onclick={handleClose}
			class="ms-auto flex items-center justify-center text-body hover:text-heading bg-transparent box-border border border-transparent hover:bg-neutral-secondary-medium focus:ring-4 focus:ring-neutral-tertiary font-medium leading-5 rounded text-sm h-8 w-8 focus:outline-none"
		>
			<span class="sr-only">Close</span>
			<Icon name="tabler:x" class="w-5 h-5" />
		</button>
	</div>
{/if}
