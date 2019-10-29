import React from 'react';
import styles from './index.css';
import classNames from 'classnames';
import {InputNumber} from '../../input-number';
import {helpText, helpTextError} from '../../../css-common/forms.css';

export class Unit extends React.Component {
  renderInput() {
    const {value, dataType, validity} = this.props.columnValue;
    const valueForUnitType = (value && (value.value !== null) && (value.value !== undefined) && `${value.value.toString()}`) || (value === "-- NA --" && value) || '';
    return <span>
      <InputNumber className={styles.input}
                   value={valueForUnitType}
                   onChange={(value) => this.props.onChange(value ? {type: dataType.subType, value} : null)}/>
      <span className={styles.unit}>{dataType.subType}</span>
    </span>;
  }

  renderData() {
    const {value, dataType} = this.props.columnValue;
    return <span>{(value && (value.value !== null) && (value.value !== undefined) && `${value.value.toString()} ${dataType.subType}`) || (value === "-- NA --" && value) ||("-")}</span>;
  }

  render() {
    return this.props.columnValue.editable ? this.renderInput() : this.renderData();
  }
}
