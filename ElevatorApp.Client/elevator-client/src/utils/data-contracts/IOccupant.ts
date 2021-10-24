import { OccupantState } from "../enums/OccupantState";

export default interface IOccupant
{
    id: number;
    currentFloor: number;
    requestedFloor: number;
    currentState: OccupantState;
}