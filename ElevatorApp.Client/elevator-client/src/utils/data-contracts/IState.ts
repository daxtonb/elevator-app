import IBuilding from './IBuilding';
import { IElevator } from './IElevator';
import IOccupant from './IOccupant';

export default interface IState {
    elevator: IElevator[];
    occupant?: IOccupant;
    building?: IBuilding;
}