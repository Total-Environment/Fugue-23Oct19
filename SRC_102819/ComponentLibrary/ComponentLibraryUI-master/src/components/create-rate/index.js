import React from 'react';
import Collapse from 'rc-collapse';
import { ConfirmationDialog } from '../confirmation-dialog';
import { AlertDialog } from '../alert-dialog';
import { InputNumber } from '../input-number';
import classNames from 'classnames';
import { idFor, tomorrowInIST } from '../../helpers';
import styles from './index.css';
import moment from 'moment-timezone';
import DatePicker from 'react-datepicker';
import { validateModel, validate } from '../../input-validator';
import {Permissioned, permissions} from "../../permissions/permissions";

export class CreateRateActual extends React.Component {

    //React life cycle methods
    //------------------------

    constructor(props) {
        super(props);
        this.state = Object.assign({}, CreateRateActual.initialState(props), {
            openCreateForm: false,
            showAddConfirm: false,
            addConfirmationMessage: "",
            cancelConfirmationMessage: "",
            showCancelConfirm: false,
        });
    }

    render() {
        return <div>
            {this.renderAlertDialog()}
            {this.renderAddConfirmDialog()}
            {this.renderCancelConfirmDialog()}
            <Collapse className={styles.addNewForm} activeKey={this.state.openCreateForm ? "form" : null}>
                <Collapse.Panel key="form" showArrow={false} header={this.renderAddNewRateButton()}>
                    <h2 className={styles.subTitle}>Create {this.props.componentType} rate</h2>
                    <div className={styles.columnsRow}>
                        {this.renderCreateForm()}
                    </div>
                    {this.renderActionButtons()}
                </Collapse.Panel>
            </Collapse>
        </div>
    }

    //Material Rate create
    //--------------------

    static initialMaterialRateState() {
        return {
            "insuranceCharges" : {
              "percentage": "",
              validity: { isValid: true, message: '' },
              validators: ['PositiveNumber'],
              type: "percentage"
            },
            "freightCharges" : {
              "percentage": "",
              validity: { isValid: true, message: '' },
              validators: ['PositiveNumber'],
              type: "percentage"
            },
            "basicCustomsDuty" :{
              "percentage": "",
              validity: { isValid: true, message: '' },
              validators: ['PositiveNumber'],
              type: "percentage"
              },
            "clearanceCharges" : {
              "percentage": "",
              validity: { isValid: true, message: '' },
              validators: ['PositiveNumber'],
              type: "percentage"
            },
            "taxVariance" : {
              "percentage": "",
              validity: { isValid: true, message: '' },
              validators: ['PositiveNumber'],
              type: "percentage"
            },
            "locationVariance" : {
              "percentage": "",
              validity: { isValid: true, message: '' },
              validators: ['PositiveNumber'],
              type: "percentage"
            },
            "marketFluctuation" : {
              "percentage": "",
              validity: { isValid: true, message: '' },
              validators: ['PositiveNumber'],
              type: "percentage"
            },
            "controlBaseRate": {
              "value": "",
              "currency": "",
              validity: { isValid: true, message: '' },
              validators: ['Mandatory', 'NaturalNumber']
            },
            "typeOfPurchase": {
                value: "",
                validity: { isValid: true, message: '' },
                validators: ['Mandatory']
            },
            "location": {
                value: "",
                validity: { isValid: true, message: '' },
                validators: ['Mandatory']
            },
            "id": "",
            "appliedOn": {
                value: "",
                validity: { isValid: true, message: '' },
                validators: ['Mandatory']
            },
            "currency": {
                value: "INR",
                validity: { isValid: true, message: '' },
                validators: ['Mandatory']
            },
        }
    }

    static initialServiceRateState() {
        return {
            "locationVariance" : {
              "percentage": "",
              validity: {
                isValid: true,
                message: "",
              },
              type: "percentage",
              validators: ['PositiveNumber']
            },
            "marketFluctuation" : {
              "percentage": "",
              validity: {
                isValid: true,
                message: "",
              },
              type: "percentage",
              validators: ['PositiveNumber']
            },
            "controlBaseRate": {
              "value": "",
              "currency": "",
              validity: { isValid: true, message: '' },
              validators: ['Mandatory', 'NaturalNumber']
            },
            "typeOfPurchase": {
                value: "",
                validity: { isValid: true, message: '' },
                validators: ['Mandatory']
            },
            "location": {
                value: "",
                validity: { isValid: true, message: '' },
                validators: ['Mandatory']
            },
            "id": "",
            "appliedOn": {
                value: "",
                validity: { isValid: true, message: '' },
                validators: ['Mandatory']
            },
            "currency": {
                value: "INR",
                validity: { isValid: true, message: '' },
                validators: ['Mandatory']
            },
        };
    }

    renderCreateForm() {
        return [
            this.renderRateMetaData("location", "Location", this.props.locationData),
            this.renderRateMetaData("typeOfPurchase", "Type of Purchase", this.props.typeOfPurchaseData),
            this.renderControlBaseRate(),
            this.renderCoefficients(this.props.componentType),
            this.renderAppliedOn()
        ]
    }

    renderCoefficients(componentType) {
        switch(componentType) {
          case 'material' :
            return this.renderMaterialCoefficients();
            break;
          case 'service' :
            return this.renderServiceCoefficients();
            break;
        }
    }

    renderMaterialCoefficients() {
      return [<div key={'insuranceCharges'} className={styles.column}>
        <dt id={idFor('insuranceCharges', 'title')} className={styles.columnTitle}>Insurance Charges</dt>
        <dd id={idFor('insuranceCharges', 'value')} className={styles.columnValue}>
          {this.renderPercentageCoefficient(this.state.insuranceCharges,'insuranceCharges')}
        </dd>
      </div>,
      <div key={'freightCharges'} className={styles.column}>
        <dt id={idFor('freightCharges', 'title')} className={styles.columnTitle}>Freight Charges</dt>
        <dd id={idFor('freightCharges', 'value')} className={styles.columnValue}>
          {this.renderPercentageCoefficient(this.state.freightCharges,'freightCharges')}
        </dd>
      </div>,
      <div key={'basicCustomsDuty'} className={styles.column}>
        <dt id={idFor('basicCustomsDuty', 'title')} className={styles.columnTitle}>Basic Customs Duty</dt>
        <dd id={idFor('basicCustomsDuty', 'value')} className={styles.columnValue}>
          {this.renderPercentageCoefficient(this.state.basicCustomsDuty,'basicCustomsDuty')}
        </dd>
      </div>,
      <div key={'clearanceCharges'} className={styles.column}>
        <dt id={idFor('clearanceCharges', 'title')} className={styles.columnTitle}>Clearance Charges</dt>
        <dd id={idFor('clearanceCharges', 'value')} className={styles.columnValue}>
          {this.renderPercentageCoefficient(this.state.clearanceCharges,'clearanceCharges')}
        </dd>
      </div>,
      <div key={'taxVariance'} className={styles.column}>
        <dt id={idFor('taxVariance', 'title')} className={styles.columnTitle}>Tax Variance</dt>
        <dd id={idFor('taxVariance', 'value')} className={styles.columnValue}>
          {this.renderPercentageCoefficient(this.state.taxVariance,'taxVariance')}
        </dd>
      </div>,
      <div key={'locationVariance'} className={styles.column}>
        <dt id={idFor('locationVariance', 'title')} className={styles.columnTitle}>Location Variance</dt>
        <dd id={idFor('locationVariance', 'value')} className={styles.columnValue}>
          {this.renderPercentageCoefficient(this.state.locationVariance,'locationVariance')}
        </dd>
      </div>,
      <div key={'marketFluctuation'} className={styles.column}>
        <dt id={idFor('marketFluctuation', 'title')} className={styles.columnTitle}>Market Fluctuation</dt>
        <dd id={idFor('marketFluctuation', 'value')} className={styles.columnValue}>
          {this.renderPercentageCoefficient(this.state.marketFluctuation,'marketFluctuation')}
        </dd>
      </div>
      ];
    }

    renderServiceCoefficients() {
      return [
        <div key={'locationVariance'} className={styles.column}>
          <dt id={idFor('locationVariance', 'title')} className={styles.columnTitle}>Location Variance</dt>
          <dd id={idFor('locationVariance', 'value')} className={styles.columnValue}>
            {this.renderPercentageCoefficient(this.state.locationVariance,'locationVariance')}
          </dd>
        </div>,
        <div key={'marketFluctuation'} className={styles.column}>
          <dt id={idFor('marketFluctuation', 'title')} className={styles.columnTitle}>Market Fluctuation</dt>
          <dd id={idFor('marketFluctuation', 'value')} className={styles.columnValue}>
            {this.renderPercentageCoefficient(this.state.marketFluctuation,'marketFluctuation')}
          </dd>
        </div>
      ]
    }

  AddCoefficientsToRateRequest(componentType, request) {
      switch(componentType) {
        case 'material' :
          request.insuranceCharges = this.state.insuranceCharges.percentage;
          request.freightCharges = this.state.freightCharges.percentage;
          request.basicCustomsDuty = this.state.basicCustomsDuty.percentage;
          request.clearanceCharges = this.state.clearanceCharges.percentage;
          request.taxVariance = this.state.taxVariance.percentage;
          request.locationVariance = this.state.locationVariance.percentage;
          request.marketFluctuation = this.state.marketFluctuation.percentage;
          break;
        case 'service' :
          request.locationVariance = this.state.locationVariance.percentage;
          request.marketFluctuation = this.state.marketFluctuation.percentage;
          break;
      }
  }

    createRateRequest() {
        let request = {};
        request.location = this.state.location.value;
        request.typeOfPurchase = this.state.typeOfPurchase.value;
        request.appliedOn = this.state.appliedOn.value;
        request.id = this.props.componentCode;
        request.controlBaseRate = {};
        request.controlBaseRate.value = this.state.controlBaseRate.value || 0;
        request.controlBaseRate.currency = this.state.currency.value || 0;
        this.AddCoefficientsToRateRequest(this.props.componentType, request);
        return request;
    }

    renderAddButton() {
        return <button id="addForm" className={styles.button} onClick={() => {
            this.validateRate();
            setTimeout(() => {
                if (!this.isValidRate()) {
                    return;
                }
                this.setState({ showAddConfirm: true, addConfirmationMessage: `You wish to add a new rate version for the ${this.props.componentType} ${this.props.componentCode}` });
            }, 0)
        }}>Add</button>
    }

    static initialState(props) {
        switch (props.componentType) {
            case 'material':
                return CreateRateActual.initialMaterialRateState();
            case 'service':
                return CreateRateActual.initialServiceRateState();
        }
    }

    renderCancelButton() {
        return <button id="cancelForm" className={styles.button} onClick={() => {
            if (!this.isMaterialRateFormFilled()) {
                this.setState({ openCreateForm: false });
                this.clearCreateForm();
            }
            else {
                this.setState({
                    showCancelConfirm: true,
                    cancelConfirmationMessage: `The changes made to ${this.props.componentType} rate will not be saved. Do you wish to continue?`
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

    renderAddNewRateButton() {
        return <button id="addNew" className={styles.addNewButton} onClick={() => this.setState({ openCreateForm: true })}>
            Add New
        </button>;
    }

    clonedState() {
        return JSON.parse(JSON.stringify(this.state));
    }

    renderRateMetaData(ref, label, masterData) {
        return <div key={label} className={styles.column}>
            <dt id={idFor(label, 'title')} className={styles.columnTitle}>{label} <abbr className={styles.requiredMarker} title="Required">*</abbr></dt>
            <dd id={idFor(label, 'value')} className={styles.columnValue}>
                <select type="text" id={ref} className={classNames({ [styles.input]: true, [styles.error]: !this.state[ref].validity.isValid })} value={this.state[ref].value} onChange={(e) => {
                    let clonedState = this.clonedState();
                    clonedState[ref].value = e.target.value;
                    validateModel(clonedState[ref]);
                    this.setState(clonedState);
                }}>
                    <option value="">--Select--</option>
                    {masterData.values.values.map(v => <option>{v}</option>)}
                </select>
                <br /><span className={styles.errorMsg}>{this.state[ref].validity.message}</span>
            </dd>
        </div>
    }

    renderAppliedOn() {
        const label = "Applied From";
        return <div key={idFor(label)} className={styles.column}>
            <dt id={idFor(label, 'title')} className={styles.columnTitle}>{label}<abbr className={styles.requiredMarker} title="Required">*</abbr></dt>
            <dd id={idFor(label, 'value')} className={styles.columnValue}>
                <DatePicker
                    selected={this.state.appliedOn.value ? moment(this.state.appliedOn.value) : ""}
                    dateFormat="DD/MM/YYYY"
                    id={idFor(label)}
                    onChange={(e) => {
                        const clonedState = this.clonedState();
                        clonedState.appliedOn.value = e;
                        validateModel(clonedState.appliedOn);
                        this.setState(clonedState);
                    }}
                    minDate={tomorrowInIST(new Date())}
                    className={classNames({ [styles.inputAppliedOn]: true, [styles.error]: !this.state.appliedOn.validity.isValid })} />
                <br /><span className={styles.calendarNotice}>*Calendar is set to IST zone</span>
                <span className={styles.errorMsg}>{this.state.appliedOn.validity.message}</span>
            </dd>
        </div>
    }

    static validateCoefficientModel(model) {
        model.validity = validate(model.percentage, model.validators);
    }

    static validateCoefficients(componentType, state) {
      switch(componentType) {
        case 'material' :
          CreateRateActual.validateCoefficientModel(state.insuranceCharges);
          CreateRateActual.validateCoefficientModel(state.freightCharges);
          CreateRateActual.validateCoefficientModel(state.basicCustomsDuty);
          CreateRateActual.validateCoefficientModel(state.clearanceCharges);
          CreateRateActual.validateCoefficientModel(state.taxVariance);
          CreateRateActual.validateCoefficientModel(state.locationVariance);
          CreateRateActual.validateCoefficientModel(state.marketFluctuation);
          break;
        case 'service' :
          CreateRateActual.validateCoefficientModel(state.locationVariance);
          CreateRateActual.validateCoefficientModel(state.marketFluctuation);
          break;
      }
    }

    validateRate() {
        let clonedState = this.clonedState();
        validateModel(clonedState.location);
        validateModel(clonedState.typeOfPurchase);
        validateModel(clonedState.appliedOn);
        validateModel(clonedState.controlBaseRate);
        validateModel(clonedState.currency);
        CreateRateActual.validateCoefficients(this.props.componentType, clonedState);
        this.setState(clonedState);
    }

    isValidCoefficients(componentType) {
      switch(componentType) {
        case 'material' :
          return this.state.insuranceCharges.validity.isValid
            && this.state.freightCharges.validity.isValid
            && this.state.basicCustomsDuty.validity.isValid
            && this.state.clearanceCharges.validity.isValid
            && this.state.taxVariance.validity.isValid
            && this.state.locationVariance.validity.isValid
            && this.state.marketFluctuation.validity.isValid;
          break;
        case 'service' :
          return this.state.locationVariance.validity.isValid
            && this.state.marketFluctuation.validity.isValid;
          break;
      }
    }

    isValidRate() {
        const areCoefficientsValid = this.isValidCoefficients(this.props.componentType);
        return this.state.location.validity.isValid
            && this.state.typeOfPurchase.validity.isValid
            && this.state.appliedOn.validity.isValid
            && this.state.controlBaseRate.validity.isValid
            && this.state.currency.validity.isValid
            && areCoefficientsValid;
    }

    isMaterialRateFormFilled() {
        return !!this.state.location.value
            || !!this.state.typeOfPurchase.value
            || !!this.state.appliedOn.value
            || !!this.state.controlBaseRate.value
            || !!this.state.currency.value;
    }

    handleSubmit() {
        if (this.props.rateAdding) {
            return;
        }
        let request = this.createRateRequest();
        this.props.onAddRate(request, this.props.componentType);
    }

    clearCreateForm() {
        this.setState(CreateRateActual.initialState(this.props));
    }

    renderControlBaseRate() {
        let label = "Control Base Rate";
        return <div key={idFor(label)} className={styles.column}>
            <dt id={idFor(label, 'title')} className={styles.columnTitle}>{label} <abbr className={styles.requiredMarker} title="Required">*</abbr></dt>
            <dd id={idFor(label, 'value')} className={styles.columnValue}>
                <InputNumber className={classNames({ [styles.inputSumCoefficient]: true, [styles.error]: !this.state.controlBaseRate.validity.isValid })} name={idFor(label)} id={idFor(label)} value={this.state.controlBaseRate.value}
                    onChange={(e) => {
                        let clonedState = this.clonedState();
                        clonedState.controlBaseRate.value = e;
                        validateModel(clonedState.controlBaseRate);
                        return this.setState(clonedState);
                    }} />
                <select id="currency" className={classNames({ [styles.inputCurrency]: true, [styles.error]: !this.state.currency.validity.isValid })} type="text" value={this.state.currency.value} onChange={(e) => {
                    let clonedState = this.clonedState();
                    clonedState.currency.value = e.target.value;
                    validateModel(clonedState.currency);
                    this.setState(clonedState);
                }}>
                    {this.props.currencyData.values.values.map(v => <option>{v}</option>)}
                </select>
                <br /><span className={styles.errorMsg}>{this.state.controlBaseRate.validity.message || this.state.currency.validity.message}</span>
            </dd>
        </div>
    }

    renderMoneyCoefficient(coefficient) {
        const filteredCoefficient = this.state.rate.coefficients.filter(c => c.name == coefficient.name)[0];
        const value = filteredCoefficient ? filteredCoefficient.value ? filteredCoefficient.value.value : 0.0 : 0.0;
        return <span>
            <InputNumber className={classNames({ [styles.inputSumCoefficient]: true, [styles.error]: !filteredCoefficient.validity.isValid })}
                name={idFor(coefficient.name)}
                id={idFor(coefficient.name)}
                value={value}
                onChange={(e) => {
                    let clonedState = this.clonedState();
                    let currentCoefficient = clonedState.rate.coefficients.find(c => c.name === coefficient.name);
                    currentCoefficient.value.value = e;
                    CreateRateActual.validateCoefficientModel(currentCoefficient);
                    this.setState(clonedState);
                }} />
            {" " + this.state.currency.value}
            <br /><span className={styles.errorMsg}>{filteredCoefficient.validity.message}</span>
        </span>
    }

    renderPercentageCoefficient(coefficient,name) {
        const filteredCoefficient = this.state[name];
        return <span>
            <InputNumber className={classNames({ [styles.inputPercentageCoefficient]: true, [styles.error]: !filteredCoefficient.validity.isValid })}
                name={idFor(name)}
                id={idFor(name)}
                value={filteredCoefficient.percentage}
                onChange={(e) => {
                    let clonedState = this.clonedState();
                    let currentCoefficient = clonedState[name];
                    currentCoefficient.percentage = e;
                    CreateRateActual.validateCoefficientModel(currentCoefficient);
                    this.setState(clonedState);
                }} />
            %
            <br /><span className={styles.errorMsg}>{filteredCoefficient.validity.message}</span>
        </span>
    }

    static isPercentageCoefficient(coefficient) {
        return coefficient.type === 'percentage';
    }

    //Alerts and Confirmation Dialogs
    //-------------------------------

    renderAlertDialog() {
        return <AlertDialog shown={!!this.props.addRateError} title="Error"
            message={this.props.addRateError}
            onClose={() => this.props.onAddRateErrorClose()} />
    }

    renderAddConfirmDialog() {
        return <ConfirmationDialog shown={this.state.showAddConfirm} title="Confirmation Required"
            id="ConfirmationDialog"
            message={this.state.addConfirmationMessage}
            onYes={() => { this.setState({ showAddConfirm: false }); this.handleSubmit(); }}
            onNo={() => this.setState({ showAddConfirm: false })} />
    }

    renderCancelConfirmDialog() {
        return <ConfirmationDialog shown={this.state.showCancelConfirm} title="Confirmation Required"
            id="CancelConfirmationDialog"
            message={this.state.cancelConfirmationMessage}
            onYes={() => {
                this.setState({ openCreateForm: false, showCancelConfirm: false });
                this.clearCreateForm();
            }}
            onNo={() => this.setState({ showCancelConfirm: false })} />
    }
}

export const CreateRate = Permissioned(CreateRateActual, getPermissions , true,null, false);

function getPermissions(props) {
  switch(props.componentType) {
    case 'material':
      return [permissions.SETUP_MATERIAL_RATE_VERSIONS];
    case 'service':
      return [permissions.SETUP_SERVICE_RATE_VERSIONS];
    default:
      return [];
  }
}
