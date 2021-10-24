import React, { ComponentProps, useEffect } from 'react';
import { connect } from 'react-redux';
import IOccupant from '../utils/data-contracts/IOccupant';
import IState from "../utils/data-contracts/IState";
import { getOccupant } from '../redux/actions/occupant';
export interface ElevatorsProps extends ComponentProps<any> {
    occupant: IOccupant | null;
    getOccupant: () => any;
}

export const Occupant = (props: ElevatorsProps) => {
    const { getOccupant, occupant } = props;

    useEffect(() => {
        if (!occupant) {
            getOccupant();
        }
    }, [getOccupant, occupant]);

    return (
        <>
            {occupant && (<>
                <h2>You</h2>
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Status</th>
                            <th>Current Floor</th>
                            <th>Requested Floor</th>
                            <th>Weight</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>{occupant.id}</td>
                            <td>{occupant.currentState}</td>
                            <td>{occupant.currentFloor}</td>
                            <td>{occupant.requestedFloor}</td>
                        </tr>
                    </tbody>
                </table></>)}
            {!occupant && <div>loading...</div>}
        </>
    );
};

const mapStateToProps = (state: IState) => ({
    occupant: state.occupant
});

const mapDispatchToProps = {
    getOccupant
};

export default connect(mapStateToProps, mapDispatchToProps)(Occupant);
