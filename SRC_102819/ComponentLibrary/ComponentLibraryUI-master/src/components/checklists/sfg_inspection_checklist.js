import React, {Component} from 'react';
import styles from '../checklists-css/quantity_evaluation_method.css';
import {Content} from "./content";
import {RemarksWithoutInstruments} from "./remarks_without_instruments";
import {SFGFooter} from "./sfg_footer";
import {logoDark} from '../sidebar/index.css';
import {Icon} from '../icon';

export class SFGInspectionChecklist extends Component {
  render() {
    if (this.props.details.content && this.props.details.content.entries) {
      return (
        <div className={styles.qem}>
          <table className={styles.qemHeaderTable}>
            <tbody>
            <tr>
              <td colSpan="2">
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
              <td colSpan="4"><b>Project Name:</b></td>
              <td colSpan="3"><b>Project No:</b></td>
            </tr>
            <tr>
              <td colSpan="3">Start Date:</td>
              <td></td>
              <td colSpan="3"><b>WO No:</b></td>
            </tr>
            <tr>
              <td colSpan="3">Finish Date:</td>
              <td></td>
              <td colSpan="2">Quantity:</td>
              <td>&lt;Unit&gt;</td>
            </tr>
            <tr>
              <td colSpan="7">Area/Location:</td>
            </tr>
            <tr>
              <td colSpan="7">Latest GFC drawing No's. referred:</td>
            </tr>
            <tr>
              <td colSpan="7">Latest Specification Sheet No’s. referred:</td>
            </tr>
            <tr>
              <td colSpan="7"></td>
            </tr>
            </tbody>
          </table>
          <Content entries={this.props.details.content.entries}/>
          <RemarksWithoutInstruments/>
          <SFGFooter/>
        </div>
      );
    }
    return <div>sfg inspection checklist data is not found</div>;
  }
}
