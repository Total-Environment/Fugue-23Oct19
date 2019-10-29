import React from 'react';
import Collapse from 'rc-collapse';
import {ConfirmationDialog} from '../confirmation-dialog';
import {AlertDialog} from '../alert-dialog';
import {InputNumber} from '../input-number';
import classNames from 'classnames';
import {idFor,tomorrowInIST} from '../../helpers';
import styles from './index.css';
import moment from 'moment-timezone';
import DatePicker from 'react-datepicker';
import {validateModel} from '../../input-validator';
import {MasterData} from "../data-types/master-data/index";
import {getViewExchangeRateHistoryPermissions} from "../../permissions/ComponentPermissions";
import {Permissioned} from "../../permissions/permissions";

export class CreateExchangeRateActual extends React.Component {

  constructor(props) {
    super(props);
    this.state = Object.assign({}, CreateExchangeRateActual.initialState(), {
      openCreateForm: false,
      showConfirm: false,
      confirmationMessage: ""
    });
  }

  renderAlertDialog() {
    return <AlertDialog shown={!!this.props.addExchangeRateError} title="Error"
                                                            message={this.props.addExchangeRateError}
                                                            onClose={() => this.props.onAddExchangeRateErrorClose()}/>;
  }

  renderAddConfirmDialog(){
    return<ConfirmationDialog shown={this.state.showAddConfirm} title="Confirmation Required"
                                                           message={this.state.addConfirmationMessage}
                                                           onYes={()=>{this.setState({showAddConfirm:false});this.handleSubmit();}}
                                                           onNo={()=>this.setState({showAddConfirm:false})}/>;
  }
  renderCancelConfirmDialog(){
    return <ConfirmationDialog shown={this.state.showCancelConfirm} title="Confirmation Required"
                                                              message={this.state.cancelConfirmationMessage}
                                                              onYes={()=>{
                                                                this.setState({openCreateForm:false,showCancelConfirm:false});
                                                                this.clearCreateForm();
                                                              }}
                                                              onNo={()=>this.setState({showCancelConfirm:false})}/>;
  }

  renderAddButton() {
    return <button id="addForm" className={styles.button} onClick={() => {
      this.validateState();
      setTimeout(() => {
        if (!this.isValidState()) {
          return;
        }
        this.setState({showAddConfirm:true,addConfirmationMessage:`Do you wish to add a new version of exchange rate?`});
      }, 0)
    }}>Add</button>
  }

  renderCancelButton(){
    return <button id="cancelForm" className={styles.button} onClick={()=>{
      if(!this.isFormFilled()){
        this.setState({openCreateForm:false});
      }
      else {
        this.setState({
          showCancelConfirm: true,
          cancelConfirmationMessage: `The changes made to exchange rate will not be saved. Do you wish to continue?`
        });
      }
    }}>Cancel</button>
  }

  renderActionButtons() {
    return <div className={styles.buttonContainer}>
      {this.renderAddButton()}
      {this.renderCancelButton()}
    </div>
  }

  renderAddNewExchangeRateButton() {
    return <button id="addNew" className={styles.addNewButton} onClick={() => this.setState({openCreateForm: true})}>
      Add New
    </button>;
  }

  render() {
    return <div>
      {this.renderAlertDialog()}
      {this.renderAddConfirmDialog()}
      {this.renderCancelConfirmDialog()}
      <Collapse className={styles.addNewForm} activeKey={this.state.openCreateForm ? "form" : null}>
        <Collapse.Panel key="form" showArrow={false} header={this.renderAddNewExchangeRateButton()}>
          <h3 className={styles.subTitle}>Add new Exchange Rate</h3>
          <div className={styles.columnsRow}>
            {this.renderCreateForm()}
          </div>
          {this.renderActionButtons()}
        </Collapse.Panel>
      </Collapse>
    </div>
  }

  renderCreateForm() {
    return [
      this.renderFromCurrency(),
      this.renderBaseConversionRate(),
      this.renderCurrencyFluctuationCoefficient(),
      this.renderAppliedFrom(),
    ]
  }

  static initialState() {
    return {
      "appliedFrom": {
        value: "",
        validity: {isValid: true, message: ''},
        validators: ['Mandatory']
      },
      "fromCurrency": {
        value: "",
        validity: {isValid: true, message: ''},
        validators: ['Mandatory']
      },
      "baseConversionRate": {
        value: "",
        validity: {isValid: true, message: ''},
        validators: ['Mandatory','PositiveNumber']
      },
      "currencyFluctuationCoefficient": {
        value: "",
        validity: {isValid: true, message: ''},
        validators: ['PositiveNumber']
      }
    }
  }

  clonedState() {
    return JSON.parse(JSON.stringify(this.state));
  }

  renderAppliedFrom() {
    const label = "Applied From";
    return <div key={label} className={styles.column}>
      <dt id={idFor(label, 'title')} className={styles.columnTitle}>{label}</dt>
      <dd id={idFor(label, 'value')} className={styles.columnValue}>
        <DatePicker id="appliedFrom" className={classNames({[styles.inputAppliedFrom]: true,[styles.error]: !this.state.appliedFrom.validity.isValid})}
                    selected={this.state.appliedFrom.value ?moment(this.state.appliedFrom.value):''}
                    minDate={tomorrowInIST()}
                    dateFormat="DD/MM/YYYY"
                    onChange={(e) => {
                      const clonedState = this.clonedState();
                      clonedState.appliedFrom.value = e;
                      validateModel(clonedState.appliedFrom);
                      this.setState(clonedState);
                    }}
        />
        <br/><span className={styles.calendarNotice}>*Calendar is set to IST zone</span>
        <span className={styles.errorMsg}>{this.state.appliedFrom.validity.message}</span>
      </dd>
    </div>
  }

  renderFromCurrency() {
    let label = "Currency";
    return <div key={label} className={styles.column}>
      <dt id={idFor(label, 'title')} className={styles.columnTitle}>{label}</dt>
      <dd id={idFor(label, 'value')} className={styles.columnValue}>
        <select id="fromCurrency" className={classNames({[styles.inputCurrency]: true,[styles.error]: !this.state.fromCurrency.validity.isValid})}
               type="text"
               value={this.state.fromCurrency.value}
               onChange={(e) => {
                 let clonedState = this.clonedState();
                 clonedState.fromCurrency.value = e.target.value;
                 validateModel(clonedState.fromCurrency);
                 this.setState(clonedState);
               }}
        >
          <option value="">--Select--</option>
          {this.props.currencyData.values.values.map(v => {
            if(v !== 'INR')
              return <option>{v}</option>
          })}
        </select>
        <br/><span className={styles.errorMsg}>{this.state.fromCurrency.validity.message}</span>
      </dd>
    </div>
  }

  renderBaseConversionRate() {
    let label = "Conversion Rate";
    const value = this.state.baseConversionRate;
    return <div key={label} className={styles.column}>
      <dt id={idFor(label, 'title')} className={styles.columnTitle}>{label}</dt>
      <dd id={idFor(label, 'value')} className={styles.columnValue}>
        <span>
          <InputNumber id="baseConversionRate" className={classNames({[styles.inputNumber]: true,[styles.error]: !this.state.baseConversionRate.validity.isValid})}
                       value={this.state.baseConversionRate.value}
                       onChange={(e) => {
                         let clonedState = this.clonedState();
                         clonedState.baseConversionRate.value = e;
                         validateModel(clonedState.baseConversionRate);
                         this.setState(clonedState);
                       }}
          />
          {" INR"}
          </span>
        <br/><span className={styles.errorMsg}>{this.state.baseConversionRate.validity.message}</span>
      </dd>
    </div>
  }

  renderCurrencyFluctuationCoefficient() {
    let label = "Currency Fluctuation Coefficient";
    const value = this.state.currencyFluctuationCoefficient;
    return <div key={label} className={styles.column}>
      <dt id={idFor(label, 'title')} className={styles.columnTitle}>{label}</dt>
      <dd id={idFor(label, 'value')} className={styles.columnValue}>
        <InputNumber id="currencyFluctuationCoefficient" className={classNames({[styles.inputNumber]: true,[styles.error]: !this.state.currencyFluctuationCoefficient.validity.isValid})}
                     value={this.state.currencyFluctuationCoefficient.value}
                     onChange={(e) => {
                       let clonedState = this.clonedState();
                       clonedState.currencyFluctuationCoefficient.value = e;
                       validateModel(clonedState.currencyFluctuationCoefficient);
                       this.setState(clonedState);
                     }}
        />
        {" %"}
        <br/><span className={styles.errorMsg}>{this.state.currencyFluctuationCoefficient.validity.message}</span>
      </dd>
    </div>
  }

  validateState() {
    let clonedState = this.clonedState();
    validateModel(clonedState.appliedFrom);
    validateModel(clonedState.fromCurrency);
    validateModel(clonedState.baseConversionRate);
    validateModel(clonedState.currencyFluctuationCoefficient);
    this.setState(clonedState);
  }

  isValidState() {
    return this.state.appliedFrom.validity.isValid
      && this.state.fromCurrency.validity.isValid
      && this.state.baseConversionRate.validity.isValid
      && this.state.currencyFluctuationCoefficient.validity.isValid;
  }

  isFormFilled(){
    return !!this.state.appliedFrom.value
      || !!this.state.fromCurrency.value
      || !!this.state.baseConversionRate.value
      || !!this.state.currencyFluctuationCoefficient.value;
  }


  handleSubmit() {
    if (this.props.exchangeRateAdding) {
      return;
    }
    let request = {};
    request.appliedFrom = this.state.appliedFrom.value;
    request.fromCurrency = this.state.fromCurrency.value;
    request.toCurrency = "INR";
    request.baseConversionRate = this.state.baseConversionRate.value;
    request.currencyFluctuationCoefficient = this.state.currencyFluctuationCoefficient.value;
    this.props.onAddExchangeRate(request);
  }

  clearCreateForm() {
    this.setState(CreateExchangeRateActual.initialState());
  }

  static isEmpty(value){
    const isValid = value != undefined && value != null && value !== '';
    return {isValid, message: isValid ? '' : 'The field cannot be left blank.'};
  }

  static isPositiveNumber(value){
    const isValid = +value >= 0;
    return {isValid, message: isValid ? '' : 'Invalid value'};
  }
}

export const CreateExchangeRate = Permissioned(CreateExchangeRateActual, getViewExchangeRateHistoryPermissions, true, null, false);
