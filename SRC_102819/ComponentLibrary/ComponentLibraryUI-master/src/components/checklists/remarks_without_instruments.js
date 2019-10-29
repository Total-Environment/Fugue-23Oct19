import React, {Component} from 'react';
import styles from '../checklists-css/remarks.css';

export class RemarksWithoutInstruments extends Component {
  render() {
    return (
      <table className={styles.remarksTable}>
        <tbody>
        <tr>
          <td>Remarks | Corrective Action:</td>
        </tr>
        <tr>
          <td></td>
        </tr>
        <tr>
          <td></td>
        </tr>
        <tr>
          <td></td>
        </tr>
        <tr>
          <td></td>
        </tr>
        <tr>
          <td></td>
        </tr>
        </tbody>
      </table>
    );
  }
}
