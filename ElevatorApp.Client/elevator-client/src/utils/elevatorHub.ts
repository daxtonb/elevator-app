import { HubConnectionBuilder } from '@microsoft/signalr';
import IBuilding from './data-contracts/IBuilding';
import { IElevator } from './data-contracts/IElevator';
import IOccupant from './data-contracts/IOccupant';
import { Direction } from './enums/Direction';
import handleSignalrError from './handleSignalrError';

/**
 * SignalR connection to server
 */
export const connection = new HubConnectionBuilder()
    .withUrl('http://localhost:5000/ElevatorHub')
    .withAutomaticReconnect()
    .build();

/**
 * 
 * @returns Retrieves all elevators from the server's building
 */
export async function getAllElevators(): Promise<IElevator[]> {
    try {
        return await connection.invoke<IElevator[]>('RequestElevators');
    } catch (error) {
        handleSignalrError(error);
    }

    return [];
}

/**
 * Sends an elevator request for the building to dispatch
 * @param direction Up or down
 */
export async function requestElevatorByDirection(direction: Direction): Promise<void> {
    try {
        return await connection.invoke<void>('RequestElevatorAsync', direction);
    } catch (error) {
        handleSignalrError(error);
    }
}

/**
 * Retrieves the occupant represented by the current session
 * @returns Occupant
 */
export async function getUserOccupant(): Promise<IOccupant> {
    try {
        return await connection.invoke<IOccupant>('RequestOccupant');
    } catch (error) {
        handleSignalrError(error);
    }

    throw new Error('Occupant could not be retrieved from server');

}

/**
 * Retrieves the building represented on the server
 * @returns Building
 */
export async function getServerBuilding(): Promise<IBuilding> {
    try {
        return await connection.invoke<IBuilding>('RequestBuilding');
    } catch (error) {
        handleSignalrError(error);
    }

    throw new Error('Building could not be retrieved from server');
}