import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr'
import type { NotificationEvents } from '@/services/notifications/notification.events.ts'
import { baseUrl } from '../core/api';

let connection: HubConnection | null = null;

function getConnection(){
  if(!connection){
    connection = new HubConnectionBuilder()
      .withUrl(`${baseUrl}/notifications`)
      .withAutomaticReconnect()
      .build();
  }

  return connection;
}

export async function startConnection() {
  const connection = getConnection();

  if(connection.state === HubConnectionState.Disconnected){
    await connection.start();
  }

}

export function onEvent<K extends keyof NotificationEvents>(
  eventName: K,
  callback: (...args: NotificationEvents[K]) => void,
) {
  getConnection().on(eventName, callback)
}
