<script lang="ts">
	interface Props {
		open?: boolean;
		initialLink?: string;
		initialCaption?: string;
		onSave?: (link: string, caption: string) => void;
		onClose?: () => void;
	}

	let {
		open = $bindable(false),
		initialLink = '',
		initialCaption = '',
		onSave,
		onClose
	}: Props = $props();

	let link = $state('');
	let caption = $state('');

	$effect(() => {
		if (open) {
			link = initialLink;
			caption = initialCaption;
		}
	});

	function handleSave() {
		onSave?.(link, caption);
		open = false;
	}

	function handleClose() {
		onClose?.();
		open = false;
	}
</script>

{#if open}
	<!-- svelte-ignore a11y_no_static_element_interactions -->
	<div
		class="fixed inset-0 z-[100] flex items-center justify-center bg-black/50 backdrop-blur-sm"
		onclick={handleClose}
		onkeydown={(e) => e.key === 'Escape' && handleClose()}
	>
		<!-- svelte-ignore a11y_click_events_have_key_events -->
		<!-- svelte-ignore a11y_no_noninteractive_element_interactions -->
		<div
			class="w-full max-w-md rounded-xl bg-white p-6 shadow-2xl"
			onclick={(e) => e.stopPropagation()}
			role="dialog"
			tabindex="-1"
		>
			<div class="mb-6 flex items-center gap-3">
				<div class="rounded-lg bg-indigo-100 p-2">
					<svg
						class="h-5 w-5 text-indigo-600"
						fill="none"
						stroke="currentColor"
						viewBox="0 0 24 24"
					>
						<path
							stroke-linecap="round"
							stroke-linejoin="round"
							stroke-width="2"
							d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"
						/>
						<path
							stroke-linecap="round"
							stroke-linejoin="round"
							stroke-width="2"
							d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"
						/>
					</svg>
				</div>
				<div>
					<h3 class="text-lg font-bold text-gray-900">Cài đặt Ảnh</h3>
					<p class="text-sm text-gray-500">Thêm link hoặc chú thích cho ảnh</p>
				</div>
			</div>

			<div class="space-y-4">
				<!-- Link Field -->
				<div>
					<label for="image-link" class="mb-1.5 block text-sm font-medium text-gray-700">
						<span class="flex items-center gap-1.5">
							<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
								<path
									stroke-linecap="round"
									stroke-linejoin="round"
									stroke-width="2"
									d="M13.828 10.172a4 4 0 00-5.656 0l-4 4a4 4 0 105.656 5.656l1.102-1.101m-.758-4.899a4 4 0 005.656 0l4-4a4 4 0 00-5.656-5.656l-1.1 1.1"
								/>
							</svg>
							Link (tùy chọn)
						</span>
					</label>
					<input
						id="image-link"
						type="url"
						bind:value={link}
						placeholder="https://example.com"
						class="w-full rounded-lg border border-gray-300 px-4 py-2.5 text-sm transition-colors focus:border-indigo-500 focus:ring-2 focus:ring-indigo-500/20"
					/>
					<p class="mt-1 text-xs text-gray-400">Khi click vào ảnh sẽ mở link này</p>
				</div>

				<!-- Caption Field -->
				<div>
					<label for="image-caption" class="mb-1.5 block text-sm font-medium text-gray-700">
						<span class="flex items-center gap-1.5">
							<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
								<path
									stroke-linecap="round"
									stroke-linejoin="round"
									stroke-width="2"
									d="M7 8h10M7 12h4m1 8l-4-4H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-3l-4 4z"
								/>
							</svg>
							Chú thích (Caption)
						</span>
					</label>
					<textarea
						id="image-caption"
						bind:value={caption}
						placeholder="Nhập chú thích cho ảnh..."
						rows="2"
						class="w-full resize-none rounded-lg border border-gray-300 px-4 py-2.5 text-sm transition-colors focus:border-indigo-500 focus:ring-2 focus:ring-indigo-500/20"
					></textarea>
					<p class="mt-1 text-xs text-gray-400">Hiển thị bên dưới ảnh, căn giữa</p>
				</div>
			</div>

			<div class="mt-6 flex justify-end gap-3">
				<button
					type="button"
					onclick={handleClose}
					class="rounded-lg px-4 py-2 text-sm font-medium text-gray-600 transition-colors hover:bg-gray-100"
				>
					Hủy
				</button>
				<button
					type="button"
					onclick={handleSave}
					class="rounded-lg bg-indigo-600 px-4 py-2 text-sm font-medium text-white transition-colors hover:bg-indigo-700"
				>
					Lưu thay đổi
				</button>
			</div>
		</div>
	</div>
{/if}
