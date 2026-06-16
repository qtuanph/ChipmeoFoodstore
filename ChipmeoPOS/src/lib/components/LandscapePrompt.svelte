<script lang="ts">
	import { onMount } from 'svelte';

	let isPortrait = false;
	let isMobile = false;

	function checkOrientation() {
		if (typeof window === 'undefined') return;

		// Check if device is mobile (width < 768px)
		isMobile = window.innerWidth < 768;

		// Check if orientation is portrait
		// We use window.innerHeight > window.innerWidth as a reliable check for visual orientation
		isPortrait = window.innerHeight > window.innerWidth;
	}

	onMount(() => {
		checkOrientation();
		window.addEventListener('resize', checkOrientation);
		window.addEventListener('orientationchange', checkOrientation);

		return () => {
			window.removeEventListener('resize', checkOrientation);
			window.removeEventListener('orientationchange', checkOrientation);
		};
	});
</script>

{#if isMobile && isPortrait}
	<div
		class="fixed inset-0 z-[9999] flex flex-col items-center justify-center bg-gray-900 p-8 text-center text-white"
	>
		<div class="mb-8 animate-bounce">
			<svg class="h-24 w-24 text-indigo-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
				<path
					stroke-linecap="round"
					stroke-linejoin="round"
					stroke-width="2"
					d="M12 18h.01M8 21h8a2 2 0 002-2V5a2 2 0 00-2-2H8a2 2 0 00-2 2v14a2 2 0 002 2z"
				/>
			</svg>
		</div>
		<h2 class="mb-4 text-2xl font-bold">Vui lòng xoay ngang thiết bị</h2>
		<p class="text-lg text-gray-300">
			Ứng dụng Chipmeo POS được tối ưu hóa cho màn hình ngang để có trải nghiệm tốt nhất.
		</p>
		<div class="mt-8">
			<svg
				class="animate-spin-slow h-16 w-16 text-gray-500"
				fill="none"
				stroke="currentColor"
				viewBox="0 0 24 24"
				style="animation-duration: 3s;"
			>
				<path
					stroke-linecap="round"
					stroke-linejoin="round"
					stroke-width="2"
					d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
				/>
			</svg>
		</div>
	</div>
{/if}

<style>
	.animate-spin-slow {
		animation: spin 3s linear infinite;
	}

	@keyframes spin {
		from {
			transform: rotate(0deg);
		}
		to {
			transform: rotate(90deg);
		}
	}
</style>
