<script lang="ts">
	interface Props {
		type?: string;
		value?: string;
		label?: string;
		disabled?: boolean;
		class?: string;
	}

	let {
		type = 'text',
		value = $bindable(''),
		label = '',
		disabled = false,
		class: className = ''
	}: Props = $props();
</script>

<div class="relative {className}">
	<input
		{type}
		bind:value
		{disabled}
		id="floating-label-{Math.random().toString(36).slice(2, 7)}"
		class="block px-3 py-2.5 w-full text-sm text-heading bg-transparent rounded-base border border-default-medium appearance-none focus:outline-none focus:ring-0 focus:border-brand peer {disabled
			? 'opacity-50'
			: ''}"
		placeholder=" "
	/>
	<label
		for="floating-label-..."
		class="absolute text-sm text-body duration-300 transform -translate-y-4 scale-75 top-2 z-10 origin-[0] bg-neutral-primary-soft px-2 peer-focus:px-2 peer-focus:text-brand peer-placeholder-shown:scale-100 peer-placeholder-shown:-translate-y-1/2 peer-placeholder-shown:top-1/2 peer-focus:top-2 peer-focus:scale-75 peer-focus:-translate-y-4 start-1 rtl:peer-focus:translate-x-1/4 rtl:peer-focus:left-auto"
		>{label}</label
	>
</div>
