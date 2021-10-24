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

export interface ButtonPanelProps extends ComponentProps<any> {
    occupant: IOccupant | null;
    building: IBuilding | null;
    getOccupant: () => any;
    getBuilding: () => any;
}

export const ButtonPanel = (props: ButtonPanelProps) => {
    const { getOccupant, occupant, getBuilding, building } = props;

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
                {occupant.currentState === OccupantState.riding && <ElevatorPanel floorCount={building.floorCount} />}
                {occupant.currentState === OccupantState.none && <FloorPanel />}
            </div>
        );
    }

    return (<div>loading...</div>);
};

const mapStateToProps = (state: IState) => ({
    occupant: state.occupant,
    building: state.building
});

const mapDispatchToProps = {
    getOccupant,
    getBuilding,
};


export default connect(mapStateToProps, mapDispatchToProps)(ButtonPanel);
