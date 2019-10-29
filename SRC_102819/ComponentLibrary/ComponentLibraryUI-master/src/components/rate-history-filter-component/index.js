import React from 'react';
import Modal from 'react-modal';
import styles from './index.css';
import {Icon} from '../icon';
import {idFor} from '../../helpers';
import {Loading} from "../loading/index";
import {validate} from '../../validator';
import {AlertDialog} from "../alert-dialog/index";
import moment from 'moment-timezone';
import DatePicker from 'react-datepicker';
import classNames from 'classnames';
import {MasterData} from "../data-types/master-data/index";

export class RateHistoryFilterComponent extends React.Component {

  constructor(props) {
    super(props);
    this.state = this.clonedState(this.props.changedDefinition);
    this.handleChange = this.handleChange.bind(this);
    this.cancelErrorDialog = this.cancelErrorDialog.bind(this);
  }

  render() {
    return (
      <Modal className={styles.filterDialog} isOpen={this.props.isOpen} contentLabel="Filter file">
        <div className={styles.header} id={idFor('filter-header')}>
          <h1 className={styles.title}>Filters</h1>
          <button className={styles.close} id={idFor('close')} onClick={(e) => {
            this.props.onClose()
          }}><Icon name="close" className={styles.close}/></button>
        </div>
        <div id={idFor('filter-data')} className={styles.columnsRow}>
          { this.renderForm()}
        </div>
        <div className={styles.actionItems}>
          <button className={styles.clear}
                  id={idFor('clear')}
                  onClick={(e) => {
                    this.setState(this.props.definition);
                    this.props.onClear(this.props.definition.filterData)
                  }}>Clear Filters
          </button>
          <button className={styles.apply}
                  id={idFor('apply')}
                  onClick={(e) => {
                    this.props.onApply(this.state.filterData)
                  }}>Apply
          </button>
        </div>
        {<AlertDialog message={this.state.error.message} title="Warning" shown={this.state.error.shown}
                      onClose={this.cancelErrorDialog}/>}
      </Modal>
    );
  }

  renderForm() {
    let elements = [];
    for (let key in this.state.filterData) {
      let keyObj = this.state.filterData[key];
      if (keyObj.type === 'MasterData')
        elements.push(this.renderMasterData(keyObj.id, keyObj.name, keyObj.value, keyObj.data));
      else if (keyObj.type === 'Date')
        elements.push(this.renderAppliedOn(keyObj.id, keyObj.name, keyObj.value));
    }
    return elements;
  }

  clonedState(state) {
    return JSON.parse(JSON.stringify(state));
  }

  renderAppliedOn(id, name, value) {
    const label = name;
    return <div key={idFor(label)} className={styles.column}>
      <dt id={idFor(label, 'title')} className={styles.columnTitle}>{label}</dt>
      <dd id={idFor(label, 'value')} className={styles.columnValue}>
        <DatePicker
          selected={ value ? moment(value) : ""}
          dateFormat="YYYY-MM-DD"
          id={idFor(label)}
          onChange={function (e) {
            const clonedState = this.context.clonedState(this.context.state);
            clonedState.filterData[this.id].value = (e && e.toISOString()) || null;
            this.context.setState(clonedState);
          }.bind({context: this, id: id})}
          className={classNames({[styles.inputAppliedOn]: true, [styles.error]: false})}/>
        <br/><span className={styles.calendarNotice}>*Calendar is set to IST zone</span>
      </dd>
    </div>
  }

  renderMasterData(field, name, value, masterDataValues) {
    let columnValue = {value: value, dataType: masterDataValues,editable:true};
    return <div key={name} className={styles.column}>
      <dt id={idFor(name, 'title')} className={styles.columnTitle}>{name}</dt>
      <dd id={idFor(name, 'value')} className={styles.columnValue}>
        <MasterData columnValue={columnValue} onChange={(value) => this.handleChange(field, value)} />
      </dd>
    </div>
  }

  cancelErrorDialog() {
    this.setState({error: {message: '', shown: false}});
    this.props.onCancelErrorDialog();
  }

  handleChange(field, value) {
    let newState = JSON.parse(JSON.stringify(this.state));
    newState['filterData'][field].value = value;
    this.setState(newState);
  }
}

