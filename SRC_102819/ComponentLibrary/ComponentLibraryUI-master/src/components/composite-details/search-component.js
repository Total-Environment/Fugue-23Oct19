import React from 'react';
import * as R from 'ramda';
import searchStyles from './search.css';
import flexboxgrid from 'flexboxgrid/css/flexboxgrid.css';
import { idFor, isFetched } from '../../helpers';
import classNames from 'classnames';
import DataType from '../data-types';
import { updateClassificationDetails } from './helper';
import { ComponentList } from './component-list';


export class SearchComponent extends React.Component {
  static get headerName() {
    return 'Component Search';
  }

  static get headerKey() {
    return 'sfg-key'
  }

  constructor(props) {
    super(props);

    this.initialState = {
      materialClassifications: null,
      serviceClassifications: null,
      activeResource: null,
      materialClassificationColumns: [],
      serviceClassificationColumns: [],
      assetClassificationColumns: [],
      statusTypeColumn: {
        name: 'Status',
        key: 'status',
        value: '',
        editable: true,
        dataType: {
          name: 'MasterData',
          values: { values: [], status: 'fetched' }
        },
        validity: { isValid: true, msg: '' }
      },
      resourceTypeColumn: {
        name: 'Resource Type',
        key: 'resource_type',
        value: '',
        editable: true,
        dataType: {
          name: 'MasterData',
          values: { values: this.props.resources || [], status: 'fetched' }
        },
        validity: { isValid: true, msg: '' }
      }
    };
    this.state = R.clone(this.initialState);

    this._onChooseComponents = this._onChooseComponents.bind(this);
    this._constructColumnsFromClassificationData = this._constructColumnsFromClassificationData.bind(this);
    this._renderFilterRow = this._renderFilterRow.bind(this);
    this._renderColumnFields = this._renderColumnFields.bind(this);
    this._triggerSearchFilter = this._triggerSearchFilter.bind(this);
    this._renderSearchResult = this._renderSearchResult.bind(this);
    this._resetFilterValues = this._resetFilterValues.bind(this);
  }

  _resetFilterValues() {
    this.props.resetSearch();
    this.setState({
      activeResource: null,
      materialClassificationColumns: this.state.materialClassificationColumns.map(c => Object.assign({}, c, { value: '' })),
      serviceClassificationColumns: this.state.serviceClassificationColumns.map(c => Object.assign({}, c, { value: '' })),
      assetClassificationColumns: this.state.assetClassificationColumns.map(c => Object.assign({}, c, { value: '' })),
      statusTypeColumn: Object.assign({}, this.state.statusTypeColumn, { value: '' }),
      resourceTypeColumn: Object.assign({}, this.state.resourceTypeColumn, { value: '' })
    })
  }

  _constructColumnsFromClassificationData(classificationData, type) {
    let columnData = [];
    let classifications = (classificationData && classificationData.values) || {};
    let columnList = classifications.columnList || [];
    let initialBlockChildren = (
      classifications.block && classifications.block.children
    );

    let classificationColumns = columnList.map(c => {
      let values = [];
      if (R.contains('level 1', c.toLowerCase())) {
        values = initialBlockChildren.map(d => d.name);
      }
      let name = type ? c
        .split(' ')
        .map((d, i) => i === 0 ? (type.charAt(0).toUpperCase() + type.slice(1)) : d)
        .join(' ') : c;

      return {
        name,
        editable: true,
        key: c.toLowerCase().replace(/ /gi, '_'),
        value: '',
        dataType: {
          name: 'MasterData',
          values: {
            status: 'fetched',
            values
          }
        },
        validity: { isValid: true, msg: '' }
      }
    });
    return classificationColumns;
  }

  componentWillMount() {
    let nextProps = this.props;
    let classificationData = nextProps.classificationData || {};
    let materialClassifications = classificationData.materialClassifications;
    let serviceClassifications = classificationData.serviceClassifications;
    let sfgClassifications = classificationData.sfgClassifications;
    let statusData = nextProps.statusData || null;

    if (materialClassifications
      && !materialClassifications.isFetching
      && !R.isEmpty(materialClassifications.values)
      && R.isNil(this.state.materialClassifications)) {
      this.setState({
        materialClassifications,
        assetClassificationColumns: this._constructColumnsFromClassificationData(materialClassifications, 'Asset'),
        materialClassificationColumns: this._constructColumnsFromClassificationData(materialClassifications)
      });
    }
    if (serviceClassifications
      && !serviceClassifications.isFetching
      && !R.isEmpty(serviceClassifications.values)
      && R.isNil(this.state.serviceClassifications)) {
      this.setState({
        serviceClassifications,
        serviceClassificationColumns: this._constructColumnsFromClassificationData(serviceClassifications)
      });
    }

    if (isFetched(sfgClassifications) && R.isNil(this.state.sfgClassifications)) {
      this.setState({
        sfgClassifications,
        sfgClassificationColumns: this._constructColumnsFromClassificationData(sfgClassifications)
      })
    }

    let values = statusData.values.values || [];
    this.setState({
      statusTypeColumn: Object.assign({}, this.state.statusTypeColumn, {
        dataType: Object.assign({}, this.state.statusTypeColumn.dataType, {
          values: Object.assign({}, this.state.statusTypeColumn.dataType.values, {
            values
          })
        })
      })
    });
  }

  _triggerSearchFilter(pageNumber) {
    let activeResource = this.state.activeResource;
    if (R.isNil(activeResource) || R.isEmpty(activeResource)) {
      return;
    }
    let activeClassificationColumns = this.state[`${activeResource}ClassificationColumns`];
    let filterData = activeClassificationColumns
      .filter(c => !R.isNil(c.value) && !R.isEmpty(c.value))
      .map(c => ({ columnValue: c.value, columnKey: c.key }));
    let triggerLevel = activeResource === 'service' || activeResource === 'sfg' ? "1" : "2";
    let groupColumn = filterData.find(c => R.contains(triggerLevel, c.columnKey));

    if (!!groupColumn) {
      let statusColumnValue = this.state.statusTypeColumn.value || '';
      if (!R.isEmpty(statusColumnValue)) {
        filterData = R.append({
          columnKey: `${activeResource === 'asset' ? 'material' : activeResource}_status`,
          columnValue: statusColumnValue
        }, filterData);
      }

      if (activeResource === 'asset') {
        filterData = R.append({ columnKey: 'can_be_used_as_an_asset', columnValue: true }, filterData);
      } else if (activeResource === 'material') {
        filterData = R.append({ columnKey: 'can_be_used_as_an_asset', columnValue: false }, filterData);
      }
      return this.props.onTriggerSearch(this.state.activeResource, pageNumber || 1, groupColumn.columnValue, filterData);
    }
  }

  // Field On Change Event
  _onFieldValueChange(key, value) {
    if (key === 'resource_type') {
      let val = value && value.toLowerCase() || '';
      return this.setState({
        resourceTypeColumn: Object.assign({}, this.state.resourceTypeColumn, { value }),
        activeResource: val
      }, this._triggerSearchFilter);
    }
    if (key === 'status') {
      return this.setState({
        statusTypeColumn: Object.assign({}, this.state.statusTypeColumn, { value }),
        filterDetails: {
          type: (this.state.filterDetails && this.state.filterDetails.type)
        }
      }, this._triggerSearchFilter);
    }
    let classificationColumnKey = `${this.state.activeResource}ClassificationColumns`;
    let activeClassificationColumn = this.state[classificationColumnKey] || [];
    if (!R.isEmpty(activeClassificationColumn)) {
      let currentKey = +(R.last(key.split('_'))) || 0;
      return this.setState({
        [classificationColumnKey]: activeClassificationColumn.map(c => {
          if (c.key === key) {
            return Object.assign({}, c, {
              value
            });
          }
          let keyNumber = +(R.last(c.key.split('_'))) || 0;
          if (keyNumber > currentKey) {
            return Object.assign({}, c, { value: '' });
          }
          return c;
        })
      }, this._triggerSearchFilter);
    }
  }

  _onChooseComponents(components) {
    this.props.onChooseComponents(components);
    this._resetFilterValues();
  }

  _renderSearchResult() {
    let headers = [{
      key: 'general',
      name: 'General'
    }, {
      key: 'specifications',
      name: 'Specifications'
    }];
    return <ComponentList
      activeResource={this.state.activeResource}
      data={this.props.searchResults}
      headers={headers}
      onChooseComponents={this._onChooseComponents}
      onTriggerSearch={this._triggerSearchFilter}
    />
  }

  _renderColumnFields(column) {
    let titleElement = <span>{column.name}</span>;
    return <div key={column.key} className={classNames(searchStyles.column, flexboxgrid['col-sm-2'])}>
      <dt id={idFor(column.name, 'title')} className={searchStyles.columnTitle}>{titleElement}</dt>
      <dd id={idFor(column.name, 'value')} className={searchStyles.columnValue}>
        <DataType
          columnName={column.name}
          columnValue={column}
          mode={this.props.mode}
          componentType={this.props.componentType}
          onChange={val => this._onFieldValueChange(column.key, val)} />
      </dd>
    </div>
  }

  _renderFilterRow(type) {
    let renderedColumn = [];
    let classificationData = {};
    switch (type) {
      case 'material':
        renderedColumn = this.state.materialClassificationColumns;
        classificationData = this.state.materialClassifications;
        break;
      case 'service':
        renderedColumn = this.state.serviceClassificationColumns;
        classificationData = this.state.serviceClassifications;
        break;
      case 'asset':
        renderedColumn = this.state.assetClassificationColumns;
        classificationData = this.state.materialClassifications;
        break;
      case 'sfg':
        renderedColumn = this.state.sfgClassificationColumns;
        classificationData = this.state.sfgClassifications;
        break;
    }
    if (!R.isEmpty(classificationData) && !R.isNil(classificationData)) {
      updateClassificationDetails(
        renderedColumn.map(c => ({ value: c, name: c.name })),
        classificationData.values.block.children
      );
    }
    return renderedColumn.map(col => this._renderColumnFields(col));
  }

  render() {
    return (
      <div className={flexboxgrid.column}>
        <div className={classNames(flexboxgrid.row, searchStyles.columnsRow)}>
          {this._renderColumnFields(this.state.resourceTypeColumn)}
          {this._renderFilterRow(this.state.activeResource)}
          {this._renderColumnFields(this.state.statusTypeColumn)}
        </div>
        <div className={classNames(flexboxgrid.row, searchStyles.listContainer)}>
          {this._renderSearchResult()}
        </div>
      </div>);
  }
}
