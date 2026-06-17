import Image from '@tiptap/extension-image';
import { mergeAttributes } from '@tiptap/core';

export const CustomImage = Image.extend({
	name: 'customImage',

	addAttributes() {
		return {
			...this.parent?.(),
			width: {
				default: '100%',
				renderHTML: (attributes) => ({
					width: attributes.width
				})
			},
			caption: {
				default: '',
				renderHTML: (attributes) => ({
					'data-caption': attributes.caption
				})
			},
			align: {
				default: 'center',
				renderHTML: (attributes) => {
					const align = attributes.align || 'center';
					const styles = [];
					const classes = ['rounded-lg', 'shadow-sm']; // Default nice styles

					if (align === 'center') {
						styles.push('display: block', 'margin-left: auto', 'margin-right: auto');
						classes.push('mx-auto', 'block');
					} else if (align === 'right') {
						styles.push('float: right', 'margin-left: 1rem');
						classes.push('float-right', 'ml-4');
					} else {
						styles.push('float: left', 'margin-right: 1rem');
						classes.push('float-left', 'mr-4');
					}

					return {
						'data-align': align,
						style: styles.join('; '),
						class: classes.join(' ')
					};
				}
			},
			href: {
				default: '',
				renderHTML: (attributes) => ({
					'data-href': attributes.href || ''
				})
			}
		};
	},

	parseHTML() {
		return [
			{
				tag: 'figure',
				getAttrs: (node) => {
					if (typeof node === 'string') return {};
					const element = node as HTMLElement;
					const img = element.querySelector('img');

					// If figure doesn't contain an image, ignore it
					if (!img) return false;

					const figcaption = element.querySelector('figcaption');
					const link = element.querySelector('a'); // Check if img is wrapped in a

					return {
						src: img.getAttribute('src'),
						alt: img.getAttribute('alt'),
						width: img.style.width || img.getAttribute('width'),
						caption: figcaption ? figcaption.textContent : '',
						href: link ? link.getAttribute('href') : null,
						align: element.getAttribute('data-align') || element.style.textAlign || 'center'
					};
				}
			},
			{
				tag: 'img',
				getAttrs: (node) => {
					if (typeof node === 'string') return {};
					const element = node as HTMLElement;
					return {
						src: element.getAttribute('src'),
						alt: element.getAttribute('alt'),
						width: element.style.width || element.getAttribute('width'),
						caption: element.getAttribute('data-caption'),
						href: element.getAttribute('data-href')
					};
				}
			}
		];
	},

	// Override renderHTML to output figure with figcaption for captions
	renderHTML({ HTMLAttributes, node }) {
		const { caption, href, ...imgAttrs } = HTMLAttributes;

		// Build figure wrapper styles based on alignment
		const align = node.attrs.align || 'center';
		let figureStyle = 'margin: 1.5rem 0;';
		if (align === 'center') {
			figureStyle += ' text-align: center;';
		} else if (align === 'right') {
			figureStyle += ' text-align: right;';
		} else {
			figureStyle += ' text-align: left;';
		}

		// Create img element
		const imgElement: any = [
			'img',
			mergeAttributes(imgAttrs, {
				style:
					align === 'center'
						? 'display: block; margin: 0 auto; max-width: 100%;'
						: 'max-width: 100%;',
				class: 'rounded-lg shadow-sm'
			})
		];

		// Wrap in anchor if href exists
		let imageContent = imgElement;
		if (node.attrs.href) {
			imageContent = [
				'a',
				{ href: node.attrs.href, target: '_blank', rel: 'noopener noreferrer' },
				imgElement
			];
		}

		// If caption exists, wrap in figure
		if (node.attrs.caption) {
			return [
				'figure',
				{ style: figureStyle, class: 'image-figure' },
				imageContent,
				[
					'figcaption',
					{
						style:
							'text-align: center; font-size: 0.875rem; color: #6b7280; font-style: italic; margin-top: 0.5rem;'
					},
					node.attrs.caption
				]
			];
		}

		// No caption - just return image (wrapped in div for alignment)
		return ['div', { style: figureStyle }, imageContent];
	},

	addNodeView() {
		return ({ node, getPos, editor }) => {
			const container = document.createElement('div');

			const getJustify = (align: string) => {
				if (align === 'left') return 'justify-start';
				if (align === 'right') return 'justify-end';
				return 'justify-center';
			};

			container.className = `image-resizer-container group relative flex flex-col items-center ${getJustify(node.attrs.align || 'center')} w-full my-6`;

			// Image wrapper to handle resize constraints
			const imgWrapper = document.createElement('div');
			imgWrapper.className = 'relative inline-block';
			imgWrapper.style.width = node.attrs.width || '100%';
			imgWrapper.style.transition = 'width 0.1s ease'; // Smooth resize

			const img = document.createElement('img');
			img.src = node.attrs.src;
			img.alt = node.attrs.alt || '';
			img.className = 'rounded-lg border border-gray-100 shadow-sm block w-full h-auto';

			// Resize Handle (Bottom Right)
			const handle = document.createElement('div');
			handle.className =
				'absolute bottom-2 right-2 w-4 h-4 bg-indigo-600 border-2 border-white rounded-full cursor-nwse-resize opacity-0 group-hover:opacity-100 transition-opacity z-10 shadow-md';

			// Settings Button (Gear Icon - Middle Right)
			const settingsBtn = document.createElement('button');
			settingsBtn.type = 'button';
			settingsBtn.className =
				'absolute top-1/2 -translate-y-1/2 right-2 w-8 h-8 bg-white/90 hover:bg-white border border-gray-200 rounded-lg flex items-center justify-center cursor-pointer opacity-0 group-hover:opacity-100 transition-all z-20 shadow-lg hover:shadow-xl hover:scale-110';
			settingsBtn.title = 'Cài đặt ảnh';
			settingsBtn.innerHTML = `
                <svg class="w-4 h-4 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"/>
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/>
                </svg>
            `;

			// Settings button click handler - dispatch custom event
			settingsBtn.addEventListener('click', (e) => {
				e.preventDefault();
				e.stopPropagation();
				const pos = getPos();
				if (typeof pos === 'number') {
					// Dispatch custom event with node position and current attrs
					const event = new CustomEvent('open-image-settings', {
						bubbles: true,
						detail: {
							pos,
							href: node.attrs.href || '',
							caption: node.attrs.caption || ''
						}
					});
					container.dispatchEvent(event);
				}
			});

			// Caption Element (Word-style, centered below image)
			const captionEl = document.createElement('div');
			captionEl.className = 'text-center text-sm text-gray-500 italic mt-0.5 max-w-full';
			captionEl.style.display = node.attrs.caption ? 'block' : 'none';
			captionEl.textContent = node.attrs.caption || '';

			// Append Elements
			imgWrapper.appendChild(img);
			imgWrapper.appendChild(handle);
			imgWrapper.appendChild(settingsBtn);
			container.appendChild(imgWrapper);
			container.appendChild(captionEl);

			// --- Logic ---

			// Resizing Logic

			// 2. Resizing Logic
			let startX = 0;
			let startWidth = 0;
			let isResizing = false;

			const onMouseMove = (e: MouseEvent) => {
				if (!isResizing) return;
				const currentX = e.clientX;
				const diffX = currentX - startX;

				// Calculate new width relative to container or pixels?
				// Simple pixel width is easier for users "Like Word"
				// But responsiveness prefers %.
				// Let's stick to pixel width for the inline style, maybe convert to % on save?
				// For now, pixels on wrapper.
				const newWidth = Math.max(100, startWidth + diffX);
				imgWrapper.style.width = `${newWidth}px`;
			};

			const onMouseUp = () => {
				if (!isResizing) return;
				isResizing = false;
				document.removeEventListener('mousemove', onMouseMove);
				document.removeEventListener('mouseup', onMouseUp);

				// Save final width to attributes
				const pos = getPos();
				if (typeof pos === 'number') {
					// Save as pixel string
					editor.view.dispatch(
						editor.state.tr.setNodeMarkup(pos, undefined, {
							...node.attrs,
							width: imgWrapper.style.width
						})
					);
				}

				// Re-enable editor interaction if needed
			};

			handle.addEventListener('mousedown', (e) => {
				e.preventDefault(); // Prevent text selection
				isResizing = true;
				startX = e.clientX;
				startWidth = imgWrapper.offsetWidth;

				document.addEventListener('mousemove', onMouseMove);
				document.addEventListener('mouseup', onMouseUp);
			});

			return {
				dom: container,
				update: (updatedNode) => {
					if (updatedNode.type.name !== CustomImage.name) return false;
					// Update image attrs if changed externally
					if (updatedNode.attrs.src !== node.attrs.src) img.src = updatedNode.attrs.src;
					if (updatedNode.attrs.width !== node.attrs.width)
						imgWrapper.style.width = updatedNode.attrs.width;

					if (updatedNode.attrs.align !== node.attrs.align) {
						container.className = `image-resizer-container group relative flex flex-col items-center ${getJustify(updatedNode.attrs.align)} w-full my-6`;
					}

					// Update caption
					if (updatedNode.attrs.caption !== node.attrs.caption) {
						captionEl.textContent = updatedNode.attrs.caption || '';
						captionEl.style.display = updatedNode.attrs.caption ? 'block' : 'none';
					}

					node = updatedNode; // Update internal ref
					return true;
				},
				selectNode: () => {
					img.classList.add('ring-2', 'ring-indigo-500');
					handle.style.opacity = '1';
				},
				deselectNode: () => {
					img.classList.remove('ring-2', 'ring-indigo-500');
					handle.style.opacity = '';
				}
			};
		};
	}
});
