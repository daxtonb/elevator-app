import { ComponentProps, useEffect } from 'react';
import { connect } from 'react-redux';
import { IElevator } from '../utils/data-contracts/IElevator';
import { getElevators, receivedElevatorUpdate } from '../redux/actions/elevator';
import { connection } from '../utils/elevatorHub';
import IState from '../utils/data-contracts/IState';

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
                        <th>State</th>
                        <th>Direction</th>
                        <th>Capacity</th>
                    </tr>
                </thead>
                <tbody>
                    {elevators.sort((a, b) => a.id - b.id).map((elevator, index) =>
                        <tr key={counter++}>
                            <td key={counter++}>{elevator.id}</td>
                            <td key={counter++}>{elevator.currentFloor}</td>
                            <td key={counter++}>{elevator.state}</td>
                            <td key={counter++}>{elevator.currentDirection}</td>
                            <td key={counter++}>{elevator.capacity}</td>
                        </tr>)}
                </tbody>
            </table>
        </>
    );
};

const mapStateToProps = (state: IState) => ({
    elevators: state.elevator
});

const mapDispatchToProps = {
    getElevators,
    receivedElevatorUpdate
};

export default connect(mapStateToProps, mapDispatchToProps)(Elevators);
