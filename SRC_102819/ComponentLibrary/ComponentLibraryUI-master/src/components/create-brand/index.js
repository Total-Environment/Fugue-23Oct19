'use strict';
import React from "react";
import { idFor } from "../../helpers";
import globalStyles from '../component/index.css';
import style from './index.css';
import DataType from '../data-types';
import { validate } from '../../validator';
import { AlertDialog } from '../alert-dialog';
import { ConfirmationDialog } from '../confirmation-dialog';
import { button } from '../../css-common/forms.css';
import classNames from 'classnames';

export class CreateBrand extends React.Component {
  constructor(props) {
    super(props);

    let brandDetails = (
      (this.props.brandDefinition
        && this.props.brandDefinition.columns
      ) || []).map(columnDefinition => Object.assign({}, columnDefinition, {
        value: null,
        editable: true,
        validity: { isValid: true, msg: '' }
      }));

    this.showConfirmationDialog = this.showConfirmationDialog.bind(this);
    this.cancelErrorDialog = this.cancelErrorDialog.bind(this);
    this.yesConfirmationDialog = this.yesConfirmationDialog.bind(this);
    this.noConfirmationDialog = this.noConfirmationDialog.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
    this.ensureMasterDataIsPresent = this.ensureMasterDataIsPresent.bind(this);

    this.initialState = {
      details: brandDetails,
      isValid: false,
      error: {
        message: '',
        shown: false
      },
      confirmation: {
        message: '',
        shown: false
      }
    };
    this.state = Object.assign({}, this.initialState);
  }

  noConfirmationDialog() {
    this.setState({ confirmation: { shown: false } });
  }

  showErrorDialog(message) {
    this.setState({ error: { message, shown: true } });
  }


  componentWillReceiveProps(nextProps) {
    if (nextProps.doneAdding) {
      this.setState(this.initialState);
    }
    this.ensureMasterDataIsPresent(nextProps);
  }

  ensureMasterDataIsPresent(props) {
    // Populate MasterData if it is ID
    let brandDefinition = (props.brandDefinition && props.brandDefinition.columns) || [];
    let masterData = props.masterData || {};
    brandDefinition
      .map(x => x.dataType)
      .filter(x => x.name === 'MasterData')
      .forEach(masterDataType => {
        if (!masterData[masterDataType.subType]) {
          return props.onMasterDataFetch(masterDataType.subType);
        }
      });
  }

  componentDidMount() {
    this.ensureMasterDataIsPresent(this.props);
  }

  cancelErrorDialog() {
    this.setState({ error: { shown: false } });
  }

  showConfirmationDialog(message) {
    this.setState({ confirmation: { message, shown: true } });
  }

  yesConfirmationDialog() {
    this.setState({ confirmation: { shown: false } });
    this.props.onCancel();
    this.setState(this.initialState);
  }

  handleColumnChange(column, value) {
    let updatedState = Object.assign(
      {},
      this.state,
      {
        details: this
          .state
          .details
          .map(c => {
            return (c.key === column.key ? Object.assign({}, column, {
              value,
              validity: validate(value, c, c.name)
            }) : c)
          })
      });

    this.setState(updatedState);
  }

  isFormValid() {
    let updatedState = Object.assign(
      {},
      this.state,
      {
        details: this
          .state
          .details
          .map(c => {
            return (Object.assign({}, c, {
              validity: validate(c.value, c, c.name)
            }))
          })
      });

    const isValid = updatedState.details.reduce((acc, brand) => brand.validity.isValid && acc, true);

    this.setState(updatedState);

    return isValid;
  }

  handleSubmit(e) {
    e.preventDefault();
    if (!this.isFormValid()) {
      this.showErrorDialog("There's an error in the form. Please fix it and submit.");
      return;
    }
    this.props.onAddBrand(this.state.details);
  }

  renderColumn(column) {
    if (column.dataType.name === 'MasterData') {
      column.dataType.values = this.props.masterData[column.dataType.subType] || { values: [], status: 'blank' };
    }
    return <div key={column.name} className={globalStyles.column}>
      <dt id={idFor(name, 'title')} className={globalStyles.columnTitle}>
        {column.name} <abbr className={globalStyles.requiredMarker}>{column.isRequired ? '*' : ''}</abbr>
      </dt>
      <DataType columnValue={column} columnName={column.name} componentType={"brand"} group={this.props.group}
        onChange={value => {
          this.handleColumnChange(column, value)
        }} />
    </div>
  }

  render() {
    let classes = classNames(button, style.addButton);
    return <div className={globalStyles.columnsRow}>
      {
        this
          .state
          .details
          .filter(column => column.key !== 'image')
          .map((column) => this.renderColumn(column))
      }

      <div className={style.action}>
        <input id="add-button"
          className={button}
          type="button"
          onClick={this.handleSubmit}
          value={this.props.isAddingBrand ? 'Adding ...' : 'Add'}
          disabled={this.props.isAddingBrand} />
        <a id="cancel-button" className={button} onClick={this.showConfirmationDialog}>Cancel</a>
      </div>
      <AlertDialog message={this.state.error.message} title="Warning" shown={this.state.error.shown}
        onClose={this.cancelErrorDialog} />
      <ConfirmationDialog message={`The brand will not be created. Do you wish to continue?`} title="Warning"
        shown={this.state.confirmation.shown} onYes={this.yesConfirmationDialog}
        onNo={this.noConfirmationDialog} />
    </div>
  }
}
