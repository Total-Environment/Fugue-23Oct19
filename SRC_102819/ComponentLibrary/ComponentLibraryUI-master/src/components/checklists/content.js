import React, {Component, PropTypes} from 'react';
import styles from '../checklists-css/content.css';

export class Content extends Component {
    renderHead(cell) {
        return <th key={cell.value}>{cell.value}</th>;
    }

    renderCell(cell) {
        return <td key={cell.key}>{cell.value}</td>;
    }

    renderEntry(entry) {
        return <tr key={entry.cells[0].value}>{entry.cells.map(this.renderCell.bind(this))}</tr>;
    }

    render() {
        return (
            <table className={styles.contentTable}>
                <thead>
                <tr>{this.props.entries[0].cells.map(this.renderHead.bind(this))}</tr>
                </thead>
                <tbody>{this.props.entries.slice(1).map(this.renderEntry.bind(this))}
                </tbody>
            </table>
        );
    }
}

Content.propTypes = {
    entries: PropTypes.arrayOf(PropTypes.shape({
        cells: PropTypes.arrayOf(PropTypes.shape({
            value: PropTypes.string,
            key: PropTypes.string,
        })),
        key: PropTypes.number,
    }))
};
