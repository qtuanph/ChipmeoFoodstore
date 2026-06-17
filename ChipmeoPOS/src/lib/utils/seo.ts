import { SITE_DOMAIN } from '$lib/config/index.js';

export interface SeoCheck {
	name: string;
	score: number;
	maxScore: number;
	status: 'good' | 'warning' | 'bad';
	message: string;
}

export function stripHtml(html: string): string {
	return html
		.replace(/<[^>]+>/g, ' ')
		.replace(/\s+/g, ' ')
		.trim();
}

export function getWordCount(text: string): number {
	const words = stripHtml(text)
		.split(/\s+/)
		.filter((w) => w.length > 0);
	return words.length;
}

export function calculateSeoChecks(
	title: string,
	content: string,
	excerpt: string,
	focusKeyword: string,
	metaTitle: string,
	metaDescription: string
): SeoCheck[] {
	const checks: SeoCheck[] = [];
	const textContent = stripHtml(content);
	const wordCount = getWordCount(content);
	const keyword = (focusKeyword || '').toLowerCase().trim();

	// 1. Title length (10 points)
	const titleLen = (metaTitle || title).length;
	if (titleLen >= 30 && titleLen <= 60) {
		checks.push({
			name: 'Độ dài tiêu đề',
			score: 10,
			maxScore: 10,
			status: 'good',
			message: `Tốt (${titleLen} ký tự)`
		});
	} else if (titleLen > 0 && titleLen < 30) {
		checks.push({
			name: 'Độ dài tiêu đề',
			score: 5,
			maxScore: 10,
			status: 'warning',
			message: `Quá ngắn (${titleLen} ký tự, nên 30-60)`
		});
	} else if (titleLen > 60) {
		checks.push({
			name: 'Độ dài tiêu đề',
			score: 5,
			maxScore: 10,
			status: 'warning',
			message: `Quá dài (${titleLen} ký tự, nên 30-60)`
		});
	} else {
		checks.push({
			name: 'Độ dài tiêu đề',
			score: 0,
			maxScore: 10,
			status: 'bad',
			message: 'Chưa có tiêu đề'
		});
	}

	// 2. Meta description (10 points)
	const descLen = (metaDescription || '').length;
	if (descLen >= 120 && descLen <= 160) {
		checks.push({
			name: 'Mô tả meta',
			score: 10,
			maxScore: 10,
			status: 'good',
			message: `Tốt (${descLen} ký tự)`
		});
	} else if (descLen > 0 && descLen < 120) {
		checks.push({
			name: 'Mô tả meta',
			score: 5,
			maxScore: 10,
			status: 'warning',
			message: `Quá ngắn (${descLen} ký tự, nên 120-160)`
		});
	} else if (descLen > 160) {
		checks.push({
			name: 'Mô tả meta',
			score: 5,
			maxScore: 10,
			status: 'warning',
			message: `Quá dài (${descLen} ký tự, nên 120-160)`
		});
	} else {
		checks.push({
			name: 'Mô tả meta',
			score: 0,
			maxScore: 10,
			status: 'bad',
			message: 'Chưa có mô tả meta'
		});
	}

	// 3. Focus keyword in title (15 points)
	if (keyword && (metaTitle || title).toLowerCase().includes(keyword)) {
		checks.push({
			name: 'Từ khóa trong tiêu đề',
			score: 15,
			maxScore: 15,
			status: 'good',
			message: 'Có từ khóa trong tiêu đề'
		});
	} else if (keyword) {
		checks.push({
			name: 'Từ khóa trong tiêu đề',
			score: 0,
			maxScore: 15,
			status: 'bad',
			message: 'Thiếu từ khóa trong tiêu đề'
		});
	} else {
		checks.push({
			name: 'Từ khóa trong tiêu đề',
			score: 0,
			maxScore: 15,
			status: 'warning',
			message: 'Chưa nhập từ khóa chính'
		});
	}

	// 4. Focus keyword in H1/H2 (10 points)
	const hasKeywordInHeading =
		keyword &&
		content
			.toLowerCase()
			.match(/<h[12][^>]*>[^<]*$/)?.[0]
			?.includes(keyword);
	if (hasKeywordInHeading) {
		checks.push({
			name: 'Từ khóa trong tiêu đề bài',
			score: 10,
			maxScore: 10,
			status: 'good',
			message: 'Có từ khóa trong H1/H2'
		});
	} else if (keyword) {
		checks.push({
			name: 'Từ khóa trong tiêu đề bài',
			score: 0,
			maxScore: 10,
			status: 'warning',
			message: 'Nên thêm từ khóa vào tiêu đề bài'
		});
	} else {
		checks.push({
			name: 'Từ khóa trong tiêu đề bài',
			score: 0,
			maxScore: 10,
			status: 'warning',
			message: 'Chưa nhập từ khóa chính'
		});
	}

	// 5. Keyword density (10 points)
	if (keyword && wordCount > 0) {
		const keywordRegex = new RegExp(keyword, 'gi');
		const keywordCount = (textContent.match(keywordRegex) || []).length;
		const density = (keywordCount / wordCount) * 100;
		if (density >= 1 && density <= 3) {
			checks.push({
				name: 'Mật độ từ khóa',
				score: 10,
				maxScore: 10,
				status: 'good',
				message: `Tốt (${density.toFixed(1)}%)`
			});
		} else if (density > 0 && density < 1) {
			checks.push({
				name: 'Mật độ từ khóa',
				score: 5,
				maxScore: 10,
				status: 'warning',
				message: `Thấp (${density.toFixed(1)}%, nên 1-3%)`
			});
		} else if (density > 3) {
			checks.push({
				name: 'Mật độ từ khóa',
				score: 3,
				maxScore: 10,
				status: 'warning',
				message: `Quá cao (${density.toFixed(1)}%, nên 1-3%)`
			});
		} else {
			checks.push({
				name: 'Mật độ từ khóa',
				score: 0,
				maxScore: 10,
				status: 'bad',
				message: 'Không tìm thấy từ khóa trong nội dung'
			});
		}
	} else {
		checks.push({
			name: 'Mật độ từ khóa',
			score: 0,
			maxScore: 10,
			status: 'warning',
			message: 'Chưa nhập từ khóa chính'
		});
	}

	// 6. Content length (15 points)
	if (wordCount >= 300) {
		checks.push({
			name: 'Độ dài nội dung',
			score: 15,
			maxScore: 15,
			status: 'good',
			message: `Tốt (${wordCount} từ)`
		});
	} else if (wordCount >= 150) {
		checks.push({
			name: 'Độ dài nội dung',
			score: 8,
			maxScore: 15,
			status: 'warning',
			message: `Hơi ngắn (${wordCount} từ, nên >300)`
		});
	} else {
		checks.push({
			name: 'Độ dài nội dung',
			score: 0,
			maxScore: 15,
			status: 'bad',
			message: `Quá ngắn (${wordCount} từ, nên >300)`
		});
	}

	// 7. Images with alt (10 points)
	const imgTags = content.match(/<img[^>]+>/g) || ([] as string[]);
	const imgsWithAlt = imgTags.filter((img) => img.includes('alt=')).length;
	if (imgTags.length === 0) {
		checks.push({
			name: 'Hình ảnh',
			score: 5,
			maxScore: 10,
			status: 'warning',
			message: 'Nên thêm hình ảnh minh họa'
		});
	} else if (imgsWithAlt === imgTags.length) {
		checks.push({
			name: 'Hình ảnh',
			score: 10,
			maxScore: 10,
			status: 'good',
			message: `${imgTags.length} ảnh có alt text`
		});
	} else {
		checks.push({
			name: 'Hình ảnh',
			score: 5,
			maxScore: 10,
			status: 'warning',
			message: `${imgTags.length - imgsWithAlt}/${imgTags.length} ảnh thiếu alt`
		});
	}

	// 8. Internal links (10 points)
	const internalLinks =
		content.match(new RegExp(`href=["'][^"']*${SITE_DOMAIN}[^"']*["']`, 'g')) || [];
	if (internalLinks.length >= 1) {
		checks.push({
			name: 'Liên kết nội bộ',
			score: 10,
			maxScore: 10,
			status: 'good',
			message: `${internalLinks.length} liên kết nội bộ`
		});
	} else {
		checks.push({
			name: 'Liên kết nội bộ',
			score: 0,
			maxScore: 10,
			status: 'warning',
			message: 'Nên thêm liên kết nội bộ'
		});
	}

	// 9. Excerpt (5 points)
	if (excerpt.length >= 50 && excerpt.length <= 200) {
		checks.push({
			name: 'Tóm tắt bài viết',
			score: 5,
			maxScore: 5,
			status: 'good',
			message: 'Đã có tóm tắt'
		});
	} else if (excerpt.length > 0) {
		checks.push({
			name: 'Tóm tắt bài viết',
			score: 3,
			maxScore: 5,
			status: 'warning',
			message: 'Tóm tắt nên 50-200 ký tự'
		});
	} else {
		checks.push({
			name: 'Tóm tắt bài viết',
			score: 0,
			maxScore: 5,
			status: 'bad',
			message: 'Chưa có tóm tắt'
		});
	}

	// 10. Has headings (5 points)
	const headings = content.match(/<h[2-4][^>]*>/g) || [];
	if (headings.length >= 2) {
		checks.push({
			name: 'Cấu trúc tiêu đề',
			score: 5,
			maxScore: 5,
			status: 'good',
			message: `${headings.length} tiêu đề phụ`
		});
	} else if (headings.length === 1) {
		checks.push({
			name: 'Cấu trúc tiêu đề',
			score: 3,
			maxScore: 5,
			status: 'warning',
			message: 'Nên thêm tiêu đề phụ'
		});
	} else {
		checks.push({
			name: 'Cấu trúc tiêu đề',
			score: 0,
			maxScore: 5,
			status: 'bad',
			message: 'Thiếu tiêu đề phụ (H2, H3)'
		});
	}

	return checks;
}

export function calculateTotalSeoScore(checks: SeoCheck[]): number {
	return checks.reduce((sum, check) => sum + check.score, 0);
}

export function calculateMaxSeoScore(checks: SeoCheck[]): number {
	return checks.reduce((sum, check) => sum + check.maxScore, 0);
}
