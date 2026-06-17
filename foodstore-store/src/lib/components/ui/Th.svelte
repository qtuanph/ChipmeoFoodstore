<script lang="ts">
	interface Props {
		class?: string;
		colspan?: number;
		children: any;
	}

	let { class: className = '', colspan, children }: Props = $props();
</script>

<th scope="col" {colspan} class="px-6 py-3 font-medium {className}">
	{@render children()}
</th>
