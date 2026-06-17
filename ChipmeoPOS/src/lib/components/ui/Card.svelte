<script lang="ts">
	interface Props {
		class?: string;
		hover?: boolean;
		children: any;
	}

	let { class: className = '', hover = false, children }: Props = $props();
</script>

<div
	class="bg-neutral-primary-soft border border-default rounded-base shadow-xs {hover
		? 'hover:shadow-md transition-shadow'
		: ''} {className}"
>
	{@render children()}
</div>
