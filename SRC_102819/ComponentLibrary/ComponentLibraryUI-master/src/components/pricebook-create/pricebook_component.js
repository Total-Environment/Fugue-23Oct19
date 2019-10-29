
import * as React from "react";
import styles from './index.css';
import Modal from 'react-modal';
import searchStyles from './index.css';
import flexboxgrid from 'flexboxgrid/css/flexboxgrid.css';
import { idFor } from '../../helpers';
import DataType from '../data-types';
import AnimateHeight from "react-animate-height";
import classNames from 'classnames';
import {isFetched, toIST} from "../../helpers";
import {Loading} from "../loading/index";
import * as R from "ramda";

export class PriceBookComponent extends React.Component {
  constructor(props) {
    super(props);
    this.initialState = {
      currentSelection: [],
      levelValues: [],
      currentLevel: 0,
      columns: []
    }

    this.state = R.clone(this.initialState);
  }

  componentWillReceiveProps(nextProps) {
    if(this.props.componentType != nextProps.componentType) {
    this.state = {
      currentSelection: [],
      levelValues: [],
      currentLevel: 0,
      columns: []
    }
    }
  }

  onFieldValueChange(level,value) {
    if(value === "")
      value = null;
    this.state.currentSelection[level].selected = value;
    let nextLevel = level+1;
    if(this.props.componentType == 'material' || level != 1){
      let childrenVals = this.state.currentSelection[level].children.filter(m=>m.name == value);
      this.state.currentSelection[nextLevel] =
        {selected: null,children: childrenVals.length == 0? null:  childrenVals[0].children};
      this.state.currentSelection.slice(nextLevel+1,3).map((m)=> {m.children = null;m.selected=null});
    }
    this.setState({
      currentLevel: nextLevel
    })

    let levelValues = this.state.currentSelection.map((m)=>m.selected);
    let nonNullLevelValues = levelValues.filter((m)=> m != null);
    this.props.accumulateResults(nonNullLevelValues.length ==0 ? null:levelValues);
  }

  getColumnsToRender() {
    if(this.state.currentSelection.length == 0) {
      this.state.currentSelection[this.state.currentLevel] =
        {selected: null, children: this.props.classificationData.values.block.children};
      if(this.props.componentType == 'material') {
        this.state.currentSelection[1] = {selected: null, children: null};
        this.state.currentSelection[2] = {selected: null, children: null};
      }
      else {
        this.state.currentSelection[1] = {selected: null, children: null};
      }
    }

    let columnList = this.props.classificationData.values.columnList;
    let columns = this.state.currentSelection.slice(0,3).map((m,i) => {
      let childrenNames = null;
      if(m.children)
        childrenNames = m.children.map((m) => m.name);
      return {
        name: columnList[i],
        key: i,
        value: m.selected || '',
        editable: true,
        dataType: {
          name: 'MasterData',
          values: {values: childrenNames || [], status: 'fetched'}
        },
        validity: {isValid: true, msg: ''}
      }
    });
    return columns;
  }

  renderColumnFields(column) {
    let titleElement = <span>{column.name}</span>;
    return <div key={column.key} className={classNames(searchStyles.column, flexboxgrid['col-sm-2'])}>
      <dt id={idFor(column.name, 'title')} className={searchStyles.columnTitle}>{titleElement}</dt>
      <dd id={idFor(column.name, 'value')} className={searchStyles.columnValue}>
        <DataType
          columnName={column.name}
          columnValue={column}
          mode={this.props.mode}
          componentType={this.props.componentType}
          onChange={val => this.onFieldValueChange(column.key, val)} />
      </dd>
    </div>
  }

  render() {
    return <div className={classNames(flexboxgrid.row, searchStyles.columnsRow)}>
      {this.getColumnsToRender().map((m)=>this.renderColumnFields(m))}
    </div>
  }


}
