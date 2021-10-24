import { HubConnectionBuilder } from '@microsoft/signalr';
import { IElevator } from './data-contracts/IElevator';
import handleSignalrError from './handleSignalrError';

export const connection = new HubConnectionBuilder()
    .withUrl('http://localhost:5000/ElevatorHub')
    .withAutomaticReconnect()
    .build();

export async function getAllElevators(): Promise<IElevator[]> {
    try {
        return await connection.invoke<IElevator[]>('RequestElevators');
    } catch (error) {
        handleSignalrError(error);
    }

    return [];
}