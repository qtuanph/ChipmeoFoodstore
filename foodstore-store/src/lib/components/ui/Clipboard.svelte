<script lang="ts">
	interface Props {
		targetId: string;
		value: string;
		children: any;
	}

	let { targetId, value, children }: Props = $props();
</script>

<div class="relative">
	<input id={targetId} type="text" class="sr-only" {value} readonly disabled />
	{@render children()}
</div>
