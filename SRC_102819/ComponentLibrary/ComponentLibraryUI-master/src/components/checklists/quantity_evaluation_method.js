import React, {Component, PropTypes} from 'react';
import {Content} from './content'
import {Remarks} from './remarks'
import {Footer} from './footer'
import styles from '../checklists-css/quantity_evaluation_method.css';
import {Icon} from '../icon';
import {logoDark} from '../sidebar/index.css';

export class QuantityEvaluationMethod extends Component {
    render() {
        if (this.props.details.content && this.props.details.content.entries) {
            return (
                <div className={styles.qem}>
                    <table className={styles.qemHeaderTable}>
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
                            <td><b>Version : V1.0 </b></td>
                        </tr>
                        <tr>
                            <td colSpan="3"><b>Project/Store Name:</b></td>
                            <td colSpan="3"><b>Project/Store No:</b></td>
                        </tr>
                        <tr>
                            <td colSpan="2">Delivery Date:</td>
                            <td></td>
                            <td colSpan="3"><b>Invoice Number:</b></td>
                        </tr>
                        <tr>
                            <td colSpan="2">Inspection Date:</td>
                            <td>PO No:</td>
                            <td colSpan="2">Quantity:</td>
                            <td>&lt;Unit&gt;</td>
                        </tr>
                        <tr>
                            <td colSpan="6">Material Code:</td>
                        </tr>
                        <tr>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td colSpan="6">Instruments Required:</td>
                        </tr>
                        <tr>
                            <td colSpan="6"></td>
                        </tr>
                        </tbody>
                    </table>
                    <Content entries={this.props.details.content.entries}/>
                    <Remarks/>
                    <Footer/>
                </div>
            );
        }
        return <div>QuantityEvaluationMethod checklist data is not found</div>;
    }
}

QuantityEvaluationMethod.propTypes = {
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
