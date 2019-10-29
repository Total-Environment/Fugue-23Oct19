import React from 'react';
import styles, {icon} from "./index.css"
import {Loading} from '../loading';
import {CreateExchangeRate} from '../create-exchange-rate';
import {Link} from 'react-router';
import {Table} from '../table';
import {idFor, isFetchedAll} from "../../helpers"
import classNames from "classnames"
import {RateHistoryFilterComponent} from "../rate-history-filter-component"
import {Icon} from "../icon/index"

export class ExchangeRateHistory extends React.Component {
  constructor(props) {
    super(props);
    this.getExchangeRate = this.getExchangeRate.bind(this);
    this.renderCurrencyINR = this.renderCurrencyINR.bind(this);
    this.handleClose = this.handleClose.bind(this);
    this.handleApplyFilters = this.handleApplyFilters.bind(this);
    this.handleClearFilters = this.handleClearFilters.bind(this);
    this.state = { filter: {shown: false}, isFilterApplied:false };
  }

  filterDefinition() {
    return {
      filterData: {currencyType: {
        name: 'Currency',
        value: '',
        id: 'currencyType',
        type: 'MasterData',
        data: this.props.currencyData
      }, appliedFrom: {
        name: 'Applied From',
        value: null,
        id: 'appliedFrom',
        type: 'Date'
      }
      }, error: {
        message: "", //this.props.errorInFilter.message,
        shown: false //this.props.errorInFilter.shown
      }
    }
  }

  filteredDefinition() {
    return {
      filterData: {currencyType: {
        name: 'Currency',
        value: this.props.currencyType || '',
        id: 'currencyType',
        type: 'MasterData',
        data: this.props.currencyData
      }, appliedFrom: {
        name: 'Applied From',
        value: this.props.appliedFrom || null,
        id: 'appliedFrom',
        type: 'Date'
      }
      }, error: {
        message: "", //this.props.errorInFilter.message,
        shown: false //this.props.errorInFilter.shown
      }
    }
  }

  componentDidMount() {
    this.fetchData(this.props);
  }

  fetchData(props) {
    props.fetchMasterDataByNameIfNeeded('currency');
    if (props.exchangeRateHistory) {
      return;
    }
    const {pageNumber, sortColumn, sortOrder, currencyType, appliedFrom} = props;
    props.onExchangeRateHistoryFetchRequest(pageNumber, sortColumn, sortOrder, currencyType, appliedFrom);
  }

  componentWillReceiveProps(nextProps) {
    this.fetchData(nextProps);
  }

  componentWillUnmount() {
    this.props.onDestroyExchangeRateHistory();
  }

  getHeaders() {
    return [
      {name: 'Currency', key: 'fromCurrency', sortKey: 'FromCurrency', sortable: true},
      {name: 'Conversion Rate', key: 'baseConversionRate', type: 'number', sortKey: 'BaseConversionRate', sortable: false},
      {name: 'Currency Fluctuation Coefficient (%)', key: 'currencyFluctuationCoefficient', type: 'number', sortKey: 'FluctuationCoefficient', sortable: false},
      {name: 'Defined Conversion Rate', key: 'definedConversionRate', type: 'number', sortKey: 'DefinedConversionRate', sortable: false},
      {name: 'Applied From', key: 'appliedFrom', type: 'number', sortKey: 'AppliedFrom', sortable: true},
    ];
  }

  getExchangeRate(exchangeRate) {
    return {
      fromCurrency: exchangeRate.fromCurrency,
      baseConversionRate: this.renderCurrencyINR(exchangeRate.baseConversionRate),
      currencyFluctuationCoefficient: exchangeRate.currencyFluctuationCoefficient,
      definedConversionRate: this.renderCurrencyINR(exchangeRate.definedConversionRate),
      appliedFrom: this.renderDate(exchangeRate.appliedFrom),
    };
  }

  renderDate(date) {
    const FormattedDate = new Date(date);
    return `${FormattedDate.getDate()}/${(FormattedDate.getMonth() + 1).toString()}/${FormattedDate.getFullYear()}`;
  }

  renderCurrencyINR(value) {
    return value ? <span className={styles.currencyContainer}>
                <span className={styles.moneyValue}>{value}</span>
                <span className={styles.moneyCurrency}>INR</span>
            </span> : '-';
  }

  getData() {
    return this.props.exchangeRateHistory.items.map(this.getExchangeRate);
  }


  render() {
    let exchangeRateHistoryElement = null;
    if (this.props.exchangeRateHistory) {
      const {items, pageNumber, batchSize, totalRecords, sortColumn, sortOrder} = this.props.exchangeRateHistory;
      if (items && items.length > 0) {
        exchangeRateHistoryElement = <Table className={styles.table}
                                            headers={this.getHeaders()}
                                            data={this.getData()}
                                            showPagination
                                            pageNumber={pageNumber}
                                            batchSize={batchSize}
                                            totalRecords={totalRecords}
                                            sortColumn={sortColumn}
                                            sortOrder={sortOrder}
                                            onPageChange={(pageNumber) => this.props.onAmendQueryParams({pageNumber})}
                                            onSortChange={(sortColumn, sortOrder) => this.props.onAmendQueryParams({
                                              sortColumn,
                                              sortOrder
                                            })}
        />;
      }
      else {
        exchangeRateHistoryElement = (<h3 className={styles.errorMessage}>{'No exchange rate history found.'}</h3>);
      }
    }
    else if (this.props.error !== undefined && this.props.error !== '') {
      exchangeRateHistoryElement = <h3 className={styles.errorMessage}>{this.props.error}</h3>;
    }
    else {
      exchangeRateHistoryElement = <Loading />;
    }
    return (<div className={styles.exchangeRateHistory}>
      <h2 className={styles.title}>
        Exchange Rates History
        {this.renderFilterButton()}
      </h2>
      <Link className={styles.backToLatest} to={`exchange-rates`}>Back to Exchange Rates</Link>
      {isFetchedAll(this.props.currencyData) && <CreateExchangeRate addExchangeRateError={this.props.addExchangeRateError}
                          onAddExchangeRateErrorClose={this.props.onAddExchangeRateErrorClose}
                          onAddExchangeRate={this.props.onAddExchangeRate}
                          exchangeRateAdding={this.props.exchangeRateAdding} currencyData={this.props.currencyData} />}
      {exchangeRateHistoryElement}
    </div>);
  }

  renderFilterButton() {
    let classes = (this.state.isFilterApplied || this.props.currencyType || this.props.appliedFrom) ? classNames(styles.filtersApplied) : classNames(styles.filter);
    return (
      [<button id={idFor('filter')} className={classes} onClick={ (event) => {
        event.preventDefault();
        this.renderFilterComponent();
      }}>{(this.state.isFilterApplied || this.props.currencyType || this.props.appliedFrom)? <Icon name="rightMark" className={icon}/>:''}Filter</button>,
        this.props.currencyData && !this.props.currencyData.isFetching
        && <RateHistoryFilterComponent
          componentType={this.props.componentType}
          definition={this.filterDefinition()}
          changedDefinition={this.filteredDefinition()}
          isOpen={this.state.filter.shown}
          onClose={this.handleClose}
          onApply={this.handleApplyFilters}
          onClear={this.handleClearFilters}/>]
    )
  }

  renderFilterComponent() {
    this.setState({ filter:{ shown: true}});
  }

  handleClose() {
    this.setState({filter: {shown: false}});
  }

  handleApplyFilters(filterData) {
    let filters = {};
    for ( let key in filterData){
      if(filterData[key].value !== null)
        filters[key] = filterData[key].value;
    }

    if(!Object.keys(filters).length) {
      this.setState({filter: {shown: false}});
      return;
    }
    // if (!this.isFormValid(filterData)) {
    //   this.setState({error: {message:'There are errors in the form. Please fix it and submit.', shown: true}});
    //   return;
    // }
    const params = {pageNumber: 1, ...filters};
    this.props.onAmendQueryParams(params);
    this.setState({filter: {shown: false}, isFilterApplied:true});
  }

  handleClearFilters(filterData) {
    let filters = {};
    for ( let key in filterData){
      filters[key] = filterData[key].value;
    }
    const params = {pageNumber: 1, ...filters};
    this.props.onAmendQueryParams(params);
    this.setState({filter: {shown: false}, isFilterApplied:false });
  }
}
