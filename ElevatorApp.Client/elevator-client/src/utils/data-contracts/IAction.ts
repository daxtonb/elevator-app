/**
 * Models an action to be used by Redux reducers
 */
export default interface IAction<T> {
    /**
     * Action type
     */
    type: string;
    /**
     * Data associated with action
     */
    payload?: T;
}