<script lang="ts">
	interface Props {
		variant?: 'primary' | 'secondary' | 'danger' | 'success' | 'ghost';
		size?: 'sm' | 'md' | 'lg';
		disabled?: boolean;
		fullWidth?: boolean;
		type?: 'button' | 'submit' | 'reset';
		onclick?: () => void;
		children: any;
	}

	let {
		variant = 'primary',
		size = 'md',
		disabled = false,
		fullWidth = false,
		type = 'button',
		onclick,
		children
	}: Props = $props();

	const variantClasses = {
		primary: 'bg-indigo-600 text-white hover:bg-indigo-700 active:bg-indigo-800',
		secondary: 'bg-gray-200 text-gray-800 hover:bg-gray-300 active:bg-gray-400',
		danger: 'bg-red-600 text-white hover:bg-red-700 active:bg-red-800',
		success: 'bg-green-600 text-white hover:bg-green-700 active:bg-green-800',
		ghost: 'bg-transparent text-gray-700 hover:bg-gray-100 active:bg-gray-200'
	};

	const sizeClasses = {
		sm: 'px-3 py-1.5 text-sm',
		md: 'px-4 py-2 text-base',
		lg: 'px-6 py-3 text-lg'
	};
</script>

<button
	{type}
	{disabled}
	{onclick}
	class="
		rounded-lg font-medium transition-all duration-150
		focus:ring-2 focus:ring-indigo-500
		focus:ring-offset-2 focus:outline-none disabled:cursor-not-allowed disabled:opacity-50
		{variantClasses[variant]}
		{sizeClasses[size]}
		{fullWidth ? 'w-full' : ''}
	"
>
	{@render children()}
</button>
