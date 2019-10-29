import React, {Component, PropTypes} from 'react';
import styles from '../checklists-css/special_terms_and_conditions.css';
import { TermsAndConditionsContent } from './terms_and_conditions_content';

export class SpecialTermsAndConditions extends Component {
    render() {
        if (this.props.details.content && this.props.details.content.entries) {
            return (
                <div className={styles.spo}>
                    <table className={styles.spoHeaderTable}>
                        <tbody>
                        <tr>
                            <td>
                                <b>{this.props.details.title}</b>
                            </td>
                        </tr>
                        </tbody>
                    </table>
                    <TermsAndConditionsContent entries={this.props.details.content.entries}/>
                </div>
            );
        }
        return <div>SpecialTermsAndConditions checklist data is not found</div>;
    }
}

SpecialTermsAndConditions.propTypes = {
    details: PropTypes.shape({
        checkListId: PropTypes.string,
        id: PropTypes.string,
        title: PropTypes.string,
        template: PropTypes.string,
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
