import IBuilding from './IBuilding';
import { IElevator } from './IElevator';
import IOccupant from './IOccupant';

/**
 * Models the state for the application's Redux store
 */
export default interface IState {
    elevator: IElevator[];
    occupant: IOccupant | null;
    building: IBuilding | null;
}