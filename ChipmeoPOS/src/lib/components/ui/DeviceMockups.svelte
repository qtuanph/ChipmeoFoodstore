<script lang="ts">
	interface Props {
		type?: 'mobile' | 'tablet' | 'laptop' | 'desktop' | 'watch';
		children: any;
		class?: string;
	}

	let { type = 'mobile', children, class: className = '' }: Props = $props();
</script>

{#if type === 'mobile'}
	<div
		class="relative mx-auto border-default bg-base border-[14px] rounded-[2.5rem] h-[600px] w-[300px] shadow-xl {className}"
	>
		<div class="h-[32px] w-[3px] bg-base absolute -start-[17px] top-[72px] rounded-s-lg"></div>
		<div class="h-[32px] w-[3px] bg-base absolute -start-[17px] top-[124px] rounded-s-lg"></div>
		<div class="h-[32px] w-[3px] bg-base absolute -start-[17px] top-[178px] rounded-s-lg"></div>
		<div class="h-[64px] w-[3px] bg-base absolute -end-[17px] top-[142px] rounded-e-lg"></div>
		<div class="rounded-[2rem] overflow-hidden w-[272px] h-[572px] bg-neutral-primary">
			{@render children()}
		</div>
	</div>
{:else if type === 'tablet'}
	<div
		class="relative mx-auto border-default bg-base border-[14px] rounded-[2.5rem] h-[454px] max-w-[341px] md:h-[682px] md:max-w-[512px] shadow-xl {className}"
	>
		<div class="rounded-[2rem] overflow-hidden w-full h-full bg-neutral-primary">
			{@render children()}
		</div>
	</div>
{:else if type === 'laptop'}
	<div class={className}>
		<div
			class="relative mx-auto border-default bg-base border-[8px] rounded-t-xl h-[172px] max-w-[301px] md:h-[294px] md:max-w-[512px]"
		>
			<div class="overflow-hidden rounded-[calc(0.75rem-8px)] w-full h-full bg-neutral-primary">
				{@render children()}
			</div>
		</div>
		<div
			class="relative mx-auto bg-base rounded-b-xl rounded-t-sm h-[17px] max-w-[351px] md:h-[21px] md:max-w-[597px]"
		>
			<div
				class="absolute left-1/2 top-0 -translate-x-1/2 rounded-b-xl w-[56px] h-[5px] md:w-[96px] md:h-[8px] bg-base"
			></div>
		</div>
	</div>
{:else if type === 'desktop'}
	<div class={className}>
		<div
			class="relative mx-auto border-default bg-base border-[16px] rounded-t-xl h-[172px] max-w-[301px] md:h-[294px] md:max-w-[512px]"
		>
			<div class="overflow-hidden rounded-[calc(0.75rem-16px)] w-full h-full bg-neutral-primary">
				{@render children()}
			</div>
		</div>
		<div
			class="relative mx-auto bg-base rounded-b-xl h-[24px] max-w-[301px] md:h-[42px] md:max-w-[512px]"
		></div>
		<div
			class="relative mx-auto bg-base rounded-b-xl h-[55px] max-w-[83px] md:h-[95px] md:max-w-[142px]"
		></div>
	</div>
{:else if type === 'watch'}
	<div class={className}>
		<div class="relative mx-auto bg-base rounded-t-[2.5rem] h-[63px] max-w-[133px]"></div>
		<div
			class="relative mx-auto bg-base border-default border-[10px] rounded-[2.5rem] h-[213px] w-[208px]"
		>
			<div class="overflow-hidden rounded-[calc(2.5rem-10px)] w-full h-full bg-neutral-primary">
				{@render children()}
			</div>
		</div>
		<div class="relative mx-auto bg-base rounded-b-[2.5rem] h-[63px] max-w-[133px]"></div>
	</div>
{/if}
