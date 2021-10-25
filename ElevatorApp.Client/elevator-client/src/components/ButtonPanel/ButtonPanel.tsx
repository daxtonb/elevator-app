import { ComponentProps, useEffect } from 'react';
import { connect } from 'react-redux';
import IOccupant from '../../utils/data-contracts/IOccupant';
import IState from '../../utils/data-contracts/IState';
import { getOccupant } from '../../redux/actions/occupant';
import IBuilding from '../../utils/data-contracts/IBuilding';
import { getBuilding } from '../../redux/actions/building';
import { OccupantState } from '../../utils/enums/OccupantState';
import ElevatorPanel from './ElevatorPanel';
import FloorPanel from './FloorPanel';
import { IElevator } from '../../utils/data-contracts/IElevator';

export interface ButtonPanelProps extends ComponentProps<any> {
    occupant: IOccupant | null;
    building: IBuilding | null;
    elevators: IElevator[];
    getOccupant: () => any;
    getBuilding: () => any;
}

/**
 * Displays the appropriate buttons based on where the occupant is
 * @param props { occupant, building, getOccupant, getBuilding }
 * @returns JSX element
 */
export const ButtonPanel = (props: ButtonPanelProps) => {
    const { getOccupant, occupant, getBuilding, building, elevators } = props;
    let elevator;

    if (occupant && occupant.elevatorId) {
        elevator = elevators.find(e => e.id === occupant.elevatorId);
    }

    useEffect(() => {
        if (!occupant) {
            getOccupant();
        }
        if (!building) {
            getBuilding();
        }
    }, [getOccupant, occupant, getBuilding, building]);

    if (occupant && building) {
        return (
            <div>
                {occupant.currentState !== OccupantState.waiting && <h2>Make a selection:</h2>}
                {occupant.currentState === OccupantState.riding && <ElevatorPanel floorCount={building.floorCount} elevator={elevator} />}
                {occupant.currentState === OccupantState.none && <FloorPanel />}
            </div>
        );
    }

    return (<div>loading...</div>);
};

const mapStateToProps = (state: IState) => ({
    occupant: state.occupant,
    building: state.building,
    elevators: state.elevator
});

const mapDispatchToProps = {
    getOccupant,
    getBuilding,
};


export default connect(mapStateToProps, mapDispatchToProps)(ButtonPanel);
