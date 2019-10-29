import React, {Component} from 'react';
import styles from '../checklists-css/footer.css';

export class SpecFooter extends Component {
  render() {
    return (
      <table className={styles.footerTable}>
        <tbody>
        <tr>
          <td><b>Stores Executive</b></td>
          <td/>
          <td><b>QC Engineer</b></td>
        </tr>
        <tr>
          <td>Name:</td>
          <td/>
          <td>Name:</td>
        </tr>
        <tr>
          <td>Date & Sign :</td>
          <td/>
          <td>Date & Sign :</td>
        </tr>
        </tbody>
      </table>
    );
  }
}

