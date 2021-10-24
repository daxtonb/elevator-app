import { ThunkDispatch } from 'redux-thunk';
import IAction from '../../utils/data-contracts/IAction';
import IBuilding from '../../utils/data-contracts/IBuilding';
import { getServerBuilding } from '../../utils/elevatorHub';
import handleSignalrError from '../../utils/handleSignalrError';
import { BUILDING_GET_SUCCESS } from './actionTypes';

export const getBuilding = () => async (dispatch: ThunkDispatch<any, any, any>) => {
    try {
        const building = await getServerBuilding();

        const action: IAction<IBuilding> = {
            type: BUILDING_GET_SUCCESS,
            payload: building
        };

        dispatch(action);
    } catch (error) {
        handleSignalrError(error);
    }
};