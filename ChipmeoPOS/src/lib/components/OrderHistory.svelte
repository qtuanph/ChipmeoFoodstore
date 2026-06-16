<script lang="ts">
	import { formatTime } from '$lib/utils/index.ts';
	import type { OrderStatusHistory } from '$lib/types/index.ts';

	let { history }: { history: OrderStatusHistory[] } = $props();

	const statusMap: Record<string, string> = {
		pending: 'Chờ xác nhận',
		confirmed: 'Đã xác nhận',
		preparing: 'Đang chuẩn bị',
		ready: 'Sẵn sàng',
		served: 'Đã phục vụ',
		paid: 'Đã thanh toán',
		cancelled: 'Đã hủy'
	};
</script>

<div class="flow-root">
	<ul role="list" class="-mb-8">
		{#each history as item, i (item.id || i)}
			<li>
				<div class="relative pb-8">
					{#if i !== history.length - 1}
						<span class="absolute top-4 left-4 -ml-px h-full w-0.5 bg-gray-200" aria-hidden="true"
						></span>
					{/if}
					<div class="relative flex space-x-3">
						<div>
							<span
								class="flex h-8 w-8 items-center justify-center rounded-full bg-gray-100 ring-8 ring-white"
							>
								<svg class="h-5 w-5 text-gray-500" viewBox="0 0 20 20" fill="currentColor">
									<path
										fill-rule="evenodd"
										d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-12a1 1 0 10-2 0v4a1 1 0 00.293.707l2.828 2.829a1 1 0 101.415-1.415L11 9.586V6z"
										clip-rule="evenodd"
									/>
								</svg>
							</span>
						</div>
						<div class="flex min-w-0 flex-1 justify-between space-x-4 pt-1.5">
							<div>
								<p class="text-sm text-gray-500">
									Chuyển sang <span class="font-medium text-gray-900"
										>{statusMap[item.toStatus] || item.toStatus}</span
									>
									{#if item.changedByName}
										bởi <span class="font-medium text-gray-900">{item.changedByName}</span>
									{/if}
								</p>
								{#if item.note}
									<p class="mt-1 text-xs text-gray-400">{item.note}</p>
								{/if}
							</div>
							<div class="text-right text-sm whitespace-nowrap text-gray-500">
								<time datetime={item.changedAt}>{formatTime(item.changedAt)}</time>
							</div>
						</div>
					</div>
				</div>
			</li>
		{/each}
		{#if history.length === 0}
			<li class="py-4 text-center text-sm text-gray-500">Chưa có lịch sử thay đổi</li>
		{/if}
	</ul>
</div>
