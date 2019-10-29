import React, {Component} from 'react';
import styles from '../checklists-css/gtc_footer.css';

export class GtcFooter extends Component {
    render() {
        return (
        <table className={styles.gtcFooterTable}>
            <tbody>
            <tr>
              <td className={styles.warning} colSpan="2">This is a LEGALLY BINDING AGREEMENT. Please READ IT CAREFULLY. If you do not understand the impact of this Agreement, please consult your attorney BEFORE signing.</td>
            </tr>
            <tr>
                <td />
                <td />
            </tr>
            <tr>
                <td>
                    <b>Vendors</b>
                </td>
                <td>
                    <b>Customers</b>
                </td>
            </tr>
            <tr>
                <td>
                    <b>Arel Solutions India Private Limited</b>
                </td>
                <td>
                    <b>Total Environment Building Systems Private Limited</b>
                </td>
            </tr>

            </tbody>
        </table>
        );
    }
}
