import React from 'react';
import styles from './index.css';
import classNames from 'classnames';

export class Constant extends React.Component {
  renderData() {
    return this.props.columnValue.value.toString();
  }

  renderInput() {
    const classes = classNames(this.props.className, styles.input);
    return <input disabled className={classes} value={this.props.columnValue.value || ''} onChange={(e) => this.props.onChange(e.target.value)} />;
  }

  render() {
    return <span>{this.props.columnValue.editable? this.renderInput() : this.renderData() }</span>;
  }
}
