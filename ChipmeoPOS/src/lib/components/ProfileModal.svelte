<script lang="ts">
	import Modal from './Modal.svelte';
	import Button from './Button.svelte';
	import { auth } from '$lib/utils/index.js';
	import { onMount } from 'svelte';
	import Toast from './Toast.svelte';
	import ImageCropper from './ImageCropper.svelte';

	import { API_BASE_URL } from '$lib/config/index.js';
	import { api } from '$lib/api/utils.js';

	let { open = $bindable() } = $props();

	let loading = $state(false);
	let saving = $state(false);
	let showToast = $state(false);
	let toastMessage = $state('');
	let toastType = $state<'success' | 'error'>('success');

	let formData = $state({
		fullName: '',
		phone: '',
		email: '',
		avatarUrl: '', // Added avatarUrl
		currentPassword: '',
		newPassword: '',
		confirmPassword: ''
	});

	let fileInput = $state<HTMLInputElement | null>(null); // For file input reference
	let cropImageSrc = $state<string | null>(null); // For ImageCropper input

	// Load profile when modal opens
	$effect(() => {
		if (open) {
			loadProfile();
		}
	});

	async function loadProfile() {
		loading = true;
		try {
			const profile = await auth.getProfile();
			formData.fullName = profile.fullName || '';
			formData.phone = profile.phone || '';
			formData.email = profile.email || '';
			formData.avatarUrl = profile.avatarUrl || ''; // Load avatarUrl
			// Reset password fields
			formData.currentPassword = '';
			formData.newPassword = '';
			formData.confirmPassword = '';
		} catch (error) {
			console.error('Failed to load profile:', error);
			toastMessage = 'Không thể tải thông tin cá nhân';
			toastType = 'error';
			showToast = true;
		} finally {
			loading = false;
		}
	}

	function handleFileSelect(event: Event) {
		const input = event.target as HTMLInputElement;
		const file = input.files?.[0];
		if (file) {
			const reader = new FileReader();
			reader.onload = (e) => {
				if (e.target?.result) {
					cropImageSrc = e.target.result as string;
				}
			};
			reader.readAsDataURL(file);
		}
		input.value = '';
	}

	async function handleCrop(blob: Blob) {
		cropImageSrc = null; // Close cropper
		await uploadAvatar(blob);
	}

	function handleCancelCrop() {
		cropImageSrc = null;
	}

	async function uploadAvatar(fileOrBlob: File | Blob) {
		try {
			loading = true;
			const uploadParams = new FormData();
			uploadParams.append('file', fileOrBlob, 'avatar.jpg');
			uploadParams.append('folder', 'avatars');

			const result = await api.upload<{ fileUrl: string }>(
				`${API_BASE_URL}/api/media/upload`,
				uploadParams
			);
			formData.avatarUrl = result.fileUrl;
		} catch (err) {
			console.error(err);
			toastMessage = 'Lỗi khi tải ảnh lên';
			toastType = 'error';
			showToast = true;
		} finally {
			loading = false;
		}
	}

	async function handleSave() {
		if (formData.newPassword && formData.newPassword !== formData.confirmPassword) {
			toastMessage = 'Mật khẩu mới không khớp';
			toastType = 'error';
			showToast = true;
			return;
		}

		if (formData.newPassword && !formData.currentPassword) {
			toastMessage = 'Vui lòng nhập mật khẩu hiện tại để đổi mật khẩu';
			toastType = 'error';
			showToast = true;
			return;
		}

		saving = true;
		try {
			const updateData = {
				fullName: formData.fullName,
				phone: formData.phone,
				email: formData.email,
				avatarUrl: formData.avatarUrl, // Include avatarUrl
				currentPassword: formData.currentPassword || null,
				newPassword: formData.newPassword || null
			};

			const updatedUser = await auth.updateProfile(updateData);

			// Update local auth store
			auth.updateUser(updatedUser);

			toastMessage = 'Cập nhật thông tin thành công';
			toastType = 'success';
			showToast = true;

			// Close modal after short delay
			setTimeout(() => {
				open = false;
			}, 1500);
		} catch (error: any) {
			console.error('Update profile error:', error);
			toastMessage = error.error || 'Cập nhật thất bại';
			toastType = 'error';
			showToast = true;
		} finally {
			saving = false;
		}
	}
</script>

<Modal bind:open title="Thông tin cá nhân" size="lg" onClose={() => (open = false)}>
	{#if loading}
		<div class="flex justify-center p-8">
			<div class="h-8 w-8 animate-spin rounded-full border-b-2 border-indigo-600"></div>
		</div>
	{:else}
		<div class="space-y-6">
			<!-- Basic Info -->

			<!-- Avatar Section -->
			<div class="mb-6 flex justify-center">
				<div class="group relative">
					<div
						class="flex h-24 w-24 items-center justify-center overflow-hidden rounded-full border-4 border-gray-100 bg-gray-50 shadow-sm"
					>
						{#if formData.avatarUrl}
							<img src={formData.avatarUrl} alt="Avatar" class="h-full w-full object-cover" />
						{:else}
							<span class="text-3xl font-bold text-gray-300">
								{formData.fullName ? formData.fullName.charAt(0).toUpperCase() : 'U'}
							</span>
						{/if}
					</div>
					<button
						onclick={() => fileInput?.click()}
						class="absolute right-0 bottom-0 rounded-full border border-gray-200 bg-white p-2 shadow-md transition-colors hover:bg-gray-50"
						title="Đổi ảnh đại diện"
					>
						<svg
							class="h-4 w-4 text-gray-600"
							fill="none"
							stroke="currentColor"
							viewBox="0 0 24 24"
						>
							<path
								stroke-linecap="round"
								stroke-linejoin="round"
								stroke-width="2"
								d="M3 9a2 2 0 012-2h.93a2 2 0 001.664-.89l.812-1.22A2 2 0 0110.07 4h3.86a2 2 0 011.664.89l.812 1.22A2 2 0 0018.07 7H19a2 2 0 012 2v9a2 2 0 01-2 2H5a2 2 0 01-2-2V9z"
							/>
							<path
								stroke-linecap="round"
								stroke-linejoin="round"
								stroke-width="2"
								d="M15 13a3 3 0 11-6 0 3 3 0 016 0z"
							/>
						</svg>
					</button>
					<input
						type="file"
						accept="image/*"
						class="hidden"
						bind:this={fileInput}
						onchange={handleFileSelect}
					/>
				</div>
			</div>

			{#if cropImageSrc}
				<ImageCropper
					imageSrc={cropImageSrc}
					aspectRatio={1}
					onCrop={handleCrop}
					onCancel={handleCancelCrop}
				/>
			{/if}

			<div class="grid grid-cols-1 gap-4 md:grid-cols-2">
				<div>
					<label for="fullName" class="mb-1 block text-sm font-medium text-gray-700"
						>Họ và tên</label
					>
					<input
						type="text"
						id="fullName"
						bind:value={formData.fullName}
						class="w-full rounded-lg border border-gray-300 px-4 py-2 focus:border-indigo-500 focus:ring-2 focus:ring-indigo-500"
					/>
				</div>
				<div>
					<label for="phone" class="mb-1 block text-sm font-medium text-gray-700"
						>Số điện thoại</label
					>
					<input
						type="tel"
						id="phone"
						bind:value={formData.phone}
						class="w-full rounded-lg border border-gray-300 px-4 py-2 focus:border-indigo-500 focus:ring-2 focus:ring-indigo-500"
					/>
				</div>
				<div class="md:col-span-2">
					<label for="email" class="mb-1 block text-sm font-medium text-gray-700">Email</label>
					<input
						type="email"
						id="email"
						bind:value={formData.email}
						class="w-full rounded-lg border border-gray-300 px-4 py-2 focus:border-indigo-500 focus:ring-2 focus:ring-indigo-500"
					/>
				</div>
			</div>

			<div class="border-t border-gray-200 pt-4">
				<h3 class="mb-4 text-sm font-medium text-gray-900">
					Đổi mật khẩu (Để trống nếu không đổi)
				</h3>
				<div class="space-y-4">
					<div>
						<label for="currentPassword" class="mb-1 block text-sm font-medium text-gray-700"
							>Mật khẩu hiện tại</label
						>
						<input
							type="password"
							id="currentPassword"
							bind:value={formData.currentPassword}
							class="w-full rounded-lg border border-gray-300 px-4 py-2 focus:border-indigo-500 focus:ring-2 focus:ring-indigo-500"
							placeholder="Nhập mật khẩu hiện tại để xác nhận thay đổi"
						/>
					</div>
					<div class="grid grid-cols-1 gap-4 md:grid-cols-2">
						<div>
							<label for="newPassword" class="mb-1 block text-sm font-medium text-gray-700"
								>Mật khẩu mới</label
							>
							<input
								type="password"
								id="newPassword"
								bind:value={formData.newPassword}
								class="w-full rounded-lg border border-gray-300 px-4 py-2 focus:border-indigo-500 focus:ring-2 focus:ring-indigo-500"
							/>
						</div>
						<div>
							<label for="confirmPassword" class="mb-1 block text-sm font-medium text-gray-700"
								>Xác nhận mật khẩu mới</label
							>
							<input
								type="password"
								id="confirmPassword"
								bind:value={formData.confirmPassword}
								class="w-full rounded-lg border border-gray-300 px-4 py-2 focus:border-indigo-500 focus:ring-2 focus:ring-indigo-500"
							/>
						</div>
					</div>
				</div>
			</div>

			<div class="flex justify-end gap-3 border-t border-gray-200 pt-4">
				<Button variant="secondary" onclick={() => (open = false)} disabled={saving}>Hủy</Button>
				<Button variant="primary" onclick={handleSave} disabled={saving}>
					{saving ? 'Đang lưu...' : 'Lưu thay đổi'}
				</Button>
			</div>
		</div>
	{/if}
</Modal>

<Toast bind:show={showToast} message={toastMessage} type={toastType} />
