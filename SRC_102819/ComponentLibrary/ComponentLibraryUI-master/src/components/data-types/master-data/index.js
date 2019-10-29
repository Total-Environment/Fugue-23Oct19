import React from 'react';
import styles from './index.css';
import {helpText, helpTextError} from '../../../css-common/forms.css';
import {ComponentStatus} from '../../component-status';
import classNames from 'classnames';
import R from 'ramda';

export class MasterData extends React.Component {

  renderOption(option) {
    let name = option;
    let value = option;
    if(!R.is(String, option)) {
      name = option.name;
      value = option.value;
    }
    return <option key={value} value={value}>{name}</option>;
  }

  renderInput() {
    const classes = classNames(this.props.className, styles.input);
    if (this.props.columnValue.filterable && !this.props.isClassification) {
      return <input className={classes} value={this.props.columnValue.value}
                    onChange={(e) => this.props.onChange(e.target.value)}/>;
    }
    return <select
      className={styles.select}
      onChange={e => this.props.onChange(e.target.value)}
      value={this.props.columnValue.value}>
      <option key="select" value="">--Select--</option>
      {this.props.columnValue.dataType.values && this.props.columnValue.dataType.values.values.map((option) => this.renderOption(option))}
    </select>;
  }

  renderData() {
    if (['material status', 'service status', 'sfg status','status','package status'].includes(this.props.columnName.toLowerCase())) {
      return <ComponentStatus className={this.props.className} value={this.props.columnValue.value}/>;
    }
    return <span>{this.props.columnValue.value}</span>;
  }

  render() {
    return this.props.columnValue.editable ? this.renderInput() : this.renderData();
  }
}
