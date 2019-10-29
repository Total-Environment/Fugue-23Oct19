import React, {Component, PropTypes} from 'react';
import {Content} from './content'
import styles from '../checklists-css/specifications_sheet.css';
import {Icon} from '../icon';
import {logoDark} from '../sidebar/index.css';
import {SpecFooter} from "./SpecFooter";

export class SpecificationsSheet extends Component {

    render() {
        if (this.props.details.content && this.props.details.content.entries) {
            return (
                <div className={styles.ss}>
                    <table className={styles.ssHeaderTable}>
                        <tbody>
                        <tr>
                            <td>
                                <b>TE/</b>
                                <b>{this.props.details.checkListId}</b>
                            </td>
                            <td rowSpan="2" colSpan="4"><b>{this.props.details.title}</b></td>
                            <td rowSpan="2"><Icon name="teLogo" className={logoDark} /></td>
                        </tr>
                        <tr>
                            <td><b>Version : V1.0</b></td>
                        </tr>
                        <tr>
                            <td><b>Project/Store Name:</b></td>
                            <td colSpan="2"></td>
                            <td colSpan="3"><b>Project/Store No:</b></td>
                        </tr>
                        <tr>
                            <td colSpan="2">Delivery Date:</td>
                            <td></td>
                            <td colSpan="3"><b>Invoice Number:</b></td>
                        </tr>
                        <tr>
                            <td colSpan="2">PO No:</td>
                            <td></td>
                            <td colSpan="2">Quantity:</td>
                            <td>&lt;Unit&gt;</td>
                        </tr>
                        <tr>
                            <td colSpan="6">Material Code:</td>
                        </tr>
                        <tr>
                            <td>Test Sheet Number</td>
                            <td>&lt;System Generated&gt;</td>
                            <td colSpan="3">Governing Standard</td>
                            <td>EN 13329</td>
                        </tr>
                        <tr>
                        </tr>
                        <tr>
                        </tr>
                        </tbody>
                    </table>
                    <Content entries={this.props.details.content.entries}/>
                    <SpecFooter />
                </div>);
        }
        return <div>SpecificationsSheet checklist data is not found</div>;
    }
}

SpecificationsSheet.propTypes = {
    details: PropTypes.shape({
        checkListId: PropTypes.string,
        id: PropTypes.string,
        template: PropTypes.string,
        title: PropTypes.string,
        content: PropTypes.shape({
            entries: PropTypes.arrayOf(PropTypes.shape({
                cells: PropTypes.arrayOf(PropTypes.shape({
                    value: PropTypes.string,
                    key: PropTypes.string,
                })),
                key: PropTypes.number,
            })),
        }),
    })
};
