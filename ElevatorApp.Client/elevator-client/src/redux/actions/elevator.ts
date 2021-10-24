import { ThunkDispatch } from 'redux-thunk';
import IAction from '../../utils/data-contracts/IAction';
import { IElevator } from '../../utils/data-contracts/IElevator';
import handleSignalrError from '../../utils/handleSignalrError';
import { ELEVATORS_GET_SUCCESS, ELEVATOR_REQUEST_SUCCESS, ELEVATOR_UPDATED } from './actionTypes';
import { getAllElevators, requestElevatorByDirection } from "../../utils/elevatorHub";
import { Direction } from '../../utils/enums/Direction';

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

export const requestElevator = (direction: Direction) => async (dispatch: ThunkDispatch<any, any, any>) => {
    try {
        await requestElevatorByDirection(direction);

        const action: IAction<null> = {
            type: ELEVATOR_REQUEST_SUCCESS,
            payload: null
        };

        dispatch(action);
    } catch (error) {
        handleSignalrError(error);
    }
};

export const receivedElevatorUpdate = (elevator: IElevator) => (dispatch: ThunkDispatch<any, any, any>) => {
    try {
        const action: IAction<IElevator> = {
            type: ELEVATOR_UPDATED,
            payload: elevator
        };

        dispatch(action);
    } catch (error) {
        handleSignalrError(error);
    }
};


