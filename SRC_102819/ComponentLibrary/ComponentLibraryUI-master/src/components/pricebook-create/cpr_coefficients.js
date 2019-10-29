import * as React from "react";
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

export class CprCoefficients extends React.Component {
  constructor(props) {
    super(props);
    this.initialState = {
      enteredCprCoefficient: null
    }
    this.state = R.clone(this.initialState);
  }

  componentWillMount() {
    this.state.enteredCprCoefficient = this.props.enteredCprValues;
  }
  componentWillReceiveProps(nextProps) {
      this.state.enteredCprCoefficient = nextProps.enteredCprValues;
  }


  onCprChange(key,val) {
    if(val === null)
       val ="";
    let newCprCoeffs = R.clone(this.state.enteredCprCoefficient);
    newCprCoeffs[key].value = val;

    if((newCprCoeffs[key].value.match(/\./g) || [] ).length > 1) {
      newCprCoeffs[key].isValid = false;
      newCprCoeffs[key].msg = 'Not a valid number';
    }
    else {
      newCprCoeffs[key].isValid = true;
      newCprCoeffs[key].msg = '';
    }


    this.setState({
      enteredCprCoefficient : newCprCoeffs,
    });
    this.props.accumulateResults(newCprCoeffs);
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
          onChange={val => this.onCprChange(column.key, val)}
          className={searchStyles.coefficientInput}
        /> %
      </dd>
    </div>
  }

  renderCprCoefficients() {
      let cprs = [{name: 'Quality | Execution Risk',key: 'cpr_coefficient1'},
                {name: 'Custom Production',key: 'cpr_coefficient2'},
                {name: 'Profit',key: 'cpr_coefficient3'},
                {name: 'Corporate | Construction OH',key: 'cpr_coefficient4'},
                {name: 'Design Fee',key: 'cpr_coefficient5'},
                {name: 'Transportation',key: 'cpr_coefficient6'},
                {name: 'Rate Increase',key: 'cpr_coefficient7'},
                {name: 'Profit Margin',key: 'cpr_coefficient8'}];

    let cprColumns = cprs.map((m) =>{return{
      name: m.name,
      key: m.key,
      value: this.state.enteredCprCoefficient[m.key].value ,
      editable: true,
      dataType: {
        name: 'Decimal',
        values: {values: null , status: 'fetched'}
      },
      validity: {isValid: this.state.enteredCprCoefficient[m.key].isValid,
                 msg: this.state.enteredCprCoefficient[m.key].msg
                }
    }});
    return cprColumns.map(m=>this.renderColumnFields(m));
  }

  render() {
    return <div className={classNames(flexboxgrid.row, searchStyles.columnsRow)}>
      {this.renderCprCoefficients()}
    </div>
  }
}


