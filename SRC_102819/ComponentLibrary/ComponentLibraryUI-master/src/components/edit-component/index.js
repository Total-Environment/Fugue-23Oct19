import React from 'react';
import {Component} from '../component';
import {Loading} from '../loading';
import styles from './index.css';
import {validate} from '../../validator';
import {button} from '../../css-common/forms.css';
import {AlertDialog} from '../alert-dialog';
import {ConfirmationDialog} from '../confirmation-dialog';
import {getHeader, head, updateColumn} from "../../helpers";

const componentTypes = {'material': 'Material','service': 'Service'};

export class EditComponent extends React.Component {
  componentDidMount() {
    if (!this.props.dependencyDefinitions[this.classifications()]) {
      this.props.onDependencyDefinitionFetch(this.classifications())
    }

    if (!this.props.details && this.props.componentType === 'material') {
      this.props.onMaterialFetchRequest(this.props.componentCode);
    }

    if (!this.props.details && this.props.componentType === 'service') {
      this.props.onServiceFetchRequest(this.props.componentCode);
    }

    if (!this.props.dependencyDefinitions[this.classifications()]) {
      this.props.onDependencyDefinitionFetch(this.classifications())
    }

    if (head(this.props.details, 'general', 'can_be_used_as_an_asset', 'value')) {
      this.setState({isAssetDefinitionMerged: true});
      this.state.isInitiallyAsset = true;
    }
  }

  constructor(props) {
    super(props);
    this.state = {
      details: this.updateDetails(props.details),
      error: {
        message: this.props.componentUpdateError.message,
        shown: !!this.props.componentUpdateError.message
      },
      confirmation: {
        message: "Changes made to the details will not be saved. Do you wish to continue?",
        shown: false,
        type: "cancel"
      },
      isAssetDefinitionMerged: false,
      isInitiallyAsset: false
    };

    this.handleColumnChange = this.handleColumnChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
    this.cancelErrorDialog = this.cancelErrorDialog.bind(this);
    this.yesConfirmationDialog = this.yesConfirmationDialog.bind(this);
    this.noConfirmationDialog = this.noConfirmationDialog.bind(this);
  }

  addAssetDetails(state, assetDefinition) {
    const assetDetails = state.oldAssetValues ? state.oldAssetValues : this.initializeDataForDefinition(assetDefinition);
    state.details = {headers: this.mergeTwoHeadersList(state.details && state.details.headers, assetDetails)};
    state.isAssetDefinitionMerged = true;
  }

  removeAssetDetails(newState, assetDefinition) {
    newState.oldAssetValues = [];
    assetDefinition.headers.forEach(header => {
      let headerFromDetails = getHeader(newState.details,header.key);
      let indexForHeader = newState.details.headers.findIndex(headerOfState => headerOfState.key === header.key);
      newState.oldAssetValues.push(headerFromDetails);
      if(indexForHeader > -1) {
        newState.details.headers.splice(indexForHeader,1);
      }
    });
    newState.isAssetDefinitionMerged = false;
  }

  mergeTwoHeadersList(first,second) {
    let newHeaders = [];
    first.forEach(stateHeader =>
    {
      let shared = false;
      second.forEach(header => {
        if(stateHeader.key === header.key) {
          shared = true;
        }
      });
      if(!shared) {newHeaders.push(stateHeader)}
    });
    return newHeaders.concat(second);
  }

  initializeDataForDefinition(definition) {
    if (!definition) return;
    return definition.headers
      .filter(header => header.key !== 'classification')
      .reduce((headerObj, header) => {
          let columns = header.columns.reduce((columnObj, column) => {
              columnObj.push({
                dataType: column.dataType,
                isRequired: column.isRequired,
                value: null,
                editable: (!(column.key === "weighted_average_purchase_rate" || column.key === "last_purchase_rate")),
                validity: {isValid: true, msg: ''},
                key: column.key,
                name: column.name
              });
              return columnObj;
            },
            []);
          headerObj.push({columns: columns, key: header.key, name: header.name});
          return headerObj;
        },
        []);
  }

  updateColumnForClassification(details, headerKey, columnName, newValue) {
    let headers = details.headers
      .reduce((headerObj, header) => {
          let columns = header.columns;
          columns = header.columns.reduce((columnObj, column) => {
              if (header.key === headerKey && column.name === columnName) {
                columnObj.push(Object.assign({}, column, newValue));
              }
              else {
                columnObj.push(column);
              }
              return columnObj;
            },
            []);
          headerObj.push({columns: columns, key: header.key, name: header.name});
          return headerObj;
        },
        []);
    return {headers};
  }

  handleColumnChange(headerKey, columnKey, value, columnName) {
    let newState = JSON.parse(JSON.stringify(this.state));
    if (this.props.componentType === "material" && ( columnKey === "material_status" || columnKey === "unit_of_measure")) {
      this.updateErrorDialogState(`Changing ${columnName} will impact existing purchase orders in Fugue. Existing purchase orders need to be closed before the ${columnName} is updated.`, newState);
    }
    if (this.props.componentType === "service" && (columnKey === "service_status" || columnKey === "unit_of_measure")) {
      this.updateErrorDialogState(`Changing ${columnName} will impact existing service orders in Fugue. Existing service orders need to be closed before the ${columnName} is updated.`, newState);
    }
    newState.details = updateColumn(newState.details, headerKey, columnKey, {validity: validate(value, head(newState.details, headerKey, columnKey), columnKey)});

    newState.details = updateColumn(newState.details, headerKey, columnKey, {value: value});

    if (headerKey === "classification") {
      const columns = this.props.dependencyDefinitions[this.classifications()].values.columnList;
      const currentColumnIndex = columns.indexOf(columnName);
      const remainingColumns = columns.filter((c, i) => i > currentColumnIndex);
      remainingColumns.map(remainingColumnName => {
        newState.details = this.updateColumnForClassification(newState.details, headerKey, remainingColumnName, {value: ''});
      });
    }

    if (headerKey === "general" && columnKey === "can_be_used_as_an_asset") {
      const group = head(newState.details, 'classification', 'material_level_2', 'value');
      if (value) {
        if (!this.props.assetDefinitions[group]) {
          this.props.onAssetDefinitionFetch(group);
        } else {
          this.addAssetDetails(newState, this.getAssetDefinition(this.props, group));
        }
      } else {
        if (newState.isAssetDefinitionMerged) {
          if (this.props.assetDefinitions[group] && !this.props.assetDefinitions[group].isFetching) {
            const currentDefinition = this.getAssetDefinition(this.props, group);
            this.removeAssetDetails(newState,currentDefinition);
          } else {
            this.props.onAssetDefinitionFetch(group);
          }
        }
      }
    }
    this.setState(newState);
  }

  getAssetDefinition(props, group) {
    const definition = props.assetDefinitions[group].values;
    return Object.assign({}, definition, {headers: definition.headers.map(h => Object.assign({}, h, {name: this.toCamelCase(h.name)}))});
  }

  classifications() {
    switch (this.props.componentType.toLowerCase()) {
      case 'material':
        return 'materialClassifications';
        break;
      case 'service':
        return 'serviceClassifications';
        break;
    }
  }

  renderTitle() {
    switch (this.props.componentType.toLowerCase()) {
      case 'material':
        return head(this.props.details, 'general', 'material_code', 'value');
        break;
      case 'service':
        return head(this.props.details.serviceDetails, 'general', 'service_code', 'value');
        break;
    }
  }

  render() {
    if (this.props.dependencyDefinitions[this.classifications()] && this.state.details) {
      return (
        <div className={styles.component}>
          <h2 className={styles.title}>{this.renderTitle()}
          </h2>
          <form onSubmit={this.handleSubmit}>
            <Component
              details={this.state.details}
              onColumnChange={this.handleColumnChange}
              dependencyDefinition={this.props.dependencyDefinitions[this.classifications()]}
              onMasterDataFetch={this.props.onMasterDataFetch}
              masterData={this.props.masterData}
              mode={"edit"}
              editable
              componentCode={this.props.componentCode}
              componentType={this.props.componentType.toLowerCase()}
            />
            <div className={styles.action}>
              <input id="update-button" className={button} type="submit"
                     value={this.props.componentUpdating ? 'Updating ...' : 'Update'}
                     disabled={this.props.componentUpdating}/>
              <a id="cancel-button" className={button} onClick={(e) => {
                e.preventDefault();
                this.setState({
                  confirmation: {
                    message: `The changes made to ${componentTypes[this.props.componentType]} ${this.props.componentCode} will not be saved. Do you wish to continue?`,
                    shown: true,
                    type: "cancel"
                  }
                });
              }}>Cancel</a>
            </div>
          </form>
          <AlertDialog message={this.state.error.message} title="Warning" shown={this.state.error.shown}
                       onClose={this.cancelErrorDialog}/>
          <ConfirmationDialog message={this.state.confirmation.message}
                              title="Warning" shown={this.state.confirmation.shown} onYes={this.yesConfirmationDialog}
                              onNo={this.noConfirmationDialog}/>
        </div>);
    }
    else if (this.props.error !== false && this.props.error !== undefined) {
      return <h3>{this.props.error}</h3>;
    }
    else {
      return <Loading/>;
    }
  }

  handleSubmit(e) {
    e.preventDefault();
    if (head(this.state.details, 'general', 'can_be_used_as_an_asset', 'value') && !this.state.isAssetDefinitionMerged) {
      this.showErrorDialog("Please wait while asset properties are loading.");
      return;
    }

    const invalid = this.isFormInvalid();
    if (invalid) {
      this.showErrorDialog(`Error in is ${invalid.header.name}: ${invalid.column.name} . Please fix it and submit.`);
      return;
    }
    if (!this.isValidDependency()) {
      this.showErrorDialog(<span>Classification values submitted for the {this.props.componentType} are not valid (or) are incomplete. <br/>Please define a valid combination or reach out to Admin for further help.</span>);
      return;
    }

    if (this.state.isInitiallyAsset && !this.state.isAssetDefinitionMerged) {
      this.setState({
        confirmation: {
          message: <span>On updating the material that it <b className={styles.highlight}>cannot be used as an Asset</b>, all the Maintenance properties will be deleted. Do you still want to continue?</span>,
          shown: true,
          type: "proceed"
        }
      });
      return;
    }

    this.postEditedDetails();
  }

  postEditedDetails() {
    switch (this.props.componentType.toLowerCase()) {
      case 'material':
        this.props.onUpdateMaterial(this.props.componentCode, this.state.details);
        break;
      case 'service':
        this.props.onUpdateService(this.props.componentCode, this.state.details);
        break;
    }
  }

  isFormInvalid() {
    const newState = JSON.parse(JSON.stringify(this.state));
    newState.details.headers
      .forEach((header) => {
        header.columns.forEach((column) => {
          newState.details = updateColumn(
            newState.details,
            header.key,
            column.key,
            {validity: validate(column.value, column, column.key)});
        })
      });
    const isInvalid = this.getFirstInvalidColumn(newState);
    this.setState(newState);

    return isInvalid;
  }

  getFirstInvalidColumn(state) {
    return state.details.headers.map(header => {
      const inValidColumn = header.columns
        .map(column => {
          const isValid = head(state.details, header.key, column.key, 'validity', 'isValid');
          return isValid ? false : column;
        }).filter(x => x)[0];
      return inValidColumn ? {header, column: inValidColumn} : false;
    }).filter(x => x)[0];
  }

  matchesDefinition(children, values) {
    if (values.length === 0) {
      return children.length === 0;
    }
    const currentValue = values[0] || "null";
    const currentColumn = children.find(column => column.name === currentValue);
    if (!currentColumn) return false;
    return this.matchesDefinition(currentColumn.children, values.slice(1));
  }

  isValidDependency() {
    const children = this.props.dependencyDefinitions[this.classifications()].values.block.children;
    let classificationHeader = getHeader(this.state.details, 'classification');
    const classificationValues = classificationHeader.columns
      .map(column => column.value);
    return this.matchesDefinition(children, classificationValues);
  }

  updateDetails(details) {
    if (!details) {
      return;
    }
    if (this.props.componentType.toLowerCase() === 'service') {
      if (details.serviceDetails && !details.serviceDetails.headers) {
        return;
      }
      details = details.serviceDetails;
    }
    if (!details.headers) {
      return;
    }
    let headers = details.headers
      .reduce((headerObj, header) => {
          let columns = header.columns.reduce((columnObj, column) => {
              columnObj.push(Object.assign({}, column, {editable: (!(column.key === "weighted_average_purchase_rate" || column.key === "last_purchase_rate"))}, {validity: {isValid: true, msg: ''}}));
              return columnObj;
            },
            []);
          headerObj.push({columns: columns, key: header.key, name: header.name});
          return headerObj;
        },
        []);
    return {headers};
  }

  updateErrorDialogState(message, state) {
    if (state.error && !state.error.shown) {
      state.error = {message, shown: true};
    }
  }

  showErrorDialog(message) {
    const newState = JSON.parse(JSON.stringify(this.state));
    this.updateErrorDialogState(message, newState);
    this.setState(newState);
  }

  cancelErrorDialog() {
    this.setState({error: {shown: false}});
    this.props.onResetError();
  }

  yesConfirmationDialog() {
    this.setState({confirmation: {shown: false}});
    if (this.state.confirmation.type === 'cancel') {
      switch (this.props.componentType.toLowerCase()) {
        case 'material':
          this.props.onCancelUpdateMaterial(this.props.componentCode);
          break;
        case 'service':
          this.props.onCancelUpdateService(this.props.componentCode);
          break;
      }
    }
    else if (this.state.confirmation.type === 'proceed') {
      this.postEditedDetails();
    }
  }

  noConfirmationDialog() {
    this.setState({confirmation: {shown: false}});
  }

  toCamelCase(str) {
    if (str.length == 0) return str;
    return str[0].toLowerCase() + str.substring(1);
  }

  componentWillReceiveProps(nextProps) {
    if (nextProps.componentUpdateError.message !== this.props.componentUpdateError.message) {
      this.showErrorDialog(nextProps.componentUpdateError.message);
    }
    if (nextProps.details !== this.props.details) {
      this.setState({details: this.updateDetails(nextProps.details)});

      if (head(nextProps.details,'general','can_be_used_as_an_asset','value')) {
        this.setState({isAssetDefinitionMerged: true, isInitiallyAsset: true});
      }
    }

    if (this.props.assetDefinitions !== nextProps.assetDefinitions) {
      if (!this.state.details) return;
      if (!head(this.state.details,'classification','material_level_2')) return;
      let group = head(this.state.details,'classification','material_level_2','value');
      if (!group) return;
      if (!nextProps.assetDefinitions[group]) return;
      if (nextProps.assetDefinitions[group].isFetching) return;
      const isAsset = head(this.state.details,'general','can_be_used_as_an_asset','value');
      const assetDefinition = this.getAssetDefinition(nextProps, group);
      const newState = JSON.parse(JSON.stringify(this.state));
      if (this.state.isAssetDefinitionMerged && !isAsset) {
        this.removeAssetDetails(newState, assetDefinition);
        this.setState(newState);
      } else if (!this.state.isAssetDefinitionMerged && isAsset) {
        this.addAssetDetails(newState, assetDefinition);
        this.setState(newState);
      }
    }
  }
}
