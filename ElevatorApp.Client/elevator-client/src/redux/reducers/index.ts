import { combineReducers } from "redux";
import elevator from "./elevator";
import occupant from './occupant';
import building from './building';

export default combineReducers({
    elevator,
    occupant,
    building
});