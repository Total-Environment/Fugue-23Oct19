import React, {Component, PropTypes} from 'react';
import {Content} from './content'
import {Remarks} from './remarks'
import {Footer} from './footer'
import styles from '../checklists-css/quality_checklist.css';
import {logoDark} from '../sidebar/index.css';
import {Icon} from '../icon';

export class QualityChecklist extends Component {
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
                            <td colSpan="3"><b>Project Name : </b></td>
                            <td colSpan="3"><b>Project No : </b></td>
                        </tr>
                        <tr>
                            <td colSpan="2">Start Date : </td>
                            <td></td>
                            <td colSpan="3"><b>SO No : </b></td>
                        </tr>
                        <tr>
                            <td colSpan="2">Finish Date : </td>
                            <td></td>
                            <td colSpan="1">Quantity : </td>
                            <td></td>
                            <td colSpan="1">&lt;Unit&gt;</td>
                        </tr>
                        <tr>
                            <td colSpan="6">Area/Location : </td>
                        </tr>
                        <tr>
                            <td colSpan="6">Latest GFC drawing No's. referred : </td>
                        </tr>
                        <tr>
                            <td colSpan="6">Latest Specification Sheet No's. referred : </td>
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
        return <div>QualityChecklist checklist data is not found</div>;
    }
}

QualityChecklist.propTypes = {
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
