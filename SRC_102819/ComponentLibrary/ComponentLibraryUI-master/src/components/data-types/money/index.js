import React, { Component } from 'react';
import { InputNumber } from '../../input-number';
import styles from './index.css';
import {helpText, helpTextError} from '../../../css-common/forms.css';
import classNames from 'classnames';
import {connect} from 'react-redux';
import {fetchMasterDataByNameIfNeeded} from '../../../actions';
import axios from 'axios';
import {logException, apiUrl, needsFetching} from "../../../helpers";
import {bindActionCreators} from "redux";

export class Money extends Component {
  render() {
    return this.props.columnValue.editable ? this.renderInput(this.props) : this.renderData(this.props.columnValue.value);
  }

  componentDidMount() {
    if(!this.props.values) {
      this.props.fetchMasterDataByNameIfNeeded('currency');
    }
  }

  renderData(value) {
    return <span>{value.amount} {value.currency}</span>;
  }

  renderInput(props) {
    const {amount, currency} = props.columnValue.value || {amount: '', currency: ''};
    return <span className={styles.inputWrap}>
      <InputNumber value={amount} className={styles.input}
            onChange={value => props.onChange(value ? {amount: value, currency} : null)}/>

      {props.values && !props.isFetching && <select value={currency} className={styles.currency} 
            type="text" 
            onChange={e => props.onChange({ amount, currency: e.target.value})}>
            <option key="select" value="">--Select--</option>
            {props.values.map(v => <option>{v}</option>)}
      </select>}   
    </span>;
  }
}

export const mapStateToProps = (state,props) => {
  return {values: state.reducer.masterDataByName['currency'] && 
  state.reducer.masterDataByName['currency'].values.length !== 0 ? state.reducer.masterDataByName['currency'].values.values : null, 
  columnValue: props.columnValue, isFetching:state.reducer.masterDataByName['currency'] && state.reducer.masterDataByName['currency'].values.isFetching};
};

export const mapDispatchToProps = (dispatch) => ({
   ...bindActionCreators({fetchMasterDataByNameIfNeeded}, dispatch)
});

export const MoneyConnector = connect(mapStateToProps, mapDispatchToProps)(Money);