import React, {Component} from 'react';
import {floatingAction} from '../../css-common/forms.css';
import styles from './index.css';
import {Loading} from '../loading';
import {Filter} from '../filter';
import {Icon} from "../icon/index";
import {Editors} from 'react-data-grid-addons';
import ReactDataGrid from 'react-data-grid';
import {tomorrowInIST, toIST, isRatesFetched, isFetched} from "../../helpers";
import moment from 'moment-timezone';
import DatePicker from 'react-datepicker';
import classNames from 'classnames';
import {Link} from "react-router";
import R from "ramda";
import {isNumber, validate} from '../../validator';
import {alertAsync, confirmAsync} from "../dialog/index";
import {DateEditor} from "../date-editor";
import {PermissionedForNonComponents} from "../../permissions/permissions";
import {getBulkRatesEditPermissions} from "../../permissions/ComponentPermissions";

const DropDownEditor = Editors.DropDownEditor;

export class RatesHome extends Component {
  constructor(props) {
    super(props);
    this.getHeader = this.getHeader.bind(this);
    this.renderDate = this.renderDate.bind(this);

    this.initialState = {
      filterDialog: {
        show: false
      },
      error: {
        show: false,
        message: ''
      },
      coefficients: {percentage: [], sum: []},
      columns: [],
      rows: [],
      minHeight: (0.88 * window.innerHeight) - 100,
    };

    this.state = Object.assign({}, this.initialState);

    // Filter Comps
    this.onFilterDialogClose = this.onFilterDialogClose.bind(this);
    this.getFilterDialogComponent = this.getFilterDialogComponent.bind(this);
    this.onApplyFilter = this.onApplyFilter.bind(this);
    this.onResetFilter = this.onResetFilter.bind(this);
    this.getValue = this.getValue.bind(this);
    this.handleGridRowsUpdated = this.handleGridRowsUpdated.bind(this);
    this.resizeHandler = this.resizeHandler.bind(this);
    this.onCellSelected = this.onCellSelected.bind(this);
    this.getHeaders = this.getHeaders.bind(this);
    this.renderDate = this.renderDate.bind(this);
    this.handleEdit = this.handleEdit.bind(this);
    this.handleGridSort = this.handleGridSort.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
    this.handleCancelEdit = this.handleCancelEdit.bind(this);
    this.validateRow = this.validateRow.bind(this);
  }

  handleEdit() {
    this.props.onEnableEdit();
  }

  /**
   * Filter Area
   */

  getFilterDialogComponent() {
    return this.state.filterDialog.show ? <Filter
      filters={this.props.filters}
      isOpen
      closeDialog={this.onFilterDialogClose}
      applyFilter={this.onApplyFilter}
      resetFilter={this.onResetFilter}
      componentType={this.props.componentType}
      classificationData={this.props.classificationData}
      masterData={this.props.masterData}
    /> : '';
  }

  onApplyFilter(filters) {
    this.setState({
      filterDialog: {
        show: false
      }
    });
    this.props.onSetNewFilters(this.props.componentType, filters);
  }

  onResetFilter(e) {
    e.preventDefault();
    this.setState({
      filterDialog: {
        show: false
      }
    });
    this.props.onResetFilter();
  }

  onFilterDialogClose(e) {
    e.preventDefault();
    this.setState({
      filterDialog: {
        show: false
      }
    });
  }

  /**
   * End Filter Area
   */

  getName(item) {
    if (this.props.componentType === 'material') {
      return {'materialName': item.materialName, 'shortDescription': item.shortDescription}
    } else {
      return {'shortDescription': item.shortDescription}
    }
  }

  getOtherColumnsForMaterialRate(rate, index) {
    return {
      'code': rate.id,
      'typeOfPurchase': rate.typeOfPurchase,
      'controlBaseRate': rate.controlBaseRate.value,
      'landedRate': rate.landedRate ? rate.landedRate.value : null,
      'currencyType': rate.controlBaseRate.currency,
      'appliedOn': {value: rate.appliedOn, onChange: this.handleGridRowsUpdated, rowIndex: index, isValid: true},
      'insuranceCharges': rate.insuranceCharges,
      'freightCharges': rate.freightCharges,
      'basicCustomsDuty': rate.basicCustomsDuty,
      'clearanceCharges': rate.clearanceCharges,
      'taxVariance': rate.taxVariance,
      'locationVariance': rate.locationVariance,
      'marketFluctuation': rate.marketFluctuation
    };
  }

  getOtherColumnsForServiceRate(rate, index) {
    return {
      'code': rate.id,
      'typeOfPurchase': rate.typeOfPurchase,
      'controlBaseRate': rate.controlBaseRate.value,
      'landedRate': rate.landedRate ? rate.landedRate.value : null,
      'currencyType': rate.controlBaseRate.currency,
      'appliedOn': {value: rate.appliedOn, onChange: this.handleGridRowsUpdated, rowIndex: index, isValid: true},
      'locationVariance': rate.locationVariance,
      'marketFluctuation': rate.marketFluctuation
    };
  }

  getOtherColumns(item, index) {
    return this.props.componentType === 'material' ?
      this.getOtherColumnsForMaterialRate(item.materialRate, index) :
      this.getOtherColumnsForServiceRate(item.serviceRate, index);
  }

  getInitialRows(rates) {
    let rows = [];
    if (rates && rates.length > 0) {
      rates.forEach((item, index) => {
        rows.push(
          {
            ...this.getName(item),
            ...this.getOtherColumns(item, index),

          });
      });
    }
    return rows;
  }

  getFirstHeaders() {
    if (this.props.componentType === 'material') {
      return [{name: 'Material Name', key: 'materialName', resizable: true, sortable: !this.props.editable},
        {name: 'Short Description', key: 'shortDescription', resizable: true, sortable: !this.props.editable},
        {name: 'Material Code', key: 'code',  resizable: true, sortable: !this.props.editable}
        ];
    } else {
      return [{
        name: 'Short Description',
        key: 'shortDescription',
        resizable: true,
        sortable: !this.props.editable
      },
        {name: 'Service Code', key: 'code',  resizable: true, sortable: !this.props.editable}];
    }
  }

  getCoefficientsForComponent(componentType,columns) {
    switch(componentType) {
      case 'material' :
        columns.push(
        {
          name: 'Insurance Charges (%)',
          key: 'insuranceCharges',
          resizable: true,
          editable: this.props.editable,
          formatter: Number,
          sortable: false
        },
        {
          name: 'Freight Charges (%)',
          key: 'freightCharges',
          resizable: true,
          editable: this.props.editable,
          formatter: Number,
          sortable: false
        },
        {
          name: 'Basic Customs Duty (%)',
          key: 'basicCustomsDuty',
          resizable: true,
          editable: this.props.editable,
          formatter: Number,
          sortable: false
        },
        {
          name: 'Clearance Charges (%)',
          key: 'clearanceCharges',
          resizable: true,
          editable: this.props.editable,
          formatter: Number,
          sortable: false
        },
        {
          name: 'Tax Variance (%)',
          key: 'taxVariance',
          resizable: true,
          editable: this.props.editable,
          formatter: Number,
          sortable: false
        },
        {
          name: 'Location Variance (%)',
          key: 'locationVariance',
          resizable: true,
          editable: this.props.editable,
          formatter: Number,
          sortable: false
        },
        {
          name: 'Market Fluctuation (%)',
          key: 'marketFluctuation',
          resizable: true,
          editable: this.props.editable,
          formatter: Number,
          sortable: false
        });
        return columns;
        break;
      case 'service' :
        columns.push({
            name: 'Location Variance (%)',
            key: 'locationVariance',
            resizable: true,
            editable: this.props.editable,
            formatter: Number,
            sortable: false
          },
          {
            name: 'Market Fluctuation (%)',
            key: 'marketFluctuation',
            resizable: true,
            editable: this.props.editable,
            formatter: Number,
            sortable: false
          });
        return columns;
        break;
    }
  }

  getHeaders(props) {
    let columns = this.getFirstHeaders();
    columns.push({
      name: 'Type of Purchase',
      key: 'typeOfPurchase',
      editable: this.props.editable,
      editor: this.props.editable ? <DropDownEditor options={this.props.typeOfPurchaseData.values.values}/> : '',
      resizable: true,
      sortable: !this.props.editable
    });
    columns.push(
      {
        name: 'Control Base Rate',
        key: 'controlBaseRate',
        resizable: true,
        editable: this.props.editable,
        formatter: ControlBaseRate,
        sortable: false
      });
    columns.push(
      {
        name: 'Currency Type',
        key: 'currencyType',
        editable: this.props.editable,
        resizable: true,
        editor: this.props.editable ? <DropDownEditor options={this.props.currencyData.values.values}/> : '',
        sortable: false
      });
      columns = this.getCoefficientsForComponent(props.componentType,columns);
      if(!this.props.editable) {
      columns.push({
        name: 'Landed Rate',
        key: 'landedRate',
        resizable: true,
        formatter: NumberWithCurrencyType,
        sortable: false,
      });
    }
    columns.push({
      name: 'Applied From',
      key: 'appliedOn',
      resizable: true,
      editable: this.props.editable,
      editor: this.props.editable?<DateEditor/>:'',
      formatter: UneditableDate,
      sortable: !this.props.editable
    });
    return columns;
  }

  resizeHandler() {
    this.setState({minHeight: (0.88 * window.innerHeight) - 100});
  }

  fetchRates(props) {
    props.onRatesFetchRequest((props.filters && props.filters.length > 0) ? props.filters : []);
  }

  fetchClassificationData(props) {
    if (!props.classificationData || (!props.classificationData.values && !props.classificationData.isFetching)) {
      props.onDependencyDefinitionFetch(`${props.componentType}Classifications`);
    }
  }

  isMasterDataFetched(props) {
    return isFetched(props.masterData['status']) && isFetched(props.masterData['location']) &&
      isFetched(props.masterData['type_of_purchase'] && isFetched(props.masterData['currency']))
  }

  fetchMasterData(props) {
    if (!this.isMasterDataFetched(props) && !props.typeOfPurchaseError && !props.statusError
      && !props.locationError && !props.currencyError) {
      props.fetchMasterDataByNameIfNeeded('status');
      props.fetchMasterDataByNameIfNeeded('location');
      props.fetchMasterDataByNameIfNeeded('type_of_purchase');
      props.fetchMasterDataByNameIfNeeded('currency');
    }
  }

  componentWillMount() {
    if (this.props.rates && this.props.rates.length > 0) {
      const initialRows = this.getInitialRows(this.props.rates);
      this.setState({rows: R.clone(initialRows), initialRows});
    } else {
      this.fetchRates(this.props);
    }
    if (typeof window !== "undefined") {
      window.addEventListener('resize', this.resizeHandler);
    }
    this.fetchClassificationData(this.props);
    this.fetchMasterData(this.props);
  }

  componentDidUpdate(prevProps) {
    if ((!R.equals(this.props.filters, prevProps.filters) || (isRatesFetched(this.props) && !this.hasRates())) && (!this.props.ratesError[this.props.componentType])) {
      this.fetchRates(this.props);
    }
    this.fetchClassificationData(this.props);
    this.fetchMasterData(this.props);

    if (this.props.rates !== prevProps.rates && !this.props.bulkRateError) {
      if (this.props.rates && this.props.rates.length > 0) {
        const initialRows = this.getInitialRows(this.props.rates);
        this.setState({rows: R.clone(initialRows), initialRows});
      }
    }
  }

  componentWillUnmount() {
    window.removeEventListener('resize', this.resizeHandler);
    this.props.onDestroyRates();
  }

  getCoefficientsFromRate(rate) {
    return rate[`${this.props.componentType}Rate`].rate.coefficients;
  }

  getCoefficientHeaders(componentType) {
    switch(componentType) {
      case 'material' :
        return [{
          name: 'Insurance Charges (%)',
          key: 'insuranceCharges',
          sortKey: 'insuranceCharges',
          type: 'number',
        },
          {
            name: 'Freight Charges (%)',
            key: 'freightCharges',
            sortKey: 'freightCharges',
            type: 'number',
          },
          {
            name: 'Basic Customs Duty (%)',
            key: 'basicCustomsDuty',
            sortKey: 'basicCustomsDuty',
            type: 'number'
          },
          {
            name: 'Clearance Charges (%)',
            key: 'clearanceCharges',
            sortKey: 'clearanceCharges',
            type: 'number'
          },
          {
            name: 'Tax Variance',
            key: 'taxVariance',
            sortKey: 'taxVariance',
            type: 'number'
          },
          {
            name: 'Location Variance (%)',
            key: 'locationVariance',
            sortKey: 'locationVariance',
            type: 'number'
          },
          {
            name: 'Market Fluctuation (%)',
            key: 'marketFluctuation',
            sortKey: 'marketFluctuation',
            type: 'number'
          }];
      case 'service' :
        return [{
          name: 'Location Variance (%)',
          key: 'locationVariance',
          sortKey: 'locationVariance',
          type: 'number'
        },
          {
            name: 'Market Fluctuation (%)',
            key: 'marketFluctuation',
            sortKey: 'marketFluctuation',
            type: 'number'
          }];
    }

  }

  getHeader(rate) {
    return [
      ...this.getFirstHeaders(),
      {name: 'Location', key: 'location', sortKey: 'Location', type: 'text'},
      {name: 'Type of Purchase', key: 'typeOfPurchase', sortKey: 'TypeOfPurchase', type: 'text'},
      {name: 'Landed Rate (%)', key: 'landedRate', sortKey: 'LandedRate.Value', type: 'number'},
      {name: 'Control Base Rate', key: 'controlBaseRate', sortKey: 'ControlBaseRate.Value', type: 'number'},
      ...this.getCoefficientHeaders(this.props.componentType),
      {name: 'Applied From', key: 'appliedOn', sortKey: 'AppliedOn', type: 'number'},
    ];
  }

  renderDate(date) {
    const FormattedDate = new Date(date);
    return `${FormattedDate.getDate()}/${(FormattedDate.getMonth() + 1).toString()}/${FormattedDate.getFullYear()}`;
  }

  getCurrency(value) {
    return value ? <span className={styles.currencyContainer}>
      <span className={styles.moneyValue}>{value.value}</span>
      <span className={styles.moneyCurrency}>{value.currency}</span>
    </span> : '-';
  }

  renderError() {
    return <h3 className={styles.errorMessage}>{this.props.ratesError[this.props.componentType]}</h3>;
  }

  renderNoRates() {
    return <h3 className={styles.errorMessage}>No {this.props.componentType} rates found</h3>;
  }

  renderEdit() {
    if(this.props.rates.length && !this.props.editable)
      return <button className={styles.button} onClick={this.handleEdit}>Edit</button>;
    else
      return <div/>;
  }

  renderFilter() {
    return [!this.props.editable && <button
      className={styles.menuButton}
      onClick={e => {
        e.preventDefault();
        this.setState({
          filterDialog: {
            show: true
          }
        })
      }}>{this.props.isFilterApplied ? <Icon name="rightMark" className={styles.icon}/> : ''}Filter</button>,
      this.getFilterDialogComponent()]
  }

  sortRows(sortColumn, sortDirection, rows) {
    const getValue = (column) => sortColumn === 'appliedOn' ? column.value : column;
    const sortCriteria = (a, b) => {
      if (sortDirection === 'ASC') {
        return (getValue(a[sortColumn]) > getValue(b[sortColumn])) ? 1 : -1;
      } else if (sortDirection === 'DESC') {
        return (getValue(a[sortColumn]) < getValue(b[sortColumn])) ? 1 : -1;
      }
    };
    const sortedResult = this.state.rows.sort(sortCriteria);
    sortedResult.forEach((row, i) => {
      row.appliedOn.rowIndex = i;
    });

    return sortedResult;
  }

  handleGridSort(sortColumn, sortDirection) {
    this.setState({
      rows: this.sortRows(sortColumn, sortDirection, this.state.rows),
      initialRows: this.sortRows(sortColumn, sortDirection, this.state.initialRows),
    });
  }

  getChangedRows(rowsWithIndex) {
    return rowsWithIndex.filter(({index, row}) => {
      let initialRow = this.state.initialRows[index];
      let getProps = R.omit(['appliedOn']);
      return !(R.equals(getProps(row), getProps(initialRow)) && R.equals(row.appliedOn.value, initialRow.appliedOn.value))
    });
  }

  async handleSubmit(e) {
    e.preventDefault();
    const rowsWithIndex = R.addIndex(R.map)((row, index) => ({index, row}), this.state.rows);
    const rowsToBeSubmitted = this.props.bulkRateError ? rowsWithIndex : this.getChangedRows(rowsWithIndex);
    if (!this.validateRows(rowsToBeSubmitted)) {
      await alertAsync("Error", 'There are errors in the rates that you have changed. Please revisit them.');
      return;
    }
    if (rowsToBeSubmitted.length === 0) {
      await alertAsync("Error", "You haven't modified any rates.");
      return;
    }
    try {
      await confirmAsync("Wait", "You wish to create new version of rate for one or more components?");
      const rates = rowsToBeSubmitted.map(({row}) => ({
        appliedOn: row.appliedOn.value,
        id: row.code,
        location: this.props.filters.find(({columnKey}) => columnKey === 'Location').columnValue,
        typeOfPurchase: row.typeOfPurchase,
        controlBaseRate: {currency: row.currencyType, value: row.controlBaseRate},
        ...this.handleCoefficients(row),
      }));
      this.props.onUpdateBulkEdit(rates);
    } catch (e) {
    }
  }

  handleCoefficients(row) {
    switch(this.props.componentType) {
      case 'material' :
        return {
            "insuranceCharges" : row.insuranceCharges,
            "freightCharges" : row.freightCharges,
            "basicCustomsDuty" : row.basicCustomsDuty,
            "clearanceCharges" : row.clearanceCharges,
            "taxVariance" : row.taxVariance,
            "locationVariance" : row.locationVariance,
            "marketFluctuation" : row.marketFluctuation
          };
      case 'service' :
        return {
            "locationVariance" : row.locationVariance,
            "marketFluctuation" : row.marketFluctuation
          };
    }
  }

  renderDataGrid() {
    const classes = classNames(styles.dataGrid, {[styles.material]: this.props.componentType === "material"});
    return <form className={classNames({[styles.editable]: this.props.editable})} onSubmit={this.handleSubmit}>
      <div className={classes}>
        <ReactDataGrid
          ref="grid"
          columns={this.getHeaders(this.props)}
          rowGetter={this.getValue}
          rowsCount={this.state.rows.length}
          minHeight={this.state.minHeight}
          rowHeight={40}
          headerRowHeight={57}
          enableCellSelect={true}
          onGridRowsUpdated={this.handleGridRowsUpdated}
          onCellSelected={this.onCellSelected}
          onGridSort={this.handleGridSort}
        />
      </div>
      {this.props.editable && <input id="update-button" className={styles.update} type="submit" value="Update"/>}
      {this.props.editable && <button className={styles.cancel} onClick={this.handleCancelEdit}>Cancel</button>}
    </form>;
  }

  async handleCancelEdit(e) {
    e.preventDefault();
    try {
      await confirmAsync("Warning", "The rates you have modified won't be saved. Are you sure you want to cancel?");
      if (this.props.bulkRateError) {
        this.props.onBulkEditClear();
        this.props.onRatesFetchRequest(this.props.filters);
      } else {
        this.setState({rows: R.clone(this.state.initialRows)});
      }
      this.props.onDisableEdit(false);
    } catch (e) {
    }
  }

  handleGridRowsUpdated({fromRow, toRow, updated}) {
    let value = Object.assign({}, updated);
    let rows = this.state.rows.slice();
    for (let i = fromRow; i <= toRow; i++) {
      rows[i] = Object.assign({}, rows[i], value);
    }
    this.setState({rows});
  }

  onCellSelected({rowIdx, idx}) {
    this.refs.grid.openCellEditor(rowIdx, idx);
  }

  getValue(i) {
    return this.state.rows[i];
  }

  areCoefficientsValid(row) {
    const allCoefficients = this.state.coefficients.percentage.concat(this.state.coefficients.sum);
    return R.all(coefficient => isNumber(row[coefficient]), allCoefficients);
  }

  // We return the validated row as well because we put isValid in appliedOn
  validateRow(row) {
    const {isValid: isAppliedOnValid, row: newRow} = this.validateAppliedOn(row);
    return {
      isValid: isAppliedOnValid && isNumber(newRow.controlBaseRate) && newRow.controlBaseRate > 0 && this.areCoefficientsValid(newRow),
      row: newRow
    };
  }

  validateRows(rowsWithIndex) {
    let allRowsValid = true;
    let newRows = this.state.rows.slice();

    rowsWithIndex.forEach(({row, index}) => {
      const {isValid, row: validatedRow} = this.validateRow(row);
      allRowsValid = allRowsValid && isValid;
      newRows[index] = validatedRow;
    });

    this.setState(Object.assign({}, this.state, {rows: newRows}));
    return allRowsValid;
  }

  validateAppliedOn(row) {
    const isValid = isAppliedOnValid(row.appliedOn.value);
    return {isValid, row: Object.assign({}, row, {appliedOn: Object.assign({}, row.appliedOn, {isValid})})};
  }

  hasRates() {
    return this.props.rates && this.props.classificationData.values && !this.props.classificationData.isFetching && this.props.masterData && this.isMasterDataFetched(this.props);
  }

  render() {
    let rateElement;
    if (this.hasRates()) {
      if (this.props.rates.length > 0) {
        rateElement = this.renderDataGrid();
      }
      else {
        rateElement = this.renderNoRates();
      }
    }
    else {
      if (this.props.ratesError[this.props.componentType] !== '') {
        rateElement = this.renderError();
      }
      else {
        rateElement = <Loading />;
      }
    }

    return (<div className={styles.ratesHome}>
      <header className={styles.header}>
        <div className={styles.left}>
          <h2>Manage {this.props.componentType} Rates</h2>
        </div>
        <div className={styles.right}>
          {(() => {
            if (this.hasRates()) {
              return <div>
                {this.renderFilter()}
                <PermissionedForNonComponents allowedPermissions={getBulkRatesEditPermissions(this.props.componentType)}>{this.renderEdit()}</PermissionedForNonComponents>
              </div>;
            }
          })()}
        </div>
      </header>
      {!this.props.bulkRateError ?
        '' :
        <div className={styles.error}>
          <h2>Error</h2>
          <p>The below records are not created as a rate version for the specified {this.props.componentType} in the Location, Mode of Purchase and the Applicable from date exists. Please revisit and submit.</p>
        </div>}
      {rateElement}
    </div>);
  }
}

function isAppliedOnValid(date) {
  if (!date) return false;
  return moment(tomorrowInIST()).isSameOrBefore(moment(toIST(date)));
}

class Date extends Component {
  render() {
    let props = this.props;
    return <DatePicker
      className={classNames({[styles.error]: !this.props.value.isValid})}
      id="appliedOn-edit"
      selected={this.props.value.value ? moment(this.props.value.value) : ''}
      minDate={tomorrowInIST()}
      dateFormat="DD/MM/YYYY"
      onChange={(e) => {
        (props.value.onChange({
          fromRow: props.value.rowIndex,
          toRow: props.value.rowIndex,
          updated: {
            "appliedOn": {
              value: e._d.toISOString(),
              onChange: props.value.onChange,
              rowIndex: props.value.rowIndex,
              isValid: isAppliedOnValid(e._d.toISOString())
            }
          }
        }))
      }}
    />
  }
}

class ControlBaseRate extends Component {
  render() {
    const classes = classNames({[styles.error]: !isNumber(this.props.value) || this.props.value <= 0});
    return <div>
      <span className={classes}>{this.props.value}</span><br/>
    </div>;
  }
}

class Number extends Component {
  render() {
    const classes = classNames({[styles.error]: !isNumber(this.props.value) || this.props.value < 0});
    return <div>
      <span className={classes}>{this.props.value}</span><br/>
    </div>;
  }
}

class NumberWithCurrencyType extends Component {
  render() {
    const classes = classNames({[styles.error]: !isNumber(this.props.value) || this.props.value < 0});
    return <div>
      <span className={classes}>{this.props.value} INR</span><br/>
    </div>;
  }
}

class UneditableDate extends Component {
  shouldComponentUpdate(nextProps) {
    return nextProps.value !== this.props.value;
  }

  render() {
    const classes = classNames({[styles.error]: !this.props.value.isValid });
    return <div>
      <span className={classes}>{moment(this.props.value.value).tz('Asia/Calcutta').format('DD/MM/YYYY')}</span><br/>
    </div>;
  }
}
