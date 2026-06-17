<script lang="ts">
	interface Props {
		variant?: 'primary' | 'secondary' | 'danger' | 'ghost';
		size?: 'sm' | 'md' | 'lg';
		disabled?: boolean;
		fullWidth?: boolean;
		type?: 'button' | 'submit' | 'reset';
		href?: string;
		onclick?: () => void;
		children: any;
	}

	let {
		variant = 'primary',
		size = 'md',
		disabled = false,
		fullWidth = false,
		type = 'button',
		href,
		onclick,
		children
	}: Props = $props();

	const variantClasses = {
		primary:
			'text-white bg-brand hover:bg-brand-strong focus:ring-4 focus:ring-brand-medium box-border border border-transparent',
		secondary:
			'text-body bg-neutral-secondary-medium hover:bg-neutral-tertiary-medium hover:text-heading focus:ring-4 focus:ring-neutral-tertiary border border-default-medium',
		danger:
			'text-fg-danger bg-neutral-secondary-medium hover:bg-danger-soft border border-danger-subtle',
		ghost:
			'text-heading bg-transparent hover:bg-neutral-secondary-medium focus:ring-4 focus:ring-neutral-tertiary border border-transparent'
	};

	const sizeClasses = {
		sm: 'px-3 py-2 text-sm',
		md: 'px-4 py-2.5 text-sm',
		lg: 'px-5 py-3 text-base'
	};
</script>

{#if href}
	<a
		{href}
		class="box-border inline-flex items-center justify-center gap-2 font-medium leading-5 shadow-xs rounded-base transition-all focus:outline-none disabled:pointer-events-none disabled:opacity-50 {variantClasses[
			variant
		]} {sizeClasses[size]} {fullWidth ? 'w-full' : ''}"
	>
		{@render children()}
	</a>
{:else}
	<button
		{type}
		{disabled}
		{onclick}
		class="box-border inline-flex items-center justify-center gap-2 font-medium leading-5 shadow-xs rounded-base transition-all focus:outline-none disabled:pointer-events-none disabled:opacity-50 {variantClasses[
			variant
		]} {sizeClasses[size]} {fullWidth ? 'w-full' : ''}"
	>
		{@render children()}
	</button>
{/if}
