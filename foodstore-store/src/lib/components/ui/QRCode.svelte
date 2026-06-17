<script lang="ts">
	interface Props {
		src: string;
		size?: number;
		alt?: string;
		showCard?: boolean;
		title?: string;
		description?: string;
		class?: string;
	}

	let {
		src,
		size = 128,
		alt = 'QR Code',
		showCard = false,
		title = '',
		description = '',
		class: className = ''
	}: Props = $props();
</script>

{#if showCard}
	<div
		class="bg-neutral-primary-soft block max-w-sm p-6 border border-default rounded-base shadow-xs {className}"
	>
		<div class="rounded-base bg-brand flex justify-center items-center p-4">
			<img {src} width={size} height={size} {alt} class="w-56 h-56 [&>svg]:!text-white" />
		</div>
		{#if title}
			<h5 class="mt-6 mb-2 text-2xl font-semibold tracking-tight text-heading">{title}</h5>
		{/if}
		{#if description}
			<p class="mb-6 text-body">{description}</p>
		{/if}
	</div>
{:else}
	<div class="inline-flex {className}">
		<img {src} width={size} height={size} {alt} class="rounded-base" />
	</div>
{/if}
