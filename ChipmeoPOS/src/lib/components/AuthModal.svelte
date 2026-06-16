<script lang="ts">
	import Modal from './Modal.svelte';
	import Button from './Button.svelte';
	import { auth } from '$lib/utils/index.js';
	import { goto } from '$app/navigation';

	let { open = $bindable(), onLoginSuccess } = $props();

	let isLogin = $state(true);
	let username = $state('');
	let password = $state('');
	let loading = $state(false);
	let errorMessage = $state('');

	function switchMode() {
		isLogin = !isLogin;
		errorMessage = '';
		username = '';
		password = '';
	}

	async function handleSubmit() {
		errorMessage = '';

		if (!isLogin) {
			errorMessage = 'Tính năng Đăng ký đang được phát triển.';
			return;
		}

		if (!username || !password) {
			errorMessage = 'Vui lòng nhập đầy đủ thông tin.';
			return;
		}

		loading = true;
		try {
			const result = await auth.loginAPI(username, password);
			if (result.success) {
				open = false;
				if (onLoginSuccess) onLoginSuccess();
			} else {
				errorMessage = result.error || 'Đăng nhập thất bại.';
			}
		} catch (err) {
			console.error(err);
			errorMessage = 'Đã có lỗi xảy ra.';
		} finally {
			loading = false;
		}
	}

	function handleKeydown(event: KeyboardEvent) {
		if (event.key === 'Enter') {
			handleSubmit();
		}
	}
</script>

<Modal bind:open title={isLogin ? 'Đăng nhập' : 'Đăng ký'} size="sm" onClose={() => (open = false)}>
	<div class="space-y-4">
		{#if errorMessage}
			<div class="rounded-lg bg-red-50 p-3 text-sm text-red-600">
				{errorMessage}
			</div>
		{/if}

		<div>
			<label for="auth-username" class="mb-1 block text-sm font-medium text-gray-700"
				>Tên đăng nhập</label
			>
			<input
				type="text"
				id="auth-username"
				bind:value={username}
				onkeydown={handleKeydown}
				class="w-full rounded-lg border border-gray-300 px-4 py-2 focus:border-indigo-500 focus:ring-2 focus:ring-indigo-500"
				placeholder="Nhập tên đăng nhập"
				disabled={loading}
			/>
		</div>

		<div>
			<label for="auth-password" class="mb-1 block text-sm font-medium text-gray-700"
				>Mật khẩu</label
			>
			<input
				type="password"
				id="auth-password"
				bind:value={password}
				onkeydown={handleKeydown}
				class="w-full rounded-lg border border-gray-300 px-4 py-2 focus:border-indigo-500 focus:ring-2 focus:ring-indigo-500"
				placeholder="Nhập mật khẩu"
				disabled={loading}
			/>
		</div>

		<div class="pt-2">
			<Button variant="primary" fullWidth onclick={handleSubmit} disabled={loading}>
				{loading ? 'Đang xử lý...' : isLogin ? 'Đăng nhập' : 'Đăng ký'}
			</Button>
		</div>

		<!-- Registration disabled -->
		<!-- <div class="text-center text-sm text-gray-600 mt-4">
            {isLogin ? 'Chưa có tài khoản?' : 'Đã có tài khoản?'}
            <button 
                class="text-indigo-600 font-medium hover:underline ml-1"
                onclick={switchMode}
            >
                {isLogin ? 'Đăng ký ngay' : 'Đăng nhập ngay'}
            </button>
        </div> -->
	</div>
</Modal>
