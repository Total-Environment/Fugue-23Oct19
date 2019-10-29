import React, {Component} from 'react';
import ReactDataGrid from 'react-data-grid';
import {Loading} from "../loading/index";
import moment from 'moment-timezone';
import DatePicker from 'react-datepicker';
import {tomorrowInIST} from "../../helpers";
import {Editors} from 'react-data-grid-addons';
import classNames from 'classnames';

const DropDownEditor = Editors.DropDownEditor;

const titles = ['USD', 'INR'];

export class EditRates extends Component {

  // componentWillMount() {
  //   if (this.props.rates && this.props.rates.items) {
  //     this.setState({rows: this.getInitialRows(this.props.rates)});
  //     return;
  //   }
  //   this.props.onRateFetchRequest();
  //   if (typeof window !== "undefined") {
  //     window.addEventListener('resize', this.resizeHandler);
  //   }
  // }

  // resizeHandler() {
  //   this.setState({minHeight: (0.88 * window.innerHeight) - 100});
  // }
  //
  // componentWillUnmount() {
  //   window.removeEventListener('resize', this.resizeHandler);
  //   this.props.onDestroyRates();
  // }

  componentWillReceiveProps(nextProps) {
    if (nextProps.rates && nextProps.rates.items !== (this.props.rates && this.props.rates.items)) {
      this.setState({rows: this.getInitialRows(nextProps.rates), columns: this.getHeaders(nextProps)});
    }
  }

  handleSubmit() {

  }

  render() {
    if (this.state.rows.length !== 0) {
      return (
        <form onSubmit={this.handleSubmit}>
        <div className={styles.dataGrid}>
          <ReactDataGrid
            ref="grid"
            columns={this.state.columns}
            rowGetter={this.getValue}
            rowsCount={this.state.rows.length}
            minHeight={this.state.minHeight}
            rowHeight={40}
            enableCellSelect={true}
            onGridRowsUpdated={this.handleGridRowsUpdated}
            onCellSelected={this.onCellSelected}
          />
        </div>
          <button className={styles.update}>Update</button>
          <button className={styles.cancel}>Cancel</button>
        </form>);
    }
    else {
      return <Loading/>;
    }
  }

  onCellSelected({rowIdx, idx}) {
    this.refs.grid.openCellEditor(rowIdx, idx);
  }

  getValue(i) {
    return this.state.rows[i];
  }

  getHeaders(props) {
    let columns = [];
    columns.push({name: 'Material Name', key: 'materialName', width: 150, resizable: true});
    columns.push({name: 'Material Code', key: 'code', width: 150, resizable: true});
    columns.push({name: 'Type of Purchase', key: 'typeOfPurchase', editable: true, width: 150, resizable: true});
    columns.push(
      {
        name: 'Currency Type',
        key: 'currencyType',
        editable: true,
        width: 150,
        resizable: true,
        editor: <DropDownEditor options={titles}/>
      });
    columns.push(
      {
        name: 'Control Base Rate',
        key: 'controlBaseRate',
        width: 150,
        resizable: true,
        editable: true,
        formatter: Number
      });
    props.rates.items[0].materialRate.rate.coefficients.map((coefficient) => {
      columns.push({
        name: coefficient.percentage !== null ? coefficient.name + "(%)" : coefficient.name,
        key: coefficient.name,
        width: 150,
        resizable: true,
        editable: true,
        formatter: Number
      });
    });
    columns.push({name: 'AppliedOn', key: 'appliedOn', width: 150, resizable: true, formatter: Date});
    return columns;
  }

  decimal(value) {
    const isValid = this.isNumber(value);
    return {isValid: isValid, msg: isValid ? '' : 'Not a valid number'};
  }

  isNumber(value) {
    return !isNaN(value) && isFinite(value);
  }

  getInitialRows(rates) {
    let rows = [];
    if (rates && rates.items) {
      rates.items.forEach((item, index) => {
        rows.push(
          {
            'materialName': item.materialName,
            'code': item.materialRate.id,
            'typeOfPurchase': item.materialRate.typeOfPurchase,
            'controlBaseRate': item.materialRate.rate.controlBaseRate.value,
            'currencyType': item.materialRate.rate.controlBaseRate.currency,
            'appliedOn': {value: item.materialRate.appliedOn, onChange: this.handleGridRowsUpdated, rowIndex: index},
            ...this.getCoefficientData(item.materialRate.rate.coefficients, index),
          });
      });
    }
    return rows;
  }

  getCurrency(value) {
    return value ? value.value : '-';
  }

  getCoefficientData(coefficients, index) {
    let result = {};
    coefficients.forEach(coefficient => {
      coefficientsList.push(coefficient.name);
      result[coefficient.name] = coefficient.percentage !== null ? coefficient.percentage : this.getCurrency(coefficient.value)
    });
    this.setState({coefficients: coefficientsList});
    return result;
  }

  getCurrentColumnIndex(value) {
    let columns = JSON.parse(JSON.stringify(this.state.columns));
    return columns.findIndex(column => column.name === value);
  }

  handleGridRowsUpdated({fromRow, toRow, updated}) {
    let value = Object.assign({}, updated);
    // if (Array.indexOf(this.state.coefficients, Object.keys(value)[0]) !== -1) {
    //   let result = this.decimal(Object.values(value)[0]);
    //   const columnIndex = this.getCurrentColumnIndex(Object.keys(value)[0]);
    //   if (!result.isValid) {
    //
    //   }
    // }
    let rows = this.state.rows.slice();
    for (let i = fromRow; i <= toRow; i++) {
      let rowToUpdate = rows[i];
      rows[i] = Object.assign({}, rowToUpdate, value);
    }
    this.setState({rows: rows});
  }
}

class Date extends Component {
  render() {
    let props = this.props;
    return <DatePicker
      id="appliedOn-edit"
      selected={this.props.value.value ? moment(this.props.value.value) : ''}
      minDate={tomorrowInIST()}
      dateFormat="DD/MM/YYYY"
      onChange={(e) => {
        (props.value.onChange({
          fromRow: props.value.rowIndex,
          toRow: props.value.rowIndex,
          updated: {
            "appliedOn": {
              value: e._d.toISOString(),
              onChange: props.value.onChange,
              rowIndex: props.value.rowIndex
            }
          }
        }))
      }}
    />
  }
}

class Number extends Component {
  render() {
    const isValid = this.decimal(this.props.value).isValid;
    const classes = classNames({[styles.error]: !isValid});
    return <div>
      <span className={classes}>{this.props.value}</span><br/>
    </div>;
  }

  decimal(value) {
    const isValid = this.isNumber(value);
    return {isValid: isValid, msg: isValid ? '' : 'Not a valid number'};
  }

  isNumber(value) {
    return !isNaN(value) && isFinite(value);
  }
}
