import React, { PropTypes } from 'react';
import Collapse from 'rc-collapse';
import styles from './index.css';
import { Icon } from '../icon';
import DataType from '../data-types';
import {col, getHeader, head, idFor, tomorrowInIST} from '../../helpers';
import { Rate } from '../rate';
import { Link } from 'react-router';
import { RentalRate } from '../rental-rate';
import moment from 'moment-timezone';
import { ComponentCompositionView } from "../component-composition-view";
import DatePicker from "react-datepicker";
import * as R from 'ramda';
import {PermissionedForNonComponents, permissions} from "../../permissions/permissions";
import {getComponentRateHistoryPermissions, getViewRentalRatePermission} from "../../permissions/ComponentPermissions";

export class Component extends React.Component {

  constructor(props) {
    super(props);
    this.state =
      {
        definitionCollapsed: true,
        numberOfColumns: 7
      };
    this.handleDateChange = this.handleDateChange.bind(this);
    this.handleLocationChange = this.handleLocationChange.bind(this);
  }

  componentDidMount() {
    this.ensureMasterDataIsPresent(this.props);
  }

  componentWillReceiveProps(props) {
    this.ensureMasterDataIsPresent(props);
  }

  getChildContext() {
    return { componentCode: this.props.componentCode, componentType: this.props.componentType };
  }

  ensureMasterDataIsPresent(props) {
    if (!props.editable) return;
    // Populate MasterData if it is ID
    props.details.headers
      .map((header) => {
      header.columns.map((column) => {
            if (column.dataType.name === "MasterData" && !(column.dataType.values && column.dataType.values.status === 'fetched')) {
              const id = column.dataType.subType;
              props.masterData[id] || props.onMasterDataFetch(id);
            }
          });
      });
  }

  getGroup() {
    switch (this.props.componentType.toLowerCase()) {
      case 'material':
        if (getHeader(this.props.details,'classification') !== undefined)
          if (head(this.props.details,'classification','material_level_2') !== undefined)
            return head(this.props.details,'classification','material_level_2','value');
        break;
      case 'service':
        if (getHeader(this.props.details,'classification') !== undefined)
          if (head(this.props.details,'classification','service_level_1') !== undefined)
            return head(this.props.details,'classification','service_level_1','value');
        break;
      case 'sfg':
        if (getHeader(this.props.details,'classification') !== undefined)
          if (head(this.props.details,'classification','sfg_level_1') !== undefined)
            return head(this.props.details,'classification','sfg_level_1','value');
        break;
    }
  }

  renderColumn(headerKey, name, value, dependency,columnKey,headerName) {
    if (this.props.editable && value.dataType.name === "MasterData" && !dependency) {
      value.dataType.values = this.props.masterData[value.dataType.subType] || { values: [], status: 'blank' };
    }
    let titleElement = null;
    if (this.props.editable && value.isRequired === true) {
      titleElement = <span>
        {name} <abbr className={styles.requiredMarker} title="Required">*</abbr>
      </span>
    } else {
      titleElement = <span>{name}</span>;
    }
    let group = this.getGroup();
    return <div key={name} className={styles.column}>
      <dt id={idFor(name, 'title')} className={styles.columnTitle}>{titleElement}</dt>
      <dd id={idFor(name, 'value')} className={styles.columnValue}>
        <DataType columnName={name} columnValue={value} columnKey={columnKey} mode={this.props.mode} group={group}
          componentType={this.props.componentType.toLowerCase()}
          onChange={(value) => this.props.onColumnChange(headerKey, columnKey, value,name)} />
      </dd>
    </div>
  }

  setValues(columns, children) {
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
    this.setValues(columns
      .filter((c, i) => i !== 0), remainingDependencyDefinition.children);
  }

  renderClassification(columns, headerName,headerKey) {
    const newColumns = JSON.parse(JSON.stringify(columns));
    const columnsArray = newColumns.map(column => ({ value: column, name: column.name }));
    if (this.props.dependencyDefinition) {
      this.setValues(columnsArray, this.props.dependencyDefinition.values.block.children);
    }
    return <Collapse.Panel className={styles.component} key={headerName} showArrow={false}
      header={<span id={idFor(headerName)}><Icon name="arrow"
        className="arrow" />{headerName}</span>}>
      <div className={styles.columnsRow}>
        {newColumns
          .map(column => this.renderColumn(headerKey, column.name, column, true,column.key,headerName))}
      </div>
      {this.props.classificationDefinition ? this.renderComponentDefinition() : ''}
    </Collapse.Panel>;
  }

  renderDefinitionTitle() {
    return <div className={styles.viewDefinitionTitle}>
      <Icon name="arrow" className="arrow" />
      <span>
        {this.state.definitionCollapsed ? "View definitions" : "Hide definitions"}
      </span>
    </div>;
  }

  changeHandler(key) {
    key.length == 2 ? this.setState({ definitionCollapsed: false }) : this.setState({ definitionCollapsed: true });
  }

  renderComponentDefinition() {
    return <Collapse defaultActiveKey="0" onChange={this.changeHandler.bind(this)}>
      <Collapse.Panel showArrow={false} className={styles.subCollapse} header={this.renderDefinitionTitle()} key="1">
        <div className={styles.definitionContent}>
          {getHeader(this.props.details,'classification').columns.map(c => this.renderComponentDefinitionRow(c.key, col(this.props.classificationDefinition, `${c.key}_definition`), c.name))}
        </div>
      </Collapse.Panel>
    </Collapse>
  }

  makeFirstLetterToLowerCase(words) {
    words = words.split(' ');
    words[0] = words[0].split('');
    words[0][0] = words[0][0].toLowerCase();
    words[0] = words[0].join('');
    return words.join(' ');
  }

  renderComponentDefinitionRow(columnKey, definition,columnName) {
    return <div className={styles.columnsRow}>
      <div className={styles.column}>
        <dt className={styles.columnTitle}>{columnName}</dt>
        <dd className={styles.columnValue}>
          <span>{head(this.props.details,'classification',columnKey,'value') || '-'}</span>
        </dd>
      </div>
      <div className={styles.otherColumn}>
        <dt className={styles.columnTitle}>{'Definition'}</dt>
        <dd className={styles.columnValue}>
          <span>{definition.value ? <span dangerouslySetInnerHTML={{__html: definition.value.split('\n').join('<br />')}} /> : '-'}</span>
        </dd>
      </div>
    </div>;
  }

  renderHeader(headerName, columns,headerKey) {
    if (this.props.editable && (headerKey === 'system_logs' || headerKey === 'edesign_specifications')) return;
    if (headerKey === "classification") return this.renderClassification(columns, headerName,headerKey);
    return (<Collapse.Panel className={styles.component} key={headerName} showArrow={false}
      header={<span id={idFor(headerName)}>
        <Icon name="arrow" className="arrow" />{headerName}</span>}>
      <div className={styles.columnsRow}>
        {
          columns
            .filter(column => column.key !== 'image' || this.props.editable)
            .map(column => this.renderColumn(headerKey, column.name, column,false,column.key,headerName))
        }
      </div>
    </Collapse.Panel>);
  }

  handleDateChange(date) {
    this.setState({ componentDate: date });
    this.props.onComponentCostFetchRequest(this.props.componentLocation, date);
  }

  handleLocationChange(event) {
    this.setState({ componentLocation: event.target.value });
    this.props.onComponentCostFetchRequest(event.target.value, this.props.componentDate);
  }


  renderLocation() {
    return <select className={styles.sortCity}
      onClick={(e) => e.stopPropagation()}
      onChange={this.handleLocationChange}
      value={this.props.componentLocation}>
      {this.props.locations.map(location => <option className={styles.sortCityOption}>{location}</option>)}
    </select>
  }

  render() {
    if (this.props.componentType === 'sfg') {
      let compositionDetail = <Collapse.Panel className={styles.composition} key={"SFG COMPOSITION"} showArrow={false} header={
        [<span id={idFor('sfg', 'coefficients')}>
          <Icon name="arrow" className="arrow" />SFG COMPOSITION
          </span>,
          <div className={styles.price}>{this.props.componentCostError ? '' : this.props.cost.totalCost.value + " " + this.props.cost.totalCost.currency}</div>,
        this.renderLocation(),
        <div onClick={e => e.stopPropagation()} className={styles.dateContainer}>
          <DatePicker
            className={styles.sortDate}
            selected={this.props.componentDate || ''}
            dateFormat="DD/MM/YYYY"
            peekNextMonth
            showMonthDropdown
            showYearDropdown
            onChange={this.handleDateChange}
          />
        </div>,
        ]
      }>
        <ComponentCompositionView composition={this.props.composition} cost={this.props.cost} error={this.props.componentCostError} />
      </Collapse.Panel>;
      let defaultActiveKeys = this.props.details.headers.map(header => header.name);
      defaultActiveKeys.push("SFG COMPOSITION");
      return <Collapse className={styles.component} defaultActiveKey={defaultActiveKeys}>
        {R.insert(2, compositionDetail, this.props.details.headers
          .map(header => this.renderHeader(header.name, header.columns,header.key)))}
      </Collapse>
    }
    else if (this.props.componentType === 'package') {
      let defaultActiveKeys = this.props.details.headers.map(header => header.name);
      let compositionDetails = <Collapse.Panel className={styles.composition} key={"PACKAGE COMPOSITION"} showArrow={false} header={
        [<span id={idFor('package', 'coefficients')}>
          <Icon name="arrow" className="arrow" />PACKAGE COMPOSITION
          </span>,
          <div className={styles.price}>{this.props.componentCostError ? '' : this.props.cost.totalCost.value + " " + this.props.cost.totalCost.currency}</div>,
          this.renderLocation(),
        <div onClick={e => e.stopPropagation()} className={styles.dateContainer}>
          <DatePicker
            className={styles.sortDate}
            selected={this.props.componentDate || ''}
            dateFormat="DD/MM/YYYY"
            peekNextMonth
            showMonthDropdown
            showYearDropdown
            onChange={this.handleDateChange}
          />
        </div>,
        ]
      }>
        <ComponentCompositionView composition={this.props.composition} cost={this.props.cost} error={this.props.componentCostError} />
      </Collapse.Panel>;
      defaultActiveKeys.push("PACKAGE COMPOSITION");
      return <Collapse className={styles.component} defaultActiveKey={defaultActiveKeys}>
        {R.insert(2, compositionDetails, this.props.details.headers
          .map(header => this.renderHeader(header.name, header.columns,header.key)))}
      </Collapse>
    }
    else {
      const rateKeys = this.shouldRenderRentalRate() ? ["rates", "rentalRates"] : ['rates'];
      return <Collapse className={styles.component}
        defaultActiveKey={this.props.details.headers.map(header => header.name).concat(rateKeys)}>
        {
          this.props.details.headers
            .map(header => this.renderHeader(header.name, header.columns,header.key))
        }
        {this.renderComponentRates()}
        {this.renderCurrentRentalRate()}
      </Collapse>
    }
  }

  renderComponentRates() {
    return this.shouldRenderComponentRate() ? <Collapse.Panel showArrow={false} key="rates" header={
      <span id={idFor("Rate")} key={idFor("Rate")}>
        <Icon name="arrow" className="arrow" />
        Rate
        <PermissionedForNonComponents allowedPermissions={getComponentRateHistoryPermissions(this.props.componentType)}>
        {<span><Link to={this.props.rateHistoryLink} className={styles.viewHistory}>View Rate History</Link></span>}
        </PermissionedForNonComponents>
      </span>}>
      {!!this.props.rateserror ? this.renderRatesErrorMsg() : this.renderRates()}
    </Collapse.Panel> : '';
  }

  renderRatesErrorMsg() {
    return <h3 id={idFor('rateserror')}>{this.props.rateserror}</h3>;
  }

  renderRates() {
    return <Rate rates={this.props.rates} componentType={this.props.componentType} componentCode={this.props.componentCode} rowStyle={styles.columnsRow} columnStyle={styles.column} />;
  }

  shouldRenderRentalRate() {
    return this.props.componentType === 'material' && !this.props.editable && head(this.props.details,'general','can_be_used_as_an_asset','value');
  }

  shouldRenderComponentRate() {
    return !this.props.editable;
  }

  renderCurrentRentalRate() {
    return (this.shouldRenderRentalRate()) ? <Collapse.Panel showArrow={false} key="rentalRates" header={
      <span id={idFor("rental rate")} key={idFor("rental rate")}>
        <Icon name="arrow" className="arrow" />
        Rental Rates
        <PermissionedForNonComponents allowedPermissions={getViewRentalRatePermission()}>
        <span>
          <Link to={this.props.rentalRateHistoryLink}
            className={styles.viewHistory}>View Rental Rate History</Link></span>
        </PermissionedForNonComponents>
      </span>}>
      <RentalRate rentalRates={this.props.rentalRates} rowStyle={styles.columnsRow} columnStyle={styles.column} />
    </Collapse.Panel> : '';
  }
}
Component.childContextTypes = {
  componentType: PropTypes.string,
  componentCode: PropTypes.string,
};
