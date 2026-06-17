<script lang="ts">
	interface Props {
		class?: string;
		colspan?: number;
		children: any;
	}

	let { class: className = '', colspan, children }: Props = $props();
</script>

<td {colspan} class="px-6 py-4 {className}">
	{@render children()}
</td>
