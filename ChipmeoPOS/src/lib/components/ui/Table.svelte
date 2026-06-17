<script lang="ts">
	interface Props {
		class?: string;
		children: any;
	}

	let { class: className = '', children }: Props = $props();
</script>

<div
	class="relative overflow-x-auto bg-neutral-primary-soft shadow-xs rounded-base border border-default {className}"
>
	<table class="w-full text-sm text-left rtl:text-right text-body">
		{@render children()}
	</table>
</div>
