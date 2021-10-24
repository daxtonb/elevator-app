import IAction from "../../utils/data-contracts/IAction";
import { IElevator } from "../../utils/data-contracts/IElevator";
import { ELEVATORS_GET_SUCCESS } from "../actions/actionTypes";

const initalState: IElevator[] = [];

export default function elevator(
    state: IElevator[] = initalState,
    action: IAction<any>
): IElevator[] {
    const { type, payload } = action;
    switch (type) {
        case ELEVATORS_GET_SUCCESS:
            return [...(payload as IElevator[])];
        default:
            return state;
    }
}