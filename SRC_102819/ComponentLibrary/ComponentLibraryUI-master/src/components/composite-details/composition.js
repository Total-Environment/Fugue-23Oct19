import React from 'react';
import searchStyles from './search.css';
import compositionListStyles from './component-list.css';
import compositionStyle from './composition.css';
import flexboxgrid from 'flexboxgrid/css/flexboxgrid.css';
import classNames from 'classnames';
import {idFor} from '../../helpers';
import DataType from '../data-types';
import {Icon} from '../icon/index';
import {button} from '../../css-common/forms.css';
import * as R from 'ramda';

export class CompositeComposition extends React.Component {
  constructor(props) {
    super(props);
    this._renderRow = this._renderRow.bind(this);
    this._renderColumnFields = this._renderColumnFields.bind(this);
    this._onRemoveItemClick = this._onRemoveItemClick.bind(this);
    this._onMakePrimaryClick = this._onMakePrimaryClick.bind(this);
    this.initialState = {};

    this.state = R.clone(this.initialState);
  }

  _onRemoveItemClick(compCode) {
    this.props.onRemoveCompositionItem(compCode);
  }

  _onMakePrimaryClick(compCode) {
    this.props.onMakePrimaryItem(compCode);
  }

  _renderRow(details) {
    let detailList = details.values || [];
    let isPrimary = details.isPrimary || false;
    let compCode = (detailList.find(c => c.key === 'component_code') || {}).value;
    let compType = (detailList.find(c => c.key === 'resource_type') || {}).value;
    return <div id={`comp_${compCode}`} key={`comp_${compCode}`} className={
      classNames(searchStyles.borderBottom,
        compositionStyle.compositionBackground,
        compositionListStyles.flex)}>
      <div key={compCode} className={classNames(
        flexboxgrid.row,
        searchStyles.columnsRow,
        compositionStyle.compositionDetailsBackground,
        compositionListStyles.flexOne)}>
        {detailList.map(c => this._renderColumnFields(c, compType, compCode, isPrimary))}
        {this.props.mode === 'create' && <p className={compositionStyle.removeButton}>
          <a href="#" onClick={e => {
            e.preventDefault();
            return this._onRemoveItemClick(compCode)
          }}>Remove</a>
        </p>}
      </div>
    </div>;
  }

  _renderColumnFields(column, type, code, isPrimary) {
    let titleElement = <span>{column.name}</span>;
    return <div key={column.key} className={classNames(compositionStyle.compositionRow, flexboxgrid['col-sm-2'])}>
      <dt id={idFor(column.name, 'title')} className={searchStyles.columnTitle}>{titleElement}</dt>
      <dd id={idFor(column.name, 'value')} className={classNames(searchStyles.columnValue)}>
        <DataType
          columnName={column.name}
          columnValue={column}
          onChange={value => {
            this.props.onDetailChange(code, column, value);
          }}
          componentType={type}/>
      </dd>
      {type && type === 'service' && column.key === 'resource_type' && this.props.componentType.toLowerCase() !== 'package' ?
        <div
          id={idFor(column.name, 'primary')}
          className={compositionListStyles.primaryCheck}>
          {this.props.mode === 'create' && !!this.props.hasPrimary ?
            <div className={compositionListStyles.makePrimary}><input
              type="checkbox"
              name="checkbox"
              checked={isPrimary || false}
              onChange={e => this._onMakePrimaryClick(code)}/><span className={compositionListStyles.primaryText}>Make Primary</span>
            </div> : ''}
        </div> : ''}
    </div>
  }

  render() {
    return <div className={classNames(flexboxgrid.column)}>
      {this.props
        .compositionDetails
        .map(this._renderRow)}
    </div>;
  }
}
