import React, {Component} from 'react';
import styles from '../checklists-css/footer.css';

export class PackageQualityChecklistFooter extends Component {
  render() {
    return (
      <table className={styles.footerTable}>
        <tbody>
        <tr>
          <td><b>Stores Executive</b></td>
          <td/>
          <td><b>Stores Manager</b></td>
        </tr>
        <tr>
          <td>Name:</td>
          <td/>
          <td>Name:</td>
        </tr>
        <tr>
          <td>Sign & Date:</td>
          <td/>
          <td>Sign & Date:</td>
        </tr>
        </tbody>
      </table>
    );
  }
}

