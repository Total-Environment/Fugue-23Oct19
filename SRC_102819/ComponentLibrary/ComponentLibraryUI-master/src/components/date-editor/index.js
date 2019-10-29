const React = require('react');
const { editors: { EditorBase } } = require('react-data-grid');
import ReactDOM from 'react-dom';
import {tomorrowInIST, toIST} from "../../helpers";
import moment from 'moment-timezone';
import styles from './index.css';

class DateEditor extends EditorBase {

  getInputNode() {
    return ReactDOM.findDOMNode(this);
  }

  getValue() {
    let newDate = moment(this.getInputNode().value).tz('Asia/Calcutta').set({hour:0,minute:0,second:0}).utc().format('YYYY-MM-DD[T]HH:mm:ss[Z]');
    return {
      [this.props.column.key]:{
        value: newDate,
        onChange: this.props.value.onChange,
        rowIndex: this.props.value.rowIndex,
        isValid: this.props.value.isValid
      }
    };
  }

  isAppliedOnValid(date) {
    if (!date) return false;
    return moment(tomorrowInIST()).isSameOrBefore(moment(toIST(date)));
  }

  renderCustomDateDisplay() {
    this.input.setAttribute("data-date", moment(toIST(this.input.value)).format('DD/MM/YYYY'))
  }

  render() {
    return (
      <input ref={(node) => this.input = node} type="date" className={styles.input}
             defaultValue={toIST(this.props.value.value)}
             min={tomorrowInIST()}
             onChange={this.renderCustomDateDisplay.bind(this)}
             onFocus={this.renderCustomDateDisplay.bind(this)}
      />
    );
  }
}

DateEditor.propTypes = {

};

module.exports = {DateEditor};
