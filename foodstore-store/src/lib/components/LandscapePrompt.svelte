<script lang="ts">
	import { onMount } from 'svelte';
	import Icon from './ui/Icon.svelte';

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
			<Icon name="tabler:device-mobile" class="h-24 w-24 text-indigo-400" />
		</div>
		<h2 class="mb-4 text-2xl font-bold">Vui lòng xoay ngang thiết bị</h2>
		<p class="text-lg text-gray-300">
			Ứng dụng Foodstore POS được tối ưu hóa cho màn hình ngang để có trải nghiệm tốt nhất.
		</p>
		<div class="mt-8">
			<Icon
				name="tabler:refresh"
				class="animate-spin-slow h-16 w-16 text-gray-500"
				style="animation-duration: 3s;"
			/>
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
