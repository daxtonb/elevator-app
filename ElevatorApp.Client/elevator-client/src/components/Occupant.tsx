import React, { ComponentProps, useEffect } from 'react';
import { connect } from 'react-redux';
import IOccupant from '../utils/data-contracts/IOccupant';
import IState from "../utils/data-contracts/IState";
import { getOccupant, updateOccupant } from '../redux/actions/occupant';
import { connection } from '../utils/elevatorHub';
import { OccupantState } from '../utils/enums/OccupantState';
import ChangeHighlight from "react-change-highlight";
export interface ElevatorsProps extends ComponentProps<any> {
    occupant: IOccupant | null;
    getOccupant: () => any;
    updateOccupant: (occupant: IOccupant) => any;
}

/**
 * Displays occupant's current data
 * @param props { occupant, getOccupant }
 * @returns JSX Element
 */
export const Occupant = (props: ElevatorsProps) => {
    const { getOccupant, occupant, updateOccupant } = props;

    useEffect(() => {
        if (!occupant) {
            getOccupant();
            connection.on('OccupantUpdated', (occupant: IOccupant) => updateOccupant(occupant));
        }
    }, [getOccupant, occupant, updateOccupant]);

    return (
        <>
            {occupant && (<>
                <h2>You</h2>
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Current Floor</th>
                            <th>Status</th>
                            <th>Requested Floor</th>
                            <th>Location</th>
                            <th>Weight</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td><ChangeHighlight><span ref={React.createRef()}>{occupant.id}</span></ChangeHighlight></td>
                            <td><ChangeHighlight><span ref={React.createRef()}>{occupant.currentFloor}</span></ChangeHighlight></td>
                            <td><ChangeHighlight><span ref={React.createRef()}>{getStateText(occupant.currentState)}</span></ChangeHighlight></td>
                            <td><ChangeHighlight><span ref={React.createRef()}>{occupant.requestedFloor}</span></ChangeHighlight></td>
                            <td><ChangeHighlight><span ref={React.createRef()}>{occupant.elevatorId ? `Elevator ${occupant.elevatorId}` : 'Floor'}</span></ChangeHighlight></td>
                            <td><ChangeHighlight><span ref={React.createRef()}>{occupant.weight}</span></ChangeHighlight></td>
                        </tr>
                    </tbody>
                </table></>)}
            {!occupant && <div>loading...</div>}
        </>
    );
};

function getStateText(state: OccupantState): string {
    switch (state) {
        case OccupantState.none:
            return 'None';
        case OccupantState.riding:
            return 'Riding';
        case OccupantState.waiting:
            return 'waiting';
        default:
            return '';
    }
}

const mapStateToProps = (state: IState) => ({
    occupant: state.occupant
});

const mapDispatchToProps = {
    getOccupant,
    updateOccupant
};

export default connect(mapStateToProps, mapDispatchToProps)(Occupant);
