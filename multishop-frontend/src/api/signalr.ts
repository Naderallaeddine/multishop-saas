import * as signalR from "@microsoft/signalr";
import { authStore } from "../store/authStore";

export const startOrderHub = async (
  tenantId: string
): Promise<signalR.HubConnection | null> => {
  const user = authStore.getUser();
  if (!user) return null;

  const conn = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5226/hubs/orders", {
      accessTokenFactory: () => user.token,
    })
    .withAutomaticReconnect()
    .build();

  await conn.start();
  await conn.invoke("JoinTenantGroup", tenantId);
  return conn;
};

export const stopOrderHub = async (
  conn: signalR.HubConnection | null | undefined
) => {
  if (conn) {
    await conn.stop();
  }
};
