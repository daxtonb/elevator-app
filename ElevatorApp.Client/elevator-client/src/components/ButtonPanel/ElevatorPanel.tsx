import React, { ComponentProps } from 'react';
import { IElevator } from '../../utils/data-contracts/IElevator';
import { requestElevatorToFloor } from '../../utils/elevatorHub';
interface ElevatorPanelProps extends ComponentProps<any> {
    floorCount: number;
    elevator?: IElevator;
}

/**
 * Represents the button panel from inside the elevator
 * @param props { floorCount }
 * @returns JSX element
 */
function ElevatorPanel(props: ElevatorPanelProps) {
    const { floorCount, elevator } = props;

    // Highlight buttons for floors with active requests
    const getButtonStyle = (floorNumber: number): React.CSSProperties | undefined => {
        if (elevator && elevator.activeRequests.findIndex(r => r.floorNumber === floorNumber) >= 0) {
            return { backgroundColor: 'yellow' };
        }

        return undefined;
    };

    const onButtonClick = async (floorNumber: number) => {
        if (elevator) {
            elevator.activeRequests.push({ floorNumber: floorNumber });
        }

        await requestElevatorToFloor(floorNumber);
    };

    const floorNumbers = [];
    for (let i = 1; i <= floorCount; i++) {
        floorNumbers.push(i);
    }

    return (
        <div>
            {floorNumbers.map(floor => <button key={floor} style={getButtonStyle(floor)} onClick={() => onButtonClick(floor)}>{floor}</button>)}
        </div>
    );
}

export default ElevatorPanel;
