import React from 'react';
import {idFor} from '../../../helpers';
import styles from './index.css';

export class Boolean extends React.Component {
  renderInput() {
    const name = idFor(this.props.columnName, 'radio');
    return <span><label className={styles.input}>
      <input type="radio"
             className={styles.radio}
             name={name}
             checked={this.props.columnValue.value == true}
             onChange={(e) => this.props.onChange(true)}
      /> Yes
    </label>
    <label className={styles.input}>
      <input type="radio"
             className={styles.radio}
             name={name}
             checked={this.props.columnValue.value == false}
             onChange={(e) => this.props.onChange(false)}
      /> No
    </label></span>;
  }

  renderData() {
    return this.props.columnValue.value ? 'Yes' : 'No';
  }

  render() {
    return <span>{this.props.columnValue.editable ? this.renderInput() : this.renderData()}</span>;
  }
}
