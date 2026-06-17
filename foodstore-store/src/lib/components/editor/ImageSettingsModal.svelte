<script lang="ts">
	import Icon from '../ui/Icon.svelte';

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
					<Icon name="tabler:settings" class="h-5 w-5 text-indigo-600" />
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
							<Icon name="tabler:link" class="h-4 w-4" />
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
							<Icon name="tabler:message" class="h-4 w-4" />
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
