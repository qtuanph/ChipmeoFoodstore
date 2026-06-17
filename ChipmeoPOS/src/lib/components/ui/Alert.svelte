<script lang="ts">
	import Icon from './Icon.svelte';

	interface Props {
		id?: string;
		type?: 'info' | 'success' | 'warning' | 'danger';
		dismissible?: boolean;
		onClose?: () => void;
		children: import('svelte').Snippet;
	}

	let {
		id = 'alert-' + crypto.randomUUID(),
		type = 'info',
		dismissible = false,
		onClose,
		children
	}: Props = $props();

	const styles: Record<string, string> = {
		info: 'text-fg-brand bg-brand-soft',
		success: 'text-fg-success bg-success-soft',
		warning: 'text-fg-warning bg-warning-soft',
		danger: 'text-fg-danger bg-danger-soft'
	};

	const closeStyles: Record<string, string> = {
		info: 'text-fg-brand hover:bg-brand-subtle focus:ring-brand-soft',
		success: 'text-fg-success hover:bg-success-subtle focus:ring-success-soft',
		warning: 'text-fg-warning hover:bg-warning-subtle focus:ring-warning-soft',
		danger: 'text-fg-danger hover:bg-danger-subtle focus:ring-danger-soft'
	};

	const iconName: Record<string, string> = {
		info: 'general/info-circle',
		success: 'general/check-circle',
		warning: 'general/exclamation-circle',
		danger: 'general/close-circle'
	};
</script>

<div {id} class="flex items-center p-4 mb-4 text-sm rounded-base {styles[type]}" role="alert">
	<Icon name={iconName[type]} class="flex-shrink-0 w-4 h-4 me-3" />
	<div class="ms-2">{@render children()}</div>
	{#if dismissible}
		<button
			type="button"
			aria-label="Close"
			onclick={onClose}
			data-dismiss-target={'#' + id}
			class="ms-auto -mx-1.5 -my-1.5 rounded-lg focus:ring-2 p-1.5 inline-flex items-center justify-center h-8 w-8 {closeStyles[
				type
			]}"
		>
			<Icon name="tabler:x" class="w-3 h-3" />
		</button>
	{/if}
</div>
