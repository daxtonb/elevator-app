import { ThunkDispatch } from 'redux-thunk';
import IOccupant from '../../utils/data-contracts/IOccupant';
import { getUserOccupant } from '../../utils/elevatorHub';
import IAction from '../../utils/data-contracts/IAction';
import { OCCUPANT_GET_SUCCESS, OCCUPANT_UPDATED } from "./actionTypes";
import handleSignalrError from '../../utils/handleSignalrError';

/**
 * Retrieves occupant representing the user of this session
 * @returns Occupant
 */
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

/**
 * Dispatches Redux to update the respective occupant in the store
 * @param occupant Occupant to be updated
 */
export const updateOccupant = (occupant: IOccupant) => (dispatch: ThunkDispatch<any, any, any>) => {
    try {
        const action: IAction<IOccupant> = {
            type: OCCUPANT_UPDATED,
            payload: occupant
        };

        dispatch(action);

    } catch (error) {
        handleSignalrError(error);
    }
};