import React, {Component, PropTypes} from 'react';
import styles from '../checklists-css/terms_and_conditions_content.css';

export class TermsAndConditionsContent extends Component {
    constructor() {
        super();
        this.renderMergeCells = this.renderMergeCells.bind(this);
        this.renderCell = this.renderCell.bind(this);
        this.renderEntry = this.renderEntry.bind(this);
    }
    renderCell(cell) {
        if(cell.value === undefined)
        {
            return;
        }
         return <td key={cell.key}>{cell.value}</td>;
    }

    renderMergeCells(entry) {
        var renderData = [];
        for (let index = 0; index < entry.cells.length; index++) {
            if(index === 0 && entry.cells[index].value === undefined){
                renderData.push(<td> </td>);
            }
            else if (index < entry.cells.length - 1 && entry.cells[index + 1].value === undefined) {
                if (index !== 0) {
                    renderData.push(<td key={entry.cells[index].key} colSpan="2">{entry.cells[index].value}</td>);
                }
            }
            else {
                renderData.push(this.renderCell(entry.cells[index]));
            }
        }
        return [renderData];
    }

    renderEntry(entry) {
        return <tr key={entry.key}>{this.renderMergeCells(entry)}</tr>;
    }

    render() {
        return (
            <table className={styles.contentTable}>
                <tbody>{this.props.entries.slice(1).map(this.renderEntry.bind(this))}
                </tbody>
            </table>
        );
    }
}

TermsAndConditionsContent.propTypes = {
    entries: PropTypes.arrayOf(PropTypes.shape({
        cells: PropTypes.arrayOf(PropTypes.shape({
            value: PropTypes.string,
            key: PropTypes.string,
        })),
        key: PropTypes.number,
    }))
};
