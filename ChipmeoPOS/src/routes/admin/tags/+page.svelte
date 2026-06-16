<script lang="ts">
	import { onMount } from 'svelte';
	import Button from '$lib/components/Button.svelte';
	import Modal from '$lib/components/Modal.svelte';
	import { TagsState } from './tags.svelte.js';

	const vm = new TagsState();

	onMount(() => {
		vm.init();
	});

	async function handleSave() {
		const error = await vm.handleSave();
		if (error) alert(error);
	}

	async function handleDelete(tag: any) {
		if (!confirm(`Xóa tag "${tag.name}"?`)) return;
		const error = await vm.handleDelete(tag);
		if (error) alert(error);
	}
</script>

<div class="p-6">
	<!-- Header -->
	<div class="mb-6 flex flex-col items-start justify-between gap-4 sm:flex-row sm:items-center">
		<div>
			<h1 class="text-2xl font-bold text-gray-900">Quản lý Tags</h1>
			<p class="text-gray-600">Phân loại bài viết theo chủ đề</p>
		</div>
		<Button variant="primary" onclick={() => vm.openNewTag()}>+ Thêm Tag</Button>
	</div>

	<!-- Tags Grid -->
	{#if vm.loading}
		<div class="py-12 text-center">
			<div class="inline-block h-8 w-8 animate-spin rounded-full border-b-2 border-amber-600"></div>
		</div>
	{:else if vm.tags.length === 0}
		<div class="rounded-xl border border-dashed border-gray-300 bg-white py-12 text-center">
			<div class="mb-4 text-4xl">🏷️</div>
			<p class="mb-4 text-lg text-gray-500">Chưa có tag nào</p>
			<Button onclick={() => vm.openNewTag()}>Tạo tag đầu tiên</Button>
		</div>
	{:else}
		<div class="grid grid-cols-2 gap-4 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5">
			{#each vm.tags as tag (tag.id)}
				<div
					role="button"
					tabindex="0"
					class="group relative w-full cursor-pointer overflow-hidden rounded-xl border border-gray-200 bg-white p-4 text-left transition-all hover:shadow-md focus:ring-2 focus:ring-amber-500 focus:outline-none"
					onclick={() => vm.openEditTag(tag)}
					onkeydown={(e) => {
						if (e.key === 'Enter' || e.key === ' ') {
							e.preventDefault();
							vm.openEditTag(tag);
						}
					}}
				>
					<div
						class="absolute top-0 bottom-0 left-0 w-1.5"
						style="background-color: {tag.color}"
					></div>

					<div class="pl-2">
						<div class="mb-2 flex items-center justify-between">
							<span class="h-2 w-2 rounded-full" style="background-color: {tag.color}"></span>
							<span class="rounded-full bg-gray-100 px-2 py-0.5 text-xs font-medium text-gray-500"
								>{tag.postCount} bài</span
							>
						</div>
						<h3 class="mb-0.5 truncate font-bold text-gray-900">{tag.name}</h3>
						<p class="mb-1 truncate text-xs text-gray-400">{tag.slug}</p>
						{#if tag.description}
							<p class="line-clamp-2 text-xs text-gray-500">{tag.description}</p>
						{/if}
					</div>

					<div class="absolute top-2 right-2 opacity-0 transition-opacity group-hover:opacity-100">
						<button
							type="button"
							onclick={(e) => {
								e.stopPropagation();
								handleDelete(tag);
							}}
							class="rounded-lg p-1.5 text-red-500 transition-colors hover:bg-red-50"
							title="Xóa tag"
						>
							<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
								<path
									stroke-linecap="round"
									stroke-linejoin="round"
									stroke-width="2"
									d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
								/>
							</svg>
						</button>
					</div>
				</div>
			{/each}
		</div>
	{/if}
</div>

<Modal
	open={vm.showModal}
	title={vm.editingTag ? 'Chỉnh sửa Tag' : 'Thêm Tag mới'}
	onClose={() => (vm.showModal = false)}
>
	<form
		class="space-y-4"
		onsubmit={(e) => {
			e.preventDefault();
			handleSave();
		}}
	>
		<div>
			<label for="tag-name" class="mb-1 block text-sm font-medium text-gray-700"
				>Tên tag <span class="text-red-500">*</span></label
			>
			<input
				type="text"
				id="tag-name"
				bind:value={vm.formData.name}
				placeholder="VD: Công thức nấu ăn"
				class="w-full rounded-lg border border-gray-300 px-4 py-2 transition-colors focus:border-amber-500 focus:ring-2 focus:ring-amber-500"
				required
			/>
		</div>

		<div>
			<label for="tag-description" class="mb-1 block text-sm font-medium text-gray-700">Mô tả</label
			>
			<textarea
				id="tag-description"
				bind:value={vm.formData.description}
				placeholder="Mô tả ngắn về tag này..."
				rows="3"
				class="w-full rounded-lg border border-gray-300 px-4 py-2 transition-colors focus:border-amber-500 focus:ring-2 focus:ring-amber-500"
			></textarea>
		</div>

		<div>
			<span class="mb-2 block text-sm font-medium text-gray-700">Màu sắc</span>
			<div class="flex flex-wrap items-center gap-3">
				{#each vm.presetColors as color (color)}
					<button
						type="button"
						onclick={() => (vm.formData.color = color)}
						class="h-8 w-8 rounded-full border-2 transition-all hover:scale-110 focus:ring-2 focus:ring-amber-500 focus:ring-offset-2 focus:outline-none {vm
							.formData.color === color
							? 'scale-110 border-gray-900 shadow-sm'
							: 'border-transparent'}"
						style="background-color: {color}"
						title={vm.colorNames[color] || color}
					></button>
				{/each}
				<div class="mx-1 h-8 w-px bg-gray-200"></div>
				<label class="group relative cursor-pointer">
					<input type="color" bind:value={vm.formData.color} class="sr-only" />
					<div
						class="flex h-8 w-8 items-center justify-center rounded-full border-2 transition-all {vm.presetColors.includes(
							vm.formData.color
						)
							? 'border-gray-200'
							: 'border-gray-900'}"
						style="background: {vm.presetColors.includes(vm.formData.color)
							? 'conic-gradient(from 180deg, red, yellow, lime, aqua, blue, magenta, red)'
							: vm.formData.color}"
					>
						{#if !vm.presetColors.includes(vm.formData.color)}
							<div class="h-full w-full rounded-full border-2 border-white"></div>
						{/if}
					</div>
				</label>
			</div>
		</div>

		<div class="flex justify-end gap-3 border-t border-gray-100 pt-4">
			<Button variant="secondary" onclick={() => (vm.showModal = false)} type="button">Hủy</Button>
			<Button variant="primary" type="submit" disabled={vm.saving || !vm.formData.name.trim()}>
				{vm.saving ? 'Đang lưu...' : 'Lưu tag'}
			</Button>
		</div>
	</form>
</Modal>
