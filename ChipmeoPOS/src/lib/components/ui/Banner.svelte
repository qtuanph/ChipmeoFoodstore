<script lang="ts">
	import { onMount } from 'svelte';
	import { initFlowbite } from 'flowbite';
	import Icon from './Icon.svelte';

	interface Props {
		id?: string;
		type?: 'info' | 'warning' | 'danger' | 'success';
		children: import('svelte').Snippet;
		dismissible?: boolean;
	}

	let {
		id = 'banner-' + crypto.randomUUID(),
		type = 'info',
		children,
		dismissible = false
	}: Props = $props();

	const bgClasses: Record<string, string> = {
		info: 'bg-brand-soft border-brand-subtle text-fg-brand',
		warning: 'bg-warning-soft border-warning-subtle text-fg-warning',
		danger: 'bg-danger-soft border-danger-subtle text-fg-danger',
		success: 'bg-success-soft border-success-subtle text-fg-success'
	};

	onMount(() => {
		initFlowbite();
	});
</script>

<div
	{id}
	class="flex items-center justify-between w-full px-4 py-3 border-b border-x-0 border-t-0 rounded-t-base {bgClasses[
		type
	]}"
	role="banner"
>
	<div class="flex items-center text-sm font-normal">{@render children()}</div>
	{#if dismissible}
		<button
			type="button"
			data-dismiss-target="#{id}"
			class="text-body bg-transparent hover:bg-neutral-tertiary hover:text-heading rounded-base text-sm w-8 h-8 ms-auto inline-flex justify-center items-center"
		>
			<Icon name="tabler:x" class="w-4 h-4" />
			<span class="sr-only">Close banner</span>
		</button>
	{/if}
</div>
