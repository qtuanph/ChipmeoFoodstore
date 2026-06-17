<script lang="ts">
	interface GalleryImage {
		src: string;
		alt?: string;
	}

	interface Props {
		images?: GalleryImage[];
		columns?: 2 | 3 | 4;
		class?: string;
	}

	let { images = [], columns = 3, class: className = '' }: Props = $props();

	const gridCols: Record<number, string> = {
		2: 'grid-cols-2 md:grid-cols-2',
		3: 'grid-cols-2 md:grid-cols-3',
		4: 'grid-cols-2 md:grid-cols-4'
	};
</script>

<div class="grid {gridCols[columns]} gap-4 {className}">
	{#each images as img}
		<div>
			<img class="h-auto max-w-full rounded-base" src={img.src} alt={img.alt || ''} />
		</div>
	{/each}
</div>
