<script lang="ts">
	import Icon from '@iconify/svelte';

	interface Props {
		icon: string;
		class?: string;
		inline?: boolean;
		width?: string | number;
		height?: string | number;
		color?: string;
		[key: string]: any;
	}

	let {
		icon,
		class: className = 'w-5 h-5',
		inline = false,
		width,
		height,
		color,
		...rest
	}: Props = $props();
</script>

<Icon {icon} class={className} {inline} {width} {height} {color} {...rest} />
