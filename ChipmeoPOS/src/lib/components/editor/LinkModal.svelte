<script lang="ts">
	import Modal from '$lib/components/ui/Modal.svelte';
	import Button from '$lib/components/ui/Button.svelte';

	let {
		open = $bindable(false),
		initialUrl = '',
		initialText = '',
		onSave,
		onClose
	} = $props<{
		open: boolean;
		initialUrl?: string;
		initialText?: string; // Tiptap might not easily support changing text of selection if it spans nodes, but for a single link it's possible.
		onSave: (url: string, text?: string) => void;
		onClose: () => void;
	}>();

	let url = $state('');
	let text = $state('');

	// Update state when modal opens
	$effect(() => {
		if (open) {
			url = initialUrl || '';
			text = initialText || '';
		}
	});

	function handleSave() {
		onSave(url, text);
		open = false;
	}
</script>

<Modal bind:open title="Chèn/Sửa Link" {onClose}>
	<form
		onsubmit={(e) => {
			e.preventDefault();
			handleSave();
		}}
		class="space-y-4"
	>
		<div>
			<label for="link-url" class="mb-1 block text-sm font-medium text-gray-700"
				>Đường dẫn (URL)</label
			>
			<input
				id="link-url"
				type="text"
				bind:value={url}
				placeholder="https://google.com"
				class="w-full rounded-lg border px-4 py-2 focus:ring-2 focus:ring-indigo-500"
			/>
		</div>

		{#if initialText !== undefined}
			<div>
				<label for="link-text" class="mb-1 block text-sm font-medium text-gray-700"
					>Văn bản hiển thị</label
				>
				<input
					id="link-text"
					type="text"
					bind:value={text}
					placeholder="Nhập văn bản..."
					class="w-full rounded-lg border px-4 py-2 focus:ring-2 focus:ring-indigo-500"
				/>
			</div>
		{/if}

		<div class="flex justify-end gap-3 pt-2">
			<Button
				variant="secondary"
				onclick={() => {
					open = false;
					onClose();
				}}
				type="button"
			>
				Hủy
			</Button>
			<Button variant="primary" type="submit">Lưu</Button>
		</div>
	</form>
</Modal>
