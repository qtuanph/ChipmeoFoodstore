<script lang="ts">
	interface Props {
		variant?: 'brand' | 'success' | 'warning' | 'danger' | 'neutral';
		size?: 'sm' | 'md';
		children: any;
	}

	let { variant = 'brand', size = 'sm', children }: Props = $props();

	const variantClasses = {
		brand: 'bg-brand-softer border border-brand-subtle text-fg-brand-strong',
		success: 'bg-green-100 border border-green-200 text-green-800',
		warning: 'bg-yellow-100 border border-yellow-200 text-yellow-800',
		danger: 'bg-red-100 border border-red-200 text-red-800',
		neutral: 'bg-neutral-tertiary-medium border border-default-medium text-body'
	};

	const sizeClasses = {
		sm: 'px-2.5 py-0.5 text-xs',
		md: 'px-3 py-1 text-sm'
	};
</script>

<span
	class="inline-flex items-center font-medium rounded {variantClasses[variant]} {sizeClasses[size]}"
>
	{@render children()}
</span>
