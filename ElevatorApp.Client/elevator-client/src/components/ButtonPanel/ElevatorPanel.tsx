import { ComponentProps } from 'react';
import { requestElevatorToFloor } from '../../utils/elevatorHub';
interface ElevatorPanelProps extends ComponentProps<any> {
    floorCount: number;
}

/**
 * Represents the button panel from inside the elevator
 * @param props { floorCount }
 * @returns JSX element
 */
function ElevatorPanel(props: ElevatorPanelProps) {
    const { floorCount } = props;

    const floorNumbers = [];
    for (let i = 1; i <= floorCount; i++) {
        floorNumbers.push(i);
    }

    return (
        <div>
            {floorNumbers.map(floor => <button key={floor} onClick={() => requestElevatorToFloor(floor)}>{floor}</button>)}
        </div>
    );
}

export default ElevatorPanel;
