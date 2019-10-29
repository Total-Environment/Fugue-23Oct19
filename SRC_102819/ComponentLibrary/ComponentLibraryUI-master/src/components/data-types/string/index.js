import React from 'react';
import styles from './index.css';
import {ReadMore} from '../../read-more';
import classNames from 'classnames';
import * as R from "ramda";

export class String extends React.Component {

  renderInput() {
    const classes = classNames(this.props.className, styles.input);
    const columnName = this.props.columnName.toLowerCase();
    if (columnName.includes('description') || columnName.includes('additional features')) {
      return <textarea className={classes} value={this.props.columnValue.value || ''} cols="30" rows="2"
                       onChange={(e) => this.props.onChange(e.target.value)}/>;
    }
    return <input className={classes} value={this.props.columnValue.value || ''}
                  onChange={(e) => this.props.onChange(e.target.value)}/>;
  }

  getText() {
    if(!(this.props.columnValue && this.props.columnValue.value)) return;
    const lines = this.props.columnValue.value.split('\n');
    if(lines.length === 1) return lines[0];
    return R.intersperse(<br/>, lines);
  }

  renderData() {
    return <ReadMore>{this.getText()}</ReadMore>;
  }

  render() {
    return this.props.columnValue.editable ? this.renderInput() : this.renderData();
  }
}
