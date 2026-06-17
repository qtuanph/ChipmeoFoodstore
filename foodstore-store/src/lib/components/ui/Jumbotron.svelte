<script lang="ts">
	interface Props {
		title?: string;
		description?: string;
		children?: any;
		class?: string;
	}

	let { title = '', description = '', children, class: className = '' }: Props = $props();
</script>

<section class="bg-neutral-primary-soft {className}">
	<div class="py-8 px-4 mx-auto max-w-screen-xl text-center lg:py-16">
		{#if title}
			<h1
				class="mb-4 text-4xl font-extrabold tracking-tight leading-none text-heading md:text-5xl lg:text-6xl"
			>
				{title}
			</h1>
		{/if}
		{#if description}
			<p class="mb-8 text-lg font-normal text-body lg:text-xl sm:px-16 lg:px-48">{description}</p>
		{/if}
		{#if children}
			<div class="flex flex-col space-y-4 sm:flex-row sm:justify-center sm:space-y-0 sm:space-x-4">
				{@render children()}
			</div>
		{/if}
	</div>
</section>
