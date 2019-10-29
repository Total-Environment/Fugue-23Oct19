import React from 'react';
import styles from './index.css';
import classNames from 'classnames';
import ReactDatePicker from 'react-datepicker';
import moment from 'moment-timezone';
import { toIST } from '../../../helpers';

export class DateTime extends React.Component {
  renderInput() {
    const classes = classNames(this.props.className, styles.input);
    return <input className={classes} type="datetime-local" value={this.props.columnValue.value || ''} />;
  };

  renderData(date) {
    if (this.props.columnValue.value == null) {
      return '-';
    }
    else
      return <span>
        {toIST(this.props.columnValue.value, 'D/M/YYYY')}
      </span>
  }

  render() {
    const date = new Date(this.props.columnValue.value);
    return (<span>{this.props.columnValue.editable ? this.renderInput() : this.renderData(date)}</span>);
  }
}


export class DatePicker extends React.Component {
  renderInput() {
    const classes = classNames(this.props.className, styles.input);

    return <ReactDatePicker
      selected={this.props.columnValue.value ? moment(this.props.columnValue.value) : ''}
      dateFormat="DD/MM/YYYY"
      showMonthDropdown={true}
      showYearDropdown={true}
      dropdownMode="select"
      onChange={e => this.props.onChange(e)}
      className={classes}
    />
  }

  renderData() {
    if (!this.props.columnValue.value) {
      return '-';
    }
    return <span>
      {toIST(this.props.columnValue.value)}
    </span>
  }
  render() {
    return (
      <span>{
        this.props.columnValue.editable ?
          this.renderInput() :
          this.renderData()
      }</span>
    )
  }
}
