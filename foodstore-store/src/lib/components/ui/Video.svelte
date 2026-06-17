<script lang="ts">
	interface Props {
		src: string;
		type?: 'html5' | 'youtube' | 'vimeo';
		poster?: string;
		aspectRatio?: string;
		class?: string;
	}

	let {
		src,
		type = 'html5',
		poster = '',
		aspectRatio = '16/9',
		class: className = ''
	}: Props = $props();
</script>

<div
	class="relative overflow-hidden rounded-base bg-neutral-primary-soft {className}"
	style="aspect-ratio: {aspectRatio}"
>
	{#if type === 'html5'}
		<video class="absolute inset-0 w-full h-full" controls {poster}>
			<source {src} />
		</video>
	{:else if type === 'youtube' || type === 'vimeo'}
		<iframe
			class="absolute inset-0 w-full h-full"
			{src}
			title={type === 'youtube' ? 'YouTube video' : 'Vimeo video'}
			frameborder="0"
			allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
			allowfullscreen
		></iframe>
	{/if}
</div>
