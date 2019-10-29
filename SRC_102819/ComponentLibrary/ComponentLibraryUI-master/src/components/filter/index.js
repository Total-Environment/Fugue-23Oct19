import React from 'react';
import filterStyles from './index.css';
import Modal from 'react-modal';
import {Tab, Tabs, TabList, TabPanel} from 'react-tabs';
import {idFor} from '../../helpers';
import {Icon} from '../icon';
import DataType from '../data-types/index';
import flexboxgrid from 'flexboxgrid/css/flexboxgrid.css';
import {button} from '../../css-common/forms.css';
import * as R from "ramda";
import {isNumber, validate} from '../../validator';
import {alertAsync} from "../dialog/index";


export class Filter extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      selectedTab: 'Classification',
      columns: this._generateInitialFilterFields(props),
    };

    this._onFilterChange = this._onFilterChange.bind(this);
    this.onApplyFilter = this.onApplyFilter.bind(this);
    this.isFormValid = this.isFormValid.bind(this);
  }


  _updateClassificationData() {
    let filterClassification = this.state.columns.find(c => c.name.toLowerCase() === 'classification');
    if (!filterClassification) {
      return;
    }
    this._setClassificationColumns(filterClassification.columns.map(c => ({value: c, name: c.name})),
      this.props.classificationData.values.block.children);
  }

  _generateInitialFilterFields(props) {
    let componentType = props.componentType;
    let filterColumnData = [];
    let classifications = (props.classificationData && props.classificationData.values) || {};
    let columnList = classifications.columnList || [];
    let initialBlockChildren = (classifications.block && classifications.block.children) || [];

    // Build classification columns
    let classificationColumns = columnList.map(c => {
      let values = [];
      if (c.toLowerCase() === `${componentType} level 1`) {
        values = initialBlockChildren.map(c => c.name);
      }
      return {
        name: c,
        editable: true,
        key: c.toLowerCase().replace(/ /gi, '_'),
        value: (R.find(({columnKey}) => columnKey === c.toLowerCase().replace(/ /gi, '_'), props.filters) || {columnValue: ''}).columnValue,
        dataType: {
          name: 'MasterData',
          value: '',
          values: {
            values
          }
        },
        validity: {isValid: true, msg: ''}
      }
    });

    // Build Other tab columns
    let otherColumns = [];
    otherColumns.push({
      name: 'Status',
      key: `${componentType}_status`,
      editable: true,
      value: (R.find(({columnKey}) => columnKey === `${componentType}_status`, props.filters) || {columnValue: ''}).columnValue,
      isRequired: true,
      dataType: {
        name: 'MasterData',
        values: {
          values: this.props.masterData && this.props.masterData['status'].values.values
        }
      },
      validity: {isValid: true, msg: ''}
    });
    otherColumns.push({
      name: 'Location',
      key: 'Location',
      isRequired: true,
      editable: true,
      value: (R.find(({columnKey}) => columnKey === 'Location', props.filters) || {columnValue: ''}).columnValue,
      dataType: {
        name: 'MasterData',
        values: {
          values: this.props.masterData && this.props.masterData['location'].values.values
        }
      },
      validity: {isValid: true, msg: ''}
    });
    otherColumns.push({
      name: 'Mode of Purchase',
      key: 'TypeOfPurchase',
      editable: true,
      value: (R.find(({columnKey}) => columnKey === 'TypeOfPurchase', props.filters) || {columnValue: ''}).columnValue,
      dataType: {
        name: 'MasterData',
        values: {
          values: this.props.masterData && this.props.masterData['type_of_purchase'].values.values
        }
      },
      validity: {isValid: true, msg: ''}
    });
    otherColumns.push({
      name: 'Applied On',
      key: 'AppliedOn',
      editable: true,
      isRequired: true,
      value: (R.find(({columnKey}) => columnKey === 'AppliedOn', props.filters) || {columnValue: ''}).columnValue,
      dataType: {
        name: 'DatePicker',
        subType: null
      },
      validity: {isValid: true, msg: ''}
    });

    filterColumnData.push({
      name: 'Classification',
      columns: classificationColumns
    });

    filterColumnData.push({
      name: 'Rate Attributes',
      columns: otherColumns
    });
    return filterColumnData;
  }

  _setClassificationColumns(columns, children) {
    const currentColumn = columns[0];
    if (!currentColumn) return;
    currentColumn.value.dataType.values = {
      values: children.map(child => child.name).filter(name => name !== "null"),
      status: 'fetched'
    };
    if (!currentColumn.value.value) {
      columns.filter((c, i) => i !== 0).forEach(column => {
        column.value.value = '';
        if (column.value.dataType.values) {
          column.value.dataType.values.values = [];
        }
      });
    }
    const remainingDependencyDefinition = children.find(child => child.name === currentColumn.value.value);
    if (!remainingDependencyDefinition) {
      columns.filter((c, i) => i !== 0).forEach(column => {
        column.value.value = '';
        if (column.value.dataType.values) {
          column.value.dataType.values.values = [];
        }
      });
      return;
    }
    this._setClassificationColumns(columns
      .filter(columnName => columnName.name !== "key" && columnName.name !== "name")
      .filter((c, i) => i !== 0), remainingDependencyDefinition.children);
  }



  _onTabChange(header) {
    this.setState({selectedHeader: header});
  }

  _renderHeader() {
    let headers = this.state.columns.map(c => c.name);
    let selectedTab = headers.indexOf(this.state.selectedHeader);
    return (<Tabs selectedIndex={selectedTab} onSelect={i => this._onTabChange(i)}>
      <TabList>
        {headers.map(h => <Tab>{h}</Tab>)}
      </TabList>
      {this.state.columns.map(c => this._renderFilterFields(c))}
    </Tabs>);
  }

  _renderColumn(column, headerName) {
    let columnName = column.name;
    return (<div className={[filterStyles.column, flexboxgrid['col-sm-3']].join(' ')}>
      <dt id={idFor(columnName, 'title')} className={filterStyles.columnTitle}>{columnName}</dt>
      <dd id={idFor(columnName, 'value')} className={filterStyles.columnValue}>
        <DataType columnName={columnName}
                  columnValue={column}
                  componentType={this.props.componentType.toLowerCase()}
                  onChange={(value) => this._onFilterChange(column.key, value, headerName, column)}
        />
      </dd>
    </div>);
  }

  isFormValid() {
    const newFilterColumns = JSON.parse(JSON.stringify(this.state.columns));
    newFilterColumns.forEach(headers => headers.columns.forEach(column => column.validity = validate(column.value, column)));
    this.setState({columns: newFilterColumns});
    const isValid = !newFilterColumns.find(header => header.columns
      .find(column => {
        if (!column.validity.isValid) return column;
        if (column.isRequired && column.value === '') return column;
      }));
    return isValid;
  }

  _onFilterChange(key, value, headerName, column) {
    let formattedValue = value && value.toISOString ? value.toISOString() : value;
    let filterColumns = this.state.columns.map(c => {
      if (c.name.toLowerCase() === headerName.toLowerCase()) {
        return Object.assign({}, c, {
          columns: c.columns.map(c => c.key === key ? Object.assign({}, c, {
            value: formattedValue
          }) : c)
        });
      }
      return c;
    });
    filterColumns.forEach(headers => headers.columns.forEach(column => column.validity = validate(column.value, column)));
    this.setState({columns: filterColumns});
  }

  onApplyFilter(e) {
    e.preventDefault();
    if (!this.isFormValid()) {
      alertAsync('Error', 'There are error in the form. Please fix them.');
      return;
    }
    const newFilters = R.flatten(this.state.columns.map(headers => headers.columns.map(c => ({
      columnKey: c.key,
      columnValue: c.value
    })))).filter(({columnValue}) => !!columnValue);
    this.props.applyFilter(newFilters);
  }

  _renderFilterFields(columns) {
    return <TabPanel>
      <div className={[filterStyles.header, flexboxgrid.row].join(' ')}>
        {columns.columns.map(c => this._renderColumn(c, columns.name))}
      </div>
    </TabPanel>;
  }

  render() {
    this._updateClassificationData();
    return <Modal
      className={filterStyles.filterDialog}
      isOpen={this.props.isOpen}
      contentLabel="Filter file">
      <div className={filterStyles.header} id={idFor('filter-header')}>
        <h1
          className={filterStyles.title}>
          Filters
        </h1>
        <button
          className={filterStyles.button}
          id={idFor('close')}
          onClick={e => this.props.closeDialog(e)}>
          <Icon name="close" className={close}/>
        </button>
      </div>
      <div>
        {this._renderHeader()}
      </div>
      <div className={filterStyles.actionItems}>
        <button
          className={[filterStyles.apply, button].join(' ')}
          id={idFor('apply')}
          onClick={e => this.onApplyFilter(e)}>
          Apply
        </button>
        <button
          className={[filterStyles.clear, button].join(' ')}
          id={idFor('clear')}
          onClick={e => this.props.resetFilter(e)}>
          Reset
        </button>
      </div>
    </Modal>
  }
}
