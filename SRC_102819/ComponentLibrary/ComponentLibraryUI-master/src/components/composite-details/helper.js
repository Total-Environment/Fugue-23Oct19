'use strict';
import * as R from 'ramda';
import {head} from "../../helpers";

export function updateClassificationDetails(columns, children) {
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
    return columns;
  }
  const remainingDependencyDefinition = children.find(child => child.name === currentColumn.value.value);
  if (!remainingDependencyDefinition) {
    columns.filter((c, i) => i !== 0).forEach(column => {
      column.value.value = '';
      if (column.value.dataType.values) {
        column.value.dataType.values.values = [];
      }
    });
    return columns;
  }
  return R.prepend(currentColumn, updateClassificationDetails(columns
    .filter(columnName => columnName.name !== "key" && columnName.name !== "name")
    .filter((c, i) => i !== 0), remainingDependencyDefinition.children));
};

export function getColumn(name, key, value, dataType, editable, validity) {
  let transformedKey = key || name.toLowerCase().split(' ').join('_');
  return {
    name: name,
    key: transformedKey,
    value,
    dataType: R.merge({name: 'String', subType: null}, dataType),
    editable,
    validity: validity || {isValid: true, msg: ''}
  };
}

export function transformCompositionDetailToColumns(detail) {
  let columns = [];
  let handlingWastage = (detail.wastagePercentages.find(c => c.name.toLowerCase() === 'handling/storage wastage%') || {}).value || '';
  let executionWastage = (detail.wastagePercentages.find(c => c.name.toLowerCase() === 'execution wastage%') || {}).value || '';
  columns.push(
    Object.assign({},
      getColumn('Resource Type', null, detail.componentType, {}, false)));
  columns.push(
    getColumn(
      'Component Code',
      'component_code',
      detail.code, {}, false)
  );
  columns.push(R.merge({isRequired: detail.componentType.toLowerCase() === 'asset'}, getColumn(
    'Unit of Measure',
    'unit_of_measure',
    detail.unitOfMeasure,
    detail.componentType.toLowerCase() === 'asset' ? {
      name: 'MasterData',
      values: {
        values: ['Daily', 'Monthly'],
        status: 'fetched'
      }
    } : {},
    false
  )));

  columns.push(
    getColumn('Coefficient', 'coefficient', detail.coefficient,
      {name: 'Decimal'}, true)
  );

  columns.push(
    getColumn('Handling/Storage Wastage%', 'handling_storage_wastage', handlingWastage,
      {name: 'Decimal'},
      true)
  );
  columns.push(
    getColumn('Execution Wastage%', 'execution_wastage', executionWastage,
      {name: 'Decimal'}, true)
  );
  return columns;
}

const nameKey = (type) => {
  const keys = {
    material: 'material_name',
    package: 'package_name',
    sfg: 'short_description',
    service: 'short_description',
    asset: 'short_description'
  };
  return keys[type.toLowerCase()];
};

export function transformCompositionDetail(detail) {
  let columns = [];
  detail.type = detail.type.toLowerCase() === 'sfg' ? 'SFG' : detail.type;
  let general = detail.headers.find(header => header.key === 'general');
  let name = head(detail, 'general', nameKey(detail.type));
  columns.push(
    Object.assign({},
      getColumn('Resource Type', null, detail.type, {}, false),
      {isPrimary: detail.isPrimary || false})
  );
  columns.push(
    getColumn(
      'Component Code',
      'component_code',
      detail.id || detail.code, {}, false)
  );
  columns.push(
    getColumn('Component Name',
      'component_name',
      ((name && name.value) || ''),
      {},
      false)
  );
  columns.push(
    detail.type.toLowerCase() === 'asset' ? R.merge({isRequired: true}, getColumn(
      'Unit of Measure',
      'unit_of_measure',
      '',
      {
        name: 'MasterData',
        values: {
          values: ['Daily', 'Monthly'],
          status: 'fetched'
        }
      },
      true
    )) : general.columns.find(column => column.key === 'unit_of_measure')
  );

  columns.push(
    getColumn('Coefficient', 'coefficient', '0',
      {name: 'Decimal'}, true)
  );

  columns.push(
    getColumn('Handling/Storage Wastage%', 'handling_storage_wastage', '0',
      {name: 'Decimal'},
      true)
  );
  columns.push(
    getColumn('Execution Wastage%', 'execution_wastage', '0',
      {name: 'Decimal'}, true)
  );
  return columns;
}
