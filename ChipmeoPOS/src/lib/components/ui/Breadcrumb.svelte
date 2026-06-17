<script lang="ts">
	import Icon from './Icon.svelte';

	interface Crumb {
		label: string;
		href?: string;
	}

	interface Props {
		items: Crumb[];
		class?: string;
	}

	let { items, class: className = '' }: Props = $props();
</script>

<nav aria-label="Breadcrumb" class={className}>
	<ol class="inline-flex items-center space-x-1 md:space-x-2 rtl:space-x-reverse">
		{#each items as item, i}
			<li class={i === 0 ? 'inline-flex items-center' : ''}>
				{#if i > 0}
					<Icon name="tabler:chevron-right" class="rtl:rotate-180 w-3 h-3 text-body mx-1" />
				{/if}
				{#if item.href && i < items.length - 1}
					<a
						href={item.href}
						class="inline-flex items-center text-sm text-body hover:text-heading {i === 0
							? 'ms-1 md:ms-2'
							: ''}"
					>
						{item.label}
					</a>
				{:else}
					<span
						class="inline-flex items-center text-sm {i === items.length - 1
							? 'text-heading'
							: 'text-body'} {i === 0 ? 'ms-1 md:ms-2' : ''}"
					>
						{item.label}
					</span>
				{/if}
			</li>
		{/each}
	</ol>
</nav>
