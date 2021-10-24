import React, { ComponentProps } from 'react';
import { connect } from 'react-redux';
import IBuilding from '../../utils/data-contracts/IBuilding';
import IOccupant from '../../utils/data-contracts/IOccupant';
import IState from '../../utils/data-contracts/IState';
import { Direction } from '../../utils/enums/Direction';
import { requestElevator } from '../../redux/actions/elevator';

export interface FloorPanelProps extends ComponentProps<any> {
    building: IBuilding | null;
    occupant: IOccupant | null;
    requestElevator: (direction: Direction) => any;
}

/**
 * Reperesents the button panel for requesting an elevator on a building floor
 * @param props { building, occupant, requestElevator }
 * @returns JSX Element
 */
export const FloorPanel = (props: FloorPanelProps) => {
    const { building, occupant, requestElevator } = props;


    if (building && occupant) {

        return (
            <div>
                {occupant.currentFloor !== building.floorCount && <button onClick={() => requestElevator(Direction.up)}>up</button>}
                {occupant.currentFloor !== 1 && <button onClick={() => requestElevator(Direction.down)}>down</button>}
            </div>
        );
    }

    return (<div>loading...</div>);
};

const mapStateToProps = (state: IState) => ({
    building: state.building,
    occupant: state.occupant
});

const mapDispatchToProps = {
    requestElevator
};


export default connect(mapStateToProps, mapDispatchToProps)(FloorPanel);
