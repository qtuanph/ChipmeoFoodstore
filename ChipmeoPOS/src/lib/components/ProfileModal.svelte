<script lang="ts">
	import { auth } from '$lib/utils/index.js';
	import Modal from './ui/Modal.svelte';
	import Button from './ui/Button.svelte';
	import Toast from './ui/Toast.svelte';
	import ImageCropper from './ImageCropper.svelte';
	import Icon from './ui/Icon.svelte';

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
		avatarUrl: '',
		currentPassword: '',
		newPassword: '',
		confirmPassword: ''
	});

	let fileInput = $state<HTMLInputElement | null>(null);
	let cropImageSrc = $state<string | null>(null);

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
			formData.avatarUrl = profile.avatarUrl || '';
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
		cropImageSrc = null;
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
				avatarUrl: formData.avatarUrl,
				currentPassword: formData.currentPassword || null,
				newPassword: formData.newPassword || null
			};

			const updatedUser = await auth.updateProfile(updateData);
			auth.updateUser(updatedUser);

			toastMessage = 'Cập nhật thông tin thành công';
			toastType = 'success';
			showToast = true;

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

<Modal bind:open title="Thông tin cá nhân" onClose={() => (open = false)}>
	{#if loading}
		<div class="flex justify-center p-8">
			<div
				class="h-8 w-8 animate-spin rounded-full border-4 border-neutral-tertiary border-t-brand"
			></div>
		</div>
	{:else}
		<div class="space-y-6">
			<div class="mb-6 flex justify-center">
				<div class="group relative">
					<div
						class="flex h-24 w-24 items-center justify-center overflow-hidden rounded-full border-4 border-default bg-neutral-secondary-medium"
					>
						{#if formData.avatarUrl}
							<img src={formData.avatarUrl} alt="Avatar" class="h-full w-full object-cover" />
						{:else}
							<span class="text-3xl font-bold text-body">
								{formData.fullName ? formData.fullName.charAt(0).toUpperCase() : 'U'}
							</span>
						{/if}
					</div>
					<button
						type="button"
						onclick={() => fileInput?.click()}
						class="absolute bottom-0 right-0 rounded-full border border-default bg-neutral-primary p-2 shadow-md transition-colors hover:bg-neutral-secondary-soft"
						title="Đổi ảnh đại diện"
					>
						<Icon name="tabler:edit" class="h-4 w-4 text-body" />
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
					<label for="fullName" class="mb-2.5 block text-sm font-medium text-heading"
						>Họ và tên</label
					>
					<input
						type="text"
						id="fullName"
						bind:value={formData.fullName}
						class="block w-full rounded-base border border-default-medium bg-neutral-secondary-medium px-3 py-2.5 text-sm text-heading shadow-xs placeholder:text-body focus:border-brand focus:ring-brand"
						placeholder="Nhập họ và tên"
					/>
				</div>
				<div>
					<label for="phone" class="mb-2.5 block text-sm font-medium text-heading"
						>Số điện thoại</label
					>
					<input
						type="tel"
						id="phone"
						bind:value={formData.phone}
						class="block w-full rounded-base border border-default-medium bg-neutral-secondary-medium px-3 py-2.5 text-sm text-heading shadow-xs placeholder:text-body focus:border-brand focus:ring-brand"
						placeholder="Nhập số điện thoại"
					/>
				</div>
			</div>

			<div>
				<label for="email" class="mb-2.5 block text-sm font-medium text-heading">Email</label>
				<input
					type="email"
					id="email"
					bind:value={formData.email}
					class="block w-full rounded-base border border-default-medium bg-neutral-secondary-medium px-3 py-2.5 text-sm text-heading shadow-xs placeholder:text-body focus:border-brand focus:ring-brand"
					placeholder="Nhập email"
				/>
			</div>

			<div class="border-t border-default pt-4">
				<h4 class="mb-4 text-sm font-medium text-heading">
					Đổi mật khẩu <span class="text-body font-normal">(để trống nếu không đổi)</span>
				</h4>
				<div class="space-y-4">
					<div>
						<label for="currentPassword" class="mb-2.5 block text-sm font-medium text-heading"
							>Mật khẩu hiện tại</label
						>
						<input
							type="password"
							id="currentPassword"
							bind:value={formData.currentPassword}
							class="block w-full rounded-base border border-default-medium bg-neutral-secondary-medium px-3 py-2.5 text-sm text-heading shadow-xs placeholder:text-body focus:border-brand focus:ring-brand"
							placeholder="Nhập mật khẩu hiện tại để xác nhận"
						/>
					</div>
					<div class="grid grid-cols-1 gap-4 md:grid-cols-2">
						<div>
							<label for="newPassword" class="mb-2.5 block text-sm font-medium text-heading"
								>Mật khẩu mới</label
							>
							<input
								type="password"
								id="newPassword"
								bind:value={formData.newPassword}
								class="block w-full rounded-base border border-default-medium bg-neutral-secondary-medium px-3 py-2.5 text-sm text-heading shadow-xs placeholder:text-body focus:border-brand focus:ring-brand"
								placeholder="Nhập mật khẩu mới"
							/>
						</div>
						<div>
							<label for="confirmPassword" class="mb-2.5 block text-sm font-medium text-heading"
								>Xác nhận mật khẩu mới</label
							>
							<input
								type="password"
								id="confirmPassword"
								bind:value={formData.confirmPassword}
								class="block w-full rounded-base border border-default-medium bg-neutral-secondary-medium px-3 py-2.5 text-sm text-heading shadow-xs placeholder:text-body focus:border-brand focus:ring-brand"
								placeholder="Nhập lại mật khẩu mới"
							/>
						</div>
					</div>
				</div>
			</div>

			<div class="flex items-center justify-end gap-3 border-t border-default pt-4">
				<Button variant="secondary" onclick={() => (open = false)} disabled={saving}>Hủy</Button>
				<Button variant="primary" onclick={handleSave} disabled={saving}>
					{saving ? 'Đang lưu...' : 'Lưu thay đổi'}
				</Button>
			</div>
		</div>
	{/if}
</Modal>

<Toast bind:show={showToast} message={toastMessage} type={toastType} />
