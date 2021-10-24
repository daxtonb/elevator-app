import { ThunkDispatch } from 'redux-thunk';
import IOccupant from '../../utils/data-contracts/IOccupant';
import { getUserOccupant } from '../../utils/elevatorHub';
import IAction from '../../utils/data-contracts/IAction';
import { OCCUPANT_GET_SUCCESS } from "./actionTypes";
import handleSignalrError from '../../utils/handleSignalrError';

export const getOccupant = () => async (dispatch: ThunkDispatch<any, any, any>) => {
    try {
        const occupant = await getUserOccupant();

        const action: IAction<IOccupant> = {
            type: OCCUPANT_GET_SUCCESS,
            payload: occupant
        };

        dispatch(action);

    } catch (error) {
        handleSignalrError(error);
    }
};