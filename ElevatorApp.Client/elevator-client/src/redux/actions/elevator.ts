import { ThunkDispatch } from 'redux-thunk';
import IAction from '../../utils/data-contracts/IAction';
import { IElevator } from '../../utils/data-contracts/IElevator';
import handleSignalrError from '../../utils/handleSignalrError';
import { ELEVATORS_GET_SUCCESS } from './actionTypes';
import { getAllElevators } from "../../utils/elevatorHub";

export const getElevators = () => async (dispatch: ThunkDispatch<any, any, any>) => {
    try {
        const elevators = await getAllElevators();

        const action: IAction<IElevator[]> = {
            type: ELEVATORS_GET_SUCCESS,
            payload: elevators
        };

        dispatch(action);

    } catch (error) {
        handleSignalrError(error);
    }
};


