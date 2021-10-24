import IAction from '../../utils/data-contracts/IAction';
import IOccupant from '../../utils/data-contracts/IOccupant';
import { OCCUPANT_GET_SUCCESS } from '../actions/actionTypes';

export default function occupant(
    state: IOccupant | null = null,
    action: IAction<any>
): IOccupant | null {
    const { type, payload } = action;
    switch (type) {
        case OCCUPANT_GET_SUCCESS:
            return payload as IOccupant;
        default:
            return state ?? null;
    }
}