<script lang="ts">
	import { onMount } from 'svelte';
	import {
		calculateSeoChecks,
		calculateTotalSeoScore,
		calculateMaxSeoScore
	} from '$lib/utils/seo.js';

	interface Props {
		title: string;
		content: string;
		excerpt: string;
		focusKeyword: string;
		metaTitle: string;
		metaDescription: string;
		onScoreChange?: (score: number) => void;
	}

	let { title, content, excerpt, focusKeyword, metaTitle, metaDescription, onScoreChange }: Props =
		$props();

	// SEO Checks
	const seoChecks = $derived(() => {
		return calculateSeoChecks(title, content, excerpt, focusKeyword, metaTitle, metaDescription);
	});

	const totalScore = $derived(() => {
		return calculateTotalSeoScore(seoChecks());
	});

	const maxScore = $derived(() => {
		return calculateMaxSeoScore(seoChecks());
	});

	const scoreColor = $derived(() => {
		const score = totalScore();
		if (score >= 80) return 'text-green-600';
		if (score >= 50) return 'text-yellow-600';
		return 'text-red-600';
	});

	const scoreBg = $derived(() => {
		const score = totalScore();
		if (score >= 80) return 'bg-green-500';
		if (score >= 50) return 'bg-yellow-500';
		return 'bg-red-500';
	});

	// Notify parent when score changes
	$effect(() => {
		const score = totalScore();
		onScoreChange?.(score);
	});
</script>

<div class="overflow-hidden rounded-lg border border-gray-200 bg-white">
	<div class="border-b border-gray-200 bg-gray-50 p-4">
		<div class="flex items-center justify-between">
			<h3 class="font-semibold text-gray-900">📊 Điểm SEO</h3>
			<div class="flex items-center gap-2">
				<span class="text-2xl font-bold {scoreColor()}">{totalScore()}</span>
				<span class="text-gray-400">/ {maxScore()}</span>
			</div>
		</div>
		<div class="mt-3 h-2 overflow-hidden rounded-full bg-gray-200">
			<div
				class="h-full transition-all duration-500 {scoreBg()}"
				style="width: {(totalScore() / maxScore()) * 100}%"
			></div>
		</div>
	</div>

	<div class="max-h-[400px] space-y-3 overflow-y-auto p-4">
		{#each seoChecks() as check (check.name)}
			<div class="flex items-start gap-3">
				<div class="mt-0.5">
					{#if check.status === 'good'}
						<span class="text-green-500">✓</span>
					{:else if check.status === 'warning'}
						<span class="text-yellow-500">⚠</span>
					{:else}
						<span class="text-red-500">✗</span>
					{/if}
				</div>
				<div class="flex-1">
					<div class="flex items-center justify-between">
						<span class="text-sm font-medium text-gray-900">{check.name}</span>
						<span class="text-xs text-gray-500">{check.score}/{check.maxScore}</span>
					</div>
					<p class="mt-0.5 text-xs text-gray-500">{check.message}</p>
				</div>
			</div>
		{/each}
	</div>

	<div class="border-t border-gray-200 bg-gray-50 p-4">
		<p class="text-xs text-gray-500">
			💡 <strong>Mẹo:</strong> Điểm SEO từ 80+ là tốt cho việc xếp hạng tìm kiếm.
		</p>
	</div>
</div>
