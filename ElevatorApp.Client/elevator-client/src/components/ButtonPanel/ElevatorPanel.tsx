import React, { ComponentProps } from 'react';

interface ElevatorPanelProps extends ComponentProps<any> {
    floorCount: number;
}

function ElevatorPanel(props: ElevatorPanelProps) {
    const { floorCount } = props;

    const floorNumbers = [];
    for (let i = floorCount; i >= 1; i--) {
        floorNumbers.push(i);
    }

    return (
        <div>
            {floorNumbers.map(floor => <button key={floor}>{floor}</button>)}
        </div>
    );
}

export default ElevatorPanel;
