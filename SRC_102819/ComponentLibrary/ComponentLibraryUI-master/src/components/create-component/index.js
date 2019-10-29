import React from 'react';
import style from './index.css';
import {Component} from '../component/index.js';
import {Loading} from '../loading';
import {validate} from '../../validator';
import {button} from '../../css-common/forms.css';
import {AlertDialog} from '../alert-dialog';
import {ConfirmationDialog} from '../confirmation-dialog';
import {getHeader, head, idFor, updateColumn} from "../../helpers";
import * as R from "ramda";

const componentTypes = {'material': 'Material','service': 'Service'};

export class CreateComponent extends React.Component {
  componentDidMount() {
    if (!this.props.dependencyDefinitions[this.classifications()]) {
      this.props.onDependencyDefinitionFetch(this.classifications());
    }
  }

  constructor(props) {
    super(props);
    this.state = {
      details: props.dependencyDefinitions[this.classifications()] ? {headers: [this.initializeDataForClassification(props.dependencyDefinitions[this.classifications()])]} : undefined,
      isValid: false,
      error: {
        message: this.props.componentCreateError.message,
        shown: !!this.props.componentCreateError.message
      },
      confirmation: {
        message: '',
        shown: false
      },
      isAssetDefinitionMerged: false,
    };
    this.handleColumnChange = this.handleColumnChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
    this.cancelErrorDialog = this.cancelErrorDialog.bind(this);
    this.showConfirmationDialog = this.showConfirmationDialog.bind(this);
    this.yesConfirmationDialog = this.yesConfirmationDialog.bind(this);
    this.noConfirmationDialog = this.noConfirmationDialog.bind(this);
    this.updateColumnForClassification = this.updateColumnForClassification.bind(this);
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

  headerTitle() {
    switch (this.props.componentType.toLowerCase()) {
      case 'material':
        return 'Create New Material';
        break;
      case 'service':
        return 'Create New Service';
        break;
    }
  }

  componentWillReceiveProps(nextProps) {
    if(nextProps.componentCreateError.message !== this.props.componentCreateError.message && !!nextProps.componentCreateError.message) {
      this.showErrorDialog(nextProps.componentCreateError.message);
    }

    if(!R.equals(this.props.definitions, nextProps.definitions)){
      if (this.state.details) {
        this.setDetails(nextProps, this.state);
      }
      return;
    }

    if (!R.equals(this.props.dependencyDefinitions[this.classifications()], nextProps.dependencyDefinitions[this.classifications()])) {
      this.setState({details: {headers: [this.initializeDataForClassification(nextProps.dependencyDefinitions[this.classifications()])]}});
    }

    if(this.props.assetDefinitions !== nextProps.assetDefinitions) {
      if(this.state.isAssetDefinitionMerged) return;
      if(!this.state.details) return;
      if(!getHeader(this.state.details,'classification','material_level_2')) return;
      let group = head(this.state.details,'classification','material_level_2','value');
      if(!group) return;
      if(!nextProps.assetDefinitions[group]) return;
      if(nextProps.assetDefinitions[group].isFetching) return;
      if(!head(this.state.details,'general','can_be_used_as_an_asset','value')) return;

      const newState = JSON.parse(JSON.stringify(this.state));
      this.addAssetDetails(newState, nextProps.assetDefinitions[group].values);
      this.setState(newState);
    }
  }

  matchesDefinition(children, values) {
    if(values.length === 0) {
      return children.length === 0;
    }
    const currentValue = values[0] || "null";
    const currentColumn = children.find(column => column.name === currentValue);
    if(!currentColumn) return false;
    return this.matchesDefinition(currentColumn.children, values.slice(1));
  }

  isValidDependency() {
    const children = this.props.dependencyDefinitions[this.classifications()].values.block.children;
    let classificationHeader = getHeader(this.state.details, 'classification');
    const classificationValues = classificationHeader.columns
      .map(column => column.value);
    return this.matchesDefinition(children, classificationValues);
  }

  showErrorDialog(message) {
    this.setState({error: {message, shown: true}});
  }

  cancelErrorDialog() {
    this.setState({error: {message: '', shown: false}});
    this.props.onCancelErrorDialog();
  }

  showConfirmationDialog(message) {
    this.setState({confirmation: {message, shown: true}});
  }

  yesConfirmationDialog() {
    this.setState({confirmation: {shown: false}});
    switch (this.props.componentType.toLowerCase()) {
      case 'material':
        this.props.onCancelMaterial();
        break;
      case "service":
        this.props.onCancelService();
        break;
    }
  }

  noConfirmationDialog() {
    this.setState({confirmation: {shown: false}});
  }

  isFormValid() {
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

    const isValid = !newState.details.headers.find(header => header.columns.find(column => !column.validity.isValid));
    this.setState(newState);
    return isValid;
  }

  handleSubmit(e) {
    e.preventDefault();

    if (head(this.state.details, 'general', 'can_be_used_as_an_asset', 'value') && !this.state.isAssetDefinitionMerged) {
      this.showErrorDialog("Please wait while asset properties are loading.");
      return;
    }

    if (!this.isFormValid()) {
      this.showErrorDialog(<span>The {componentTypes[this.props.componentType]} details submitted are either incomplete (or) not valid. <br/>Please check and fix the errors in the form and resubmit.</span>);
      return;
    }
    if (!this.isValidDependency()) {
      this.showErrorDialog(
        <span>Classification values submitted for the {componentTypes[this.props.componentType]} are not valid (or) are incomplete. <br/>Please define a valid combination or reach out to Admin for further help.</span>);
      return;
    }

    switch (this.props.componentType.toLowerCase()) {
      case 'material':
        this.props.onAddMaterial(this.state.details);
        break;
      case 'service':
        this.props.onAddService(this.state.details);
        break;
    }
  }

  setDetailsForClassification(details) {
    return {headers: details.headers.filter(header => header.key === 'classification')};
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

  initializeDataForClassification(dependencyDefinition) {
    if (!dependencyDefinition || dependencyDefinition.isFetching) return;
    const columns = dependencyDefinition.values.columnList.reduce((obj, columnName) => {
      obj.push({
        dataType: {name: 'MasterData', values: {status: 'fetched', values: []}, value: ''},
        editable: true,
        validity: {isValid: true, msg: ''},
        name:columnName,
        key: columnName.toLowerCase().split(' ').join('_')
      });
      return obj;
    }, []);
    return {columns,name: 'Classification', key: 'classification'};
  }

  addAssetDetails(state, assetDefinition) {
    const assetDetails = state.oldAssetValues ? state.oldAssetValues : this.initializeDataForDefinition(assetDefinition);
    state.details = {headers: this.mergeTwoHeadersList(state.details && state.details.headers, assetDetails)};
    state.isAssetDefinitionMerged = true;
  }

  handleColumnChange(headerKey, columnKey, value, columnName) {
    let newState = JSON.parse(JSON.stringify(this.state));

    newState.details = updateColumn(newState.details, headerKey, columnKey, {validity: validate(value, head(newState.details, headerKey, columnKey), columnKey)});
    newState.details = updateColumn(newState.details, headerKey, columnKey, {value: value});

    if(headerKey === "general" && columnKey === "can_be_used_as_an_asset") {
      const group = head(newState.details,'classification','material_level_2','value');
      if(value) {
        if(!this.props.assetDefinitions[group]) {
          this.props.onAssetDefinitionFetch(group);
          newState.isAssetDefinitionMerged = false;
        } else if(this.props.assetDefinitions[group].isFetching) {
          newState.isAssetDefinitionMerged = false;
        } else {
          this.addAssetDetails(newState, this.props.assetDefinitions[group].values);
        }
      }
      else {
        if(newState.isAssetDefinitionMerged) {
          // Assumption: If this is true, we assume that the corresponding definition is present
          const currentDefinition = this.props.assetDefinitions[group].values;
          newState.oldAssetValues = [];
          currentDefinition.headers.forEach(header => {
            let headerFromDetails = getHeader(newState.details,header.key);
            let indexForHeader = newState.details.headers.findIndex(headerOfState => headerOfState.key === header.key);
            newState.oldAssetValues.push(headerFromDetails);
            if(indexForHeader > -1) {
              newState.details.headers.splice(indexForHeader,1);
            }
          });
          newState.isAssetDefinitionMerged = false;
        }
      }
    }

    if (headerKey === "classification") {
      // Reset the values of all the remaining levels
      const columns = this.props.dependencyDefinitions[this.classifications()].values.columnList;
      const currentColumnIndex = columns.indexOf(columnName);
      const remainingColumns = columns.filter((c, i) => i > currentColumnIndex);
      remainingColumns.map(remainingColumnName => {
        newState.details = this.updateColumnForClassification(newState.details, headerKey, remainingColumnName, {value: ''});
      });
      if (columnKey === 'material_level_2' || columnKey === 'material_level_1' || columnKey === 'service_level_1') {
        this.setDetails(this.props, newState);
        if (columnKey === 'material_level_2' && value !== '' && !this.props.definitions[value]) {
          this.props.onMaterialDefinitionFetch(value);
        }
        else if (columnKey === 'service_level_1' && value !== '' && !this.props.definitions[value]) {
          this.props.onServiceDefinitionFetch(value);
        }
      }
      else {
        this.setState(newState);
      }
    }
    else {
           this.setState(newState);
    }
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

  setDetails(props, state) {
    let details;
    let stateDetails = JSON.parse(JSON.stringify(state.details));
    switch (this.props.componentType.toLowerCase()) {
      case 'material':
        const materialLevel2Value = head(stateDetails,'classification','material_level_2','value');

        if (materialLevel2Value !== '' && props.definitions[materialLevel2Value]) {
          let otherHeaders = this.initializeDataForDefinition(props.definitions[materialLevel2Value]);
          let newHeaders = this.mergeTwoHeadersList(stateDetails.headers,otherHeaders);
          details = {headers: newHeaders};
        } else {
          details = this.setDetailsForClassification(stateDetails);
        }
        break;
      case 'service':
        const serviceLevel1Value = head(stateDetails,'classification','service_level_1','value');

        if (serviceLevel1Value !== '' && props.definitions[serviceLevel1Value]) {
          let otherHeaders = this.initializeDataForDefinition(props.definitions[serviceLevel1Value]);
          let newHeaders = this.mergeTwoHeadersList(stateDetails.headers,otherHeaders);
          details = {headers: newHeaders};
        } else {
          details = this.setDetailsForClassification(stateDetails);
        }
        break;
    }
    this.setState({details: details, isAssetDefinitionMerged: false, oldAssetValues: undefined});
  }

  render() {
    if(this.props.dependencyDefinitionError[this.props.componentType] && this.props.dependencyDefinitionError[this.props.componentType].type === "NetworkError") {
      return <h3 id={idFor('error')}>{this.props.dependencyDefinitionError[this.props.componentType].error}</h3>;
    }
    else if(this.props.dependencyDefinitionError[this.props.componentType]) {
      return <h3 id={idFor('error')}>{this.props.dependencyDefinitionError[this.props.componentType]}</h3>;
    }
    else if (!this.props.dependencyDefinitions[this.classifications()] || this.props.dependencyDefinitions[this.classifications()].isFetching || !this.state.details) {
      return <Loading />;
    }
    return (
      <div className={style.component}>
      <h1>{this.headerTitle()}</h1>
      <form onSubmit={this.handleSubmit}>
        <Component
          details={this.state.details}
          onMasterDataFetch={this.props.onMasterDataFetch}
          masterData={this.props.masterData}
          onColumnChange={this.handleColumnChange}
          mode={"create"}
          editable={true}
          dependencyDefinition={this.props.dependencyDefinitions[this.classifications()] }
          componentType={this.props.componentType.toLowerCase()}
        />
        <div className={style.action}>
          <input id="add-button" className={button} type="submit" value={this.props.componentAdding ? 'Adding ...' : 'Add'} disabled={this.props.componentAdding} />
          <a id="cancel-button" className={button} onClick={this.showConfirmationDialog}>Cancel</a>
        </div>
      </form>
      <AlertDialog message={this.state.error.message} title="Warning" shown={this.state.error.shown} onClose={this.cancelErrorDialog} />
      <ConfirmationDialog message={`The ${this.props.componentType} will not be created. Do you wish to continue?`} title="Warning" shown={this.state.confirmation.shown} onYes={this.yesConfirmationDialog} onNo={this.noConfirmationDialog} />
    </div>);
  }
}
