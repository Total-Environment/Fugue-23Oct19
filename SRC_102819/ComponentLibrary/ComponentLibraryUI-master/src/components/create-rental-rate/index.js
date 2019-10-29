import React from 'react';
import {AlertDialog} from '../alert-dialog';
import {ConfirmationDialog} from '../confirmation-dialog';
import Collapse from 'rc-collapse';
import styles from './index.css';
import {idFor,tomorrowInIST} from '../../helpers';
import {InputNumber} from '../input-number';
import classNames from 'classnames';
import {validateModel} from '../../input-validator';
import DatePicker from 'react-datepicker';
import moment from 'moment-timezone';
import {Loading} from '../loading';
import {Permissioned, permissions} from "../../permissions/permissions";

export class CreateRentalRateActual extends React.Component {
  constructor(props){
    super(props);
    this.state=Object.assign({},CreateRentalRateActual.initialState(),{
      openCreateForm:false,
      showAddConfirm:false,
      addConfirmationMessage:"",
      cancelConfirmationMessage:"",
      showCancelConfirm:false,
    });
  }

  static initialState(){
    return {
      "rentalRateValue": {
        "value": "",
        "currency": "",
        validity:{isValid:true,message:""},
        validators: ['Mandatory','PositiveNumber'],
      },
      "unitOfMeasure": {
        value:"",
        validity:{isValid:true,message:""},
        validators: ['Mandatory'],
      },
      appliedFrom:{
        value:"",
        validity:{isValid:true,message:""},
        validators: ['Mandatory'],
      },
      currency:{
        value:"INR",
        validity:{isValid:true,message:""},
        validators: ['Mandatory'],
      }
    }
  };

  render() {
    return <div>
      {this.renderAlertDialog()}
      {this.renderAddConfirmDialog()}
      {this.renderCancelConfirmDialog()}
      <Collapse className={styles.addNewForm} activeKey={this.state.openCreateForm ? "form" : null}>
        <Collapse.Panel key="form" showArrow={false} header={this.renderAddNewRateButton()}>
          <h2 className={styles.subTitle}>Create Material Rental Rate</h2>
          <div className={styles.columnsRow}>
            {this.renderCreateForm()}
          </div>
          {this.renderActionButtons()}
        </Collapse.Panel>
      </Collapse>
    </div>
  }

  clonedState() {
    return JSON.parse(JSON.stringify(this.state));
  }

  renderCreateForm() {
    return [
      this.renderRateMetaData("unitOfMeasure","Rental Unit"),
      this.renderRentalRate(),
      this.renderAppliedFrom(),
    ]
  }

  renderActionButtons(){
    return <div className={styles.buttonContainer}>
      {this.renderAddButton()}
      {this.renderCancelButton()}
    </div>
  }

  renderAddButton(){
    return <button id="addForm" className={styles.button} onClick={()=>{
      this.validateForm();
      setTimeout(()=>{
        if(!this.isValidRate()){
          return;
        }
        this.setState({showAddConfirm:true,addConfirmationMessage:`Do you wish to add a new version of rental rate?`});
      },0)
    }}>Add</button>
  }

  renderCancelButton(){
    return <button id="cancelForm" className={styles.button} onClick={()=>{
      if(!this.isFormFilled()){
        this.setState({openCreateForm:false});
        this.clearCreateForm();
      }
      else {
        this.setState({
          showCancelConfirm: true,
          cancelConfirmationMessage: `The changes made to rental rate will not be saved. Do you wish to continue?`
        });
      }
    }}>Cancel</button>
  }

  renderAddNewRateButton(){
    return <button id="addNew" className={styles.addNewButton} onClick={()=>this.setState({openCreateForm:true})}>
      Add New
    </button>;
  }

  renderRentalRate() {
    let label = "Rental Rate";
    return <div key={label} className={styles.column}>
      <dt id={idFor(label, 'title')} className={styles.columnTitle}>{label}</dt>
      <dd id={idFor(label, 'value')} className={styles.columnValue}>
        <InputNumber className={classNames({[styles.inputSumCoefficient]: true,[styles.error]:!this.state.rentalRateValue.validity.isValid})} name={label} value={this.state.rentalRateValue.value}
                     id="rentalRate"
                     onChange={(e) => {
                       let clonedState = this.clonedState();
                       clonedState.rentalRateValue.value = e;
                       validateModel(clonedState.rentalRateValue);
                       return this.setState(clonedState);
                     }}/>
        <select id="currency" className={classNames({[styles.inputCurrency]: true,[styles.error]:!this.state.currency.validity.isValid})} type="text" value={this.state.currency.value} onChange={(e) => {
          let clonedState = this.clonedState();
          clonedState.currency.value  = e.target.value;
          validateModel(clonedState.currency);
          this.setState(clonedState);
        }}>
          {this.props.currencyData.values.values.map(v => <option>{v}</option>)}
        </select>
        <br/><span className={styles.errorMsg}>{this.state.rentalRateValue.validity.message || this.state.currency.validity.message}</span>
      </dd>
    </div>
  }

  renderRateMetaData(ref, label) {
    return <div key={label} className={styles.column}>
      <dt id={idFor(label, 'title')} className={styles.columnTitle}>{label}</dt>
      <dd id={idFor(label, 'value')} className={styles.columnValue}>
        <select id={ref} className={classNames({[styles.input]: true,[styles.error]:!this.state[ref].validity.isValid})}
          value={this.state[ref].value}  onChange={(e) => {
            let clonedState = this.clonedState();
            if(e.target.value !== "Select") {
              clonedState[ref].value = e.target.value;
              validateModel(clonedState[ref]);
              this.setState(clonedState);
            }
          }}>
          <option>--Select--</option>
          {this.props.rentalUnitData.values.values.map(v => <option>{v}</option>)}
        </select>
        <br/><span className={styles.errorMsg}>{this.state[ref].validity.message}</span>
      </dd>
    </div>
  }

  renderUnitOfMeasures(){
      return this.props.unitOfMeasureForRentalRate.map(u=><option value={u}>{u}</option>);
  }

  renderAppliedFrom() {
    const label = "Applied From";
    return <div key={idFor(label)} className={styles.column}>
      <dt id={idFor(label, 'title')} className={styles.columnTitle}>{label}</dt>
      <dd id={idFor(label, 'value')} className={styles.columnValue}>
        <DatePicker
          selected={this.state.appliedFrom.value ? moment(this.state.appliedFrom.value):""}
          dateFormat="DD/MM/YYYY"
          onChange={(e)=>{
            const clonedState = this.clonedState();
            clonedState.appliedFrom.value = e;
            validateModel(clonedState.appliedFrom);
            this.setState(clonedState);
          }}
          id="appliedFrom"
          minDate={tomorrowInIST(new Date())}
          className={classNames({[styles.inputAppliedOn]: true,[styles.error]:!this.state.appliedFrom.validity.isValid})}/>
        <br/><span className={styles.calendarNotice}>*Calendar is set to IST zone</span>
        <span className={styles.errorMsg}>{this.state.appliedFrom.validity.message}</span>
      </dd>
    </div>
  }


  validateForm(){
    let clonedState = this.clonedState();
    validateModel(clonedState.rentalRateValue);
    validateModel(clonedState.currency);
    validateModel(clonedState.unitOfMeasure);
    validateModel(clonedState.appliedFrom);
    this.setState(clonedState);
  }

  isFormFilled(){
    return !!this.state.rentalRateValue.value
      || !!this.state.currency.value
      || !!this.state.unitOfMeasure.value
      || !!this.state.appliedFrom.value;
  }

  isValidRate() {
    return this.state.rentalRateValue.validity.isValid
      && this.state.currency.validity.isValid
      && this.state.unitOfMeasure.validity.isValid
      && this.state.appliedFrom.validity.isValid;
  }

  clearCreateForm(){
    this.setState(CreateRentalRateActual.initialState());
  }

  handleSubmit() {
    if(this.props.rentalRateAdding){
      return;
    }
    let request = this.createRateRequest();
    this.props.onAddRentalRate(this.props.componentCode,request);
  }

  createRateRequest(){
    let request = {};
    request.rentalRateValue ={};
    request.rentalRateValue.value = this.state.rentalRateValue.value;
    request.rentalRateValue.currency = this.state.currency.value;
    request.unitOfMeasure = this.state.unitOfMeasure.value;
    request.appliedFrom = this.state.appliedFrom.value;
    return request;
  }

  renderAlertDialog() {
    return <AlertDialog shown={!!this.props.addRentalRateError} title="Error"
                        message={this.props.addRentalRateError}
                        onClose={() => this.props.onAddRentalRateErrorClose()}/>
  }

  renderAddConfirmDialog() {
    return <ConfirmationDialog shown={this.state.showAddConfirm} title="Confirmation Required"
                               id="addConfirmation"
                               message={this.state.addConfirmationMessage}
                               onYes={() => {
                                 this.setState({showAddConfirm: false});
                                 this.handleSubmit();
                               }}
                               onNo={() => this.setState({showAddConfirm: false})}/>
  }

  renderCancelConfirmDialog() {
    return <ConfirmationDialog shown={this.state.showCancelConfirm} title="Confirmation Required"
                               id="cancelConfirmation"
                               message={this.state.cancelConfirmationMessage}
                               onYes={() => {
                                 this.setState({openCreateForm: false, showCancelConfirm: false});
                                 this.clearCreateForm();
                               }}
                               onNo={() => this.setState({showCancelConfirm: false})}/>
  }

}

export const CreateRentalRate = Permissioned(CreateRentalRateActual, getPermissions , true,null, false);

function getPermissions(props) {
  return [permissions.SETUP_MATERIAL_RENTAL_RATE_VERSIONS];
}
