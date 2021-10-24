import IAction from '../../utils/data-contracts/IAction';
import IBuilding from '../../utils/data-contracts/IBuilding';
import { BUILDING_GET_SUCCESS } from '../actions/actionTypes';

export default function building(
    state: IBuilding | null = null,
    action: IAction<any>
): IBuilding | null {
    const { type, payload } = action;
    switch (type) {
        case BUILDING_GET_SUCCESS:
            return payload as IBuilding;
        default:
            return state ?? null;
    }
}