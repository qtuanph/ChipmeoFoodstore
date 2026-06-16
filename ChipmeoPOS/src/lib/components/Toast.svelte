<script lang="ts">
	interface Props {
		message: string;
		type?: 'success' | 'error' | 'info' | 'warning';
		show: boolean;
		onClose?: () => void;
	}

	let { message, type = 'info', show = $bindable(), onClose }: Props = $props();

	const icons = {
		success: '✓',
		error: '✕',
		info: 'ℹ',
		warning: '⚠'
	};

	const colors = {
		success: 'bg-green-500',
		error: 'bg-red-500',
		info: 'bg-blue-500',
		warning: 'bg-yellow-500'
	};

	$effect(() => {
		if (show) {
			const timer = setTimeout(() => {
				show = false;
				onClose?.();
			}, 3000);
			return () => clearTimeout(timer);
		}
	});
</script>

{#if show}
	<div class="animate-slide-in fixed top-4 right-4 z-50">
		<div
			class="flex items-center gap-3 {colors[
				type
			]} min-w-[300px] rounded-lg px-6 py-4 text-white shadow-lg"
		>
			<span class="text-2xl">{icons[type]}</span>
			<span class="flex-1 font-medium">{message}</span>
			<button
				onclick={() => {
					show = false;
					onClose?.();
				}}
				class="text-white/80 transition-colors hover:text-white"
				aria-label="Close"
			>
				<svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
					<path
						stroke-linecap="round"
						stroke-linejoin="round"
						stroke-width="2"
						d="M6 18L18 6M6 6l12 12"
					/>
				</svg>
			</button>
		</div>
	</div>
{/if}

<style>
	@keyframes slide-in {
		from {
			transform: translateX(100%);
			opacity: 0;
		}
		to {
			transform: translateX(0);
			opacity: 1;
		}
	}

	.animate-slide-in {
		animation: slide-in 0.3s ease-out;
	}
</style>
