<script lang="ts">
	import Button from '$lib/components/ui/Button.svelte';
	import { goto } from '$app/navigation';
</script>

<div class="flex min-h-screen flex-col items-center justify-center bg-gray-50 p-4">
	<div class="max-w-md space-y-6 text-center">
		<div
			class="mx-auto flex h-24 w-24 items-center justify-center rounded-full bg-red-100 text-5xl text-red-600"
		>
			⚠️
		</div>

		<h1 class="text-3xl font-bold text-gray-900">Có lỗi xảy ra</h1>

		<p class="text-gray-600">
			Rất tiếc, đã có lỗi xảy ra trong quá trình xử lý hoặc bạn không có quyền truy cập trang này.
		</p>

		<div class="pt-4">
			<Button variant="primary" onclick={async () => await goto('/')}>Về trang chủ</Button>
		</div>
	</div>
</div>
