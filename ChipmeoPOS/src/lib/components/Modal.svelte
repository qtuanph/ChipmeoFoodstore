<script lang="ts">
	interface Props {
		open: boolean;
		onClose: () => void;
		title?: string;
		size?: 'sm' | 'md' | 'lg' | 'xl';
		children: any;
		closeOnOutsideClick?: boolean;
	}

	let {
		open = $bindable(),
		onClose,
		title,
		size = 'md',
		children,
		closeOnOutsideClick = true
	}: Props = $props();

	const sizeClasses = {
		sm: 'max-w-md',
		md: 'max-w-lg',
		lg: 'max-w-2xl',
		xl: 'max-w-4xl'
	};

	function handleBackdropClick(e: MouseEvent) {
		if (e.target === e.currentTarget && closeOnOutsideClick) {
			onClose();
		}
	}
</script>

{#if open}
	<div
		class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4 backdrop-blur-sm"
		onclick={handleBackdropClick}
		role="dialog"
		aria-modal="true"
		tabindex="-1"
		onkeydown={(e) => e.key === 'Escape' && onClose()}
	>
		<div
			class="w-full rounded-2xl bg-white shadow-2xl {sizeClasses[size]} max-h-[90vh] overflow-auto"
		>
			{#if title}
				<div class="flex items-center justify-between border-b border-gray-200 p-6">
					<h2 class="text-xl font-semibold text-gray-900">{title}</h2>
					<button
						onclick={onClose}
						class="rounded-lg p-1 text-gray-400 transition-colors hover:bg-gray-100 hover:text-gray-600"
						aria-label="Close modal"
					>
						<svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
							<path
								stroke-linecap="round"
								stroke-linejoin="round"
								stroke-width="2"
								d="M6 18L18 6M6 6l12 12"
							/>
						</svg>
					</button>
				</div>
			{/if}
			<div class="p-6">
				{@render children()}
			</div>
		</div>
	</div>
{/if}
