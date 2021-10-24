import { OccupantState } from "../enums/OccupantState";

/**
 * Models an occupant that can ride an elevator
 */
export default interface IOccupant {
    id: number;
    currentFloor: number;
    requestedFloor: number;
    currentState: OccupantState;
    weight: number;
    elevatorId: number;
}