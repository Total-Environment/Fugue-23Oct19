import React from 'react';
import { HeaderColumn } from "../header-column/index";
import * as R from 'ramda';
import { updateClassificationDetails } from "../composite-details/helper";
import { updateIn } from "../../helpers";
import { validate } from '../../validator';

export class CreateHeaderColumn extends React.Component {
  constructor(props) {
    super(props);
    this.handleChange = this.handleChange.bind(this);
  }

  componentWillMount() {
    this.ensureMasterDataIsPresent(this.props);
  }

  componentWillReceiveProps(nextProps) {
    this.ensureMasterDataIsPresent(nextProps);
  }

  ensureMasterDataIsPresent(props) {
    // Populate MasterData if it is ID
    props.definition.headers
      .map(header => {
        header.columns.map((column) => {
          if (column.dataType.name === "MasterData" && !(column.dataType.values && column.dataType.values.status === 'fetched')) {
            const id = column.dataType.subType;
            props.masterData[id] || props.onMasterDataFetch(id);
          }
        });
      });
  }

  handleChange(headerKey, columnKey, value,columnName) {
    return this.props.onDetailsChange(headerKey, columnKey, value,columnName);
  }

  appendClassificationValues() {
    return this.props.definition.headers.filter(h => (h.key !== 'system_logs' && h.key !== 'edesign_specifications')).map(header => {
      return header.key === 'classification' ?
        R.merge(header, {
          columns: updateClassificationDetails(header.columns.map(c => ({
            value: c,
            name: c.name
          })), this.props.dependencyDefinition.values.block.children).map(c => c.value)
        }) : this.appendMasterDataValues(header);
    })
  }

  appendMasterDataValues(header) {
    return R.merge(header, {
      columns: header.columns
        .map(column =>
          column.dataType.name === 'MasterData' && R.is(String, column.dataType.subType) ?
            updateIn(column, this.props.masterData[column.dataType.subType], 'dataType', 'values') :
            column)
    });
  }

  render() {
    const values = this.appendClassificationValues();
    const classification = values.find(h => h.key === 'classification');
    const group = ((classification && classification.columns.find(c => R.contains(`level_1`, c.key))) || {}).value;
    return <HeaderColumn
      headers={values}
      group={group}
      componentType={this.props.componentType}
      onChange={this.handleChange}
    />;
  }
}
