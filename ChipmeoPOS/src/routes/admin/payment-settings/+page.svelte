<script lang="ts">
	import { onMount } from 'svelte';
	import Button from '$lib/components/ui/Button.svelte';
	import Card from '$lib/components/ui/Card.svelte';
	import Modal from '$lib/components/ui/Modal.svelte';
	import { PaymentSettingsState } from './payment-settings.svelte.js';

	const vm = new PaymentSettingsState();

	onMount(() => {
		vm.init();
	});
</script>

<div class="p-6">
	<div class="mb-6 flex items-center justify-between">
		<div>
			<h1 class="text-2xl font-bold text-gray-900">Quản lý tài khoản thanh toán</h1>
			<p class="text-gray-600">Cấu hình tài khoản nhận tiền QR Code</p>
		</div>
		<Button variant="primary" onclick={() => vm.openCreateDialog()}>+ Thêm tài khoản</Button>
	</div>

	{#if vm.loading}
		<div class="py-12 text-center">
			<div
				class="inline-block h-12 w-12 animate-spin rounded-full border-b-2 border-indigo-600"
			></div>
		</div>
	{:else if vm.settings.length === 0}
		<div class="rounded-lg border-2 border-dashed border-gray-200 bg-white py-12 text-center">
			<p class="mb-4 text-lg text-gray-500">Chưa có tài khoản thanh toán nào</p>
			<Button variant="primary" onclick={() => vm.openCreateDialog()}
				>Thêm tài khoản đầu tiên</Button
			>
		</div>
	{:else}
		<div class="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3">
			{#each vm.settings as setting (setting.id)}
				<div
					class="relative rounded-xl border border-gray-100 bg-white p-6 shadow-sm transition-shadow hover:shadow-md {setting.isDefault
						? 'bg-indigo-50/50 ring-2 ring-indigo-500'
						: ''}"
				>
					{#if setting.isDefault}
						<div
							class="absolute top-4 right-4 rounded bg-indigo-600 px-2 py-1 text-xs font-bold text-white"
						>
							MẶC ĐỊNH
						</div>
					{/if}

					<div class="mb-4">
						<h3 class="mb-1 text-lg font-bold text-gray-900">{setting.bankName}</h3>
						<div class="space-y-2 text-sm">
							<div class="flex justify-between border-b border-gray-100 pb-1">
								<span class="text-gray-500">Số tài khoản:</span>
								<span class="font-medium text-gray-900">{setting.bankAccount}</span>
							</div>
							<div class="flex justify-between border-b border-gray-100 pb-1">
								<span class="text-gray-500">Chủ tài khoản:</span>
								<span class="font-medium text-gray-900 uppercase"
									>{setting.bankAccountName || 'N/A'}</span
								>
							</div>
							<div class="flex justify-between border-b border-gray-100 pb-1">
								<span class="text-gray-500">Mã ngân hàng:</span>
								<span class="font-medium text-gray-900">{setting.bankId}</span>
							</div>
							<div class="flex justify-between">
								<span class="text-gray-500">Template:</span>
								<span class="font-medium text-gray-900">{setting.template}</span>
							</div>
						</div>
					</div>

					<div class="mt-4 flex flex-wrap gap-2 border-t border-gray-100 pt-4">
						<button
							class="rounded bg-indigo-50 px-3 py-1.5 text-sm font-medium text-indigo-600 transition-colors hover:bg-indigo-100"
							onclick={() => vm.openEditDialog(setting)}
						>
							Sửa
						</button>
						{#if !setting.isDefault}
							<button
								class="rounded bg-gray-100 px-3 py-1.5 text-sm font-medium text-gray-600 transition-colors hover:bg-gray-200"
								onclick={() => vm.toggleDefault(setting)}
							>
								Đặt mặc định
							</button>
							<button
								class="ml-auto rounded bg-red-50 px-3 py-1.5 text-sm font-medium text-red-600 transition-colors hover:bg-red-100"
								onclick={() => vm.deleteSetting(setting.id)}
							>
								Xóa
							</button>
						{/if}
					</div>
				</div>
			{/each}
		</div>
	{/if}
</div>

<Modal
	bind:open={vm.showDialog}
	title={vm.editingId ? 'Sửa tài khoản' : 'Thêm tài khoản mới'}
	onClose={() => (vm.showDialog = false)}
>
	<form
		onsubmit={(e) => {
			e.preventDefault();
			vm.handleSave();
		}}
		class="space-y-4"
	>
		<div>
			<label for="bankId" class="mb-1 block text-sm font-medium text-gray-700"
				>Chọn ngân hàng *</label
			>
			<select
				id="bankId"
				bind:value={vm.formData.bankId}
				onchange={() => vm.onBankSelect()}
				required
				class="w-full rounded-lg border px-4 py-2 focus:ring-2 focus:ring-indigo-500"
			>
				<option value="">-- Chọn ngân hàng --</option>
				{#each vm.banks as bank (bank.bin)}
					<option value={bank.bin}>{bank.shortName} - {bank.name}</option>
				{/each}
			</select>
			<p class="mt-1 text-xs text-gray-500">Tên ngân hàng sẽ tự động điền sau khi chọn</p>
		</div>

		<div>
			<label for="bankName" class="mb-1 block text-sm font-medium text-gray-700"
				>Tên ngân hàng</label
			>
			<input
				id="bankName"
				type="text"
				bind:value={vm.formData.bankName}
				readonly
				class="w-full cursor-not-allowed rounded-lg border bg-gray-50 px-4 py-2 text-gray-600"
			/>
		</div>

		<div>
			<label for="bankAccount" class="mb-1 block text-sm font-medium text-gray-700"
				>Số tài khoản</label
			>
			<input
				id="bankAccount"
				type="text"
				bind:value={vm.formData.bankAccount}
				placeholder="VD: 108873756885"
				class="w-full rounded-lg border px-4 py-2 focus:ring-2 focus:ring-indigo-500"
			/>
		</div>

		<div>
			<label for="bankAccountName" class="mb-1 block text-sm font-medium text-gray-700"
				>Chủ tài khoản</label
			>
			<input
				id="bankAccountName"
				type="text"
				bind:value={vm.formData.bankAccountName}
				placeholder="VD: NGUYEN QUOC TUAN"
				class="w-full rounded-lg border px-4 py-2 uppercase focus:ring-2 focus:ring-indigo-500"
			/>
		</div>

		<div>
			<label for="template" class="mb-1 block text-sm font-medium text-gray-700">Template QR</label>
			<select
				id="template"
				bind:value={vm.formData.template}
				class="w-full rounded-lg border px-4 py-2 focus:ring-2 focus:ring-indigo-500"
			>
				<option value="compact">Compact (Nhỏ gọn)</option>
				<option value="compact2">Compact 2 (Nhỏ gọn 2)</option>
				<option value="print">Print (In ấn)</option>
				<option value="qr_only">QR Only (Chỉ mã QR)</option>
			</select>
		</div>

		<div class="flex items-center gap-4 pt-2">
			<label class="flex cursor-pointer items-center gap-2">
				<input
					type="checkbox"
					bind:checked={vm.formData.isActive}
					class="h-4 w-4 rounded text-indigo-600 focus:ring-indigo-500"
				/>
				<span class="text-sm font-medium text-gray-700">Kích hoạt</span>
			</label>

			<label class="flex cursor-pointer items-center gap-2">
				<input
					type="checkbox"
					bind:checked={vm.formData.isDefault}
					class="h-4 w-4 rounded text-indigo-600 focus:ring-indigo-500"
				/>
				<span class="text-sm font-medium text-gray-700">Đặt làm mặc định</span>
			</label>
		</div>

		<div class="mt-4 flex gap-3 border-t pt-4">
			<Button variant="primary" type="submit" fullWidth={true}>
				{vm.editingId ? 'Cập nhật' : 'Thêm tài khoản'}
			</Button>
			<Button variant="secondary" onclick={() => (vm.showDialog = false)} fullWidth={true}
				>Hủy</Button
			>
		</div>
	</form>
</Modal>
