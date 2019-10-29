import React from 'react';
import Modal from 'react-modal';
import styles from './index.css';
import { close } from './index.css';
import { Icon } from '../icon';
import {head, idFor, updateColumn} from '../../helpers';
import { Loading } from "../loading/index";
import { FilterDataComponent } from '../filter-data-component';
import { validate } from '../../validator';
import { AlertDialog } from "../alert-dialog/index";

export class FilterComponent extends React.Component {

  static defaultProps = {
    validBrandFields: ['brand_series', 'manufacturers_code', 'manufacturers_name', 'brand_code'],
    enums: {
      GENERIC_BRAND: 'Generic Brand'
    }
  };

  constructor(props) {
    super(props, FilterComponent.defaultProps);
    this.state = {
      filterData: props.modifiedDefinition || this.initializeDataForDefinition(
        props.componentType === 'SFG' ?
          (props.definitions['sfg'] && props.definitions['sfg'].values) :
          props.componentType === 'package' ? (props.definitions['package'] && props.definitions['package'].values):
          props.definitions[props.group]), error: {
            message: this.props.errorInFilter.message,
            shown: this.props.errorInFilter.shown
          }
    };
    this.handleColumnChange = this.handleColumnChange.bind(this);
    this.cancelErrorDialog = this.cancelErrorDialog.bind(this);
    this.isValidBrandField = this.isValidBrandField.bind(this);

  }

  isValidBrandField(field) {
    return this.props.validBrandFields.indexOf(field.replace(/\//g, '_').replace(/'/g, '').toLowerCase()) > -1;
  }



  componentDidMount() {
    if (this.props.componentType === 'material' && this.props.group && !this.props.definitions[this.props.group]) {
      this.props.onMaterialDefinitionFetch(this.props.group);
      this.props.onBrandDefinitionFetch();
    }

    if (this.props.componentType === 'service' && this.props.group && !this.props.definitions[this.props.group]) {
      this.props.onServiceDefinitionFetch(this.props.group);
    }

    if (this.props.componentType === 'SFG' && !this.props.definitions['sfg']) {
      this.props.onSFGDefinitionFetch();
    }

    if (this.props.componentType === 'package' && !this.props.definitions['package']) {
      this.props.onPackageDefinitionFetch();
    }

    if(!this.props.dependencyDefinitions[this.classifications()]) {
      this.props.onDependencyDefinitionFetch(this.classifications());
    }
  }

  classifications() {
    switch (this.props.componentType.toLowerCase()) {
      case 'material':
        return 'materialClassifications';
        break;
      case 'service':
        return 'serviceClassifications';
        break;
      case 'sfg':
        return 'sfgClassifications';
        break;
      case 'package':
        return 'packageClassifications';
        break;
    }
  }

  render() {
    return (
      <Modal className={styles.filterDialog} isOpen={true} contentLabel="Filter file">
        <div className={styles.header} id={idFor('filter-header')}>
          <h1 className={styles.title}>Filters</h1>
          <button className={styles.button} id={idFor('close')} onClick={(e) => { e.preventDefault(); this.props.onClose() }}><Icon name="close" className={close} /></button>
        </div>
        <div id={idFor('filter-data')}>
          {(this.props.definitions && this.props.definitions[this.props.group] && this.props.dependencyDefinitions[this.classifications()]) ?
            <FilterDataComponent definition={this.state.filterData}
              group={this.props.group}
              componentType={this.props.componentType}
              onColumnChange={this.handleColumnChange}
              dependencyDefinition={this.props.dependencyDefinitions[this.classifications()]}
              levels={this.props.levels}
            /> : <Loading />}
        </div>
        <div className={styles.actionItems}>
          <button className={styles.apply}
            id={idFor('apply')}
            onClick={(e) => { e.preventDefault(); this.props.onApply(this.state.filterData) }}>Apply</button>
          <button className={styles.clear}
            id={idFor('clear')}
            onClick={(e) => { e.preventDefault(); this.props.onClear(this.state.filterData) }}>Clear Filters</button>
        </div>
        {<AlertDialog message={this.state.error.message} title="Warning" shown={this.state.error.shown} onClose={this.cancelErrorDialog} />}
      </Modal>);
  }

  cancelErrorDialog() {
    this.setState({ error: { message: '', shown: false } });
    this.props.onCancelErrorDialog();
  }

  handleColumnChange(headerKey, columnKey, value) {
    let newState = JSON.parse(JSON.stringify(this.state));
    let header = newState.filterData.headers.find(header => header.key === headerKey);
    if(!header) {
      return;
    }
    newState.filterData = updateColumn(newState.filterData, headerKey, columnKey, {validity: validate(value, head(newState.filterData, headerKey, columnKey), columnKey)});
    newState.filterData = updateColumn(newState.filterData, headerKey, columnKey, {value: value});
    this.setState(newState);
  }

  concatBrandColumns(headerDefinitions = []) {
    let _self = this;
    let genericBrand = (this.props.brandDefinitions || {})[_self.props.enums.GENERIC_BRAND];
    let brandColumns = ((genericBrand && genericBrand.columns) || []).filter(x => _self.isValidBrandField(x.key));

    let purchaseIndex = headerDefinitions.findIndex(x => (x && x.key) === 'purchase');

    if (purchaseIndex > -1) {
      let columns = (headerDefinitions[purchaseIndex].columns || []);
      // Append brand columns to existing purchase columns
      let newColumns = [];
      columns.forEach(definitionColumn =>
      {
        let shared = false;
        brandColumns.forEach(brandColumn => {
          if(definitionColumn.key === brandColumn.key) {
            shared = true;
          }
        });
        if(!shared) {newColumns.push(definitionColumn)}
      });
      headerDefinitions[purchaseIndex].columns = newColumns.concat(brandColumns).filter(x => x.key !== 'approved_brands');
    }
    return headerDefinitions;
  }

  setDetails(props, state) {
    let filterData = Object.assign({}, state.filterData, this.initializeDataForDefinition(
      props.componentType === 'SFG' ? (props.definitions['sfg'] && props.definitions['sfg'].values) :
        props.componentType === 'package' ? (props.definitions['package'] && props.definitions['package'].values) :
        props.definitions[props.group]));
    this.setState({ filterData: filterData, brandDefinition: props.brandDefinitions });
  }

  initializeDataForDefinition(definition) {
    if (!definition) return;
    let headers = this.concatBrandColumns(definition.headers)
      .filter(header => header.key !== "maintenance" && header.key !== "system_logs" && header.key !== 'edesign_specifications')
      .reduce((headerObj, header) => {
          let columns = header.columns.filter(column => column.dataType.name !== 'Constant'
          && column.dataType.name !== 'StaticFile' && column.dataType.name !== 'CheckList')
            .filter(column => column.dataType.name !== 'Array'
            || (column.dataType.subType && column.dataType.subType.name !== 'StaticFile'))
            .reduce((columnObj, column) => {
              columnObj.push({
                dataType: column.dataType,
                isRequired: column.isRequired,
                value: null,
                editable: true,
                filterable: true,
                validity: {isValid: true, msg: ''},
                key: column.key,
                name: column.name,
                isClassification: header.key === "classification"
              });
              return columnObj;
            },
            []);
          headerObj.push({columns: columns, key: header.key, name: header.name});
          return headerObj;
        },
        []);
    return {headers};
  }

  componentWillReceiveProps(nextProps) {
    if (this.props.definitions[this.props.group] !== nextProps.definitions[this.props.group]) {
      this.setDetails(nextProps, this.state);
    }

    if (nextProps.errorInFilter.shown && !this.state.error.shown) {
      this.setState({ error: { message: nextProps.errorInFilter.message, shown: nextProps.errorInFilter.shown } });
    }
  }
}

