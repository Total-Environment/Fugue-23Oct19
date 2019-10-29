import React, {Component} from 'react';
import styles from '../checklists-css/footer.css';

export class SFGFooter extends Component {
  render() {
    return (
      <table className={styles.footerTable}>
        <tbody>
        <tr>
          <td><b>QC Engineer</b></td>
          <td/>
          <td><b>Site Engineer</b></td>
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

