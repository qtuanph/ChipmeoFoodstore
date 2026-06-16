<script lang="ts">
	interface Props {
		padding?: boolean;
		hover?: boolean;
		children: any;
	}

	let { padding = true, hover = false, children }: Props = $props();
</script>

<div
	class="
		rounded-xl border border-gray-200 bg-white shadow-sm
		{padding ? 'p-6' : ''}
		{hover ? 'transition-shadow duration-200 hover:shadow-md' : ''}
	"
>
	{@render children()}
</div>
