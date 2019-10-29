import * as React from "react";
import styles from './index.css';
import Modal from 'react-modal';
import searchStyles from './index.css';
import flexboxgrid from 'flexboxgrid/css/flexboxgrid.css';
import {PriceBookComponent} from './pricebook_component';
import { idFor,tomorrowInIST } from '../../helpers';
import DataType from '../data-types';
import {CprCoefficients} from './cpr_coefficients';
import {button} from '../../css-common/forms.css';
import DatePicker from "react-datepicker";
import moment from 'moment-timezone';
import {alertAsync, confirmAsync} from '../dialog';
import {Icon} from '../icon';
import AnimateHeight from "react-animate-height";
import classNames from 'classnames';
import {isFetched, toIST} from "../../helpers";
import {Loading} from "../loading/index";
import * as R from "ramda";


export class
PriceBookCreate extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      activeResourceType: null,
      selectedToEnterCode: false,
      selectedClassificationDropDown: true,
      enteredComponentCode: null,
      enteredLevelValues: null,
      enteredCprValues: {cpr_coefficient1:{value:"0",isValid:true,msg:''},
                         cpr_coefficient2:{value:"0",isValid:true,msg:''},
                         cpr_coefficient3:{value:"0",isValid:true,msg:''},
                         cpr_coefficient4:{value:"0",isValid:true,msg:''},
                         cpr_coefficient5:{value:"0",isValid:true,msg:''},
                         cpr_coefficient6:{value:"0",isValid:true,msg:''},
                         cpr_coefficient7:{value:"0",isValid:true,msg:''},
                         cpr_coefficient8:{value:"0",isValid:true,msg:''},
                        },
      selectedDate: tomorrowInIST(new Date()),
      enteredProjectCode: null,
      isComponentOpen: true
    }
    this.handleSubmit = this.handleSubmit.bind(this);
  }

  componentWillMount () {
    if(!this.props.classificationData) {
      this.props.fetchDependencyDefinition('material');
      this.props.fetchDependencyDefinition('service');
      this.props.fetchDependencyDefinition('sfg');
      this.props.fetchDependencyDefinition('package');
      this.props.onFetchProjects();
    }

  }

  accumulateResults(key,val) {
    switch (key) {
      case 'level_values':
        this.state.enteredLevelValues = val;
        break;
      case 'cpr_values':
        this.state.enteredCprValues = val;
        break;
    }
  }
  onResourceTypeChange(key,val) {
    if(key === 'resource_type') {
      if(val==="")
        val = null;
      if(val === null) {
        this.resetState();
      }
      else {
        this.setState({activeResourceType : val,
                       enteredComponentCode: null,
                       enteredLevelValues: null,});
        this.props.onDestroyComponentDetails();
      }
    }
    else if(key === 'component_code') {
        this.setState({enteredComponentCode: val});
    }
    else if(key === 'projectCode') {
      if(val==="")
        val = null;
        this.setState({enteredProjectCode: val});
    }

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
          onChange={val => this.onResourceTypeChange(column.key, val)} />
      </dd>
    </div>
  }

  renderResourceType() {
    let resourceTypes = ['Material', 'Service', 'SFG','Package'];
    let fields =  {
      name: 'Resource',
      key: 'resource_type',
      value: this.state.activeResourceType || '',
      editable: true,
      dataType: {
        name: 'MasterData',
        values: {values: resourceTypes || [], status: 'fetched'}
      },
      validity: {isValid: true, msg: ''}
    }
     return this.renderColumnFields(fields);
  }


  fetchClassificationDetails(e){
     this.props.onGetComponentDetails(
       this.state.activeResourceType.toLowerCase() + 's', this.state.enteredComponentCode);
  }



  renderComponentCodeTextBox() {
    return <div key={'component_code'} className={classNames(searchStyles.column, flexboxgrid['col-sm-2'])}>
      <dt id={idFor('component code','title')} className={searchStyles.columnTitle}>Component Code</dt>
      <dd id={idFor('component code','value')} className={searchStyles.columnValue}>
        <input  className={classNames(this.props.className, styles.input)} value={this.state.enteredComponentCode || ''}
                onChange={(e) => this.onResourceTypeChange("component_code", e.target.value)}
                onBlur={(e)=> this.fetchClassificationDetails() }
        />
      </dd>
    </div>
  }


  renderComponent(resourceType) {
    let resourceClassification = resourceType+ "Classifications";
    if(this.isDataFetched(resourceClassification) == true ) {
      return <PriceBookComponent classificationData={this.props.classificationData[resourceClassification]}
                                 componentType={resourceType} accumulateResults={val => this.accumulateResults("level_values",val)}/>
    }
    else {
      return <Loading/>
    }
  }


  isDataFetched(resourceType) {
    if(this.props.classificationData[resourceType] &&
      this.props.classificationData[resourceType].isFetching == false)  {
      return true;
    }
    else {
      return false;
    }
  }


  validate() {
    if (this.state.activeResourceType === null) {
      alertAsync('Error', 'Please select resource type');
      return false;
    }

    if (this.state.selectedClassificationDropDown === true &&
        this.state.enteredLevelValues === null) {
      alertAsync('Error', 'Please select classification levels or choose to provide component code');
      return false;
    }

    if(this.state.selectedToEnterCode === true &&
      this.state.enteredComponentCode === null){
      alertAsync('Error', 'Please provide component code or choose to select classification levels');
      return false;
    }

    if(this.state.selectedDate == null) {
      alertAsync('Error', 'Please provide Applicable  From date');
      return false;
    }
  }


  handleSubmit(e) {
    e.preventDefault();

    let level1=null;
    let level2=null;
    let level3=null;

    if(this.validate() === false){
      return;
    }

    if(this.state.enteredComponentCode === null) {
      if(this.state.activeResourceType === 'Material') {
        level1 = this.state.enteredLevelValues[0],
          level2 = this.state.enteredLevelValues[1],
          level3 = this.state.enteredLevelValues[2]

      }
      else {
        level1 = this.state.enteredLevelValues[0],
          level2 = this.state.enteredLevelValues[1]
      }
    }

    let Columns =
      Object.values(this.state.enteredCprValues).map((m,i ) => { return {
        key: "coefficient" + (i+1),
        name: i==7? "Profit Margin":"Coefficient " +(i+1),
        value:{
        type:"%",
          value: m.value || 0
        }
      }});

    let resourceType = this.state.activeResourceType;
    if(resourceType === 'Sfg') {
      resourceType = 'SFG';
    }


    let dataToPost = {
      "CprCoefficient":{Columns},
      "componentType":resourceType,
      "code":this.state.enteredComponentCode,
      "projectCode": this.state.enteredProjectCode,
      "appliedFrom":moment(this.state.selectedDate),
      "level1":level1,
      "level2":level2,
      "level3":level3
    };


    return confirmAsync('Confirmation Required', 'You wish to add a new CPR version for the specified component(s)?').then(() => {
      return this.props.onAddCpr(dataToPost);
    }).then(()=>{
      this.props.onCloseModal(this.props.callingComponent);
      return this.setState({isComponentOpen: false});
    });
  }

  resetState() {
    this.setState ({
      activeResourceType: null,
      selectedToEnterCode: false,
      selectedClassificationDropDown: true,
      enteredComponentCode: null,
      enteredLevelValues: null,
      enteredCprValues: {cpr_coefficient1:{value:"0",isValid:true,msg:''},
                         cpr_coefficient2:{value:"0",isValid:true,msg:''},
                         cpr_coefficient3:{value:"0",isValid:true,msg:''},
                         cpr_coefficient4:{value:"0",isValid:true,msg:''},
                         cpr_coefficient5:{value:"0",isValid:true,msg:''},
                         cpr_coefficient6:{value:"0",isValid:true,msg:''},
                         cpr_coefficient7:{value:"0",isValid:true,msg:''},
                         cpr_coefficient8:{value:"0",isValid:true,msg:''},
                        },
      selectedDate: tomorrowInIST(new Date()),
      enteredProjectCode: null,
      isComponentOpen: true
    });
  }

  onCancelClick() {
    return confirmAsync('Warning', 'CPR will not be created. Do you wish to continue?').then(() => {
      this.resetState();
      this.props.onDestroyComponentDetails();
    });


  }



  renderAddCancelButtons() {
    return <div className={styles.action}>
      <input id="add-button" className={button} type="submit" value='Add' />
       <a className={button}
          onClick={e => this.onCancelClick()}>Cancel</a>
    </div>
  }



  renderLevel(title,key,value) {
    let titleElement = <span>{title}</span>;
    return <div key={key} className={classNames(searchStyles.column, flexboxgrid['col-sm-2'])}>
      <dt id={idFor(title, 'title')} className={searchStyles.columnTitle}>{titleElement}</dt>
      <dd id={idFor(value, 'value')} className={searchStyles.columnValue}>
         <input disabled  value={value} className={classNames(this.props.className, styles.input)}/>
      </dd>
    </div>
  }

  renderLevelValues() {
    let numLevels;
    if(this.state.activeResourceType === 'Material' ) {
      numLevels = 3;
    } else {
      numLevels = 2;
    }

    if(this.props.classificationLevels &&
      this.props.classificationLevels.componentType == this.state.activeResourceType.toLowerCase()+'s') {
      return this.props.classificationLevels.levels.slice(0,numLevels).map((m)=>this.renderLevel(m.name,m.key,m.value));
    }
    else {
      return undefined;
    }
  }

  handleDateChange(date) {
    this.setState({
      selectedDate: date
    });
  }

  renderDatePicker() {
    return <div onClick={e => e.stopPropagation()} className={searchStyles.dateContainer}>
      <dt id={idFor('applicable from','title')} className={searchStyles.columnTitle}>Applicable From</dt>
      <dd id={idFor('applicable from','value')} className={searchStyles.columnValue}>
        <DatePicker
          selected={this.state.selectedDate ? moment(this.state.selectedDate) : ''}
          dateFormat="DD/MM/YYYY"
          onChange={(val) => this.handleDateChange(val)}
          className={classNames(searchStyles.input)}
          minDate={tomorrowInIST(new Date())}
        />
      </dd>
    </div>
  }

  renderProjectException() {
     if(this.props.projects && this.props.projects.isFetching === false) {
      let projects = this.props.projects.values;
      let fields =  {
        name: 'Project',
        key: 'projectCode',
        dataType: {name: 'Project', projects:projects},
        editable: true,
        value: this.state.enteredProjectCode || '',
        validity: {isValid: true, msg: ''}
      };
      return this.renderColumnFields(fields);
    }
    else {
      return undefined;
    }
  }

  renderCloseButton() {
    return <button className={searchStyles.closeBtn} id={'search-close'} onClick={(e) => {
      e.preventDefault();
      this.props.onCloseModal(this.props.callingComponent);
      return this.setState({
        isComponentOpen: false
      });
    }}><Icon name="close" className={close}/></button>
  }

  onToggleComponentCodeInput(origin,e) {
    if(origin.state.selectedToEnterCode === false) {
      let selected = !origin.state.selectedToEnterCode;
      origin.setState({selectedToEnterCode: selected,
                       selectedClassificationDropDown: !selected,
                       enteredLevelValues: null});

    }
  }


  onToggleClassificationDropDown(origin,e) {
    if(origin.state.selectedClassificationDropDown === false) {
      let selected = !origin.state.selectedClassificationDropDown;
      origin.setState({selectedClassificationDropDown: selected,
                       selectedToEnterCode: !selected,
                       enteredComponentCode: null,
                       enteredProjectCode: null});
      origin.props.onDestroyComponentDetails();
    }
  }


  renderToggleButton(callbackMethod,selection,origin) {
     return <span><label className={searchStyles.radioButton}>
    <input type="radio"
                  className={styles.radio}
                  name={name}
                  checked={selection}
                  onChange={(e) => callbackMethod(origin,e)}
           />
        </label></span>

  }


  renderDisabledComponentCode() {
    return <div key={'disabled_component_code'} className={classNames(searchStyles.column, flexboxgrid['col-sm-2'])}>
      <dt id={idFor('disabled-component-code','title')} className={searchStyles.columnTitle}>Component Code</dt>
      <dd id={idFor('disabled-component-code','value')} className={searchStyles.columnValue}>
        <input  disabled className={classNames(this.props.className, styles.input)} value={this.state.enteredComponentCode || ''}
        />
      </dd>
    </div>
  }


  renderDisabledProjectException() {
    return <div key={'disabled_project_exception'} className={classNames(searchStyles.column, flexboxgrid['col-sm-2'])}>
      <dt id={idFor('title')} className={searchStyles.columnTitle}>Project</dt>
      <dd id={idFor('value')} className={searchStyles.columnValue}>
        <input  disabled className={classNames(this.props.className, styles.input)} value={this.state.enteredProjectCode || 'Global'}
        />
      </dd>
    </div>
  }


  renderComponentCodeInputAndProject() {
    if(this.state.activeResourceType != null) {
      if(this.state.selectedClassificationDropDown === true) {
        return <div className={classNames(flexboxgrid.row, searchStyles.columnsRow)}>
          {this.renderToggleButton(this.onToggleComponentCodeInput,this.state.selectedToEnterCode,this)}
          {this.renderDisabledComponentCode()}
          {this.renderDisabledProjectException()}
        </div>
      }
      else {
        return <div className={classNames(flexboxgrid.row, searchStyles.columnsRow)}>
          {this.renderToggleButton(this.onToggleComponentCodeInput,this.state.selectedToEnterCode,this)}
          {this.renderComponentCodeTextBox()}
          {this.renderProjectException()}
        </div>
      }
    }
    else {
      return undefined;
    }
  }

  renderOneDisabledClassificationLevel(title,value) {
    return <div key={title} className={classNames(searchStyles.column, flexboxgrid['col-sm-2'])}>
      <dt id={idFor('title')} className={searchStyles.columnTitle}>{title}</dt>
      <dd id={idFor('value')} className={searchStyles.columnValue}>
        <input  disabled className={classNames(this.props.className, styles.input)} value={value || ''}
        />
      </dd>
    </div>
  }

  renderDisabledClassificationLevels() {
    let numLevels = 2;
    if(this.state.activeResourceType === 'Material') {
      numLevels = 3;
    }

    if(this.state.enteredLevelValues === null) {
      return ["--Select--","--Select--","--Select--"].slice(0,numLevels).
             map((m,i)=> this.renderOneDisabledClassificationLevel(this.state.activeResourceType+' Level '+(i+1),m));
    }
    else {
      return this.state.enteredLevelValues.slice(0,numLevels).
             map((m,i)=> this.renderOneDisabledClassificationLevel(this.state.activeResourceType+' Level '+(i+1),m || '--Select--') );
    }
  }

  renderClassificationDropDown() {
    if(this.state.activeResourceType != null) {
      if(this.state.selectedToEnterCode === true && this.props.classificationLevels){
        return <div className={classNames(flexboxgrid.row, searchStyles.columnsRow)}>
          {this.renderToggleButton(this.onToggleClassificationDropDown,this.state.selectedClassificationDropDown,this)}
          {this.renderLevelValues()}
        </div>
      }
      else if(this.state.selectedToEnterCode === true && !this.props.classificationLevels) {
        return <div className={classNames(flexboxgrid.row, searchStyles.columnsRow)}>
          {this.renderToggleButton(this.onToggleClassificationDropDown,this.state.selectedClassificationDropDown,this)}
          {this.renderDisabledClassificationLevels()}
        </div>
      }
      else {
        return <div className={classNames(flexboxgrid.row, searchStyles.columnsRow)}>
        {this.renderToggleButton(this.onToggleClassificationDropDown,this.state.selectedClassificationDropDown,this)}
        {this.renderComponent(this.state.activeResourceType.toLowerCase())}
      </div>
      }
    }
    else {
      return undefined;
    }
  }



  render() {
    return <Modal isOpen={this.state.isComponentOpen} contentLabel="Modal" className={styles.dialog} >
      <form onSubmit={this.handleSubmit}>
        <div className={classNames(flexboxgrid.row, searchStyles.closeBtnContainer)}>
          {this.renderCloseButton()}
        </div>
        <div className={classNames(flexboxgrid.row, searchStyles.mainPageContainer)}>
          {this.renderResourceType()}
          {this.renderDatePicker()}
        </div>
        {this.renderClassificationDropDown()}
        {this.renderComponentCodeInputAndProject()}
        <div className={classNames( searchStyles.horizontalRuleStyle)}>
          <hr></hr>
        </div>
        <CprCoefficients enteredCprValues={this.state.enteredCprValues} accumulateResults={val => this.accumulateResults("cpr_values",val)}/>
        {this.renderAddCancelButtons()}
      </form>
    </Modal>
  }

}




