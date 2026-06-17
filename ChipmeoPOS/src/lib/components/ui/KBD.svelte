<script lang="ts">
	interface Props {
		children: any;
		class?: string;
	}

	let { children, class: className = '' }: Props = $props();
</script>

<kbd
	class="px-2 py-1.5 text-xs font-semibold text-heading bg-neutral-secondary-soft border border-default rounded-base {className}"
>
	{@render children()}
</kbd>
