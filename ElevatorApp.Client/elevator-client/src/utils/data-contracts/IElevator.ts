import { Direction } from "../enums/Direction";
import { ElevatorState } from "../enums/ElevatorState";

export interface IElevator {
   id: number;
   state: ElevatorState;
   occupantCount: number;
   capacity: number;
   currentFloor: number;
   currentDirection: Direction;
}