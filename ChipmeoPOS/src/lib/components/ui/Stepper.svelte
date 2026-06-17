<script lang="ts">
	import Icon from './Icon.svelte';

	interface Step {
		label: string;
		description?: string;
	}

	interface Props {
		steps?: Step[];
		current?: number;
		vertical?: boolean;
		class?: string;
	}

	let { steps = [], current = 0, vertical = false, class: className = '' }: Props = $props();
</script>

{#if vertical}
	<ol
		class="items-center w-full space-y-4 sm:flex sm:space-x-8 sm:space-y-0 rtl:space-x-reverse {className}"
	>
		{#each steps as step, i}
			<li
				class="flex items-center {i <= current
					? 'text-fg-brand'
					: 'text-body'} space-x-3 rtl:space-x-reverse"
			>
				<span
					class="flex items-center justify-center w-10 h-10 {i <= current
						? 'bg-brand-softer'
						: 'bg-neutral-tertiary'} rounded-full lg:h-12 lg:w-12 shrink-0"
				>
					{#if i < current}
						<Icon name="tabler:check" class="w-5 h-5 text-fg-brand" />
					{:else}
						<Icon name="tabler:circle" class="w-5 h-5 text-body" />
					{/if}
				</span>
				<div>
					<h3 class="font-medium leading-tight">{step.label}</h3>
					{#if step.description}
						<p class="text-sm">{step.description}</p>
					{/if}
				</div>
			</li>
		{/each}
	</ol>
{:else}
	<ol
		class="flex items-center w-full text-sm font-medium text-center text-body sm:text-base {className}"
	>
		{#each steps as step, i}
			<li
				class="flex md:w-full items-center {i <= current ? 'text-fg-brand' : ''} {i <
				steps.length - 1
					? "sm:after:content-[''] after:w-full after:h-1 after:border-b after:border-default after:border-px after:hidden sm:after:inline-block after:mx-6 xl:after:mx-10"
					: ''}"
			>
				<span
					class="flex items-center {i < steps.length - 1
						? "after:content-['/'] sm:after:hidden after:mx-2 after:text-fg-disabled"
						: ''}"
				>
					{#if i < current}
						<Icon name="tabler:check" class="w-5 h-5 me-1.5" />
					{/if}
					{step.label}
				</span>
			</li>
		{/each}
	</ol>
{/if}
