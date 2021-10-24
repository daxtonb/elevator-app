import React, { ComponentProps, useEffect, useState } from 'react';
import { connect } from 'react-redux';
import IReduxStore from '../utils/data-contracts/IReduxStore';
import { IElevator } from '../utils/data-contracts/IElevator';
import { getElevators } from '../redux/actions/elevator';
import { throws } from 'assert';

export interface ElevatorsProps extends ComponentProps<any> {
    elevators: IElevator[];
    getElevators: () => any;
}

export const Elevators = (props: ElevatorsProps) => {
    const { getElevators, elevators } = props;


    useEffect(() => {
        if (!elevators || !elevators.length) {
            getElevators();
        }
    }, [getElevators, elevators]);
    return (
        <>
            <h2>Elevators</h2>
            <table>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Floor</th>
                        <th>State</th>
                        <th>Direction</th>
                        <th>Capacity</th>
                    </tr>
                </thead>
                <tbody>
                    {elevators.map(elevator =>
                        <tr>
                            <td>{elevator.id}</td>
                            <td>{elevator.currentFloor}</td>
                            <td>{elevator.state}</td>
                            <td>{elevator.currentDirection}</td>
                            <td>{elevator.capacity}</td>
                        </tr>)}
                </tbody>
            </table>
        </>
    );
};

const mapStateToProps = (state: IReduxStore) => ({
    elevators: state.elevator
});

const mapDispatchToProps = {
    getElevators
};

export default connect(mapStateToProps, mapDispatchToProps)(Elevators);
