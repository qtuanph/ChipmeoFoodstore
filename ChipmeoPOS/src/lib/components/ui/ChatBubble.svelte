<script lang="ts">
	import { onMount } from 'svelte';
	import { initFlowbite } from 'flowbite';
	import Icon from './Icon.svelte';

	interface Props {
		avatar?: string;
		name?: string;
		time?: string;
		message?: string;
		status?: string;
		outgoing?: boolean;
		children?: any;
		actions?: { label: string; onclick?: () => void }[];
	}

	let {
		avatar = '',
		name = '',
		time = '',
		message = '',
		status = '',
		outgoing = false,
		children,
		actions = []
	}: Props = $props();

	const id = 'chat-' + crypto.randomUUID();

	onMount(() => {
		initFlowbite();
	});
</script>

<div class="flex items-start gap-2.5 {outgoing ? 'flex-row-reverse' : ''}">
	{#if avatar}
		<img class="w-8 h-8 rounded-full" src={avatar} alt={name} />
	{/if}
	<div
		class="flex flex-col w-full max-w-[320px] leading-1.5 p-4 bg-neutral-secondary-soft rounded-e-base {outgoing
			? 'rounded-se-base'
			: 'rounded-es-base'}"
	>
		<div class="flex items-center space-x-1.5 rtl:space-x-reverse">
			<span class="text-sm font-semibold text-heading">{name}</span>
			<span class="text-sm text-body">{time}</span>
		</div>
		{#if children}
			{@render children()}
		{:else if message}
			<p class="text-sm py-2.5 text-body">{message}</p>
		{/if}
		{#if status}
			<span class="text-sm text-body">{status}</span>
		{/if}
	</div>
	{#if actions.length > 0}
		<button
			id={id + '-button'}
			data-dropdown-toggle={id + '-menu'}
			data-dropdown-placement="bottom-start"
			aria-label="Actions"
			class="inline-flex self-center items-center text-body hover:text-heading bg-neutral-primary box-border border border-transparent hover:bg-neutral-tertiary focus:ring-4 focus:ring-neutral-tertiary rounded-base p-1.5 focus:outline-none"
			type="button"
		>
			<Icon name="tabler:dots" class="w-6 h-6" />
		</button>
		<div
			id={id + '-menu'}
			class="z-10 bg-neutral-primary-medium border border-default-medium rounded-base shadow-lg w-40 block hidden"
		>
			<ul class="p-2 text-sm text-body font-medium" aria-labelledby={id + '-button'}>
				{#each actions as action}
					<li>
						<button
							type="button"
							onclick={action.onclick}
							class="block w-full p-2 hover:bg-neutral-tertiary-medium hover:text-heading rounded-md text-left"
							>{action.label}</button
						>
					</li>
				{/each}
			</ul>
		</div>
	{/if}
</div>
