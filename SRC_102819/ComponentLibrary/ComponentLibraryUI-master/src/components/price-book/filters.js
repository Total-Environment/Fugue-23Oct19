import * as React from "react";
import {HeaderColumn} from "../header-column";
import {col, updateClassificationDetails} from "../../helpers";

const fillFilterValues = (filters, filterValues) => filters.map(filter => ({
  ...filter,
  value: filterValues[filter.key] || ''
}));

const isClassificationColumn = column => ['level1', 'level2', 'level3'].includes(column.key);

const getDependencyDefinition = (componentType, dependencyDefinitions) => {
  let key = componentType.toLowerCase();
  return dependencyDefinitions[`${key}Classifications`];
};
const populateDependencyValues = (values, dependencyDefinitions) => {
  const componentType = col({columns: values}, 'componentType', 'value');
  if (!componentType) return values;

  const dependencyDefinition = getDependencyDefinition(componentType, dependencyDefinitions);
  const classificationColumns = values.filter(isClassificationColumn);
  const updatedClassificationColumns = updateClassificationDetails(classificationColumns, dependencyDefinition.values.block.children);
  return values.map(c => isClassificationColumn(c) ? col({columns: updatedClassificationColumns}, c.key) : c);
};
const filters = (projects) => [
  {
    name: 'Component Type',
    key: 'componentType',
    dataType: {
      name: 'MasterData',
      values: {isFetching: false, values: ['Material', 'Service', 'SFG', 'Package']}
    },
    editable: true,
    value: '',
    validity: {isValid: true, msg: ''}
  },
  {
    name: 'Applied On',
    key: 'appliedOn',
    dataType: {name: 'Date'},
    editable: true,
    value: '',
    validity: {isValid: true, msg: ''}
  },
  {
    name: 'Classification Level 1',
    key: 'level1',
    dataType: {name: 'MasterData'},
    editable: true,
    value: '',
    validity: {isValid: true, msg: ''}
  },
  {
    name: 'Classification Level 2',
    key: 'level2',
    dataType: {name: 'MasterData'},
    editable: true,
    value: '',
    validity: {isValid: true, msg: ''}
  },
  {
    name: 'Classification Level 3',
    key: 'level3',
    dataType: {name: 'MasterData'},
    editable: true,
    value: '',
    validity: {isValid: true, msg: ''}
  },
  {
    name: 'Project',
    key: 'projectCode',
    dataType: {name: 'Project', projects},
    editable: true,
    value: '',
    validity: {isValid: true, msg: ''}
  },
];

const changeClassificationFilterValues = (columnKey, value) => {
  if (!['componentType', 'level1', 'level2'].includes(columnKey)) {
    return {[columnKey]: value};
  }
  if (columnKey === 'componentType') {
    return {
      [columnKey]: value,
      ...changeClassificationFilterValues('level1', '')
    };
  } else if (columnKey === 'level1') {
    return {
      [columnKey]: value,
      ...changeClassificationFilterValues('level2', '')
    };
  } else if (columnKey === 'level2') {
    return {
      [columnKey]: value,
      level3: '',
    };
  }
};

const hideLevel3ValuesIfRequired = ({componentType}, columns) => (componentType && componentType.toLowerCase()) !== 'material' ? columns.filter(c => c.key !== 'level3') : columns;

export const PriceBookFilters = (props) => {
  return <HeaderColumn
    headers={[{
      name: 'Cost Price Ratio',
      key: 'price-book',
      columns: hideLevel3ValuesIfRequired(props.filters, populateDependencyValues(fillFilterValues(filters(props.projects), props.filters), props.dependencyDefinitions))
    }]}
    activeHeaders={['price-book']}
    onChange={(headerKey, columnKey, value) => props.onChange(changeClassificationFilterValues(columnKey, value))}
    showArrow={false}
  />;
};
