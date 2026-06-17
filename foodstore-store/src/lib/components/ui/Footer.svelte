<script lang="ts">
	interface FooterLink {
		label: string;
		href: string;
	}

	interface Props {
		brand?: { img?: string; text?: string; url?: string };
		links?: FooterLink[];
		copyright?: string;
		class?: string;
	}

	let { brand = {}, links = [], copyright = '', class: className = '' }: Props = $props();
</script>

<footer
	class="bg-neutral-primary-soft rounded-base shadow-xs border border-default m-4 {className}"
>
	<div class="w-full mx-auto max-w-screen-xl p-4 md:flex md:items-center md:justify-between">
		<span class="text-sm text-body sm:text-center">{@html copyright}</span>
		<ul class="flex flex-wrap items-center mt-3 text-sm font-medium text-body sm:mt-0">
			{#each links as link}
				<li>
					<a href={link.href} class="hover:underline me-4 md:me-6">{link.label}</a>
				</li>
			{/each}
		</ul>
	</div>
</footer>
