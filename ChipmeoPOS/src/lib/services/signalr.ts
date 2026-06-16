import * as signalR from '@microsoft/signalr';
import { API_BASE_URL } from '$lib/api/index.ts';

class SignalRService {
	private connection: signalR.HubConnection | null = null;
	private static instance: SignalRService;
	private eventHandlers: Map<string, Function[]> = new Map();

	private constructor() {}

	public static getInstance(): SignalRService {
		if (!SignalRService.instance) {
			SignalRService.instance = new SignalRService();
		}
		return SignalRService.instance;
	}

	public async startConnection(token: string): Promise<void> {
		if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
			return;
		}

		const hubUrl = API_BASE_URL + '/hubs/app';

		this.connection = new signalR.HubConnectionBuilder()
			.withUrl(hubUrl, {
				accessTokenFactory: () => token,
				skipNegotiation: true,
				transport: signalR.HttpTransportType.WebSockets
			})
			.withAutomaticReconnect()
			.build();

		this.connection.onclose((error) => {
			console.warn('SignalR Connection closed', error);
		});

		this.connection.onreconnecting((error) => {
			console.warn('SignalR Reconnecting...', error);
		});

		this.connection.onreconnected((connectionId) => {
			console.log('SignalR Reconnected', connectionId);
		});

		try {
			await this.connection.start();
			console.log('SignalR Connected');
			this.registerEvents();
		} catch (err) {
			console.error('SignalR Connection Error: ', err);
		}
	}

	private registerEvents() {
		if (!this.connection) return;
	}

	public on(eventName: string, callback: (...args: any[]) => void) {
		if (!this.eventHandlers.has(eventName)) {
			this.eventHandlers.set(eventName, []);
			if (this.connection) {
				this.connection.on(eventName, (...args) => {
					this.trigger(eventName, ...args);
				});
			}
		}
		this.eventHandlers.get(eventName)?.push(callback);

		if (this.connection) {
			this.connection.off(eventName);
			this.connection.on(eventName, (...args) => {
				this.trigger(eventName, ...args);
			});
		}
	}

	public off(eventName: string, callback: Function) {
		const handlers = this.eventHandlers.get(eventName);
		if (handlers) {
			const index = handlers.indexOf(callback);
			if (index !== -1) {
				handlers.splice(index, 1);
			}
		}
	}

	private trigger(eventName: string, ...args: any[]) {
		const handlers = this.eventHandlers.get(eventName);
		if (handlers) {
			handlers.forEach((callback) => callback(...args));
		}
	}

	public async stopConnection() {
		if (this.connection) {
			await this.connection.stop();
			this.connection = null;
		}
	}
}

export const signalRService = SignalRService.getInstance();
