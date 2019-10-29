import React from 'react';
import styles from './index.css';
import {idFor} from '../../../helpers';
import {InputNumber} from '../../input-number';
import classNames from 'classnames';
import {helpText, helpTextError} from '../../../css-common/forms.css';
import {Decimal} from "../decimal/index";



export class Range extends React.Component {
  renderInput() {
    const classes = classNames(this.props.className, styles.input);
    const name = idFor(this.props.columnName, 'text');
    let columnValue = Object.assign({}, this.props.columnValue, {dataType: {name:'Decimal',subType:null}});
    if(this.props.columnValue.filterable) {
      return (<Decimal
        className={this.props.className}
        columnValue={columnValue}
        group={this.props.group}
        onChange={this.props.onChange}
      />);
    }
    const {from, to} = this.props.columnValue.value || {from: null, to: null};
    return <span>
      <InputNumber className={classes} name={name} value={from}
                   onChange={(value) => this.props.onChange(value ? {from: value, to, unit: this.props.columnValue.dataType.subType} : null)}/>
      {' - '}
      <InputNumber className={classes} name={name} value={to}
                   onChange={(value) => this.props.onChange(from ? {from, to: value, unit: this.props.columnValue.dataType.subType} : null)}/>
      <span className={styles.subType}>{this.props.columnValue.dataType.subType}</span>
    </span>;
  }


  renderData(columnValue) {
    return <span>
      {columnValue.value.from}
      {columnValue.value.to && `-${columnValue.value.to}`}
      {columnValue.dataType.subType}
    </span>;
  }

  render() {
    const columnValue = this.props.columnValue;
    return this.props.columnValue.editable ? this.renderInput() : this.renderData(columnValue);
  }
}
