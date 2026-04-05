import * as signalR from "@microsoft/signalr";
import { authStore } from "../store/authStore";

let connection: signalR.HubConnection | null = null;

export const startOrderHub = async (tenantId: string) => {
  const user = authStore.getUser();
  if (!user) return;

  connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5226/hubs/orders")
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();

  await connection.start();
  await connection.invoke("JoinTenantGroup", tenantId);
  return connection;
};

export const stopOrderHub = async () => {
  if (connection) {
    await connection.stop();
    connection = null;
  }
};
