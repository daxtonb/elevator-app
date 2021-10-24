import { Direction } from "../enums/Direction";
import { ElevatorState } from "../enums/ElevatorState";

/**
 * Models an elevator inside a building
 */
export interface IElevator {
   /**
    * Elevator unique identifier
    */
   id: number;
   /**
    * State of elevator
    */
   state: ElevatorState;
   /**
    * Total number of occupants in elevator
    */
   occupantCount: number;
   /**
    * Percentage of max weight capacity from the net weight of occupants
    */
   capacity: number;
   /**
    * Floor that the elevator is currently on
    */
   currentFloor: number;
   /**
    * Direction elevator is moving
    */
   currentDirection: Direction;
}