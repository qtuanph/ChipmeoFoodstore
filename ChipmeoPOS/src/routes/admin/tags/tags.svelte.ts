import { tagsAPI, type Tag, type CreateTagDto } from '$lib/api/tags.js';

interface TagFormData {
	name: string;
	description: string;
	color: string;
}

export class TagsState {
	tags = $state<Tag[]>([]);
	loading = $state(true);
	showModal = $state(false);
	editingTag = $state<Tag | null>(null);
	saving = $state(false);

	formData = $state<TagFormData>({
		name: '',
		description: '',
		color: '#f59e0b'
	});

	readonly presetColors = [
		'#f59e0b',
		'#ef4444',
		'#10b981',
		'#3b82f6',
		'#8b5cf6',
		'#ec4899',
		'#6b7280',
		'#14b8a6'
	];

	readonly colorNames: Record<string, string> = {
		'#f59e0b': 'Vàng cam',
		'#ef4444': 'Đỏ',
		'#10b981': 'Xanh lá',
		'#3b82f6': 'Xanh dương',
		'#8b5cf6': 'Tím',
		'#ec4899': 'Hồng',
		'#6b7280': 'Xám',
		'#14b8a6': 'Xanh ngọc'
	};

	async init() {
		await this.loadTags();
	}

	async loadTags() {
		this.loading = true;
		try {
			this.tags = await tagsAPI.getAll();
		} catch (err) {
			console.error('Failed to load tags:', err);
		} finally {
			this.loading = false;
		}
	}

	openNewTag() {
		this.editingTag = null;
		this.formData = { name: '', description: '', color: '#f59e0b' };
		this.showModal = true;
	}

	openEditTag(tag: Tag) {
		this.editingTag = tag;
		this.formData = {
			name: tag.name,
			description: tag.description || '',
			color: tag.color
		};
		this.showModal = true;
	}

	async handleSave(): Promise<string | null> {
		if (!this.formData.name.trim()) return null;

		this.saving = true;
		try {
			if (this.editingTag) {
				await tagsAPI.update(this.editingTag.id, this.formData);
			} else {
				await tagsAPI.create(this.formData as CreateTagDto);
			}
			this.showModal = false;
			await this.loadTags();
			return null;
		} catch (err: unknown) {
			return err instanceof Error ? err.message : 'Lỗi khi lưu tag';
		} finally {
			this.saving = false;
		}
	}

	async handleDelete(tag: Tag): Promise<string | null> {
		try {
			await tagsAPI.delete(tag.id);
			await this.loadTags();
			return null;
		} catch (err: unknown) {
			return err instanceof Error ? err.message : 'Lỗi khi xóa tag';
		}
	}
}
