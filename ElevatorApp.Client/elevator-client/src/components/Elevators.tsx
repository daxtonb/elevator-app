import React, { ComponentProps, useEffect } from 'react';
import { connect } from 'react-redux';
import { IElevator } from '../utils/data-contracts/IElevator';
import { getElevators, receivedElevatorUpdate } from '../redux/actions/elevator';
import { connection } from '../utils/elevatorHub';
import IState from '../utils/data-contracts/IState';
import { ElevatorState } from '../utils/enums/ElevatorState';
import ChangeHighlight from "react-change-highlight";
import { Direction } from '../utils/enums/Direction';

export interface ElevatorsProps extends ComponentProps<any> {
    elevators: IElevator[];
    getElevators: () => any;
    receivedElevatorUpdate: (elevator: IElevator) => any;
}

/**
 * Displays the current status of the elevators
 * @param props { elevators, getElevators, receievedElevatorUpdate }
 * @returns JSX element
 */
export const Elevators = (props: ElevatorsProps) => {
    const { getElevators, elevators, receivedElevatorUpdate } = props;
    let counter = 100;

    useEffect(() => {
        if (!elevators || !elevators.length) {
            getElevators();
            connection.on('ReceiveElevatorUpdate', (res: IElevator) => {
                console.log(res);
                receivedElevatorUpdate(res);
            });
        }
    }, [getElevators, elevators, receivedElevatorUpdate]);

    return (
        <>
            <h2>Elevators</h2>
            <table>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Floor</th>
                        <th>Status</th>
                        <th>Direction</th>
                        <th>Occupants</th>
                        <th>Capacity</th>
                    </tr>
                </thead>
                <tbody>
                    {elevators.sort((a, b) => a.id - b.id).map((elevator, index) =>
                        <tr key={counter++}>
                            <td key={counter++}><ChangeHighlight><span ref={React.createRef()}>{elevator.id}</span></ChangeHighlight></td>
                            <td key={counter++}><ChangeHighlight><span ref={React.createRef()}>{elevator.currentFloor}</span></ChangeHighlight></td>
                            <td key={counter++}><ChangeHighlight><span ref={React.createRef()}>{getStateText(elevator.state)}</span></ChangeHighlight></td>
                            <td key={counter++}><ChangeHighlight><span ref={React.createRef()}>{getDirectionText(elevator.currentDirection)}</span></ChangeHighlight></td>
                            <td key={counter++}><ChangeHighlight><span ref={React.createRef()}>{elevator.occupantCount}</span></ChangeHighlight></td>
                            <td key={counter++}><ChangeHighlight><span ref={React.createRef()}>{elevator.capacity}%</span></ChangeHighlight></td>
                        </tr>)}
                </tbody>
            </table>
        </>
    );
};

function getStateText(state: ElevatorState): string {
    switch (state) {
        case ElevatorState.doorsClosed:
            return 'Doors Closed';
        case ElevatorState.doorsOpen:
            return 'Doors Open';
        case ElevatorState.moving:
            return 'Moving';
        case ElevatorState.ready:
            return 'Ready';
        default:
            return '';
    }
}

function getDirectionText(direction: Direction): string {
    switch (direction) {
        case Direction.none:
            return 'None';
        case Direction.up:
            return 'Up';
        case Direction.down:
            return 'Down';
        default:
            return '';
    }
}

const mapStateToProps = (state: IState) => ({
    elevators: state.elevator
});

const mapDispatchToProps = {
    getElevators,
    receivedElevatorUpdate
};

export default connect(mapStateToProps, mapDispatchToProps)(Elevators);
